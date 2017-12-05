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
        //TODO Bild beim einfügen konvertieren
        [Display(Name ="Bild"), FileExtensions(Extensions =".jpg", ErrorMessage ="Das Bild muss im Format jpg hochgeladen werden!")]
        public byte[] Image { get; set; }
        [Required(ErrorMessage ="Bitte geben Sie Ihren Vornamen ein."), Display(Name ="Vorname")]
        public String FirstName { get; set; }

        [Required(ErrorMessage ="Bitte geben Sie Ihren Nachnamen ein."), Display(Name ="Nachname")]
        public String LastName { get; set; }
                
        [Required(ErrorMessage ="Bitte geben Sie eine E-Mail Adresse ein."), RegularExpression("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$", ErrorMessage ="Ihre E-Mail-Adresse ist nicht gültig!"),Display(Name ="E-Mail"), MaxLength(150, ErrorMessage ="Ihr E-Mail-Adresse ist zu lange!")]
        public String EMail { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage ="Ihre Telefonnummer darf nur aus Zifferen bestehen!"), Display(Name ="Telefonnummer"), MinLength(10,ErrorMessage ="Ihre Telefonnummer ist zu kurz!"), MaxLength(15, ErrorMessage ="Ihre Telefonnummer ist zu lange!")]
        public String PhoneNumber { get; set; }

        [Display(Name ="Beschreibung"), StringLength(500, ErrorMessage ="Ihre Beschreibung ist zu lange!")]
        public String Description { get; set; }

        [Required(ErrorMessage ="Bitte geben Sie Ihr Geburtstdatum ein."), Column(TypeName ="datetime2"), Display(Name ="Geburtsdatum")] //sonst SQL-Exception
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage ="Bitte geben Sie Ihre möglichen Nachhilfezeiten ein."), Display(Name ="Zeit"), MaxLength(150, ErrorMessage ="Sie haben zu viele Zeiten eingegeben")]
        public String Time { get; set; }

        [Required(ErrorMessage ="Bitte geben Sie Ihren gewünschten Stundensatz ein."), Display(Name ="Stundensatz in €"), Range(0,20,ErrorMessage ="Der Stundensatz muss zwischen 0 und 20 sein!")]
        public int Price { get; set; }

        [Display(Name="Abteilung"), ForeignKey("Department_Id")]
        public Department Department { get; set; }
        public int Department_Id { get; set; }

        [ForeignKey("Class_Id"), Display(Name ="Klasse")]
        public SchoolClass Class { get; set; }
        public int Class_Id { get; set; }
        public string IdentityName { get; set; }
        [Display(Name ="Geschlecht"), Required]
        public string Gender { get; set; }
        public bool Accepted { get; set; }
        //if a tutor is refreshed, the id can be inserted here
        public int OldTutorId { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<TeacherComment> Comments { get; set; }
    }
}
