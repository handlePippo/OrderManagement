using AutoMapper;
using OrderManagement.Category.Api.Application.DTOs;
using OrderManagement.Category.Api.Application.Extensions;
using OrderManagement.Category.Api.Application.Interfaces;
using OrderManagement.Category.Api.Application.Repositories;
using OrderManagement.Category.Api.Domain.Pagination;

namespace OrderManagement.Category.Api.Application.Services
{
    /// <summary>
    /// Category service.
    /// </summary>
    public sealed class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _repository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="repository"></param>
        public CategoryService(IMapper mapper, ICategoryRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) => _repository.ExistsAsync(id, cancellationToken);

        public async Task<IReadOnlyList<CategoryDto>> ListAsync(ListRequestDto dto, CancellationToken cancellationToken)
        {
            var pagination = _mapper.Map<ListRequest>(dto);
            var categories = await _repository.ListAsync(pagination, cancellationToken);

            return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _repository.GetAsync(id, cancellationToken);
            if (category is null)
            {
                return null;
            }

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task CreateAsync(CreateCategoryDto entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var category = _mapper.Map<Domain.Entities.Category>(entity);

            await _repository.AddAsync(category, cancellationToken);
        }

        public async Task UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var category = new Domain.Entities.Category(id);
            category.ApplyPatchFrom(dto);

            await _repository.UpdateAsync(category, cancellationToken);
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken) => _repository.DeleteAsync(id, cancellationToken);
    }
}