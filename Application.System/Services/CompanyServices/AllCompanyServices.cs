using Application.System.DTO;
using Application.System.Interface.ICompanyOparation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.CompanyServices
{
    public class AllCompanyServices : IAllCompanyOparation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllCompanyServices(IUnitOfRepository unitOfRepository)
        {
            _unitOfWork = unitOfRepository;
        }

        public async Task<Response<CompanyDTO>> CreateAsync(CompanyDTO companyDTO)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(companyDTO.Name))
                    return Response<CompanyDTO>.Failure("Company name is required", "400");

                if (companyDTO.DiscountRate < 0 || companyDTO.DiscountRate > 100)
                    return Response<CompanyDTO>.Failure("Discount rate must be between 0-100", "400");

                // Map DTO to entity
                var company = new Company
                {
                    Name = companyDTO.Name,
                    Description = companyDTO.Description,
                    FromDate = companyDTO.FromDate,
                    ToDate = companyDTO.ToDate,
                    DiscountRate = companyDTO.DiscountRate
                };

                await _unitOfWork._Company.AddAsync(company);

                // Update DTO with generated ID
                companyDTO.Id_Company = company.Id_Company;
                return Response<CompanyDTO>.Success(companyDTO, "Company created successfully");
            }
            catch (Exception ex)
            {
                return Response<CompanyDTO>.Failure($"Failed to create company: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var company = await _unitOfWork._Company.GetByIdAsync(id);
                if (company == null)
                    return Response.Failure("Company not found", "404");

                await _unitOfWork._Company.DeleteAsync(id);
                return Response.Success("Company deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete company: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<CompanyDTO>>> GetAllAsync()
        {
            try
            {
                var companies = await _unitOfWork._Company.GetAllAsync();
                var companyDTOs = companies.Select(MapToDTO).ToList();
                return Response<List<CompanyDTO>>.Success(companyDTOs, "All companies retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<CompanyDTO>>.Failure($"Failed to retrieve companies: {ex.Message}", "500");
            }
        }

        public async Task<Response<CompanyDTO>> GetByIdAsync(int id)
        {
            try
            {
                var company = await _unitOfWork._Company.GetByIdAsync(id);
                if (company == null)
                    return Response<CompanyDTO>.Failure("Company not found", "404");

                return Response<CompanyDTO>.Success(MapToDTO(company), "Company retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<CompanyDTO>.Failure($"Failed to retrieve company: {ex.Message}", "500");
            }
        }

        public async Task<Response<CompanyDTO>> UpdateAsync(CompanyDTO companyDTO)
        {
            try
            {
                var company = await _unitOfWork._Company.GetByIdAsync(companyDTO.Id_Company);
                if (company == null)
                    return Response<CompanyDTO>.Failure("Company not found", "404");

                // Validate required fields
                if (string.IsNullOrEmpty(companyDTO.Name))
                    return Response<CompanyDTO>.Failure("Company name is required", "400");

                if (companyDTO.DiscountRate < 0 || companyDTO.DiscountRate > 100)
                    return Response<CompanyDTO>.Failure("Discount rate must be between 0-100", "400");

                // Update properties
                company.Name = companyDTO.Name;
                company.Description = companyDTO.Description;
                company.FromDate = companyDTO.FromDate;
                company.ToDate = companyDTO.ToDate;
                company.DiscountRate = companyDTO.DiscountRate;

                await _unitOfWork._Company.UpdateAsync(company);
                return Response<CompanyDTO>.Success(MapToDTO(company), "Company updated successfully");
            }
            catch (Exception ex)
            {
                return Response<CompanyDTO>.Failure($"Failed to update company: {ex.Message}", "500");
            }
        }

        private CompanyDTO MapToDTO(Company company)
        {
            return new CompanyDTO
            {
                Id_Company = company.Id_Company,
                Name = company.Name,
                Description = company.Description,
                FromDate = company.FromDate,
                ToDate = company.ToDate,
                DiscountRate = company.DiscountRate
            };
        }
    }
}
