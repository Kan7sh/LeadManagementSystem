using LeadManagementSystem.Models;
using LeadManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace LeadManagementSystem.Controllers
{

	[Authorize(Roles = "Manager")]
	public class ManagerController : Controller
	{

		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IManagerServices _managerServices;
		private readonly IAgentServices _agentServices;
		private readonly ILeadServices _leadServices;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IManagerServices managerService, IAgentServices agentServices, ILeadServices leadServices, ILogger<ManagerController> logger)
        {
			_userManager = userManager;
			_roleManager = roleManager;
			_managerServices = managerService;
			_agentServices = agentServices;
			_leadServices = leadServices;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
                var currentUser = User;
                var user = await _userManager.GetUserAsync(currentUser);

                List<Agent> agents = await _agentServices.GetAgentListByManagerId(user.Id);

                List<Lead> leads = await _leadServices.GetLeadsListByAgents(agents);

                return View(leads);
            }
            catch(Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();

        }


        public async Task<IActionResult> CheckDelete(string agentId)
		{
			try
			{
                List<Lead> leads = await _leadServices.GetLeadsByAgentId(agentId);
                return Json(leads.Count);
            }
            catch(Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

			return View();
        }



		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAgent(Agent agent)
		{
			try
			{
                var user = await _userManager.FindByIdAsync(agent.Id);
                user.Email = agent.Email;
                user.UserName = agent.Email;


                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    Manager manager = await _managerServices.GetManagerByID(agent.ManagerID);

                    agent.ManagerName = manager.Fullname;

                    await _agentServices.UpdateAgent(agent);

                    List<Lead> leads = await _leadServices.GetLeadsByAgentId(agent.Id);

                    foreach (Lead lead in leads)
                    {
                        await _leadServices.UpdateAgentNameForLead(lead, agent.Fullname);
                    }

                    return RedirectToAction("Index");

                }

                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError(string.Empty, "The email already exists.");
                        TempData["ErrorMessage"] = "The email already exists.";
                        break;
                    }
                }






                return View(agent);
            }catch(Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

			return View();

        }


		public async Task<IActionResult> EditAgent(string agentId)
		{
			try
			{
                Agent? agent = await _agentServices.GetAgentByID(agentId);

                return View(agent);
            }catch (Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

			return View();

        }

		public async Task<IActionResult> AddAgent()
		{
			try
			{
                var currentUser = User;
                var user = await _userManager.GetUserAsync(currentUser);
                ViewBag.UserId = user.Id;
                return View();
            }catch (Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

			return View();

        }

		public async Task<IActionResult> AddLead()
		{
			try
			{
                var currentUser = User;
                var user = await _userManager.GetUserAsync(currentUser);

                List<Agent> agentList = await _agentServices.GetAgentListByManagerId(user.Id);
                ViewBag.AgentList = agentList;
                return View();
            }catch(Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }
			return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddLead(Lead lead)
		{

			try
			{
                List<Agent> agentList = [];
                if (ModelState.IsValid)
                {

                    Lead? tempLead = await _leadServices.GetLeadByEmail(lead.Email);


                    if (tempLead != null)
                    {
                        ModelState.AddModelError(string.Empty, "The email already exists.");
                        TempData["ErrorMessage"] = "The email already exists.";
                        agentList = await _agentServices.GetAgentList();
                        ViewBag.AgentList = agentList;
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


                var currentUser = User;
                var user = await _userManager.GetUserAsync(currentUser);

                agentList = await _agentServices.GetAgentListByManagerId(user.Id);
                ViewBag.AgentList = agentList;
                return View();

            }
			catch(Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

			return View();


        }

		public async Task<IActionResult> DeleteAgent(string agentId)
		{

			try
			{

                var user = await _userManager.FindByIdAsync(agentId);

                var result = await _userManager.DeleteAsync(user);


                Agent agent = await _agentServices.GetAgentByID(agentId);
                await _agentServices.DeleteAgent(agent);

                return RedirectToAction("Agents");
            }catch(Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

			return View();
        }

		public async Task<IActionResult> EditLead(int leadId)
		{
			try
			{
                var currentUser = User;
                var user = await _userManager.GetUserAsync(currentUser);
                Lead lead = await _leadServices.GetLeadById(leadId);
                List<Agent> agentList = await _agentServices.GetAgentListByManagerId(user.Id);
                ViewBag.AgentList = agentList;
                return View(lead);
			}
			catch (Exception ex)
			{
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }
			return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditLead(Lead lead)
		{
			try
			{
                List<Agent> agentList = [];
                if (ModelState.IsValid)
                {

                    Lead? tempLead = await _leadServices.GetLeadByEmail(lead.Email);
                    if (tempLead != null && tempLead.Id != lead.Id)
                    {
                        ModelState.AddModelError(string.Empty, "The email already exists.");
                        TempData["ErrorMessage"] = "The email already exists.";
                        var currentUser = User;
                        var user = await _userManager.GetUserAsync(currentUser);
                        agentList = await _agentServices.GetAgentListByManagerId(user.Id);
                        ViewBag.AgentList = agentList;
                        return View();
                    }

                    Lead outdatedLead = await _leadServices.GetLeadById(lead.Id);

                    bool changedStatus = outdatedLead.Status != lead.Status;


                    if (outdatedLead.AgentId == lead.AgentId)
                    {

                        outdatedLead.Status = lead.Status;
                        outdatedLead.Email = lead.Email;
                        outdatedLead.UserName = lead.UserName;
                        outdatedLead.Address = lead.Address;
                        outdatedLead.PhoneNumber = lead.PhoneNumber;
                        outdatedLead.Source = lead.Source;
                        outdatedLead.City = lead.City;

                        await _leadServices.UpdateLead(outdatedLead);

                        if (changedStatus)
                        {
                            LeadStatus leadStatus = new LeadStatus
                            {
                                LeadId = lead.Id,
                                Remark = "Manager changed the status",
                                DateTime = DateTime.Now,
                                Status = lead.Status,
                                AgentId = outdatedLead.AgentId,
                                AgentName = outdatedLead.AgentName
                            };
                            await _leadServices.AddLeadStatus(leadStatus);

                        }

                    }
                    else
                    {
                        Agent agent = await _agentServices.GetAgentByID(lead.AgentId);

                        outdatedLead.Status = lead.Status;
                        outdatedLead.Email = lead.Email;
                        outdatedLead.UserName = lead.UserName;
                        outdatedLead.Address = lead.Address;
                        outdatedLead.PhoneNumber = lead.PhoneNumber;
                        outdatedLead.Source = lead.Source;
                        outdatedLead.City = lead.City;
                        outdatedLead.AgentId = lead.AgentId;
                        outdatedLead.AgentName = agent.Fullname;

                        await _leadServices.UpdateLead(outdatedLead);

                        string remark = "Manager changed the agent";

                        if (changedStatus)
                        {
                            remark += " and status";
                        }


                        LeadStatus leadStatus = new LeadStatus
                        {
                            LeadId = lead.Id,
                            Remark = remark,
                            DateTime = DateTime.Now,
                            Status = lead.Status,
                            AgentId = lead.AgentId,
                            AgentName = agent.Fullname
                        };

                        await _leadServices.AddLeadStatus(leadStatus);

                    }

                    return RedirectToAction("Index", "Manager");

                }

                agentList = await _agentServices.GetAgentList();
                ViewBag.AgentList = agentList;
                return View();

            }
            catch (Exception ex)
			{

                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");
            }
            return View();

        }


		public async Task<IActionResult> Agents()
		{
            try
            {
                var currentUser = User;
                var user = await _userManager.GetUserAsync(currentUser);

                List<Agent> agents = await _agentServices.GetAgentListByManagerId(user.Id);

                return View(agents);
            }catch(Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();

        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddAgent(Agent agent)
		{

            try
            {
                if (ModelState.IsValid)
                {

                    var user = new IdentityUser();
                    user.Email = agent.Email;
                    user.UserName = agent.Email;
                    var result = await _userManager.CreateAsync(user, agent.PasswordHash);

                    if (result.Succeeded)
                    {

                        Manager manager = await _managerServices.GetManagerByID(agent.ManagerID);

                        agent.ManagerName = manager.Fullname;
                        agent.Id = user.Id;

                        await _userManager.AddToRoleAsync(user, "Agent");

                        _agentServices.AddAgent(agent);

                        return RedirectToAction("Agents");
                    }

                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError(string.Empty, "The email already exists.");
                            TempData["ErrorMessage"] = "The email already exists.";
                            break;
                        }
                    }
                }
                var currentUser = User;
                var user2 = await _userManager.GetUserAsync(currentUser);
                ViewBag.UserId = user2.Id;
                return View();
            }catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();

        }
	}
}
