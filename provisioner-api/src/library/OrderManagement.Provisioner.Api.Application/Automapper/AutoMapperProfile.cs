using AutoMapper;
using OrderManagement.Provisioner.Api.Application.DTOs.Addresses;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.DTOs.Users;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;
using System.Text;

namespace OrderManagement.Provisioner.Api.Application.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureUserMapping();
            ConfigureAddressMapping();
            ConfigureTokenMapping();
        }

        private void ConfigureUserMapping()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<CreateUserDto, User>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore())
                .ForMember(m => m.PasswordHash, opt => opt.MapFrom(src => Convert.ToBase64String(Encoding.UTF8.GetBytes(src.Password))));

            CreateMap<UpdateUserDto, User>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore())
                .ForMember(m => m.PasswordHash, opt => opt.Ignore());
        }

        private void ConfigureAddressMapping()
        {
            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<CreateAddressDto, Address>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.UserId, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());

            CreateMap<UpdateAddressDto, Address>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.UserId, opt => opt.Ignore())
                .ForMember(m => m.CreatedAt, opt => opt.Ignore())
                .ForMember(m => m.ModifiedAt, opt => opt.Ignore());
        }

        private void ConfigureTokenMapping()
        {
            CreateMap<TokenRequestDto, TokenRequest>()
                .ForMember(m => m.Password, opt => opt.MapFrom(src => Convert.ToBase64String(Encoding.UTF8.GetBytes(src.Password))));
        }
    }
}