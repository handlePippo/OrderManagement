using AutoMapper;
using OrderManagement.Category.Api.Infrastructure.Entities;

namespace OrderManagement.Category.Api.Infrastructure.Configuration.Automapper
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            ConfigureCategoryMapping();
        }

        private void ConfigureCategoryMapping()
        {
            CreateMap<Domain.Entities.Category, CategoryEntity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((_, _, srcMember) => ConsiderOnlyValorizedProperty(srcMember)));

            CreateMap<CategoryEntity, Domain.Entities.Category>();
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
