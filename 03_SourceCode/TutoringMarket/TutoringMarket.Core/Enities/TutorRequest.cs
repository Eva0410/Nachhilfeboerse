using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class TutorRequest : EntityObject
    {
        public DateTime Date { get; set; }

        public int Tutor_Id { get; set; }
        [ForeignKey((nameof(Tutor_Id)))]
        public Tutor Tutor { get; set; }
        public string SchoolClass { get; set; }
    }
}
