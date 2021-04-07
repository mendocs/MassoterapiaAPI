using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Massoterapia.Application.user.Interfaces;
using Massoterapia.Application.user.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Massoterapia.API.Controllers
{



    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;         
        private readonly IConfiguration _configuration;

        private CryptographSettings cryptographSettings => GetCryptographSettings();

        public UserController(IUserService userService, ILogger<UserController> logger,  IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _configuration = configuration;

        }     

        private CryptographSettings GetCryptographSettings() => _configuration.GetSection("CryptographyPassword").Get<CryptographSettings>();
        

        [HttpPost]
        public ActionResult<Boolean> Authenticate( [FromBody] UserInputModel userImputModel )
        {
            try
            {
                userImputModel.Interation = cryptographSettings.Interation;
                userImputModel.NumberSalt = cryptographSettings.NumberSalt;
                userImputModel.Nhash = cryptographSettings.Nhash;

                var result = _userService.Authenticate(userImputModel);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message, userImputModel);
                return new StatusCodeResult(500);
            }
        }        

    }
}