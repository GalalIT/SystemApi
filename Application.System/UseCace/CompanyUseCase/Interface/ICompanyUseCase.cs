using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.CompanyUseCase.Interface
{
    public interface ICompanyUseCase
    {
        Task<Response<CompanyDTO>> CreateCompanyAsync(CompanyDTO companyDTO);
        Task<Response<List<CompanyDTO>>> GetAllCompaniesAsync();
        Task<Response> DeleteCompanyAsync(int id);
        Task<Response<CompanyDTO>> GetCompanyByIdAsync(int id);
        Task<Response<CompanyDTO>> UpdateCompanyAsync(CompanyDTO companyDTO);
    }
}
