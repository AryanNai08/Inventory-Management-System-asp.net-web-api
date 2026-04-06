using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork) 
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            if (users.Count <= 0) 
            {
                throw new NotFoundException("Table is empty!!");
            }


            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            if(id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var user=await _userRepository.GetByIdAsync(id);

            if (user == null) 
            {
                throw new NotFoundException($"User with id:{id} not found");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }
            var userexist = await _userRepository.GetByIdAsync(id);
            if (userexist == null)
                throw new BadRequestException($"User with id:{id} not found");

            await _userRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
            
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
            if (id <= 0)
            {
                 throw new BadRequestException("Id must be greater than 0");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) 
            {
                throw new NotFoundException($"User with id:{id} not found");
            }

            //without any returntype
            _mapper.Map(dto, user);
            user.ModifiedDate=DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
