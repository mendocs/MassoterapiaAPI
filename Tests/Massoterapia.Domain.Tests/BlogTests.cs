using System.IO;
using System.Net.Mime;
using System.Reflection;
using System;
using Xunit;
using Massoterapia.Domain.Entities;
using Newtonsoft.Json;
using JsonNet.ContractResolvers;
using Newtonsoft.Json.Linq;
using Massoterapia.Domain.Validations;

namespace Massoterapia.Domain.Tests
{
    public class BlogTests
    {
        [Fact]
        public void blog_contract_not_valid()
        {
            var blog = new Blog();

            BlogValidationContract blogValidationContract = new BlogValidationContract(blog);

            var msg = blogValidationContract.Notifications.AllInvalidations();

            Assert.False(blogValidationContract.IsValid);

        }

        [Fact]
        public void blog_contract_valid()
        {
            var blog = new Blog("title","titlenfd","imagecard","Tag","text");

            BlogValidationContract blogValidationContract = new BlogValidationContract(blog);

            var msg = blogValidationContract.Notifications.AllInvalidations();

            Assert.True(blogValidationContract.IsValid);

        }         
    }

    
}