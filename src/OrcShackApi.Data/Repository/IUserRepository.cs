using OrcShackApi.Core.Models;

namespace OrcShackApi.Data.Repository
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        Task<User> Create(User user, string password);
        Task Update(User user, string password);
        Task<bool> UpdatePassword(string email, string oldPassword, string newPassword);
        Task Delete(int id);
    }
}
