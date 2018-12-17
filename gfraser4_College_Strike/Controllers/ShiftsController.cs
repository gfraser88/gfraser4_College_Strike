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
using Microsoft.AspNetCore.Authorization;
//Graeme Fraser
//Project Part 5
namespace gfraser4_College_Strike.Controllers
{
    [Authorize(Roles = "Steward,Admin")]
    public class ShiftsController : Controller
    {
        private readonly CollegeStrikeContext _context;

        public ShiftsController(CollegeStrikeContext context)
        {
            _context = context;
        }

        // GET: Shifts
        public async Task<IActionResult> Index()
        {
            ViewData["MemberID"] = new SelectList(_context.Members
                .OrderBy(c => c.FullName), "ID", "FullName");
            PopulateDropDownLists();
            ViewData["Filtering"] = "";

            ViewData["AssignmentID"] = new SelectList(_context.Assignments
                .OrderBy(c => c.AssignmentName), "ID", "AssignmentName");
            PopulateDropDownLists();
            ViewData["Filtering"] = "";

            var collegeStrikeContext = _context.Shifts.Include(s => s.Assignment).Include(s => s.Member);
            return View(await collegeStrikeContext.ToListAsync());
        }

        // GET: Shifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts
                .Include(s => s.Assignment)
                .Include(s => s.Member)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // GET: Shifts/Create
        public IActionResult Create()
        {
            var shift = new Shift();
            PopulateDropDownLists();
            return View();
        }

        // POST: Shifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ShiftDate,AssignmentID,MemberID")] Shift shift)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(shift);
            return View(shift);
        }

        // GET: Shifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(shift);
            return View(shift);
        }

        // POST: Shifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion)
        {
            //Go get the patient to update
            var shiftToUpdate = await _context.Shifts.SingleOrDefaultAsync(p => p.ID == id);
            //Check that you got it or exit with a not found error
            if (shiftToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Shift>(shiftToUpdate, "",
                s => s.ShiftDate, s => s.MemberID, s => s.AssignmentID))
            {
                try
                {
                    _context.Entry(shiftToUpdate).Property("RowVersion").OriginalValue = RowVersion;
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
                    var clientValues = (Shift)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Patient was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Shift)databaseEntry.ToObject();
                        if (databaseValues.ShiftDate != clientValues.ShiftDate)
                            ModelState.AddModelError("ShiftDate", "Current value: "
                                + databaseValues.ShiftDate);
                        if (databaseValues.AssignmentID != clientValues.AssignmentID)
                        {
                            Assignment databaseAssignment = await _context.Assignments.SingleOrDefaultAsync(i => i.ID == databaseValues.AssignmentID);
                            ModelState.AddModelError("AssignmentID", $"Current value: {databaseAssignment?.AssignmentName}");
                        }
                        if (databaseValues.MemberID != clientValues.MemberID)
                        {
                            Member databaseMember = await _context.Members.SingleOrDefaultAsync(i => i.ID == databaseValues.MemberID);
                            ModelState.AddModelError("MemberID", $"Current value: {databaseMember?.FullName}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                        shiftToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.InnerException.Message.Contains("IX_Shift_ShiftDate_MemberID"))
                    {
                        ModelState.AddModelError("ShiftDate", "Unable to save changes. Remember, you cannot a duplicate member on the same date.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            //Validaiton Error so give the user another chance.
            PopulateDropDownLists(shiftToUpdate);
            return View(shiftToUpdate);
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts
                .Include(s => s.Assignment)
                .Include(s => s.Member)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropDownLists(Shift shift = null)
        {
            var aQuery = from a in _context.Assignments
                         orderby a.AssignmentName
                         select a;
            var mQuery = from m in _context.Members
                         orderby m.FullName
                         select m;
            ViewData["AssignmentID"] = new SelectList(aQuery, "ID", "AssignmentName", shift?.AssignmentID);
            ViewData["MemberID"] = new SelectList(mQuery, "ID", "FullName", shift?.MemberID);
        }

        private bool ShiftExists(int id)
        {
            return _context.Shifts.Any(e => e.AssignmentID == id);
        }
    }
}
