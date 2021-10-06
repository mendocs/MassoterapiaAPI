using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using SharedCore.Repositories;

namespace Massoterapia.Domain.Interfaces
{
    public interface IBlogRepository : IRepositoryWrite<Blog>
    {
        Task<Blog> QueryByTitleNFD(string titleNFD);

        Task<IQueryable<Blog>> QueryAll(bool active);
    }
}