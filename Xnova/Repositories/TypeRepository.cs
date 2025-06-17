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
    public class TypeRepository : GenericRepository<Models.Type>
    {
        public TypeRepository(XnovaContext context ) => _context = context;
        public async Task<List<Models.Type>> GetAllAsync()
        {
            return await _context.Types.Include(p => p.Fields).ToListAsync();
        }

        public async Task<Models.Type> GetByIdAsync(int id)
        {
            var result = await _context.Types.Include(p => p.Fields).FirstAsync(p => p.Id == id);

            return result;
        }
    }
}
