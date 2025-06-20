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
    public class RelationshipRepository : GenericRepository<Relationship>
    {
        public RelationshipRepository(XnovaContext context) => _context = context;

        public async Task<List<Relationship>> GetAllAsync()
        {
            return await _context.Relationships.Include(p => p.Friends).ToListAsync();
        }

        public async Task<Relationship> GetByIdAsync(int id)
        {
            var result = await _context.Relationships.Include(p => p.Friends).FirstAsync(p => p.Id == id);

            return result;

        }
    }

}
