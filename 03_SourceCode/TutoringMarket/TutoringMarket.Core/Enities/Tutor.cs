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
        //TODO Bild einfügen
        [Required, Display(Name ="Vorname")]
        public String FirstName { get; set; }

        [Required, Display(Name ="Nachname")]
        public String LastName { get; set; }

        [Required, RegularExpression("^[_a-Z0-9-]+(.[a-Z0-9-]+)@[a-Z0-9-]+(.[a-Z0-9-]+)*(.[a-Z]{2,4})$"),Display(Name ="E-Mail")]
        public String EMail { get; set; }

        [RegularExpression("^[0-9]*$"), Display(Name ="Telefonnummer")]
        public String PhoneNumber { get; set; }

        [Display(Name ="Beschreibung"), StringLength(300)]
        public String Description { get; set; }

        [Required, Column(TypeName ="datetime2"), Display(Name ="Geburtsdatum")] //sonst SQL-Exception
        public DateTime Birthday { get; set; }

        [Required, Display(Name ="Zeit")]
        public String[] Time { get; set; }

        [Required, Display(Name ="Preis")]
        public double Price { get; set; }

        //TODO Testdatensätze einfügen
        [Display(Name="Abteilung"), Required, ForeignKey("Department_Id")]
        public Department Department { get; set; }
        public int Department_Id { get; set; }

        [ForeignKey("Class_Id"), Display(Name ="Klasse"), Required]
        public Class Class { get; set; }
        public int Class_Id { get; set; }
    }
}
