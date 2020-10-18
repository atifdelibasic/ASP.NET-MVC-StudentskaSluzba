using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentskaSluzba.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Ime { get; set; }
        [Required]
        public string Prezime { get; set; }
        [Required]
        public DateTime? DatumRodjenja { get; set; }
        public string AdresaStanovanja { get; set; }
        public string JMBG { get; set; }
        public string Email { get; set; }

        [ForeignKey("GodinaStudija")]
        public int GodinaStudijaId { get; set; }
        [Display(Name ="Godina studija")]
        public virtual GodinaStudija GodinaStudija { get; set; }

        public int GradId { get; set; }
        [ForeignKey("GradId")]
        public virtual Grad Grad { get; set; }

        public virtual List<StudentiPredmeti> Uspjeh { get; set; } = new List<StudentiPredmeti>();
    }
}
