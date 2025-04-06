using Domin.System.Entities;
using Domin.System.IRepository.ICompanyRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using global::System.ComponentModel.DataAnnotations;
namespace Domin.System.Test.IRepository
{
    public class IAllCompanyRepositoryTests
    {
        private readonly Mock<IAllCompanyRepository> _mockRepo;
        private readonly List<Company> _testCompanies;

        public IAllCompanyRepositoryTests()
        {
            // Initialize test data with valid companies
            _testCompanies = new List<Company>
        {
            new Company {
                Id_Company = 1,
                Name = "شركة التقنية المحدودة",
                Description = "شركة متخصصة في حلول البرمجيات",
                FromDate = DateTime.Now.AddYears(-1),
                ToDate = DateTime.Now.AddYears(1),
                DiscountRate = 10
            },
            new Company {
                Id_Company = 2,
                Name = "شركة البرمجيات المتحدة",
                Description = "شركة رائدة في تطوير التطبيقات",
                FromDate = DateTime.Now.AddMonths(-6),
                ToDate = DateTime.Now.AddMonths(6),
                DiscountRate = 15
            }
        };

            _mockRepo = new Mock<IAllCompanyRepository>();

            /* Base Repository Methods */
            var mockDbSet = new Mock<DbSet<Company>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testCompanies.FirstOrDefault(c => c.Id_Company == id));

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testCompanies);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Company>()))
                .ReturnsAsync((Company c) =>
                {
                    // Manually trigger validation including IValidatableObject
                    var validationContext = new ValidationContext(c);
                    var validationResults = new List<ValidationResult>();

                    // This validates both DataAnnotations and IValidatableObject
                    Validator.TryValidateObject(c, validationContext, validationResults, true);

                    // Also call the Validate method explicitly
                    var customValidations = c.Validate(validationContext);
                    validationResults.AddRange(customValidations);

                    if (validationResults.Any())
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    _testCompanies.Add(c);
                    return c;
                });

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Company>()))
                .ReturnsAsync((Company c) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(c, new ValidationContext(c), validationResults, true))
                    {
                        throw new ValidationException("Validation failed");
                    }

                    var existing = _testCompanies.FirstOrDefault(x => x.Id_Company == c.Id_Company);
                    if (existing != null)
                    {
                        existing.Name = c.Name;
                        existing.Description = c.Description;
                        existing.FromDate = c.FromDate;
                        existing.ToDate = c.ToDate;
                        existing.DiscountRate = c.DiscountRate;
                    }
                    return existing;
                });

            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testCompanies.FirstOrDefault(c => c.Id_Company == id);
                    if (toDelete != null) _testCompanies.Remove(toDelete);
                    return toDelete;
                });

            _mockRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Company, bool>>>()))
                .ReturnsAsync((Expression<Func<Company, bool>> predicate) =>
                    _testCompanies.Any(predicate.Compile()));
        }

        /* Validation Tests */
        [Fact]
        public async Task AddAsync_InvalidCompany_ThrowsValidationException()
        {
            // Arrange - Invalid company (name too short, dates invalid)
            var invalidCompany = new Company
            {
                Name = "A",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddDays(-1), // Invalid (before FromDate)
                DiscountRate = 0 // Invalid (below range)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _mockRepo.Object.AddAsync(invalidCompany));
        }

        [Fact]
        public async Task UpdateAsync_InvalidDates_ThrowsValidationException()
        {
            // Arrange
            var invalidUpdate = new Company
            {
                Id_Company = 1,
                Name = "Valid Name",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddDays(-1), // Invalid
                DiscountRate = 20
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _mockRepo.Object.UpdateAsync(invalidUpdate));
        }

        /* Base Repository Tests */
        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsCompanyWithArabicName()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.Equal("شركة التقنية المحدودة", result.Name);
            Assert.Equal(10, result.DiscountRate);
        }

        [Fact]
        public async Task AddAsync_ValidCompany_IncludesAllProperties()
        {
            var newCompany = new Company
            {
                Name = "شركة جديدة",
                Description = "وصف الشركة الجديدة",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddYears(1),
                DiscountRate = 25
            };

            var result = await _mockRepo.Object.AddAsync(newCompany);

            Assert.Equal("شركة جديدة", result.Name);
            Assert.Equal("وصف الشركة الجديدة", result.Description);
            Assert.Equal(25, result.DiscountRate);
            Assert.Contains(newCompany, _testCompanies);
        }

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ChangesAllProperties()
        {
            var update = new Company
            {
                Id_Company = 1,
                Name = "اسم معدل",
                Description = "وصف معدل",
                FromDate = DateTime.Now.AddDays(1),
                ToDate = DateTime.Now.AddYears(2),
                DiscountRate = 30
            };

            var result = await _mockRepo.Object.UpdateAsync(update);

            Assert.Equal("اسم معدل", result.Name);
            Assert.Equal("وصف معدل", result.Description);
            Assert.Equal(30, result.DiscountRate);
            Assert.Equal(DateTime.Now.AddYears(2).Date, result.ToDate.Date);
        }

        /* Date Validation Tests */
        [Theory]
        [InlineData(0)]  // Equal dates - should fail
        [InlineData(-1)] // ToDate before FromDate - should fail
        public async Task AddAsync_InvalidDateRange_ThrowsException(int daysToAdd)
        {
            // Arrange
            var invalidCompany = new Company
            {
                Name = "شركة الاختبار",
                FromDate = DateTime.Now.Date, // Use Date to avoid time component issues
                ToDate = DateTime.Now.Date.AddDays(daysToAdd),
                DiscountRate = 10
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidCompany));

            Assert.Contains("يجب أن يكون تاريخ الانتهاء بعد تاريخ البداية", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ValidDates_ShouldPass()
        {
            // Arrange
            var validCompany = new Company
            {
                Name = "شركة صالحة",
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date.AddDays(1), // Valid (ToDate after FromDate)
                DiscountRate = 10
            };

            // Act
            var result = await _mockRepo.Object.AddAsync(validCompany);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("شركة صالحة", result.Name);
        }
        /* Discount Rate Validation */
        [Theory]
        [InlineData(0)]   // Below minimum
        [InlineData(51)]  // Above maximum
        public async Task AddAsync_InvalidDiscountRate_ThrowsException(int rate)
        {
            var invalidCompany = new Company
            {
                Name = "شركة الخصم",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddYears(1),
                DiscountRate = rate
            };

            await Assert.ThrowsAsync<ValidationException>(() => _mockRepo.Object.AddAsync(invalidCompany));
        }

        /* Arabic Field Validation */
        [Fact]
        public async Task AddAsync_ArabicName_Success()
        {
            var arabicCompany = new Company
            {
                Name = "شركة العربية",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddYears(1),
                DiscountRate = 5
            };

            var result = await _mockRepo.Object.AddAsync(arabicCompany);
            Assert.Contains(arabicCompany, _testCompanies);
        }

        [Fact]
        public void Db_Property_ReturnsDbSet()
        {
            var result = _mockRepo.Object.Db;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<DbSet<Company>>(result);
        }
    }
}
