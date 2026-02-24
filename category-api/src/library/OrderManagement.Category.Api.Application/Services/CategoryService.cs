using AutoMapper;
using OrderManagement.Category.Api.Application.DTOs;
using OrderManagement.Category.Api.Application.Extensions;
using OrderManagement.Category.Api.Application.Interfaces;
using OrderManagement.Category.Api.Application.Repositories;

namespace OrderManagement.Category.Api.Application.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _repository;
        public CategoryService(IMapper mapper, ICategoryRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken cancellationToken)
        {
            var categories = await _repository.ListAsync(cancellationToken);
            if (categories is null)
            {
                return null!;
            }

            return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _repository.GetAsync(id, cancellationToken);
            if (category is null)
            {
                return null;
            }

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task AddAsync(CreateCategoryDto entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var category = _mapper.Map<Domain.Entities.Category>(entity);

            await _repository.AddAsync(category, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var category = new Domain.Entities.Category(id);
            category.ApplyPatchFrom(dto);

            await _repository.UpdateAsync(category, cancellationToken);
        }
    }
}