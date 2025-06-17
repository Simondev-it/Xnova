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
    public  class ImageRepository : GenericRepository<Image>
    {
        public ImageRepository(XnovaContext context) => _context = context;

        //public async Task<List<Image>> GetAllAsync()
        //{
        //    return await _context.Images.Include(p => p.).ToListAsync();
        //}

        //public async Task<Image> GetByIdAsync(int id)
        //{
        //    var result = await _context.Fields.Include(p => p.Bookings).FirstAsync(p => p.Id == id);

        //    return result;

        //}
    }
}
