using Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.ViewModels.User;
using Repository;

namespace Business.Implementations;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IHashingHelper _hashingHelper;

    public UserService(DataContext context, IHashingHelper authorizationHelper)
    {
        _context = context;
        _hashingHelper = authorizationHelper;
    }
    
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.UserId == id);
    }

    public async Task<User?> AddAsync(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username)) return null;

        user.Password = _hashingHelper.HashPassword(user.Password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(UserUpdateViewModel user, int id)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        if (existingUser == null) return false;
        
        _context.Entry(existingUser).CurrentValues.SetValues(user);

        await _context.SaveChangesAsync();
        return true;
    }
}