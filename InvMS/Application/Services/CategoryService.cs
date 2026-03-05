using Application.DTOs;
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
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository,IMapper mapper) 
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByNameAsync(dto.Name);

            if (category != null)
            {
                throw new BadRequestException("Category name already exists");
            }

            var newcategory = _mapper.Map<Category>(dto);
            newcategory.CreatedDate=DateTime.Now;
            await _categoryRepository.AddAsync(newcategory);
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
            await _categoryRepository.SoftDeleteAsync(id);
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

            _mapper.Map(dto,category);
            category.ModifiedDate= DateTime.Now;

            await _categoryRepository.UpdateAsync(category);
            return true;
        }
    }
}
