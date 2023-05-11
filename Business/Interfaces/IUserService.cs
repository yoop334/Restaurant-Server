using Model.Entities;
using Model.ViewModels.User;

namespace Business.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    
    Task<User?> AddAsync(User user);

    Task<bool> UpdateAsync(UserUpdateViewModel user, int id);
}