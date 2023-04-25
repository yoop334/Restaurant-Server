using Model.Entities;

namespace Business.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    
    Task<User?> AddAsync(User user);
}