using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Nlayer.Core;
using Nlayer.Core.DTOs;
using Nlayer.Core.Repositories;
using Nlayer.Core.Services;
using Nlayer.Core.UnitOfWorks;
using NLayer.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Caching;

public class ProductServiceWithCaching : IProdutService
{
    private const string CacheProductKey = "productsCache";
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IPoductRepository _poductRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductServiceWithCaching(IMapper mapper, IMemoryCache memoryCache, IPoductRepository poductRepository, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _memoryCache = memoryCache;
        _poductRepository = poductRepository;
        _unitOfWork = unitOfWork;


        if (!_memoryCache.TryGetValue(CacheProductKey, out _))
        {
            _memoryCache.Set(CacheProductKey, _poductRepository.GetProductsWithCategory().Result);
        }
    }

    public async Task<Product> AddAsync(Product entity)
    {
        await _poductRepository.AddAsync(entity);
        await _unitOfWork.CommitAsync();
        await CacheAllProductsAsync();
        return entity;
    }

    public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
    {
        await _poductRepository.AddRangeAsync(entities);
        await _unitOfWork.CommitAsync();
        await CacheAllProductsAsync();
        return entities;
    }

    public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_memoryCache.Get<IEnumerable<Product>>(CacheProductKey));
    }

    public Task<Product> GetByIdAsync(int id)
    {
        var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);
        if(product == null)
            throw new NotFoundException($"{typeof(Product).Name}({id}) not found");

        return Task.FromResult(product);
            
    }

    public  Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory()
    {
        var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);
        var productsWithCategoryDto=_mapper.Map<List<ProductWithCategoryDto>>(products);
        return Task.FromResult(CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsWithCategoryDto));
    }

    public async Task RemoveAsync(Product entity)
    {
        _poductRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
        await CacheAllProductsAsync();
    }

    public async Task RemoveRangeAsync(IEnumerable<Product> entities)
    {
        _poductRepository.RemoveRange(entities);
        await _unitOfWork.CommitAsync();
        await CacheAllProductsAsync();
    }

    public async Task UpdateAsync(Product entity)
    {
        _poductRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        await CacheAllProductsAsync();
    }

    public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
    {
        return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
    }

    public async Task CacheAllProductsAsync()
    {
        _memoryCache.Set(CacheProductKey, await _poductRepository.GetAll().ToListAsync());
    }
}