using LeadManagementSystem.Data.Context;
using LeadManagementSystem.Models;
using LeadManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadManagementSystem.Services.Services
{
    public class ManagerServices : IManagerServices
    {

        public readonly ApplicationDbContext _context;
        public ManagerServices(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<bool> AddManager(Manager manager)
        {
            _context.Managers.Add(manager);

            return await Save();
        }

        public async Task<bool> DeleteManager(Manager manager)
        {
            _context.Remove(manager);
            return await Save();
        }

        public async Task<Manager?> GetManagerByID(string id)
        {
            return _context.Managers.Where(o => o.Id == id).FirstOrDefault();
            
        }

        public async Task<List<Manager>> GetManagerList()
        {
            return await _context.Managers.ToListAsync();

        }

        public async Task<bool> Save()
        {
            var saved = _context.SaveChangesAsync();
            return await saved > 0 ? true : false;
        }

        public async Task<bool> UpdateManager(Manager manager)
        {
            _context.Update(manager);

            return await Save();


        }
    }
}
