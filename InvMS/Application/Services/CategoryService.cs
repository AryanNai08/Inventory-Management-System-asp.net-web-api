using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork) 
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByNameAsync(dto.Name);

            if (category != null)
            {
                throw new BadRequestException("Category name already exists");
            }

            var newcategory = _mapper.Map<Category>(dto);
            newcategory.CreatedDate=DateTime.UtcNow;
            await _categoryRepository.AddAsync(newcategory);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(newcategory);
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories=await _categoryRepository.GetAllAsync();

            if (categories.Count <= 0)
            {
                throw new NotFoundException("No Categories found!!");
            }

            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var category=await _categoryRepository.GetByIdAsync(id);

            if(category == null)
            {
                throw new NotFoundException($"Category with id:{id} not found");
            }

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                throw new NotFoundException($"Category with id:{id} not found");
            }

            // Check if any products are linked to this category
            if (await _productRepository.ExistsByCategoryIdAsync(id))
            {
                throw new BadRequestException("Cannot delete — products are linked to this category");
            }

            await _categoryRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                throw new NotFoundException($"Category with id:{id} not found");
            }

            if (!string.Equals(category.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name);
                if (existingCategory != null)
                {
                    throw new BadRequestException("Category name already exists");
                }
            }

            _mapper.Map(dto, category);
            category.ModifiedDate = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
