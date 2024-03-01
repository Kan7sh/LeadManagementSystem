using LeadManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadManagementSystem.Services.Interfaces
{
    public interface IAgentServices
    {

        public Task<bool> AddAgent(Agent agent);

        public Task<Agent?> GetAgentByID(string id);

        public Task<List<Agent>> GetAgentListByManagerId(string id);

        public Task<List<Agent>> GetAgentList();

        public Task<bool> UpdateAgent(Agent agent);
        public Task<bool> DeleteAgent(Agent agent);


    }
}
