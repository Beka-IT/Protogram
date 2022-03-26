using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Services
{
    public class UserService : IUserService
    {
        AppDbContext db;
        public UserService(AppDbContext context)
        {
            db = context;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await db.Users.ToListAsync();
            return users;
        }
        public async Task<User> GetUserAsync(int id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u=>u.UserId == id);
            return user;
        }
        public async Task<User> AddUserAsync(User user)
        {
            if(db.Users.Any(u=>u.Login == user.Login))
            {
                return null;
            }
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }
        
        

        public async Task<User> UpdateUserAsync(User user)
        {
            User updatedUser = null;
            if(db.Users.Any(u=>u.UserId == user.UserId))
            {
                db.Users.Update(user);
                updatedUser = user;
                await db.SaveChangesAsync();
            }
            return updatedUser;
        }
        public async Task<User> DeleteUserAsync(int id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u=>u.UserId == id);
            if(user != null)
            {
                db.Remove(user);
                await db.SaveChangesAsync();
                
            }
            return user;
        }
    }
}