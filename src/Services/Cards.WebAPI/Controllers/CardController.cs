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
using Core.Management.Common.Projections;

using static Core.Domain.Common.Constants;

namespace Cards.WebAPI.Controllers;

[Route("v{version:apiVersion}/card")]
[Authorize(Policy = nameof(AuthPolicy.GlobalRights))]
public class CardController(IMapper mapper, ICardRepository cardRepository) : ControllerBase
{
    #region Card

    /// <summary>
    /// Create a card
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Returns "OK" on successful creation</response>
    /// <response code="400">Returns "BadRequest" on invalid model</response>
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpPost, Route("create")]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<CardDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateCard([FromBody, Required] CardRequest request)
    {
        Card card = await cardRepository.CreateCard(request.Name, request.Description, request.Color);
        return Created(string.Empty, new ResponseDto<CardDto> { Data = new[] { mapper.Map<CardDto>(card) } });
    }

    /// <summary>
    /// Get card given the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">Returns "OK" on successful update</response>
    /// <response code="400">Returns "BadRequest" on invalid model</response>  
    /// <response code="403">Returns "Forbidden" on denied card access</response>  
    /// <response code="404">Returns "NotFound" on card not found</response>
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpGet, Route("{id}")]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<CardDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCardById([FromRoute,
        Range(1, long.MaxValue, ErrorMessage = "id must be greater than 0"),
        Required(AllowEmptyStrings = false, ErrorMessage = "id must be provided")] string id)
    {
        Card? card = await cardRepository.GetCardById(long.Parse(id));
        return Ok(new ResponseDto<CardDto> { Data = card is null ? Enumerable.Empty<CardDto>() : new[] { mapper.Map<CardDto>(card) } });
    }

    /// <summary>
    /// Retrieves paginated users based on specified filter parameters 
    /// </summary>
    /// <param name="searchTerm">name, color, status, createdat</param>
    /// <param name="sortColumn">name, color, status, createdat</param>
    /// <param name="sortOrder">asc, desc</param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <response code="200">Returns "OK" on successful search</response>
    /// <response code="400">Returns "BadRequest" on invalid model</response>
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpGet, Route("search")]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<CardDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchCards([FromQuery]string? searchTerm, [FromQuery] string? sortColumn, [FromQuery] string? sortOrder, [FromQuery] int page, [FromQuery] int pageSize)
    {
        page = page > 0 ? page : DefaultPage; pageSize = pageSize > 0 ? pageSize : DefaultPageSize;

        PaginatedList<Card> paginatedList  = await cardRepository.SearchCards(searchTerm, sortColumn, sortOrder, page, pageSize);

        return Ok(new ResponseDto<CardDto>
        {
            Data = mapper.Map<List<CardDto>>(paginatedList.Items),
            Pagination = mapper.Map<Pagination>(paginatedList)           
        });
    }

    /// <summary>
    /// Update card details
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Returns "OK" on successful update</response>
    /// <response code="400">Returns "BadRequest" on invalid model</response>  
    /// <response code="403">Returns "Forbidden" on denied card access</response>  
    /// <response code="404">Returns "NotFound" on card not found</response>
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpPatch, Route("edit")]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<CardDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> EditUser([FromBody, Required] EditCardRequest request)
    {
        Card card = await cardRepository.EditCard(long.Parse(request.CardId), request.Name, request.Description, request.Color, request.CardStatusId);

        return Ok(new ResponseDto<CardDto> { Data = new[] { mapper.Map<CardDto>(card) } });
    }

    /// <summary>
    /// Delete card given the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">Returns "OK" on successful delete</response>
    /// <response code="400">Returns "BadRequest" on invalid model</response>  
    /// <response code="403">Returns "Forbidden" on denied card access</response>  
    /// <response code="404">Returns "NotFound" on card not found</response>
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpDelete, Route("{id}")]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<CardDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteCard([FromRoute,
        Range(1, long.MaxValue, ErrorMessage = "id must be greater than 0"),
        Required(AllowEmptyStrings = false, ErrorMessage = "id must be provided")] string id)
    {
        Card card = await cardRepository.DeleteCard(long.Parse(id));
        return Ok(new ResponseDto<CardDto> { Data = new[] { mapper.Map<CardDto>(card) } });
    }

    /// <summary>
    /// Retrieves all statuses a card can transition through 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Returns "OK" with a list of statuses</response>
    /// <response code="401">Returns "Unauthorized" when user is not authenticated</response> 
    [HttpGet, Route("status")]
    [Produces(MediaTypeNames.Application.Json), Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ResponseDto<CardStatusDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCardStatus()
    {
        return Ok(new ResponseDto<CardStatusDto>
        {
            Data = mapper.Map<List<CardStatusDto>>(await cardRepository.GetCardStatus())
        });
    }

    #endregion
}