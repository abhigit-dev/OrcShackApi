using OrcShackApi.Core.Models;
using OrcShackApi.Data.Repository;

namespace OrcShackApi.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User?> Authenticate(string email, string password)
        {
            _logger.LogInformation("Authenticating user with email: {Email}", email);
            var user = await _userRepository.Authenticate(email, password);
            _logger.LogInformation(user != null ? "User authenticated successfully" : "Failed to authenticate user");
            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            _logger.LogInformation("Getting all users");
            var users = await _userRepository.GetAll();
            _logger.LogInformation("Retrieved {Count} users", users.Count());
            return users;
        }

        public async Task<User> GetById(int id)
        {
            _logger.LogInformation("Getting user by id: {Id}", id);
            var user = await _userRepository.GetById(id);
            _logger.LogInformation(user != null ? "User retrieved successfully" : "Failed to retrieve user");
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            _logger.LogInformation("Getting user by email: {email}", email);
            var user = await _userRepository.GetByEmail(email);
            _logger.LogInformation(user != null ? "User retrieved successfully" : "Failed to retrieve user");
            return user;
        }

        public async Task<User> Create(User user, string password)
        {
            _logger.LogInformation("Creating new user");
            var newUser = await _userRepository.Create(user, password);
            _logger.LogInformation("User created successfully");
            return newUser;
        }

        public async Task Update(User user, string password)
        {
            _logger.LogInformation("Updating user with id: {Id}", user.Id);
            await _userRepository.Update(user, password);
            _logger.LogInformation("User updated successfully");
        }

        public async Task<bool> UpdatePassword(string email, string oldPassword, string newPassword)
        {
            _logger.LogInformation("Updating password for user with email: {Email}", email);
            var result = await _userRepository.UpdatePassword(email, oldPassword, newPassword);
            _logger.LogInformation(result ? "Password updated successfully" : "Failed to update password");
            return result;
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation("Deleting user with id: {Id}", id);
            await _userRepository.Delete(id);
            _logger.LogInformation("User deleted successfully");
        }
    }
}
