using Microsoft.AspNetCore.Mvc;
using TorayEMS.Data;
using TorayEMS.Models;

namespace TorayEMS.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentsApiController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAll()
        {
            return Ok(await _departmentRepository.GetAllAsync());
        }

        // GET: api/departments/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Department>> GetById(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return NotFound(new { message = $"Department dengan id {id} tidak ditemukan." });
            return Ok(department);
        }

        // GET: api/departments/summary  (jumlah karyawan per departemen)
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<Department>>> GetSummary()
        {
            return Ok(await _departmentRepository.GetEmployeeCountByDepartmentAsync());
        }
    }
}
