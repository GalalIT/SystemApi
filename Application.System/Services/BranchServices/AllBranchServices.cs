using Application.System.DTO;
using Application.System.Interface.IBranchOperation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.BranchServices
{
    public class AllBranchServices : IAllBranchOperation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllBranchServices(IUnitOfRepository unitOfRepository)
        {
            _unitOfWork = unitOfRepository;
        }

        public async Task<Response<BranchDTO>> CreateAsync(BranchDTO branchDTO)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(branchDTO.Name))
                    return Response<BranchDTO>.Failure("Branch name is required", "400");

                if (string.IsNullOrEmpty(branchDTO.Address))
                    return Response<BranchDTO>.Failure("Address is required", "400");

                // Map DTO to entity
                var branch = new Branch
                {
                    Name = branchDTO.Name,
                    Address = branchDTO.Address,
                    City = branchDTO.City,
                    Phone = branchDTO.Phone,
                    IsActive = branchDTO.IsActive ?? true
                };

                await _unitOfWork._Branch.AddAsync(branch);

                // Update DTO with generated ID
                branchDTO.Id_Branch = branch.Id_Branch;
                return Response<BranchDTO>.Success(branchDTO, "Branch created successfully");
            }
            catch (Exception ex)
            {
                return Response<BranchDTO>.Failure($"Failed to create branch: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork._Branch.GetByIdAsync(id);
                if (branch == null)
                    return Response.Failure("Branch not found", "404");

                await _unitOfWork._Branch.DeleteAsync(id);
                return Response.Success("Branch deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete branch: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<BranchDTO>>> GetAllAsync()
        {
            try
            {
                var branches = await _unitOfWork._Branch.GetAllAsync();
                var branchDTOs = branches.Select(MapToDTO).ToList();
                return Response<List<BranchDTO>>.Success(branchDTOs, "All branches retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<BranchDTO>>.Failure($"Failed to retrieve branches: {ex.Message}", "500");
            }
        }

        public async Task<string> GetBranchNameById(int branchId)
        {
            try
            {
                return await _unitOfWork._Branch.GetBranchNameById(branchId) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        public async Task<Response<BranchDTO>> GetByIdAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork._Branch.GetByIdAsync(id);
                if (branch == null)
                    return Response<BranchDTO>.Failure("Branch not found", "404");

                return Response<BranchDTO>.Success(MapToDTO(branch), "Branch retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<BranchDTO>.Failure($"Failed to retrieve branch: {ex.Message}", "500");
            }
        }

        public async Task<Response<BranchDTO>> UpdateAsync(BranchDTO branchDTO)
        {
            try
            {
                var branch = await _unitOfWork._Branch.GetByIdAsync(branchDTO.Id_Branch);
                if (branch == null)
                    return Response<BranchDTO>.Failure("Branch not found", "404");

                // Update properties
                branch.Name = branchDTO.Name;
                branch.Address = branchDTO.Address;
                branch.City = branchDTO.City;
                branch.Phone = branchDTO.Phone;
                branch.IsActive = branchDTO.IsActive ?? branch.IsActive;

                await _unitOfWork._Branch.UpdateAsync(branch);
                return Response<BranchDTO>.Success(MapToDTO(branch), "Branch updated successfully");
            }
            catch (Exception ex)
            {
                return Response<BranchDTO>.Failure($"Failed to update branch: {ex.Message}", "500");
            }
        }

        private BranchDTO MapToDTO(Branch branch)
        {
            return new BranchDTO
            {
                Id_Branch = branch.Id_Branch,
                Name = branch.Name,
                Address = branch.Address,
                City = branch.City,
                Phone = branch.Phone,
                IsActive = branch.IsActive
            };
        }
    }

}
