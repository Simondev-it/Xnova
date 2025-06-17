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
    public class VenueRepository : GenericRepository<Venue>
    {
        public VenueRepository(XnovaContext context) => _context = context;
        public async Task<List<Venue>> GetAllAsync()
        {
            return await _context.Venues.Include(p => p.Fields).ToListAsync();
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            var result = await _context.Venues.Include(p => p.Fields).FirstAsync(p => p.Id == id);

            return result;
        }
    }
}
