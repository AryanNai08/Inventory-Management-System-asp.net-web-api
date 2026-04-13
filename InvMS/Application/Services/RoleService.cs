using Application.DTOs.Auth;
using Application.DTOs.RolesAndPrivileges;
using Domain.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces;
using Application.Interfaces.RoleAndPrivileges;
using Domain.Interfaces.Auth;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePrivilegeRepository _rolePrivilegeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(
            IRoleRepository roleRepository, 
            IRolePrivilegeRepository rolePrivilegeRepository,
            IUserRepository userRepository,
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _rolePrivilegeRepository = rolePrivilegeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateRoleAsync(RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.Name))
                throw new BadRequestException("Please enter a role name!");

            if (await _roleRepository.RoleExistsAsync(roleDto.Name))
                throw new BadRequestException("Role name is already exists");

            var role = _mapper.Map<Role>(roleDto);
            
            await _roleRepository.CreateRoleAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();

            //var roleDtos = roles.Select(r => new RoleDto
            //{
            //    RoleName = r.Name,
            //    Description = r.Description,
            //}).ToList();

            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found!");

            var roleDto = _mapper.Map<RoleDto>(role);
           
            return roleDto;
        }

        public async Task UpdateRoleAsync(int id, UpdateRoleDto roleDto)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            if (string.IsNullOrWhiteSpace(roleDto.Name))
                throw new BadRequestException("Please enter a role name!");

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found!");

            _mapper.Map(roleDto, role);

            await _roleRepository.UpdateRoleAsync(role);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task DeleteRoleAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new NotFoundException("Role not found!");

            // 1. Check if role has privileges
            if (await _rolePrivilegeRepository.AnyPrivilegeInRoleAsync(id))
            {
                throw new BadRequestException("Cannot delete role: It still has assigned privileges.");
            }

            // 2. Check if role is assigned to users
            if (await _userRepository.AnyUserHasRoleAsync(id))
            {
                throw new BadRequestException("Cannot delete role: It is currently assigned to one or more users.");
            }

            await _roleRepository.DeleteRoleAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
