using System.Text.Json.Serialization;

namespace OrcShackApi.Core.Models
{
    public class User
    {
        public User()
        {
            Role = "User"; // Default role
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        [JsonIgnore]
        public int FailedLoginAttemptCount { get; set; }
        [JsonIgnore]
        public DateTime?  AccountLockedUntil { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<Rating> Ratings { get; set; }
    }
}
