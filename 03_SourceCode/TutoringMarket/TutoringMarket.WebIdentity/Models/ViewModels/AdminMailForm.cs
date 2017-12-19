using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class AdminMailForm
    {
        [Required]
        public string Nachricht { get; set; }
    }
}
