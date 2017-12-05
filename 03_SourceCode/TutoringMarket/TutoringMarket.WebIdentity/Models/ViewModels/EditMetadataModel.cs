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
        public List<Subject> Subjects { get; set; }
        public Subject NewSubject { get; set; }
        public void Init(IUnitOfWork uow)
        {
            this.Subjects = uow.SubjectRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
        }
    }
}
