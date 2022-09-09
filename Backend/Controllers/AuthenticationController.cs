using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using System.Collections.Generic;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppUserService _service;
        public AuthenticationController(AppUserService service)
        {
            _service = service; 

                
        }
        [HttpGet]
        public ActionResult<List<AppUser>> Get()
        {
            return Ok( _service.Get());
            
        }
        [HttpGet("{Id:length(24)}")]
        public ActionResult<AppUser> Get(string Id)
        {
            return Ok( _service.Get(Id));
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public ActionResult<AppUser> SignUp(AppUser User)
        {
            var user=_service.Create(User);
            return user;

        }
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]

        public  ActionResult Login([FromBody]  AppUser user)
        {
            var token = _service.Authenticate(user.Email, user.PasswordHash);
            if (token == null)
                return Unauthorized("no user found");
            else
                return Ok(new { token, user });
        }
        [AllowAnonymous]
        [Route("validatetoken/{token}")]
        [HttpGet]
        public  ActionResult<bool> validateToken([FromRoute]string token)
        {
           var isValid=_service.ValidateToken(token);
            if (isValid)
                return Ok(token);
            else
                return Unauthorized("Token Expired or Incorrect");



        }
        

    } 
}
