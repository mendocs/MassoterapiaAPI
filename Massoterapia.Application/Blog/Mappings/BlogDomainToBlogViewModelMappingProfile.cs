using AutoMapper;
using Massoterapia.Application.Blog.Models;

namespace Massoterapia.Application.Blog.Mappings
{
    public class BlogDomainToBlogViewModelMappingProfile: Profile
    {
        public BlogDomainToBlogViewModelMappingProfile()
        {   
                CreateMap<Domain.Entities.Blog,BlogViewModel>();
            
        }
    }
}