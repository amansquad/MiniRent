using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services;
using MiniRent.Backend.Config;
using Xunit;

namespace MiniRent.Tests;

public class PropertyServiceTests
{
    private readonly IMapper _mapper;

    public PropertyServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreatePropertyAsync_ShouldCreateProperty()
    {
        // Arrange
        var context = GetDbContext();
        var service = new PropertyService(context, _mapper);
        var ownerId = Guid.NewGuid();
        var createDto = new PropertyCreateDto
        {
            Title = "Test Property",
            Address = "123 Test St",
            Area = 100,
            Bedrooms = 2,
            Bathrooms = 1,
            MonthlyRent = 1000,
            City = "Test City",
            State = "TS",
            ZipCode = "12345",
            Country = "TestLand",
            PropertyType = "Apartment"
        };

        // Act
        var result = await service.CreatePropertyAsync(createDto, ownerId);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createDto.Title);
        result.Address.Should().Be(createDto.Address);

        var propertyInDb = await context.Properties.FirstOrDefaultAsync(p => p.Id == result.Id);
        propertyInDb.Should().NotBeNull();
        propertyInDb!.CreatedById.Should().Be(ownerId);
    }

    [Fact]
    public async Task GetPropertiesAsync_ShouldFilterByStatus()
    {
        // Arrange
        var context = GetDbContext();
        var ownerId = Guid.NewGuid();
        context.Properties.AddRange(new List<Property>
        {
            new Property { Id = Guid.NewGuid(), Title = "Prop 1", Status = PropertyStatus.Available, CreatedById = ownerId, Address = "Addr 1", City = "C1", State = "S1", Country = "CN", ZipCode = "Z1", PropertyType = "T1" },
            new Property { Id = Guid.NewGuid(), Title = "Prop 2", Status = PropertyStatus.Rented, CreatedById = ownerId, Address = "Addr 2", City = "C2", State = "S2", Country = "CN", ZipCode = "Z2", PropertyType = "T2" }
        });
        await context.SaveChangesAsync();

        var service = new PropertyService(context, _mapper);
        var filter = new PropertyFilterDto { Status = "Available" };

        // Act
        var (properties, totalCount) = await service.GetPropertiesAsync(filter);

        // Assert
        totalCount.Should().Be(1);
        properties.Should().HaveCount(1);
        properties[0].Status.Should().Be("Available");
    }

    [Fact]
    public async Task GetPropertyByIdAsync_ShouldReturnProperty_WhenExists()
    {
        // Arrange
        var context = GetDbContext();
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var property = new Property 
        { 
            Id = propertyId, 
            Title = "Single Prop", 
            CreatedById = ownerId,
            Address = "Addr", City = "City", State = "State", Country = "Country", ZipCode = "Zip", PropertyType = "Type"
        };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var service = new PropertyService(context, _mapper);

        // Act
        var result = await service.GetPropertyByIdAsync(propertyId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(propertyId);
        result.Title.Should().Be("Single Prop");
    }
}
