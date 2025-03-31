using Application.System.DTO;
using Application.System.Interface.ICompanyOperation;
using Application.System.UseCace.CompanyUseCase.Interface;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.CompanyUseCase.Implement
{
    public class CompanyUseCase : ICompanyUseCase
    {
        private readonly IAllCompanyOperation _companyOperation;

        public CompanyUseCase(IAllCompanyOperation companyOperation)
        {
            _companyOperation = companyOperation;
        }

        public async Task<Response<CompanyDTO>> CreateCompanyAsync(CompanyDTO companyDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(companyDTO.Name))
                    return Response<CompanyDTO>.Failure("Company name is required", "400");

                if (companyDTO.DiscountRate < 0 || companyDTO.DiscountRate > 100)
                    return Response<CompanyDTO>.Failure("Discount rate must be between 0-100", "400");

                return await _companyOperation.CreateAsync(companyDTO);
            }
            catch (Exception ex)
            {
                return Response<CompanyDTO>.Failure($"Failed to create company: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<CompanyDTO>>> GetAllCompaniesAsync()
        {
            try
            {
                return await _companyOperation.GetAllAsync();
            }
            catch (Exception ex)
            {
                return Response<List<CompanyDTO>>.Failure($"Failed to retrieve companies: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteCompanyAsync(int id)
        {
            try
            {
                var company = await _companyOperation.GetByIdAsync(id);
                if (!company.Succeeded)
                    return Response.Failure("Company not found", "404");

                return await _companyOperation.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete company: {ex.Message}", "500");
            }
        }

        public async Task<Response<CompanyDTO>> GetCompanyByIdAsync(int id)
        {
            try
            {
                return await _companyOperation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                return Response<CompanyDTO>.Failure($"Failed to retrieve company: {ex.Message}", "500");
            }
        }

        public async Task<Response<CompanyDTO>> UpdateCompanyAsync(CompanyDTO companyDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(companyDTO.Name))
                    return Response<CompanyDTO>.Failure("Company name is required", "400");

                if (companyDTO.DiscountRate < 0 || companyDTO.DiscountRate > 100)
                    return Response<CompanyDTO>.Failure("Discount rate must be between 0-100", "400");

                return await _companyOperation.UpdateAsync(companyDTO);
            }
            catch (Exception ex)
            {
                return Response<CompanyDTO>.Failure($"Failed to update company: {ex.Message}", "500");
            }
        }
    }
}
