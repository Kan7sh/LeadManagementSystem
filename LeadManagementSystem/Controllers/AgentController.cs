using LeadManagementSystem.Models;
using LeadManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace LeadManagementSystem.Controllers
{
    [Authorize(Roles = "Agent")]
    public class AgentController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IManagerServices _managerServices;
        private readonly IAgentServices _agentServices;
        private readonly ILeadServices _leadServices;

        public AgentController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IManagerServices managerService, IAgentServices agentServices, ILeadServices leadServices)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _managerServices = managerService;
            _agentServices = agentServices;
            _leadServices = leadServices;
        }



        public async Task<IActionResult> Index()
        {

            var currentUser = User;
            var user = await _userManager.GetUserAsync(currentUser);


            List<Lead> leads = await _leadServices.GetLeadsByAgentId(user.Id);

            return View(leads);
        }



        public async Task<IActionResult> EditLead(int leadId)
        {

            Lead lead = await _leadServices.GetLeadById(leadId);

            return View(lead);
        }

        public async Task<IActionResult> ChangeLeadStatus(int leadId)
        {

            Lead lead = await _leadServices.GetLeadById(leadId);

            return View(lead);
        }

        public async Task<IActionResult> AddLead()
        {
            var currentUser = User;
            var user = await _userManager.GetUserAsync(currentUser);

            Agent agent = await  _agentServices.GetAgentByID(user.Id);

            ViewBag.AgentId = user.Id;


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLead(Lead lead)
        {
            if (ModelState.IsValid)
            {

                Lead? tempLead = await _leadServices.GetLeadByEmail(lead.Email);


                if (tempLead != null)
                {
                    ModelState.AddModelError(string.Empty, "The email already exists.");
                    TempData["ErrorMessage"] = "The email already exists.";
                  
                    return View();
                }
                else
                {

                    Agent? agent = await _agentServices.GetAgentByID(lead.AgentId);
                    lead.AgentName = agent.Fullname;
                    lead.Status = "New";
                    await _leadServices.AddLead(lead);
                    Lead newLead = await _leadServices.GetLeadByEmail(lead.Email);

                    LeadStatus leadStatus = new LeadStatus
                    {
                        LeadId = newLead.Id,
                        Remark = "New",
                        DateTime = DateTime.Now,
                        Status = "New",
                        AgentId = lead.AgentId,
                        AgentName = lead.AgentName
                    };

                    _leadServices.AddLeadStatus(leadStatus);

                    return RedirectToAction("Index");
                }


            }

            return View();


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLead(Lead lead)
        {

            Lead? tempLead = await _leadServices.GetLeadByEmail(lead.Email);
            if (tempLead != null && tempLead.Id != lead.Id)
            {
                ModelState.AddModelError(string.Empty, "The email already exists.");
                TempData["ErrorMessage"] = "The email already exists.";

                return View();
            }

            tempLead.UserName = lead.UserName;
            tempLead.Email = lead.Email;
            tempLead.Address = lead.Address;    
            tempLead.City = lead.City;
            tempLead.Source = lead.Source;
            lead.PhoneNumber = lead.PhoneNumber; 
            await _leadServices.UpdateLead(tempLead);
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeLeadStatus(int id, string Remark, string status)
        {
            Lead lead = await _leadServices.GetLeadById(id);

            lead.Status = status;

            await _leadServices.UpdateLead(lead);

            LeadStatus leadStatus = new LeadStatus
            {
                Remark = Remark,
                LeadId = lead.Id,
                DateTime = DateTime.Now,
                AgentId = lead.AgentId,
                AgentName = lead.AgentName,
                Status = status
            };

            await _leadServices.AddLeadStatus(leadStatus);

            return RedirectToAction("Index");
        }
    }
}
