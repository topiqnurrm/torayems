using Microsoft.AspNetCore.Mvc;
using TorayEMS.Data;
using TorayEMS.Models;

namespace TorayEMS.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentsController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return View(departments);
        }

        // GET: Departments/Create
        public IActionResult Create() => View();

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (!ModelState.IsValid) return View(department);

            await _departmentRepository.CreateAsync(department);
            TempData["SuccessMessage"] = $"Departemen '{department.DepartmentName}' berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId) return BadRequest();
            if (!ModelState.IsValid) return View(department);

            var updated = await _departmentRepository.UpdateAsync(department);
            if (!updated) return NotFound();

            TempData["SuccessMessage"] = $"Departemen '{department.DepartmentName}' berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departmentRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Departemen berhasil dihapus.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Departemen tidak dapat dihapus karena masih memiliki data karyawan terkait.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
