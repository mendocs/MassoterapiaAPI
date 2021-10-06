using AutoMapper;
using Massoterapia.Application.Blog.Models;

namespace Massoterapia.Application.Blog.Mappings
{
    public class BlogInputModelToBlogDomainMappingProfile : Profile
    {

        public BlogInputModelToBlogDomainMappingProfile()
        {   
                CreateMap<BlogInputModel,Domain.Entities.Blog>();
            
        }        
    }
}