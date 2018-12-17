using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Models
{
    public class Shift : Auditable
    {
        public int ID { get; set; }

        [Display(Name = "Shift Date")]
        [Required(ErrorMessage = "You cannot leave the Shift Date blank.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime ShiftDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must select an Assignment.")]
        [Display(Name = "Assignment")]
        public int AssignmentID { get; set; }

        public Assignment Assignment { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "You must select an Member.")]
        [Display(Name = "Member")]
        public int MemberID { get; set; }

        public Member Member { get; set; }

    }
}
