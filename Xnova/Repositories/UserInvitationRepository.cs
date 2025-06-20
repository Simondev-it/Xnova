using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xnova.Base;
using Xnova.Models;

namespace Xnova.Repositories
{
    public class UserInvitationRepository : GenericRepository<UserInvitation>
    {
        public UserInvitationRepository(XnovaContext context) => _context = context;
    }
}

