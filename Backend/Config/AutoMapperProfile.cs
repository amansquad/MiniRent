using AutoMapper;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;

namespace MiniRent.Backend.Config;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // Property mappings
        CreateMap<Property, PropertyDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : ""))
            .ForMember(dest => dest.RecentRentals, opt => opt.MapFrom(src => src.RentalHistory.OrderByDescending(r => r.CreatedAt).Take(3)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Amenities))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
            
        CreateMap<PropertyCreateDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PropertyStatus.Available))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedById, opt => opt.Ignore());
            
        CreateMap<PropertyUpdateDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom((src, dest) => 
                !string.IsNullOrEmpty(src.Status) && Enum.TryParse<PropertyStatus>(src.Status, true, out var status) 
                ? status : dest.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Rental mappings
        CreateMap<RentalRecord, RentalRecordDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PropertyAddress, opt => opt.MapFrom(src => src.Property.Address))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : ""))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Property.CreatedById))
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.FullName : src.TenantName))
            .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));
            
        CreateMap<RentalCreateDto, RentalRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => RentalStatus.Active))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());
            
        CreateMap<RentalUpdateDto, RentalRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.StartDate, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityDeposit, opt => opt.Ignore())
            .ForMember(dest => dest.MonthlyRent, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());

        // Inquiry mappings
        CreateMap<RentalInquiry, RentalInquiryDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PropertyAddress, opt => opt.MapFrom(src => src.Property != null ? src.Property.Address : null))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : ""))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Property != null ? src.Property.CreatedById : null));
            
        CreateMap<InquiryCreateDto, RentalInquiry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InquiryStatus.New))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());
            
        CreateMap<InquiryUpdateDto, RentalInquiry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.Phone, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());

        // New mappings
        CreateMap<PropertyImage, PropertyImageDto>();
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // New mappings for expanding schema
        CreateMap<Amenity, AmenityDto>();
        CreateMap<AmenityCreateDto, Amenity>();
        
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer.FullName));
        CreateMap<ReviewCreateDto, Review>();
    }
}
