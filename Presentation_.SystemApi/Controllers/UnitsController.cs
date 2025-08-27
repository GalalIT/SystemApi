using Application.System.DTO;
using Application.System.UseCace.UnitUseCase.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_.SystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitUseCase _unitUseCase;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(IUnitUseCase unitUseCase, ILogger<UnitsController> logger)
        {
            _unitUseCase = unitUseCase;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Response<UnitDTO>>> CreateUnit([FromBody] UnitDTO unitDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(unitDTO.Name))
                    return BadRequest(Response<UnitDTO>.Failure("Unit name is required", "400"));

                if (unitDTO.Branch_Id <= 0)
                    return BadRequest(Response<UnitDTO>.Failure("Branch ID is invalid", "400"));

                var result = await _unitUseCase.CreateUnitAsync(unitDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unit");
                return StatusCode(500, Response<UnitDTO>.Failure("Internal server error", "500"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteUnit(int id)
        {
            try
            {
                var result = await _unitUseCase.DeleteUnitAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting unit {id}");
                return StatusCode(500, Response<int>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Response<List<UnitDTO>>>> GetAllUnits()
        {
            try
            {
                var result = await _unitUseCase.GetAllUnitsAsync();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all units");
                return StatusCode(500, Response<List<UnitDTO>>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("with-branches")]
        public async Task<ActionResult<Response<List<UnitWithBranchNameDTO>>>> GetAllUnitsWithBranches()
        {
            try
            {
                var result = await _unitUseCase.GetAllUnitsIncludeToBranchAsync();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting units with branch details");
                return StatusCode(500, Response<List<UnitWithBranchNameDTO>>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<Response<List<UnitWithBranchNameDTO>>>> GetUnitsByBranch(int branchId)
        {
            try
            {
                if (branchId <= 0)
                    return BadRequest(Response<List<UnitWithBranchNameDTO>>.Failure("Invalid branch ID", "400"));

                var result = await _unitUseCase.GetAllUnitsByBranchAsync(branchId);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting units for branch {branchId}");
                return StatusCode(500, Response<List<UnitWithBranchNameDTO>>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<UnitDTO>>> GetUnitById(int id)
        {
            try
            {
                var result = await _unitUseCase.GetUnitByIdAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unit {id}");
                return StatusCode(500, Response<UnitDTO>.Failure("Internal server error", "500"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response<UnitDTO>>> UpdateUnit(int id, [FromBody] UnitDTO unitDTO)
        {
            try
            {
                if (id != unitDTO.Id_Unit)
                    return BadRequest(Response<UnitDTO>.Failure("ID mismatch", "400"));

                if (string.IsNullOrWhiteSpace(unitDTO.Name))
                    return BadRequest(Response<UnitDTO>.Failure("Unit name is required", "400"));

                if (unitDTO.Branch_Id <= 0)
                    return BadRequest(Response<UnitDTO>.Failure("Branch ID is invalid", "400"));

                var result = await _unitUseCase.UpdateUnitAsync(unitDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating unit {id}");
                return StatusCode(500, Response<UnitDTO>.Failure("Internal server error", "500"));
            }
        }
    }

}
