using System;
using System.Collections.Generic;
using Massoterapia.Application.Blog.Interfaces;
using Massoterapia.Application.Blog.Models;
using Massoterapia.Application.Blog.Services;
using Massoterapia.Domain.Interfaces;
using Massoterapia.Infra.Data.Mongo.Repositories;
using Xunit;

namespace Massoterapia.Application.Tests
{
    public class BlogRepositoryTests
    {
        IBlogRepository blogRepository;
        IBlogService blogService;        

        public BlogRepositoryTests()
        {
            this.blogRepository = new  BlogRepository(RepositoryConfiguration.ConnFactory(), RepositoryConfiguration.DatabaseName, RepositoryConfiguration.CollectionBlog);

            this.blogService = new BlogService(configurations.FakeMapper(),this.blogRepository);
            
        }

        [Fact]
        public void RepositoryReal_ceate_blog_corrected()
        {
            BlogInputModel blogImputModel = configurations.GetBlogInputModel();

            blogImputModel.Title = "title_2";
            
            BlogViewModel blogViewModelSaved = this.blogService.Create(blogImputModel).Result;

            Assert.Equal("title_2", blogViewModelSaved.Title);
        }

        [Fact]
        public void RepositoryReal_ceate_blog_inactive_corrected()
        {
            BlogInputModel blogImputModel = configurations.GetBlogInputModel();

            blogImputModel.Active = false;
            blogImputModel.Title = "title_1";
            
            BlogViewModel blogViewModelSaved = this.blogService.Create(blogImputModel).Result;

            Assert.Equal("title", blogViewModelSaved.Title);
        }

        [Fact]
        public void RepositoryReal_update_blog_corrected()
        {
            BlogViewModel blogViewModelSaved = this.blogService.SearchByKey(new System.Guid("620e0d10-e6fa-45e4-a30b-44570542c6d5")).Result;

            BlogInputModel blogInputModel = new BlogInputModel()
            {
                Active = blogViewModelSaved.Active,
                key = blogViewModelSaved.Key,
                ImageCard = blogViewModelSaved.ImageCard,
                Tags = blogViewModelSaved.Tags,
                Text = blogViewModelSaved.Text,
                Title = blogViewModelSaved.Title + "_update1",
                TitleNFD = blogViewModelSaved.TitleNFD
            };
            
            long blogUpdateResult = this.blogService.Update(blogInputModel).Result;

            Assert.Equal(1, blogUpdateResult);
        }


        [Fact]
        public void RepositoryReal_search_blog_titlenfd_found()
        {   
            BlogViewModel blogViewModelSaved = this.blogService.SearchByTitleNFD("titlenfd").Result;

            Assert.Equal("title", blogViewModelSaved.Title);
        }

        [Fact]
        public void RepositoryReal_search_blog_titlenfd_not_found()
        {
            BlogViewModel blogViewModelSaved = this.blogService.SearchByTitleNFD("titlenfd_notFound").Result;

            Assert.Null(blogViewModelSaved);
        }         

        [Fact]
        public void RepositoryReal_search_blog_key_found()
        {   
            BlogViewModel blogViewModelSaved = this.blogService.SearchByKey(new System.Guid("620e0d10-e6fa-45e4-a30b-44570542c6d5")).Result;

            Assert.Equal("title", blogViewModelSaved.Title);
        }


        [Fact]
        public void RepositoryReal_search_blog_key_not_found()
        {   
            BlogViewModel blogViewModelSaved = this.blogService.SearchByKey(new System.Guid("ca3aa919-c935-4c9f-b304-7d744dbe050e")).Result;

            Assert.Null(blogViewModelSaved);
        }        


        [Fact]
        public void RepositoryReal_search_all_true()
        {   
            IList<BlogViewModel> blogViewModelList = this.blogService.SearchAll(true).Result;

            Assert.Equal(2,blogViewModelList.Count);
        }         

        [Fact]
        public void RepositoryReal_search_all_false()
        {   
            IList<BlogViewModel> blogViewModelList = this.blogService.SearchAll(false).Result;

            Assert.Equal(1,blogViewModelList.Count);
        } 

        [Fact]
        public void RepositoryReal_search_all()
        {   
            IList<BlogViewModel> blogViewModelList = this.blogService.SearchAll().Result;

            Assert.Equal(3,blogViewModelList.Count);
        } 

    }
}