using UserManagementService.Models;

namespace UserManagementService.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
    }
}