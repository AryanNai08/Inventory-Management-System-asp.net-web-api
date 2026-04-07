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

namespace Application.Services
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PrivilegeService(IPrivilegeRepository privilegeRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ReadPrivilegeDto>> GetAllPrivilegesAsync()
        {
            var privileges = await _privilegeRepository.GetAllPrivilegesAsync();
            return _mapper.Map<List<ReadPrivilegeDto>>(privileges);
        }

        public async Task<ReadPrivilegeDto> GetPrivilegeByIdAsync(int id)
        {
            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            return _mapper.Map<ReadPrivilegeDto>(privilege);
        }

        public async Task CreatePrivilegeAsync(PrivilegeDto dto)
        {
            if (await _privilegeRepository.PrivilegeExistsAsync(dto.Name))
                throw new BadRequestException("Privilege already exists");

            var privilege = new Privilege
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _privilegeRepository.CreatePrivilegeAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePrivilegeAsync(int id, PrivilegeDto dto)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            if (!string.Equals(privilege.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _privilegeRepository.PrivilegeExistsAsync(dto.Name))
                    throw new BadRequestException("Privilege name already exists!");
            }

            
            _mapper.Map(dto, privilege);

            await _privilegeRepository.UpdatePrivilegeAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePrivilegeAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Enter a valid id number!");

            var privilege = await _privilegeRepository.GetPrivilegeByIdAsync(id);
            if (privilege == null) throw new NotFoundException("Privilege not found");

            await _privilegeRepository.DeletePrivilegeAsync(privilege);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
