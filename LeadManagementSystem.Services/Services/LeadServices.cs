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
    public class LeadServices : ILeadServices
    {

        public readonly ApplicationDbContext _context;


        public LeadServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddLead(Lead lead)
        {
            _context.Leads.Add(lead);

            return await Save();

        }

        public async Task<bool> AddLeadStatus(LeadStatus leadStatus)
        {
            _context.LeadStatuses.Add(leadStatus);

            return await Save();
        }

        public async Task<Lead?> GetLeadByEmail(string email)
        {
            return _context.Leads.Where(o => o.Email == email).FirstOrDefault();
        }

        public async Task<Lead> GetLeadById(int id)
        {
            return _context.Leads.Where(o => o.Id == id).FirstOrDefault();
        }

        public async Task<List<Lead>> GetLeadList()
        {
            return await _context.Leads.ToListAsync();
        }

        public async Task<List<Lead>> GetLeadListByStatus(string status)
        {
            return await _context.Leads.Where(lead=>lead.Status==status).ToListAsync();
        }

        public async Task<List<Lead>> GetLeadsByAgentId(string id)
        {
            return await _context.Leads.Where(lead => lead.AgentId == id).ToListAsync();

        }

        public async Task<List<Lead>> GetLeadsListByAgents(List<Agent> agents)
        {
            var agentIds = agents.Select(a => a.Id).ToList();

            List<Lead> leads = await _context.Leads
                .Where(lead => agentIds.Contains(lead.AgentId))
                .ToListAsync();

            return leads;
        }

        public async Task<List<LeadStatus>> GetLeadStatusListByLeadId(int leadId)
        {
            return await _context.LeadStatuses.Where(leadStatus => leadStatus.LeadId == leadId).ToListAsync();
        }


        public async Task<bool> Save()
        {
            var saved = _context.SaveChangesAsync();
            return await saved > 0 ? true : false;
        }

        public async Task<bool> UpdateAgentNameForLead(Lead lead, string agentName)
        {
           lead.AgentName = agentName;
           _context.Update(lead);
            return await Save();
        }

        public async Task<bool> UpdateLead(Lead lead)
        {
            _context.Update(lead);

            return await Save();
        }
    }
}
