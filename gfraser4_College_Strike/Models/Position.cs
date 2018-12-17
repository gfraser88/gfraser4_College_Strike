using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Models
{
    public class Position
    {

        public int ID { get; set; }

        [Display(Name = "Position Title")]
        [Required(ErrorMessage = "You cannot leave position title blank.")]
        [StringLength(50, ErrorMessage = "Title cannot more than 50 chacters long.")]
        public string Title { get; set; }

        public virtual ICollection<MemberPosition> MemberPositions { get; set; }
    }
}
