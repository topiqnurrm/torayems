using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TorayEMS.Data;
using TorayEMS.Models;

namespace TorayEMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public HomeController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalEmployees = await _employeeRepository.GetTotalCountAsync(),
                TotalActiveEmployees = await _employeeRepository.GetActiveCountAsync(),
                TotalDepartments = (await _departmentRepository.GetAllAsync()).Count,
                EmployeeCountByDepartment = await _departmentRepository.GetEmployeeCountByDepartmentAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
