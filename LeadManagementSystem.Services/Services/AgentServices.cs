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
    public class AgentServices : IAgentServices
    {
        public readonly ApplicationDbContext _context;
        public AgentServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAgent(Agent agent)
        {
            _context.Agents.Add(agent);

            return await Save();
        }

        public async Task<bool> DeleteAgent(Agent agent)
        {
            _context.Remove(agent);
            return await Save();
        }

        public async  Task<Agent?> GetAgentByID(string id)
        {
            return _context.Agents.Where(o => o.Id == id).FirstOrDefault();
        }

        public async Task<List<Agent>> GetAgentList()
        {
            return await _context.Agents.ToListAsync();
        }


        public async Task<List<Agent>> GetAgentListByManagerId(string id)
        {
            return await _context.Agents.Where(agent=>agent.ManagerID == id).ToListAsync();

        }

        public async Task<bool> Save()
        {
            var saved = _context.SaveChangesAsync();
            return await saved > 0 ? true : false;
        }

        public async Task<bool> UpdateAgent(Agent agent)
        {
            _context.Update(agent);

            return await Save();
        }
    }
}
