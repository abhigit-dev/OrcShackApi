using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrcShackApi.Core.Models;
using OrcShackApi.Data.Helper;

namespace OrcShackApi.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly OrcShackApiContext _context;
        private readonly ILogger<UserRepository> _logger;
        private const int MaxFailedAttempts = 5; 
        private const int LockoutMinutes = 2;

        public UserRepository(OrcShackApiContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> Authenticate(string email, string password)
        {
            _logger.LogInformation("Authenticating user with email: {Email}", email);
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                _logger.LogError("User not found");
                throw new NotFoundException("User not found");
            }
            
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                user.FailedLoginAttemptCount++;

                _logger.LogWarning("Failed login attempt {AttemptCount} for user {Email}", user.FailedLoginAttemptCount, email);
                if (user.FailedLoginAttemptCount >= MaxFailedAttempts)
                {
                    user.AccountLockedUntil = DateTime.Now.AddMinutes(LockoutMinutes);
                    _logger.LogWarning("Account locked until {LockoutEnd} due to too many failed login attempts", user.AccountLockedUntil);
                    throw new AccountLockedException();
                }
                
                _logger.LogError("Authentication Failed for User {Name}", user.Name);
                throw new NotFoundException($"Authentication Failed for User {user.Name}");
            }

            _logger.LogInformation("User authenticated successfully");
            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            _logger.LogInformation("Getting all users");
            var users = await _context.Users.ToListAsync();
            _logger.LogInformation("Retrieved {Count} users", users.Count());
            return users;
        }

        public async Task<User> GetById(int id)
        {
            _logger.LogInformation("Getting user by id: {Id}", id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new NotFoundException("User not found");
            }
            _logger.LogInformation("User retrieved successfully");
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            _logger.LogInformation("Getting user by email: {email}", email);
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new NotFoundException("User not found");
            }
            _logger.LogInformation("User retrieved successfully");
            return user;
        }
        
        public async Task<User> Create(User user, string password)
        {
            _logger.LogInformation("Creating new user");
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
                throw new Exception("Email \"" + user.Email + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully");
            return user;
        }

        public async Task Update(User userParam, string password = null)
        {
            _logger.LogInformation("Updating user with id: {Id}", userParam.Id);
            var user = await _context.Users.FindAsync(userParam.Id);

            if (user == null)
            {
                _logger.LogError("User not found");
                throw new NotFoundException("User not found");
            }

            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                // email has changed so check if the new email is already taken
                if (_context.Users.Any(x => x.Email == userParam.Email))
                    throw new Exception("Email " + userParam.Email + " is already taken");

                user.Email = userParam.Email;
            }

            // update user properties
            user.Name = userParam.Name;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User updated successfully");
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation("Deleting user with id: {Id}", id);
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User deleted successfully");
            }
            else
            {
                _logger.LogError("User not found");
            }
        }

        public async Task<bool> UpdatePassword(string email, string oldPassword, string newPassword)
        {
            _logger.LogInformation("Updating password for user with email: {Email}", email);
            // Get the user from the database
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }

            // Verify the old password
            if (!VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogError("Failed to verify old password");
                return false;
            }

            // Hash the new password
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            // Update the user's password
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Save the changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password updated successfully");
            return true;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }

            return true;
        }
    }
}
