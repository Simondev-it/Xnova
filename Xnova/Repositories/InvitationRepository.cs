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
    public class InvitationRepository : GenericRepository<Invitation>
    {
        public InvitationRepository(XnovaContext context) => _context = context;

        public async Task<List<Invitation>> GetAllAsync()
        {
            return await _context.Invitations.Include(p => p.UserInvitations).ToListAsync();
        }

        public async Task<Invitation> GetByIdAsync(int id)
        {
            var result = await _context.Invitations.Include(p => p.UserInvitations).FirstAsync(p => p.Id == id);

            return result;

        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
