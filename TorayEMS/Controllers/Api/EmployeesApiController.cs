using Microsoft.AspNetCore.Mvc;
using TorayEMS.Data;
using TorayEMS.Models;

namespace TorayEMS.Controllers.Api
{
    /// <summary>
    /// REST API Controller untuk resource Employee. Mengembalikan JSON.
    /// Contoh endpoint:
    ///   GET    /api/employees
    ///   GET    /api/employees/5
    ///   POST   /api/employees
    ///   PUT    /api/employees/5
    ///   DELETE /api/employees/5
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesApiController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET: api/employees
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return Ok(employees);
        }

        // GET: api/employees/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = $"Employee dengan id {id} tidak ditemukan." });
            }
            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var newId = await _employeeRepository.CreateAsync(employee);
            employee.EmployeeId = newId;

            return CreatedAtAction(nameof(GetById), new { id = newId }, employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest(new { message = "Id pada route tidak sama dengan Id pada body." });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = await _employeeRepository.UpdateAsync(employee);
            if (!updated)
            {
                return NotFound(new { message = $"Employee dengan id {id} tidak ditemukan." });
            }

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _employeeRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Employee dengan id {id} tidak ditemukan." });
            }

            return NoContent();
        }
    }
}
