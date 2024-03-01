using LeadManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadManagementSystem.Services.Interfaces
{
    public interface ILeadServices
    {
        public Task<bool> AddLead(Lead lead);

        public Task<Lead?> GetLeadByEmail(string email);

        public Task<List<Lead>> GetLeadList();

        public Task<List<Lead>> GetLeadsByAgentId(string id);

        public Task<List<LeadStatus>> GetLeadStatusListByLeadId(int leadId);

        public Task<List<Lead>> GetLeadsListByAgents(List<Agent> agents);
        public Task<List<Lead>> GetLeadListByStatus(string status);

        public Task<bool> UpdateAgentNameForLead(Lead lead, string agentName);


        public Task<bool> UpdateLead(Lead lead);

        public Task<Lead> GetLeadById(int id);

        public Task<bool> AddLeadStatus(LeadStatus leadStatus);


    }
}
