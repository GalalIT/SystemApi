using Domin.System.Entities;
using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.UserRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Test.IRepository
{
    public class IUserRepositoryTests
    {
        private readonly Mock<IAllUserRepository> _mockRepo;
        private readonly List<ApplicationUser> _testUsers;

        public IUserRepositoryTests()
        {
            // Initialize Arabic test data
            _testUsers = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "user1",
                Name = "محمد أحمد",
                Branch_Id = 1,
                UserName = "mohamed.ahmed@example.com",
                Email = "mohamed.ahmed@example.com"
            },
            new ApplicationUser
            {
                Id = "user2",
                Name = "أحمد خالد",
                Branch_Id = 2,
                UserName = "ahmed.khaled@example.com",
                Email = "ahmed.khaled@example.com"
            }
        };

            _mockRepo = new Mock<IAllUserRepository>();

            /* Repository Method */
            _mockRepo.Setup(r => r.GetUserBranchIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) =>
                {
                    if (string.IsNullOrEmpty(userId))
                        return 0;

                    return _testUsers
                        .Where(u => u.Id == userId)
                        .Select(u => u.Branch_Id)
                        .FirstOrDefault();
                });
        }

        /* Positive Tests */
        [Fact]
        public async Task GetUserBranchIdAsync_ValidUserId_ReturnsCorrectBranchId()
        {
            // Arrange
            var userId = "user1";

            // Act
            var result = await _mockRepo.Object.GetUserBranchIdAsync(userId);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetUserBranchIdAsync_DifferentUser_ReturnsCorrectBranchId()
        {
            // Arrange
            var userId = "user2";

            // Act
            var result = await _mockRepo.Object.GetUserBranchIdAsync(userId);

            // Assert
            Assert.Equal(2, result);
        }

        /* Negative Tests */
        [Fact]
        public async Task GetUserBranchIdAsync_InvalidUserId_ReturnsZero()
        {
            // Arrange
            var userId = "nonexistent_user";

            // Act
            var result = await _mockRepo.Object.GetUserBranchIdAsync(userId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetUserBranchIdAsync_EmptyUserId_ReturnsZero()
        {
            // Arrange
            var userId = "";

            // Act
            var result = await _mockRepo.Object.GetUserBranchIdAsync(userId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetUserBranchIdAsync_NullUserId_ReturnsZero()
        {
            // Act
            var result = await _mockRepo.Object.GetUserBranchIdAsync(null);

            // Assert
            Assert.Equal(0, result);
        }

        /* Edge Cases */
        [Fact]
        public async Task GetUserBranchIdAsync_UserWithDefaultBranchId_ReturnsZero()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "user3",
                Branch_Id = 0 // Invalid branch ID
            };
            _testUsers.Add(testUser);

            var userId = "user3";

            // Act
            var result = await _mockRepo.Object.GetUserBranchIdAsync(userId);

            // Assert
            Assert.Equal(0, result);
        }
    }

    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly AllUserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UsersTestDb")
                .Options;

            _context = new AppDbContext(options);
            _repository = new AllUserRepository(_context);

            // Seed Arabic test data
            _context.Users.Add(new ApplicationUser
            {
                Id = "user1",
                Name = "محمد أحمد",
                Branch_Id = 1
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetUserBranchIdAsync_ValidUser_ReturnsBranchId()
        {
            var result = await _repository.GetUserBranchIdAsync("user1");
            Assert.Equal(1, result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
