﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class AdministrationsAreaModel
    {
        public List<String> Admins { get; set; }
        public string NewAdmin { get; set; }
        public async Task GetAdmins(UserManager<ApplicationUser> um)
        {
            var admins  = await um.GetUsersInRoleAsync("Admin");
            this.Admins = admins.Select(u => u.UserName).ToList();
        }
    }
}
