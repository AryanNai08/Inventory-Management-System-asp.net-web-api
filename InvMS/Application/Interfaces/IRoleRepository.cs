using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRoleRepository
    {
        
            Task<Role> GetByIdAsync(int id);
        
    }
}
