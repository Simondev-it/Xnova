using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xnova.Base;
using Xnova.Models;

namespace Xnova.Repositories
{
    public class FieldRepository : GenericRepository<Field>
    {

        public FieldRepository(XnovaContext context) => _context = context;

        public async Task<List<Field>> GetAllAsync()
        {
            return await _context.Fields.Include(p => p.Bookings).ToListAsync();
        }

        public async Task<Field> GetByIdAsync(int id)
        {
            var result = await _context.Fields.Include(p => p.Bookings).FirstAsync(p => p.Id == id);

            return result;

        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
