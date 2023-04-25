using Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
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
}