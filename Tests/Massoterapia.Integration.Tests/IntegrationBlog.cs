using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Massoterapia.Application.Blog.Models;
using Massoterapia.Domain.Entities;
using Massoterapia.Domain.Tests;
using Massoterapia.Integration.Tests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;
using System.Reflection;

namespace Massoterapia.Integration.Tests
{
    public class IntegrationBlog
    {
        private HttpRequestMessage request;

        HttpClient client ;
        JsonSerializerSettings settings;

        private IntegrationBase _integrationBase;
        public IntegrationBlog()
        {
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://localhost:5001/api/v1/blog"),
            };           

            client = FactoryWebApplication.GetWebApplication(); 

            _integrationBase = new IntegrationBase(request,client);
        }

        public static BlogInputModel GetBlogInputModel()
        {
            var blogInputModel = new BlogInputModel
            {
                key = new Guid("620e0d10-e6fa-45e4-a30b-44570542c6d5"),
                Title = "title_update_" + DateTime.Now.ToShortDateString() + "__" + DateTime.Now.ToShortTimeString(),
                Text = "text",
                ImageCard = "imagecard",
                Active = true,
                Tags = "tags",
                TitleNFD = "titlenfd" 
            };            

            return blogInputModel;
        }        

        private int SearchAllResult(ConfiguredTaskAwaitable<HttpResponseMessage> response)
        {
            var responseInfo = response.GetAwaiter().GetResult();

            if (responseInfo.StatusCode == HttpStatusCode.OK)
            {
                var resultHttpRequest = responseInfo.Content.ReadAsStringAsync().Result;

                var BlogListResult = JsonConvert.DeserializeObject<IList<BlogViewModel>>(resultHttpRequest,settings);

                return BlogListResult.Count;
            }
            else
                return 0;
        } 

        private BlogViewModel Searchbykey(ConfiguredTaskAwaitable<HttpResponseMessage> response)
        {
            var responseInfo = response.GetAwaiter().GetResult();

            if (responseInfo.StatusCode == HttpStatusCode.OK)
            {
                var resultHttpRequest = responseInfo.Content.ReadAsStringAsync().Result;

                var BlogListResult = JsonConvert.DeserializeObject<BlogViewModel>(resultHttpRequest,settings);

                return BlogListResult;
            }
            else
                return null;
        }        


        [Theory]
        [InlineData("ca3aa919-c935-4c9f-b304-7d744dbe050e", System.Net.HttpStatusCode.NotFound)]
        [InlineData("ca3aa919-c935-4c9f-b304-7565656", System.Net.HttpStatusCode.BadRequest)]
        [InlineData("620e0d10-e6fa-45e4-a30b-44570542c6d5", System.Net.HttpStatusCode.OK)]
        
        public void Integration_get_blog_statusCode(string key, System.Net.HttpStatusCode StatusResult)
        {
   
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/getkey/{key}").ConfigureAwait(false);
 
            var responseInfo = response.GetAwaiter().GetResult();

            var result = responseInfo.Content.ReadAsStringAsync().Result;

            Assert.Equal(StatusResult,responseInfo.StatusCode);
            
        }          

        
        [Fact]       
        public void Integration_get_blog_searchall()
        {
   
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/search").ConfigureAwait(false);

            Assert.Equal(6, SearchAllResult(response) );   
        }  

        [Fact]       
        public void Integration_get_blog_searchall_true()
        {
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/search/true").ConfigureAwait(false);
            Assert.Equal(5, SearchAllResult(response) );
            
        } 

        [Fact]       
        public void Integration_get_blog_searchall_false()
        {
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/search/false").ConfigureAwait(false);
            Assert.Equal(1, SearchAllResult(response) );
        }        

        [Fact]       
        public void Integration_get_blog_searchbykey_object()
        {
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/getkey/620e0d10-e6fa-45e4-a30b-44570542c6d5").ConfigureAwait(false);
            Assert.Contains("title_update", Searchbykey(response).Title );
        }

        [Fact]       
        public void Integration_get_blog_searchbykey_notFound()
        {
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/getkey/ca3aa919-c935-4c9f-b304-7d744dbe050e").ConfigureAwait(false);
            Assert.Equal(null, Searchbykey(response));
        }    

        [Fact]       
        public void Integration_get_blog_searchtitlenfd_found()
        {
            var response = client.GetAsync($"{request.RequestUri.AbsolutePath}/searchtitlenfd/titlenfd-3").ConfigureAwait(false);
            Assert.Equal("titlenfd-3", Searchbykey(response).TitleNFD);
        }

        [Fact]       
        public void Integration_blog_update_ok()
        {
            BlogInputModel blogImputModel = GetBlogInputModel();

            var responseInfo = this._integrationBase.PutResult(blogImputModel);

            var resultado = responseInfo.Content.ReadAsStringAsync().Result;
            var patientUpdateResult = JsonConvert.DeserializeObject<long>(resultado,settings);

            Assert.Equal(1 , patientUpdateResult );
        }


        [Fact]       
        public void Integration_blog_update_fail()
        {
            BlogInputModel blogImputModel = GetBlogInputModel();
            blogImputModel.key = new Guid("ca3aa919-c935-4c9f-b304-7d744dbe050e");

            var responseInfo = this._integrationBase.PutResult(blogImputModel);

            var patientUpdateResult = JsonConvert.DeserializeObject<long>(responseInfo.Content.ReadAsStringAsync().Result,settings);

            Assert.Equal(0 , patientUpdateResult );
        }

        [Fact]       
        public void Integration_blog_create_ok()
        {
            BlogInputModel blogInputModel = GetBlogInputModel();
            blogInputModel.Title = "title create_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString();
            blogInputModel.key = Guid.Empty;

            var responseInfo = this._integrationBase.PostResult(blogInputModel);

            var resultado = responseInfo.Content.ReadAsStringAsync().Result;
            var blogCreateResult = JsonConvert.DeserializeObject<BlogViewModel>(resultado,settings);

            Assert.Contains("title create_" , blogCreateResult.Title );
        }


        [Theory]
        [InlineData("Title", "Título do Blog não pode ser vazio")]
        [InlineData("TitleNFD","Título para URL não pode ser vazio")]
        [InlineData("ImageCard","A imagem de capa do blog não pode ser vazia")]
        [InlineData("Tags","Tags do Blog não pode ser vazio")]
        [InlineData("Text","Texto do Blog não pode ser vazio")]
        public void Integration_blog_create_fail(string fieldEmpty, string messageError)
        {
            BlogInputModel blogInputModel = GetBlogInputModel();

            blogInputModel.GetType().GetProperty(fieldEmpty).SetValue(blogInputModel,"");           

            var responseInfo = this._integrationBase.PostResult(blogInputModel);

            var resultado = responseInfo.Content.ReadAsStringAsync().Result;

            Assert.Contains(messageError,resultado);
        }


    }
}