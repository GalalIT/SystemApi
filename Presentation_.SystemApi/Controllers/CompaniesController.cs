using Application.System.DTO;
using Application.System.UseCace.CompanyUseCase.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyUseCase _companyUseCase;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            ICompanyUseCase companyUseCase,
            ILogger<CompaniesController> logger)
        {
            _companyUseCase = companyUseCase;
            _logger = logger;
        }

        // POST api/companies
        [HttpPost]
        public async Task<ActionResult<Response<CompanyDTO>>> CreateCompany([FromBody] CompanyDTO companyDTO)
        {
            try
            {
                var result = await _companyUseCase.CreateCompanyAsync(companyDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, Response<CompanyDTO>.Failure("Internal server error", "500"));
            }
        }

        // GET api/companies
        [HttpGet]
        public async Task<ActionResult<Response<List<CompanyDTO>>>> GetAllCompanies()
        {
            try
            {
                var result = await _companyUseCase.GetAllCompaniesAsync();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all companies");
                return StatusCode(500, Response<List<CompanyDTO>>.Failure("Internal server error", "500"));
            }
        }

        // GET api/companies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<CompanyDTO>>> GetCompany(int id)
        {
            try
            {
                var result = await _companyUseCase.GetCompanyByIdAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting company {id}");
                return StatusCode(500, Response<CompanyDTO>.Failure("Internal server error", "500"));
            }
        }

        // PUT api/companies/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<CompanyDTO>>> UpdateCompany(int id, [FromBody] CompanyDTO companyDTO)
        {
            try
            {
                if (id != companyDTO.Id_Company)
                {
                    return BadRequest(Response<CompanyDTO>.Failure("ID in URL and body must match", "400"));
                }

                var result = await _companyUseCase.UpdateCompanyAsync(companyDTO);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating company {id}");
                return StatusCode(500, Response<CompanyDTO>.Failure("Internal server error", "500"));
            }
        }

        // DELETE api/companies/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteCompany(int id)
        {
            try
            {
                var result = await _companyUseCase.DeleteCompanyAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting company {id}");
                return StatusCode(500, Response<string>.Failure("Internal server error", "500"));
            }
        }
    }

}
