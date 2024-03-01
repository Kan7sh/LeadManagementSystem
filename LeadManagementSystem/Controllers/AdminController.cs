using LeadManagementSystem.Models;
using LeadManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace LeadManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IManagerServices _managerServices;
        private readonly IAgentServices _agentServices;
        private readonly ILeadServices _leadServices;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IManagerServices managerService, IAgentServices agentServices, ILeadServices leadServices, ILogger<AdminController> logger)
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
                List<Lead> leads = await _leadServices.GetLeadList();
                return View(leads);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");
            }
            return View();

        }

        public async Task<IActionResult> Managers()
        {

            try
            {
                List<Manager> managersList = await _managerServices.GetManagerList();

                return View(managersList);
            }catch(Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();

        }

        public async Task<IActionResult> Agents()
        {

            try
            {
                List<Agent> agentList = await _agentServices.GetAgentList();

                return View(agentList);
            }catch (Exception ex)
            {
                       _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();

        }

        public async Task<IActionResult> LeadStatuses(int leadId)
        {

            try
            {
                List<LeadStatus> leadStatusesLead = await _leadServices.GetLeadStatusListByLeadId(leadId);

                return View(leadStatusesLead);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"{e.Message} Error occurred at {DateTime.Now}");

            }
            return View();
        }

        public IActionResult AddManager()
        {
            return View();
        }


        public async Task<IActionResult> Leads(string leadStatus)
        {
            try
            {
                if (leadStatus == null)
                {
                    leadStatus = TempData["Status"] as string;
                }

                List<Lead> leads = await _leadServices.GetLeadListByStatus(leadStatus);
                return View(leads);
            }
            catch (Exception ex)
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

                List<Manager> managersList = await _managerServices.GetManagerList();
                ViewBag.ManagerList = managersList;
                return View(agent);
            }
            catch (Exception ex)
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





                List<Manager> managersList = await _managerServices.GetManagerList();
                ViewBag.ManagerList = managersList;
                return View(agent);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();
        }

        public async Task<IActionResult> EditManager(string managerId)
        {
            try
            {
                Manager manager = await _managerServices.GetManagerByID(managerId);
                return View(manager);
            }catch(Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditManager(Manager manager)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(manager.Id);
                user.Email = manager.Email;
                user.UserName = manager.Email;


                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {

                    await _managerServices.UpdateManager(manager);

                    List<Agent> agents = await _agentServices.GetAgentListByManagerId(manager.Id);

                    foreach (Agent agent in agents)
                    {
                        agent.ManagerName = manager.Fullname;
                        await _agentServices.UpdateAgent(agent);
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




                return View();

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
                Lead lead = await _leadServices.GetLeadById(leadId);
                List<Agent> agentList = await _agentServices.GetAgentList();
                ViewBag.AgentList = agentList;
                return View(lead);
            }catch(Exception ex)
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
                        agentList = await _agentServices.GetAgentList();
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
                                Remark = "Admin changed the status",
                                DateTime = DateTime.Now,
                                Status = lead.Status,
                                AgentId = outdatedLead.AgentId,
                                AgentName = "Admin"
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

                        string remark = "Admin changed the agent";

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
                    TempData["Status"] = outdatedLead.Status;

                    return RedirectToAction("Leads", "Admin", outdatedLead.Status);

                }

                agentList = await _agentServices.GetAgentList();
                ViewBag.AgentList = agentList;
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
                List<Agent> agentList = await _agentServices.GetAgentList();
                ViewBag.AgentList = agentList;
                return View();
            }catch(Exception ex)
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
            }catch(Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return Json("");

        }

        public async Task<IActionResult> CheckDeleteForManager(string managerId)
        {
            try
            {
                List<Agent> agent = await _agentServices.GetAgentListByManagerId(managerId);
                return Json(agent.Count);
            }catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return Json("");

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
            }catch  (Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }
            return View();
        }

        public async Task<IActionResult> DeleteManager(string managerId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(managerId);

                var result = await _userManager.DeleteAsync(user);


                Manager manager = await _managerServices.GetManagerByID(managerId);
                await _managerServices.DeleteManager(manager);

                return RedirectToAction("Managers");
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


                agentList = await _agentServices.GetAgentList();
                ViewBag.AgentList = agentList;
                return View();

            }catch(Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();



        }


        public async Task<IActionResult> AddAgent()
        {
            try
            {
                List<Manager> managersList = await _managerServices.GetManagerList();
                ViewBag.ManagerList = managersList;
                return View();
            }
            catch(Exception ex)
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
                List<Manager> managersList = await _managerServices.GetManagerList();
                ViewBag.ManagerList = managersList;
                return View();

            }catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddManager(Manager manager)
        {

            try
            {
                if (ModelState.IsValid)
                {


                    var user = new IdentityUser();
                    user.Email = manager.Email;
                    user.UserName = manager.Email;
                    var result = await _userManager.CreateAsync(user, manager.PasswordHash);

                    if (result.Succeeded)
                    {

                        manager.Id = user.Id;

                        await _userManager.AddToRoleAsync(user, "Manager");

                        _managerServices.AddManager(manager);

                        return RedirectToAction("Managers");
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
                return View(manager);
            }catch(Exception ex)
            {
                _logger.LogInformation($"{ex.Message} Error occurred at {DateTime.Now}");

            }
            return View();
        }
    }
}
