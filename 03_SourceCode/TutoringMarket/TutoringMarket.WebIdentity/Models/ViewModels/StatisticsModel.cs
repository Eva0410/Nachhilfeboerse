using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Statistics;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class StatisticsModel
    {
        public List<DataPoint> TutorsPerClass { get; set; }
        public List<DataPoint> TutorsPerGender { get; set; }
        public string DepartmentWithMostTutors { get; set; }
        public int TutorsCount { get; set; }
        public int TutorsWithImagePercentage { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.TutorsPerClass = uow.GetTutorsPerClass();
            this.TutorsPerGender = uow.GetTutorsPerGender();
            this.DepartmentWithMostTutors = uow.GetDepartmentWithMostTutors();
            this.TutorsCount = uow.GetTutorsCount();
            this.TutorsWithImagePercentage = uow.GetTutorsWithImagePercentage();
        }
    }
}
