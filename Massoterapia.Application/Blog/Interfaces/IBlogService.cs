using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Massoterapia.Application.Blog.Models;

namespace Massoterapia.Application.Blog.Interfaces
{
    public interface IBlogService
    {
         Task<BlogViewModel> Create(BlogInputModel blogInputModel);
         Task<long> Update(BlogInputModel blogInputModel);      

         Task<IList<BlogViewModel>> SearchAll(bool active);

         Task<IList<BlogViewModel>> SearchAll();

         Task<BlogViewModel> SearchByKey(Guid key);

         Task<BlogViewModel> SearchByTitleNFD (string TitleNFD);
    }
}