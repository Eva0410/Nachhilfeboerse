using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Models.ViewModels
{
    public class EditMetadataModel
    {
        public List<Department> Departments { get; set; }
        public List<SchoolClass> Classes { get; set; }
        public List<Subject> Subjects { get; set; }
        public Department NewDepartment { get; set; }
        public SchoolClass NewClass { get; set; }
        public Subject NewSubject { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Departments = uow.DepartmentRepository.Get(orderBy: ord => ord.OrderBy(d => d.Name)).ToList();
            this.Classes = uow.ClassRepository.Get(orderBy: ord => ord.OrderBy(c => c.Name)).ToList();
            this.Subjects = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
        }
    }
}
