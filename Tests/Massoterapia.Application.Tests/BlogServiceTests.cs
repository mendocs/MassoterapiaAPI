using System;
using System.Collections.Generic;
using AutoMapper;
using Massoterapia.Application.Blog.Interfaces;
using Massoterapia.Application.Blog.Models;
using Massoterapia.Application.Blog.Services;
using Xunit;

namespace Massoterapia.Application.Tests
{
    public class BlogServiceTests
    {

        IMapper _mapper;
        IBlogService blogService;
        public BlogServiceTests()
        {
            this._mapper = configurations.FakeMapper();

            this.blogService = new BlogService(this._mapper,configurations.FakeBlogRepository().Object);
        }

        [Fact]
        public void mapper_domain_to_view_corrected()
        {

            var blog = configurations.GetBlogDomainFake();

            BlogViewModel _BlogViewModel = _mapper.Map<BlogViewModel> (blog);

            
            Assert.Equal("title",_BlogViewModel.Title);

        }         

        [Fact]
        public void mapper_input_to_domain_corrected()
        {

            var blogInputModel = configurations.GetBlogInputModel();
            
            Domain.Entities.Blog _BlogDomain = _mapper.Map<Domain.Entities.Blog> (blogInputModel);

            Assert.Equal("title",_BlogDomain.Title);

        }     


        [Fact]
        public void create_blog_incorrected_no_titleNDF()
        {

            var blogInputModel = configurations.GetBlogInputModel();

            blogInputModel.TitleNFD="";
                       
            var ex = Assert.ThrowsAsync<ArgumentException> (()=> this.blogService.Create(blogInputModel));

            Assert.Contains("não pode ser vazio", ex.Result.Message );
        }      

        [Fact]
        public void create_blog_incorrected_no_text()
        {

            var blogInputModel = configurations.GetBlogInputModel();

            blogInputModel.Text ="";
                       
            var ex = Assert.ThrowsAsync<ArgumentException> (()=> this.blogService.Create(blogInputModel));

            Assert.Contains("não pode ser vazio", ex.Result.Message );
        }      

        [Fact]
        public void create_blog_corrected()
        {

            var blogInputModel = configurations.GetBlogInputModel();

            var blogviewmodel = this.blogService.Create(blogInputModel);

            Assert.Equal("title", blogviewmodel.Result.Title);
        }       

        [Fact]
        public void create_blog_update_corrected()
        {

            var blogInputModel = configurations.GetBlogInputModel();

            var blogviewmodel = this.blogService.Update(blogInputModel);

            Assert.Equal(1, blogviewmodel.Result);
        }       


        [Fact]
        public void create_blog_update_fail()
        {

            var blogInputModel = configurations.GetBlogInputModel();

            blogInputModel.Text ="";

            var ex = Assert.ThrowsAsync<ArgumentException> (()=> this.blogService.Create(blogInputModel));

            Assert.Contains("não pode ser vazio", ex.Result.Message );
        }

        [Fact]
        public void blog_SearchByKey_corrected()
        {
            var blogResult = this.blogService.SearchByKey(new Guid("ca3aa909-c935-4c9f-b304-7d744dbe050e")).Result;

            Assert.Equal("title", blogResult.Title );
        }                    

        [Fact]
        public void blog_SearchByKey_fail()
        {
            var blogResult = this.blogService.SearchByKey(new Guid()).Result;

            Assert.Null(blogResult);
        }

        [Fact]
        public void blog_SearchBytitleNFD_fail()
        {
            var blogResult = this.blogService.SearchByTitleNFD("title").Result;

            Assert.Null(blogResult);
        }

        [Fact]
        public void blog_SearchBytitleNFD_corrected()
        {
            var blogResult = this.blogService.SearchByTitleNFD("titlenfd").Result;

             Assert.Equal("title", blogResult.Title );
        }        

        [Fact]
        public void blog_SearchAll_true_corrected()
        {
            var blogResult = this.blogService.SearchAll(true).Result;

             Assert.Equal(3, blogResult.Count);
        }      

        [Fact]
        public void blog_SearchAll_false_corrected()
        {
            var blogResult = this.blogService.SearchAll(false).Result;

             Assert.Equal(2, blogResult.Count);
        }            

    }
}