using AutoMapper;
using OrderManagement.Product.Api.Application.DTOs;
using OrderManagement.Product.Api.Application.Extensions;
using OrderManagement.Product.Api.Application.Interfaces;
using OrderManagement.Product.Api.Application.Repositories;

namespace OrderManagement.Product.Api.Application.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _repository;
        public ProductService(IMapper mapper, IProductRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken token) => await _repository.ExistsAsync(id, token);

        public async Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken token)
        {
            var products = await _repository.ListAsync(token);
            if (products is null)
            {
                return null!;
            }

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }

        public async Task<IReadOnlyList<ProductDto>> GetRangeAsync(GetProductRangeDto dto, CancellationToken token)
        {
            var range = _mapper.Map<Domain.Entities.GetProductRange>(dto);

            var products = await _repository.GetRangeAsync(range, token);
            if (products is null)
            {
                return Array.Empty<ProductDto>()!;
            }

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetAsync(int id, CancellationToken token)
        {
            var product = await _repository.GetAsync(id, token);
            if (product is null)
            {
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task CreateAsync(CreateProductDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var product = _mapper.Map<Domain.Entities.Product>(dto);

            await _repository.AddAsync(product, token);
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var product = await _repository.GetAsync(id, token)
                ?? throw new InvalidOperationException($"Product {id} not found.");

            product.ApplyPatchFrom(dto);

            await _repository.UpdateAsync(product, token);
        }

        public async Task DeleteAsync(int id, CancellationToken token)
        {
            await _repository.DeleteAsync(id, token);
        }
    }
}