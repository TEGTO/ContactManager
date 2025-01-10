using AutoMapper;
using ContractManager.Communication.Dtos;
using ContractManager.Communication.Dtos.Endpoints.Update;

namespace ContactManager.Client
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ContactResponse, UpdateRequest>();
        }
    }
}