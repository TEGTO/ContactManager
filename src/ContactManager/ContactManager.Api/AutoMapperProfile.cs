using AutoMapper;
using ContactManager.Api.Data.Entities;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Update;

namespace ContactManager.Api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UpdateRequest, Contact>();
            CreateMap<FileContact, Contact>();

            CreateMap<Contact, ContactResponse>();
        }
    }
}