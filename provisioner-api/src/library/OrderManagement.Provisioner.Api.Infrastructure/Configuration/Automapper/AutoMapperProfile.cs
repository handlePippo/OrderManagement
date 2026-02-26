using AutoMapper;
using OrderManagement.Provisioner.Api.Domain.Entities;
using OrderManagement.Provisioner.Api.Infrastructure.Entities;

namespace OrderManagement.Provisioner.Api.Infrastructure.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureUserMappings();
            ConfigureAddressMappings();
        }

        private void ConfigureUserMappings()
        {
            CreateMap<User, UserEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => ConsiderOnlyValorizedProperty(srcMember)));

            CreateMap<UserEntity, User>();
        }

        private void ConfigureAddressMappings()
        {
            CreateMap<Address, AddressEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => ConsiderOnlyValorizedProperty(srcMember)));

            CreateMap<AddressEntity, Address>();
        }

        private static bool ConsiderOnlyValorizedProperty(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is not string str)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
