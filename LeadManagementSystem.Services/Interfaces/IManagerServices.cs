using LeadManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadManagementSystem.Services.Interfaces
{
    public interface IManagerServices
    {

        public Task<bool> AddManager(Manager manager);

        public Task<Manager?> GetManagerByID(string id);

        public Task<List<Manager>> GetManagerList();

        public Task<bool> UpdateManager(Manager manager);
        public Task<bool> DeleteManager(Manager manager);

    }
}
