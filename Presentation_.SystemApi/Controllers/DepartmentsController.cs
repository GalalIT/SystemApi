using Application.System.DTO;
using Application.System.UseCace.DepartmentUseCase.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentUseCase _departmentUseCase;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(
            IDepartmentUseCase departmentUseCase,
            ILogger<DepartmentsController> logger)
        {
            _departmentUseCase = departmentUseCase;
            _logger = logger;
        }

        // POST api/departments
        [HttpPost]
        public async Task<ActionResult<Response<DepartmentDTO>>> CreateDepartment(
            [FromBody] DepartmentDTO departmentDTO)
        {
            try
            {
                var result = await _departmentUseCase.CreateDepartment(departmentDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, Response<DepartmentDTO>.Failure("Internal server error", "500"));
            }
        }

        // GET api/departments
        [HttpGet]
        public async Task<ActionResult<Response<List<DepartmentDTO>>>> GetAllDepartments()
        {
            try
            {
                var result = await _departmentUseCase.GetAllDepartments();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all departments");
                return StatusCode(500, Response<List<DepartmentDTO>>.Failure("Internal server error", "500"));
            }
        }

        // GET api/departments/with-branch-info
        [HttpGet("with-branch-info")]
        public async Task<ActionResult<Response<List<DepartmentDTO>>>> GetAllDepartmentsWithBranchInfo()
        {
            try
            {
                var result = await _departmentUseCase.GetAllDepartmentsWithBranchInfo();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting departments with branch info");
                return StatusCode(500, Response<List<DepartmentDTO>>.Failure("Internal server error", "500"));
            }
        }

        // GET api/departments/by-branch/{branchId}
        [HttpGet("by-branch/{branchId}")]
        public async Task<ActionResult<Response<List<DepartmentDTO>>>> GetDepartmentsByBranch(int branchId)
        {
            try
            {
                var result = await _departmentUseCase.GetAllDepartmentsByUserBranch(branchId);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting departments for branch {branchId}");
                return StatusCode(500, Response<List<DepartmentDTO>>.Failure("Internal server error", "500"));
            }
        }

        // GET api/departments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<DepartmentDTO>>> GetDepartment(int id)
        {
            try
            {
                var result = await _departmentUseCase.GetDepartment(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting department {id}");
                return StatusCode(500, Response<DepartmentDTO>.Failure("Internal server error", "500"));
            }
        }

        // PUT api/departments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<DepartmentDTO>>> UpdateDepartment(
            int id,
            [FromBody] DepartmentDTO departmentDTO)
        {
            try
            {
                if (id != departmentDTO.Id_Department)
                {
                    return BadRequest(Response<DepartmentDTO>.Failure("ID in URL and body must match", "400"));
                }

                var result = await _departmentUseCase.UpdateDepartment(departmentDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating department {id}");
                return StatusCode(500, Response<DepartmentDTO>.Failure("Internal server error", "500"));
            }
        }

        // DELETE api/departments/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteDepartment(int id)
        {
            try
            {
                var result = await _departmentUseCase.DeleteDepartment(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting department {id}");
                return StatusCode(500, Response<string>.Failure("Internal server error", "500"));
            }
        }
    }

}
