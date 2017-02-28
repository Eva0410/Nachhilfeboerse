using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;

namespace TutoringMarket.Core.Enities
{
    public class Tutor : EntityObject
    {
        //TODO Bild einfügen
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String EMail { get; set; }
        public String PhoneNumber { get; set; }
        public String Description { get; set; }
        [Column(TypeName ="datetime2")] //sonst SQL-Exception
        public DateTime Birthday { get; set; }
        public String[] Time { get; set; }
        public double Price { get; set; }
        //TODO M-M Relation implementieren
        public Subject[] Subjects { get; set; }
        //TODO Testdatensätze einfügen
        //[ForeignKey("Department_Id")]
        //public Department Department { get; set; }
        //public int Department_Id { get; set; }
        //[ForeignKey("Class_Id")]
        //public Class Class { get; set; }
        //public int Class_Id { get; set; }
    }
}
