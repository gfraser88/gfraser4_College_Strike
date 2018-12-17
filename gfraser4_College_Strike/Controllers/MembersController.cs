using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gfraser4_College_Strike.Data;
using gfraser4_College_Strike.Models;
using Microsoft.EntityFrameworkCore.Storage;
using gfraser4_College_Strike.ViewModels;
using System.Data;
using gfraser4_College_Strike.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Controllers
{   [Authorize]
    public class MembersController : Controller
    {
        
        private readonly CollegeStrikeContext _context;

        public MembersController(CollegeStrikeContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index(string SearchStringName, string SearchStringPhone, string SearchStringEmail, int? AssignmentID, int? PositionID, int? page, string SearchString, string sortDirection, string actionButton, string sortField = "Email")
        {

            ViewData["PositionID"] = new SelectList(_context.Positions
                .OrderBy(p => p.Title), "ID", "Title");

            PopulateDropDownLists();
            ViewData["Filtering"] = "";  //Assume not filtering

            var members = from m in _context.Members
                .Include(m => m.Assignment)
                .Include(m => m.MemberPositions)
                .ThenInclude(mp => mp.Position)
                          select m;

            //var CollegeStrikeContext = _context.Members
            //    .Include(p => p.Assignment)
            //    .Include(p => p.MemberPositions).ThenInclude(pc => pc.Position);

            if (AssignmentID.HasValue)
            {
                members = members.Where(p => p.AssignmentID == AssignmentID);
                ViewData["Filtering"] = " in";
            }

            if (!String.IsNullOrEmpty(SearchStringName))
            {
                members = members.Where(p => p.LastName.ToUpper().Contains(SearchStringName.ToUpper())
                                       || p.FirstName.ToUpper().Contains(SearchStringName.ToUpper()));
                ViewData["Filtering"] = " in";
            }

            if (!String.IsNullOrEmpty(SearchStringPhone))
            {
                members = members.Where(p => Convert.ToString(p.Phone).Contains(SearchStringPhone));
                ViewData["Filtering"] = " in";
            }
            if (!String.IsNullOrEmpty(SearchStringEmail))
            {
                members = members.Where(p => p.eMail.ToUpper().Contains(SearchStringEmail.ToUpper()));
                ViewData["Filtering"] = " in";
            }
            if (PositionID.HasValue)
            {
                members = members.Where(p => p.MemberPositions.Any(c => c.PositionID == PositionID));
                ViewData["Filtering"] = " in";
            }

            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
            {
                page = 1;//Reset back to first page when sorting or filtering

                if (actionButton != "Filter")//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = String.IsNullOrEmpty(sortDirection) ? "desc" : "";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            if (sortField == "Member")
            {
                if (String.IsNullOrEmpty(sortDirection))
                {
                    members = members
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
                else
                {
                    members = members
                        .OrderByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName);
                }
            }
            else if (sortField == "Phone")
            {
                if (String.IsNullOrEmpty(sortDirection))
                {
                    members = members
                        .OrderBy(p => p.Phone);
                }
                else
                {
                    members = members
                        .OrderByDescending(p => p.Phone);
                }
            }
            else if (sortField == "Assignment")
            {
                if (String.IsNullOrEmpty(sortDirection))
                {
                    members = members
                        .OrderBy(p => p.Assignment.AssignmentName);
                }
                else
                {
                    members = members
                        .OrderByDescending(p => p.Assignment.AssignmentName);
                }
            }
            else //Sorting by DOB - the default sort order
            {
                if (String.IsNullOrEmpty(sortDirection))
                {
                    members = members.OrderBy(p => p.eMail);
                }
                else
                {
                    members = members.OrderByDescending(p => p.eMail);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = 3;//Change as required
            var pagedData = await PaginatedList<Member>.CreateAsync(members.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Members/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Assignment)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        [Authorize(Roles = "Steward,Admin")]
        public IActionResult Create()
        {
            var member = new Member();
            member.MemberPositions = new List<MemberPosition>();
            PopulateAssignedPositionData(member);
            PopulateDropDownLists();
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Steward,Admin")]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,eMail,AssignmentID")] Member member, string[] selectedPositions, IFormFile thePicture)
        {
            try
            {
                if (selectedPositions != null)
                {
                    member.MemberPositions = new List<MemberPosition>();
                    foreach (var pos in selectedPositions)
                    {
                        var posToAdd = new MemberPosition { MemberID = member.ID, PositionID = int.Parse(pos) };
                        member.MemberPositions.Add(posToAdd);
                    }
                }

                if (ModelState.IsValid)
                {
                    if (thePicture != null)
                    {
                        string mimeType = thePicture.ContentType;
                        long fileLength = thePicture.Length;
                        if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                        {
                            if (mimeType.Contains("image"))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await thePicture.CopyToAsync(memoryStream);
                                    member.imageContent = memoryStream.ToArray();
                                }
                                member.imageMimeType = mimeType;
                                member.imageFileName = thePicture.FileName;
                            }
                        }
                    }
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DataException dex)
            {
                if (dex.InnerException.Message.Contains("IX_Members_eMail"))
                {
                    ModelState.AddModelError("eMail", "Unable to save changes. Remember, you cannot have duplicate Emails.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Unable to spit");
            }
            PopulateAssignedPositionData(member);
            PopulateDropDownLists(member);
            return View(member);
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "Admin,Steward")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(p => p.Assignment)
                .Include(p => p.MemberPositions).ThenInclude(p => p.Position)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.ID == id);

            if (member == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(member);
            PopulateAssignedPositionData(member);
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Steward,Admin")]
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion, string[] selectedPositions, string chkRemoveImage, IFormFile thePicture)
        {

            var memberToUpdate = await _context.Members
                .Include(p => p.Assignment)
                .Include(p => p.MemberPositions).ThenInclude(p => p.Position)
                .SingleOrDefaultAsync(p => p.ID == id);
            //Check that you got it or exit with a not found error
            if (memberToUpdate == null)
            {
                return NotFound();
            }

            UpdateMemberPositions(selectedPositions, memberToUpdate);
            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Member>(memberToUpdate, "",
                m => m.Phone, m => m.eMail, m => m.AssignmentID, m => m.FirstName, m => m.LastName))
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        memberToUpdate.imageContent = null;
                        memberToUpdate.imageMimeType = null;
                        memberToUpdate.imageFileName = null;
                    }
                    else
                    {
                        if (thePicture != null)
                        {
                            string mimeType = thePicture.ContentType;
                            long fileLength = thePicture.Length;
                            if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                            {
                                if (mimeType.Contains("image"))
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        await thePicture.CopyToAsync(memoryStream);
                                        memberToUpdate.imageContent = memoryStream.ToArray();
                                    }
                                    memberToUpdate.imageMimeType = mimeType;
                                    memberToUpdate.imageFileName = thePicture.FileName;
                                }
                            }
                        }
                    }
                    _context.Entry(memberToUpdate).Property("RowVersion").OriginalValue = RowVersion;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Member)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Member was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Member)databaseEntry.ToObject();
                        if (databaseValues.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Current value: "
                                + databaseValues.FirstName);
                        if (databaseValues.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Current value: "
                                + databaseValues.LastName);
                        if (databaseValues.Phone != clientValues.Phone)
                            ModelState.AddModelError("Phone", "Current value: "
                                + String.Format("{0:(###) ###-####}", databaseValues.Phone));
                        if (databaseValues.eMail != clientValues.eMail)
                            ModelState.AddModelError("eMail", "Current value: "
                                + databaseValues.eMail);
                        if (databaseValues.AssignmentID != clientValues.AssignmentID)
                        {
                            Assignment databaseAssignment = await _context.Assignments.SingleOrDefaultAsync(i => i.ID == databaseValues.AssignmentID);
                            ModelState.AddModelError("AssignmentID", $"Current value: {databaseAssignment?.AssignmentName}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                        memberToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.InnerException.Message.Contains("IX_Members_eMail"))
                    {
                        ModelState.AddModelError("eMail", "Unable to save changes. Remember, you cannot have duplicate Emails.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            //Validaiton Error so give the user another chance.
            PopulateDropDownLists(memberToUpdate);
            PopulateAssignedPositionData(memberToUpdate);
            return View(memberToUpdate);
        }

        // GET: Members/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Assignment)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropDownLists(Member member = null)
        {
            var aQuery = from a in _context.Assignments
                         orderby a.AssignmentName
                         select a;
            ViewData["AssignmentID"] = new SelectList(aQuery, "ID", "AssignmentName", member?.AssignmentID);
        }

        private void PopulateAssignedPositionData(Member member)
        {
            var allPositions = _context.Positions;
            var mPositions = new HashSet<int>(member.MemberPositions.Select(b => b.PositionID));
            var viewModel = new List<AssignedPositionVM>();
            foreach (var pos in allPositions)
            {
                viewModel.Add(new AssignedPositionVM
                {
                    PositionID = pos.ID,
                    PositionTitle = pos.Title,
                    Assigned = mPositions.Contains(pos.ID)
                });
            }
            ViewData["Positions"] = viewModel;
        }

        private void UpdateMemberPositions(string[] selectedPositions, Member memberToUpdate)
        {
            if (selectedPositions == null)
            {
                memberToUpdate.MemberPositions = new List<MemberPosition>();
                return;
            }

            var selectedPositionsHS = new HashSet<string>(selectedPositions);
            var memberPos = new HashSet<int>
                (memberToUpdate.MemberPositions.Select(p => p.PositionID));//IDs of the currently selected conditions
            foreach (var pos in _context.Positions)
            {
                if (selectedPositionsHS.Contains(pos.ID.ToString()))
                {
                    if (!memberPos.Contains(pos.ID))
                    {
                        memberToUpdate.MemberPositions.Add(new MemberPosition { MemberID = memberToUpdate.ID, PositionID = pos.ID });
                    }
                }
                else
                {
                    if (memberPos.Contains(pos.ID))
                    {
                        MemberPosition positionToRemove = memberToUpdate.MemberPositions.SingleOrDefault(c => c.PositionID == pos.ID);
                        _context.Remove(positionToRemove);
                    }
                }
            }
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }
    }
}
