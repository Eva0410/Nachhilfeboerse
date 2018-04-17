using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoringMarket.Core.Enities
{
    public class AcceptStatistics : EntityObject
    {
        public bool? TutorAccepted { get; set; }
        public bool? ReviewAccepted { get; set; }
    }
}
