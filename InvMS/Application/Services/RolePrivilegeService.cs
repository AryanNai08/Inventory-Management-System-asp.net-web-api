using Application.DTOs.RolesAndPrivileges;
using Domain.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces;
using Application.Interfaces.RoleAndPrivileges;

namespace Application.Services
{
    public class RolePrivilegeService : IRolePrivilegeService
    {
        private readonly IRolePrivilegeRepository _rolePrivilegeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RolePrivilegeService(IRolePrivilegeRepository rolePrivilegeRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _rolePrivilegeRepository = rolePrivilegeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task AssignPrivilegeToRoleAsync(RolePrivilegeDto dto)
        {
            var role = await _rolePrivilegeRepository.GetRoleWithPrivilegesAsync(dto.RoleId);

            if (role == null)
                throw new NotFoundException("Role not found");

            var privilege = await _rolePrivilegeRepository.GetPrivilegeByIdAsync(dto.PrivilegeId);

            if (privilege == null)
                throw new NotFoundException("Privilege not found");

            if (role.Privileges.Any(p => p.Id == dto.PrivilegeId))
                throw new BadRequestException("Privilege already assigned to role");

            role.Privileges.Add(privilege);

            await _rolePrivilegeRepository.UpdateRoleAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemovePrivilegeFromRoleAsync(int roleId, int privilegeId)
        {
            var role = await _rolePrivilegeRepository.GetRoleWithPrivilegesAsync(roleId);

            if (role == null)
                throw new NotFoundException("Role not found");

            var privilege = role.Privileges.FirstOrDefault(p => p.Id == privilegeId);

            if (privilege == null)
                throw new NotFoundException("Privilege not assigned to role");

            role.Privileges.Remove(privilege);

            await _rolePrivilegeRepository.UpdateRoleAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ReadPrivilegeDto>> GetPrivilegesByRoleIdAsync(int roleId)
        {
            var role = await _rolePrivilegeRepository.GetRoleWithPrivilegesAsync(roleId);

            if (role == null)
                throw new NotFoundException("Role not found");

            return _mapper.Map<List<ReadPrivilegeDto>>(role.Privileges);
        }
    }
}
