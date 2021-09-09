using AddressMicroservice.Service.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressMicroservice.Service.Services.AddressService
{
    public interface IAddressService
    {
        Task<IEnumerable<Domain.Entities.Address>> GetAll();

        Task<IEnumerable<Domain.Entities.Address>> GetUserAddresses(Guid userId);

        Task AddAddress(AddAddressModel model);

        Task DeleteAddress(int addressId);

        Task<Domain.Entities.Address> GetById(int id);
    }
}