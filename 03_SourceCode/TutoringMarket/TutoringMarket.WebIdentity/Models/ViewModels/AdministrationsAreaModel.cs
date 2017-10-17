using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class AdministrationsAreaModel
    {
        public List<String> Admins { get; set; }
        public string NewAdmin { get; set; }
        public List<String> Classes { get; set; }
        public async Task GetAdmins(UserManager<ApplicationUser> um, IUnitOfWork uow)
        {
            var admins  = await um.GetUsersInRoleAsync("Admin");
            this.Admins = admins.Select(u => u.UserName).ToList();

            this.Classes = uow.ClassRepository.Get(filter: c => c.Tutors.Count == 0, orderBy: ord => ord.OrderBy(c => c.Name)).Select(c => c.Name).ToList();
        }
    }
}
