﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RewardsPOC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RewardsPOC.Controllers
{
    public class RewardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RewardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> AddRewardPoints()
        {
            var users = await _userManager.Users.ToListAsync();
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            List<ApplicationUser> result = GetChildren(users, userId);
            return View("AddCredit", result);
        }

        [HttpGet("AddPoints/{id}")]
        public async Task<ActionResult> AddPoints(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.RewardPoints += 5;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("AddRewardPoints");
        }
        [HttpGet("RemovePoints/{id}")]
        public async Task<ActionResult> RemovePoints(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.RewardPoints -= 5;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("AddRewardPoints");
        }

        public List<ApplicationUser> GetChildren(List<ApplicationUser> users, int parentId)
        {
            return users
                    .Where(c => c.ManagerId == parentId)
                    .Select(c => new ApplicationUser
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        Email = c.Email,
                        ManagerId = c.ManagerId,
                        RewardPoints = c.RewardPoints,
                        Children = GetChildren(users, c.Id)
                    })
                    .ToList();
        }
    }
}
