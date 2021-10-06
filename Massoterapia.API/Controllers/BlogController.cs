using System.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Massoterapia.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Massoterapia.Application.Blog.Interfaces;
using Massoterapia.Application.Blog.Models;
using Newtonsoft.Json;

namespace Massoterapia.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]    
    public class BlogController : ControllerBase_Core //ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;         
        private readonly IConfiguration _configuration;         


        public BlogController(IBlogService blogService, ILogger<BlogController> logger,  IConfiguration configuration)
        : base (logger)
        {
            _blogService = blogService;
            _logger = logger;
            _configuration = configuration;

        }  

        [HttpGet]
        [Route("getkey/{key}")]
        public IActionResult GetBlogByKey( Guid key)
        {
            Task tarefa = new Task<BlogViewModel>(() => _blogService.SearchByKey(key).Result);
            return ResultSearch(tarefa);
        }


        [HttpGet]
        [Route("search")]
        public IActionResult GetBlogsSearchAll()
        {
            Task tarefa = new Task<IList<BlogViewModel>>(() => _blogService.SearchAll().Result);
            return ResultSearch(tarefa);
        }

        [HttpGet]
        [Route("search/{active:bool}")]
        public IActionResult GetBlogsSearchAllActive(bool active)
        {
            Task tarefa = new Task<IList<BlogViewModel>>(() => _blogService.SearchAll(active).Result);
            return ResultSearch(tarefa); 
        }

        [HttpGet]
        [Route("searchtitlenfd/{titlenfd}")]
        public IActionResult GetBlogsSearchTitleNFD(string titleNFD)
        { 
            Task tarefa = new Task<BlogViewModel>(() => _blogService.SearchByTitleNFD(titleNFD).Result);
            return ResultSearch(tarefa); 
        }        


        [HttpPost]
        public IActionResult Create( [FromBody] BlogInputModel blogInputModel )
        {         
            try
            {
                Task tarefa = new Task<BlogViewModel>(() => _blogService.Create(blogInputModel).Result);
                            
                return ResultSearch(tarefa,true,"Create");
            }
            catch (ArgumentException exception)
            {
                return BadRequest($"{exception.Message}");
            }
        }

        [HttpPut]
        public IActionResult UpdateBlogAsync()
        {
            BlogInputModel blogFromBody = JsonConvert.DeserializeObject<BlogInputModel>(JsonFromBody());
            
            Task tarefa = new Task<long>(() => _blogService.Update(blogFromBody).Result);
            
            return ResultSearch(tarefa);            
        }

    }


    
}