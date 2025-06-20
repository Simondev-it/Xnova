using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xnova.Base;
using Xnova.Models;

namespace Xnova.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(XnovaContext context) => _context = context;
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.Include(p => p.Bookings).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var result = await _context.Users.Include(p => p.Bookings).FirstAsync(p => p.Id == id);

            return result;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            var result = await _context.Users.Include(p => p.Bookings).FirstAsync(p => p.Email == email);

            return result;
        }
        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Email == username); // Điều chỉnh theo trường thực tế
        }
        public async Task<User> GetUserByCredentialsAsync(string username, string password)
        {
            // Giả định bạn có phương thức để kiểm tra mật khẩu đã được băm
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == username && u.Password == password);
        }
        public async Task<User?> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.FirstOrDefaultAsync(predicate);
        }
    }
    }
