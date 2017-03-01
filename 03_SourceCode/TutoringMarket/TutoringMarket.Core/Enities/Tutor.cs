using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;

namespace TutoringMarket.Core.Enities
{
    public class Tutor : EntityObject
    {
        //TOOD Weitere [] einfügen
        //TODO Bild einfügen
        [Required, Display(Name ="Vorname")]
        public String FirstName { get; set; }
        [Required, Display(Name ="Nachname")]
        public String LastName { get; set; }
        //TODO Regex einfügen
        [Required]
        public String EMail { get; set; }
        //TODO Regex
        [Required]
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
