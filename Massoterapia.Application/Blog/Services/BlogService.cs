using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Massoterapia.Application.Blog.Interfaces;
using Massoterapia.Application.Blog.Models;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Domain.Validations;

namespace Massoterapia.Application.Blog.Services
{
    public class BlogService : IBlogService
    {
        private IBlogRepository BlogRepository; 
        private readonly IMapper _mapper;

        public BlogService(IMapper mapper, IBlogRepository blogRepository )
        {
            this.BlogRepository = blogRepository;
            this._mapper = mapper;
        } 


        private void Validation(Domain.Entities.Blog blogTobeSaved )
        {
            BlogValidationContract blogValidationContract = new BlogValidationContract(blogTobeSaved);
            
            if ( !(blogValidationContract.IsValid) )
            {
                var mesagensError = blogValidationContract.Notifications.AllInvalidations();
                throw new System.ArgumentException(mesagensError);
            }

        }

        private Domain.Entities.Blog MapperInputViewToDomain(BlogInputModel blogInputModel)
        {
            return _mapper.Map<Domain.Entities.Blog> (blogInputModel);
        }

        public Task<BlogViewModel> Create(BlogInputModel blogInputModel)
        {
            Domain.Entities.Blog blogToBeSaved = MapperInputViewToDomain(blogInputModel);
            blogToBeSaved.SetItensConstructor();

            this.Validation(blogToBeSaved);

            Domain.Entities.Blog blogSaved = this.BlogRepository.Insert(blogToBeSaved).Result;

            BlogViewModel blogSavedViewModel = _mapper.Map<BlogViewModel> (blogSaved);

            return Task.FromResult(blogSavedViewModel);
        }

        public Task<long> Update(BlogInputModel blogInputModel)
        {
            Domain.Entities.Blog blogToBeUpdate = MapperInputViewToDomain(blogInputModel);
            this.Validation(blogToBeUpdate);
            return Task.FromResult(this.BlogRepository.Update(blogToBeUpdate).Result); 
        }

        public Task<IList<BlogViewModel>> SearchAll(bool active)
        {
            IQueryable<Domain.Entities.Blog> BlogFromDatabase = this.BlogRepository.QueryAll(active).Result;
            return ReturnSearchAll(BlogFromDatabase);

        }

        private Task<IList<BlogViewModel>> ReturnSearchAll(IQueryable<Domain.Entities.Blog> BlogFromDatabase)
        {
            if (BlogFromDatabase != null)
            {
                List<Domain.Entities.Blog> ListBlogs = BlogFromDatabase.ToList();
                IList<BlogViewModel> BlogFromDatabaseViewModel = _mapper.Map<List<BlogViewModel>>(ListBlogs);
                return Task.FromResult( BlogFromDatabaseViewModel);
            }
            else
                return Task.FromResult<IList<BlogViewModel>>(null);
        }


        public Task<IList<BlogViewModel>> SearchAll()
        {
            IQueryable<Domain.Entities.Blog> BlogFromDatabase = this.BlogRepository.QueryAll().Result;
            return ReturnSearchAll(BlogFromDatabase);
        }


        public Task<BlogViewModel> SearchByKey(Guid key)
        {
            Domain.Entities.Blog BlogFromDatabase =  null;

            try{
                BlogFromDatabase =  this.BlogRepository.Query(key).Result;
            }
            catch (Exception){}


            if (BlogFromDatabase != null)
            {
                BlogViewModel BlogFromDatabaseViewModel = _mapper.Map<BlogViewModel>(BlogFromDatabase);
                return Task.FromResult( BlogFromDatabaseViewModel);
            }
            else
                return Task.FromResult<BlogViewModel>(null);

        }

        public Task<BlogViewModel> SearchByTitleNFD(string TitleNFD)
        {
            Domain.Entities.Blog BlogFromDatabase = null; 

            try{
                BlogFromDatabase = this.BlogRepository.QueryByTitleNFD(TitleNFD).Result;
            }
            catch (Exception){}

            if (BlogFromDatabase != null)
            {
                BlogViewModel BlogFromDatabaseViewModel = _mapper.Map<BlogViewModel> (BlogFromDatabase);
                return Task.FromResult( BlogFromDatabaseViewModel);
            }
            else
                return Task.FromResult<BlogViewModel>( null);
        }
    }
}