using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;

namespace TutoringMarket.Core.Enities
{
    public class EntityObject : IEntityObject
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        public byte[] Timestamp
        {
            get;
            set;
        }
    }
}
