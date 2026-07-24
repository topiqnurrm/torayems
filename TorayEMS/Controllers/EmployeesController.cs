using Microsoft.AspNetCore.Mvc;
using TorayEMS.Data;
using TorayEMS.Models;

namespace TorayEMS.Controllers
{
    /// <summary>
    /// MVC Controller: menangani tampilan (Views) untuk CRUD data Employee.
    /// </summary>
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeesController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsDropdown();
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsDropdown(employee.DepartmentId);
                return View(employee);
            }

            await _employeeRepository.CreateAsync(employee);
            TempData["SuccessMessage"] = $"Karyawan '{employee.FullName}' berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();

            await PopulateDepartmentsDropdown(employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsDropdown(employee.DepartmentId);
                return View(employee);
            }

            var updated = await _employeeRepository.UpdateAsync(employee);
            if (!updated) return NotFound();

            TempData["SuccessMessage"] = $"Data karyawan '{employee.FullName}' berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Data karyawan berhasil dihapus.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDepartmentsDropdown(int? selectedId = null)
        {
            var departments = await _departmentRepository.GetAllAsync();
            ViewBag.DepartmentId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                departments, "DepartmentId", "DepartmentName", selectedId);
        }
    }
}
