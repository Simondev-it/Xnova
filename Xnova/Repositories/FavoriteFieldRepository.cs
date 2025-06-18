using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xnova.Base;
using Xnova.Models;

namespace Xnova.Repositories
{
    public class FavoriteFieldRepository : GenericRepository<FavoriteField>
    {
        public FavoriteFieldRepository(XnovaContext context) => _context = context;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
