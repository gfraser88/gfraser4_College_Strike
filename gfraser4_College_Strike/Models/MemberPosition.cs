using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Models
{
    public class MemberPosition
    {
        public int PositionID { get; set; }
        public Position Position { get; set; }

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
