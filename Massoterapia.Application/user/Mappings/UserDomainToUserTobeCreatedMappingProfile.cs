using System.Reflection.Metadata;
using AutoMapper;
using Massoterapia.Application.user.models;
using Massoterapia.Domain.Entities;

namespace Massoterapia.Application.user.Mappings
{
    public class UserDomainToUserTobeCreatedMappingProfile : Profile
    {
        public UserDomainToUserTobeCreatedMappingProfile()
        {   
                CreateMap<UserInputModel,User>();
            
        }        
    }
}