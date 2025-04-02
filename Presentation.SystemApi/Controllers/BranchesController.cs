using Application.System.DTO;
using Application.System.UseCace.BranchUseCase.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchUseCase _branchUseCase;
        private readonly ILogger<BranchesController> _logger;

        public BranchesController(
            IBranchUseCase branchUseCase,
            ILogger<BranchesController> logger)
        {
            _branchUseCase = branchUseCase;
            _logger = logger;
        }

        // POST api/branches
        [HttpPost]
        public async Task<ActionResult<Response<BranchDTO>>> CreateBranch([FromBody] BranchDTO branchDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(branchDTO.Name))
                    return BadRequest(Response<BranchDTO>.Failure("Branch name is required"));

                if (string.IsNullOrEmpty(branchDTO.Address))
                    return BadRequest(Response<BranchDTO>.Failure("Address is required"));

                var result = await _branchUseCase.CreateBranchAsync(branchDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating branch");
                return StatusCode(500, Response<BranchDTO>.Failure("Internal server error", "500"));
            }
        }

        // GET api/branches
        [HttpGet]
        public async Task<ActionResult<Response<List<BranchDTO>>>> GetAllBranches()
        {
            try
            {
                var result = await _branchUseCase.GetAllBranchesAsync();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all branches");
                return StatusCode(500, Response<List<BranchDTO>>.Failure("Internal server error", "500"));
            }
        }

        // GET api/branches/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<BranchDTO>>> GetBranch(int id)
        {
            try
            {
                var result = await _branchUseCase.GetBranchByIdAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting branch {id}");
                return StatusCode(500, Response<BranchDTO>.Failure("Internal server error", "500"));
            }
        }

        // GET api/branches/{id}/name
        [HttpGet("{id}/name")]
        public async Task<ActionResult<Response<string>>> GetBranchName(int id)
        {
            try
            {
                var result = await _branchUseCase.GetBranchNameByIdAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting branch name for {id}");
                return StatusCode(500, Response<string>.Failure("Internal server error", "500"));
            }
        }

        // PUT api/branches/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<BranchDTO>>> UpdateBranch(int id, [FromBody] BranchDTO branchDTO)
        {
            try
            {
                if (id != branchDTO.Id_Branch)
                    return BadRequest(Response<BranchDTO>.Failure("ID mismatch between URL and body"));

                if (string.IsNullOrEmpty(branchDTO.Name))
                    return BadRequest(Response<BranchDTO>.Failure("Branch name is required"));

                if (string.IsNullOrEmpty(branchDTO.Address))
                    return BadRequest(Response<BranchDTO>.Failure("Address is required"));

                var result = await _branchUseCase.UpdateBranchAsync(branchDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating branch {id}");
                return StatusCode(500, Response<BranchDTO>.Failure("Internal server error", "500"));
            }
        }

        // DELETE api/branches/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteBranch(int id)
        {
            try
            {
                var result = await _branchUseCase.DeleteBranchAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting branch {id}");

                return StatusCode(500, Response<string>.Failure("Internal server error", "500"));
            }
        }
    }
}
