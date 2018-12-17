using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gfraser4_College_Strike.Models;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Data
{
    public static class CSSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new CollegeStrikeContext(
                serviceProvider.GetRequiredService<DbContextOptions<CollegeStrikeContext>>()))
            {
                // Look for any Members.  Since we can't have Members without Assignments.
                if (!context.Positions.Any())
                {
                    context.Positions.AddRange(
                     new Position
                     {
                         Title = "Negotiation"
                     },
                     new Position
                     {
                         Title = "Picket Captain"
                     },
                     new Position
                     {
                         Title = "Media Spokesperson"
                     }
                       );
                    context.SaveChanges();
                }
                if (!context.Assignments.Any())
                {
                    context.Assignments.AddRange(
                     new Assignment
                     {
                         AssignmentName = "Campus Gate 1"
                     },
                     new Assignment
                     {
                         AssignmentName = "Campus Gate 2"
                     },
                     new Assignment
                     {
                         AssignmentName = "Campus Gate 3"
                     },
                     new Assignment
                     {
                         AssignmentName = "Shuttle Driver"
                     },
                     new Assignment
                     {
                         AssignmentName = "Kitchen Help"
                     },
                     new Assignment
                     {
                         AssignmentName = "Sign Preparation"
                     }
                       );
                    context.SaveChanges();
                }
                if (!context.Members.Any())
                {
                    context.Members.AddRange(
                    new Member
                    {
                        FirstName = "Joe",
                        LastName = "Smith",
                        Phone = 9055551212,
                        eMail = "jsmith@outlook.com",
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Kitchen Help").ID
                    },
                    new Member
                    {
                        FirstName = "Bill",
                        LastName = "Johnson",
                        Phone = 9055551212,
                        eMail = "bjohnson@outlook.com",
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Campus Gate 2").ID
                    },
                    new Member
                    {
                        FirstName = "Susie",
                        LastName = "McDonald",
                        Phone = 9055551213,
                        eMail = "smcdonald@outlook.com",
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Shuttle Driver").ID
                    },
                    new Member
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        Phone = 9055551234,
                        eMail = "jdoe@outlook.com",
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Campus Gate 1").ID
                    });
                    context.SaveChanges();
                }
                if (!context.Shifts.Any())
                {
                    context.Shifts.AddRange(
                    new Shift
                    {
                        ShiftDate = DateTime.Parse("11/12/2018"),
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Kitchen Help").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Susie" && m.LastName == "McDonald").ID
                    },
                    new Shift
                    {
                        ShiftDate = DateTime.Parse("11/20/2018"),
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Sign Preparation").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Bill" && m.LastName == "Johnson").ID
                    },
                    new Shift
                    {
                        ShiftDate = DateTime.Parse("11/23/2018"),
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Campus Gate 1").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Jane" && m.LastName == "Doe").ID
                    },
                    new Shift
                    {
                        ShiftDate = DateTime.Parse("11/30/2018"),
                        AssignmentID = context.Assignments.FirstOrDefault(a => a.AssignmentName == "Campus Gate 1").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Bill" && m.LastName == "Johnson").ID
                    });
                    context.SaveChanges();
                }
                if (!context.MemberPositions.Any())
                {
                    context.MemberPositions.AddRange(
                    new MemberPosition
                    {
                        PositionID = context.Positions.FirstOrDefault(p => p.Title == "Media Spokesperson").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Susie" && m.LastName == "McDonald").ID
                    },
                    new MemberPosition
                    {
                        PositionID = context.Positions.FirstOrDefault(p => p.Title == "Media Spokesperson").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Joe" && m.LastName == "Smith").ID
                    },
                    new MemberPosition
                    {
                        PositionID = context.Positions.FirstOrDefault(p => p.Title == "Negotiation").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Bill" && m.LastName == "Johnson").ID
                    },
                    new MemberPosition
                    {
                        PositionID = context.Positions.FirstOrDefault(p => p.Title == "Picket Captain").ID,
                        MemberID = context.Members.FirstOrDefault(m => m.FirstName == "Jane" && m.LastName == "Doe").ID
                    });
                    context.SaveChanges();
                }

            }
        }
    }
}

