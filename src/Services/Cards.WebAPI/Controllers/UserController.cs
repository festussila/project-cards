using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

using AutoMapper;

using Core.Domain.Entities;
using Cards.WebAPI.Models.Common;
using Core.Management.Interfaces;
using Cards.WebAPI.Models.DTOs.Requests;
using Cards.WebAPI.Models.DTOs.Responses;

namespace Cards.WebAPI.Controllers;

[Route("v{version:apiVersion}/user")]
[Authorize(Policy = nameof(AuthPolicy.GlobalRights))]
public class UserController(IUserRepository userRepository, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Used to sign in a registered user
    /// </summary>
    /// <param name="request"></param>  
    /// <returns></returns>
    /// <response code="200">Returns "OK" on successful sign in</response>
    /// <response code="400">Returns "BadRequest" on invalid model</response>  
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpPost, Route("sign-in"), AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<SignInDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignIn([FromBody, Required] SignInRequest request)
    {
        (string token, long expires, User user) = await userRepository.SignIn(request.Email, request.Password);

        return Ok(new ResponseDto<SignInDto> { Data = new[] { new SignInDto(token, expires, mapper.Map<UserDto>(user)) } });
    }
}