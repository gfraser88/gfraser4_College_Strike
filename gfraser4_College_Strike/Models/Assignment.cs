using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Models
{
    public class Assignment : Auditable
    {
        public Assignment()
        {
            this.Members = new HashSet<Member>();
        }

        public int ID { get; set; }

        [Display(Name = "Assignment")]
        [Required(ErrorMessage = "You cannot leave the Assignment Name blank.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Assignment Name must be between 5 and 200 characters long.")]
        public string AssignmentName { get; set; }

        public virtual ICollection<Member> Members { get; set; }

        public virtual ICollection<Shift> Shifts { get; set; }
    }
}
