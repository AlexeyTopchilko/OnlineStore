using AddressMicroservice.Domain.Entities;
using AddressMicroservice.Repository.Repository;
using AddressMicroservice.Service.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressMicroservice.Service.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IRepositoryGeneric<Address> _addressRepository;

        public AddressService(IRepositoryGeneric<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }
        public async Task AddAddress(AddAddressModel model)
        {
            var address = new Address
            {
                UserId = model.UserId,
                City = model.City,
                HouseNumber = model.HouseNumber,
                Street = model.Street
            };

            await _addressRepository.CreateAsync(address);
        }

        public async Task<IEnumerable<Address>> GetAll()
        {
            var addresses = await _addressRepository.GetAllAsync();
            return addresses;
        }

        public async Task<IEnumerable<Address>> GetUserAddresses(Guid userId)
        {
            var addresses = await _addressRepository.GetByPredicate(_ => _.UserId == userId && _.DeletedDate == null);
            return addresses;
        }

        public async Task DeleteAddress(int addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            await _addressRepository.DeleteAsync(address);
        }

        public async Task<Address> GetById(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            return address;
        }
    }
}