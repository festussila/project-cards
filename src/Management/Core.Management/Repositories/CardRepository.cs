using System.Net;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

using IdGen;

using Core.Domain.Enums;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Management.Interfaces;
using Core.Management.Common.Projections;
using Core.Domain.Infrastructure.Database;

using static Core.Domain.Common.Constants;
using static Core.Management.Repositories.HelperRepository;

namespace Core.Management.Repositories;

public class CardRepository : ICardRepository
{
    private readonly CardContext context;
    private readonly IIdGenerator<long> idGenerator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CardRepository(IIdGenerator<long> idGenerator, IHttpContextAccessor httpContextAccessor, CardContext context)
    {
        this.context = context;
        this.idGenerator = idGenerator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Card> CreateCard(string name, string? description, string? color)
    {
        ValidatedParameter(nameof(Card.Name), name, out name, throwException: true);
        name = name.ToUpper();

        ValidatedParameter(nameof(Card.Description), description, out description, throwException: false);

        ValidatedParameter(nameof(Card.Color), color, out color, throwException: false);
        color = color.ToUpper();

        IsValidHexColorCode(color, throwException: true);

        Card card = new()
        {
            CardId = idGenerator.CreateId(),
            Name = name,
            Description = description.Length < 1 ? null : description,
            Color = color.Length < 1 ? null : color,
            CardStatusId = (int)CardStates.ToDo
        };

        await context.Cards.AddAsync(card).ConfigureAwait(false);

        await context.SaveChangesAsync().ConfigureAwait(false);

        return card;
    }

    public async Task<Card> EditCard(long cardId, string? name, string? description, string? color, int? cardStatusId)
    {
        ValidatedParameter(nameof(Card.Name), name, out name, throwException: false);
        name = name.ToUpper();

        ValidatedParameter(nameof(Card.Description), description, out description, throwException: false);

        ValidatedParameter(nameof(Card.Color), color, out color, throwException: false);
        color = color.ToUpper();

        if (color.Length > 0)
        {
            IsValidHexColorCode(color, throwException: true);
        }

        cardStatusId?.IsDefinedEnum(typeof(CardStates), nameof(CardStatus));

        Card card = await GetCard(cardId).ConfigureAwait(false);

        if (!UserHasCardAccess(card.CreatedById))
            throw new GenericException("Cannot edit card if you are not the creator or admin", "CA006", HttpStatusCode.Forbidden);

        card.Name = name.Length > 0 ? name : card.Name;
        card.Description = description.Length > 0 ? description : null;
        card.Color = color.Length > 0 ? color : null;
        card.CardStatusId = cardStatusId ?? card.CardStatusId;

        await context.SaveChangesAsync().ConfigureAwait(false);

        return card;
    }

    public async Task<PaginatedList<Card>> SearchCards(string? searchFilter, string? sortColumn, string? sortOrder, int page, int pageSize)
    {
        ValidatedParameter("Search Filter", searchFilter, out searchFilter, throwException: false);
        ValidatedParameter("Sort Column", sortColumn, out sortColumn, throwException: false);
        ValidatedParameter("Sort Order", sortOrder, out sortOrder, throwException: false);

        IQueryable<Card> cardQuery = context.Cards.Include(x => x.CardStatus).Include(x => x.CreatedBy).AsQueryable();

        //searching by filters - name, color, status and date of creation
        if (searchFilter.Length > 0)
        {
            //reduce scanning by being more precise through validation of search filter
            if (IsValidHexColorCode(searchFilter))
            {
                cardQuery = cardQuery.Where(x => x.Color!.Contains(searchFilter));
            }
            else if (DateTime.TryParse(searchFilter, out DateTime createdAt))
            {
                cardQuery = cardQuery.Where(x => x.CreatedAt.Date == createdAt.Date);
            }
            else if (IsValidEnum(searchFilter.RemoveWhitespaces(), typeof(CardStates)))
            {
                int statusId = (int)Enum.Parse<CardStates>(searchFilter.RemoveWhitespaces(), true);
                cardQuery = cardQuery.Where(x => x.CardStatusId == statusId);
            }
            else
            {
                cardQuery = cardQuery.Where(x => x.Name.Contains(searchFilter));
            }
        }

        //get matrix of what user has access to
        (long grantedUserId, bool isAdmin) = CardAccessMatrix();

        if (!isAdmin)
        {
            cardQuery = cardQuery.Where(x => x.CreatedById == grantedUserId);
        }

        //sorting and ordering - name, color, status, date of creation
        if (sortOrder?.ToLower() == "desc")
        {
            cardQuery = cardQuery.OrderByDescending(GetSortProperty(sortColumn));
        }
        else
        {
            //handles when order is asc or not specified
            cardQuery = cardQuery.OrderBy(GetSortProperty(sortColumn));
        }

        //execute query and get projected results
        return await PaginatedList<Card>.GetAsync(cardQuery, page, pageSize).ConfigureAwait(false);
    }

    public async Task<Card> GetCardById(long cardId)
    {
        Card card = await GetCard(cardId).ConfigureAwait(false);

        if (!UserHasCardAccess(card.CreatedById))
            throw new GenericException("Cannot retrieve card if you are not the creator or admin", "CA008", HttpStatusCode.Forbidden);

        return card;
    }

    public async Task<Card> DeleteCard(long cardId)
    {
        Card card = await GetCard(cardId).ConfigureAwait(false);

        if (!UserHasCardAccess(card.CreatedById))
            throw new GenericException("Cannot delete card if you are not the creator or admin", "CA007", HttpStatusCode.Forbidden);

        context.Cards.Remove(card);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return card;
    }

    public async Task<List<CardStatus>> GetCardStatus() => await context.CardStatuses.ToListAsync().ConfigureAwait(false);


    #region Internal Helpers

    private async Task<Card> GetCard(long cardId)
    {
        return await context.Cards.Include(x => x.CardStatus).Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.CardId == cardId).ConfigureAwait(false)
            ?? throw new GenericException($"Card '{cardId}' could not be found", "CA005", HttpStatusCode.NotFound);
    }

    private bool UserHasCardAccess(long userId)
    {
        string? token = httpContextAccessor.HttpContext?.GetTokenAsync(AccessToken).Result;

        if (string.IsNullOrEmpty(token)) return false;

        JwtSecurityToken jwtToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);

        long subjectId = Convert.ToInt64(jwtToken.Claims.First(claim => claim.Type == ClaimsSub).Value);
        bool hasAdminRole = jwtToken.Claims.Where(claim => claim.Type == ClaimTypes.Role).Any(x => x.Value == nameof(Roles.Admin));

        return userId == subjectId || hasAdminRole;
    }

    private (long grantedUserId, bool isAdmin) CardAccessMatrix()
    {
        string? token = httpContextAccessor.HttpContext?.GetTokenAsync(AccessToken).Result;

        if (string.IsNullOrEmpty(token)) return (0, false);

        JwtSecurityToken jwtToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);

        long subjectId = Convert.ToInt64(jwtToken.Claims.First(claim => claim.Type == ClaimsSub).Value);
        bool hasAdminRole = jwtToken.Claims.Where(claim => claim.Type == ClaimTypes.Role).Any(x => x.Value == nameof(Roles.Admin));

        return (subjectId, hasAdminRole);
    }

    private static Expression<Func<Card, object>> GetSortProperty(string? sortColumn) =>
       sortColumn?.ToLower() switch
       {
           "name" => card => card.Name,
           "color" => card => card.Color!,
           "status" => card => card.CardStatusId,
           "createdat" => card => card.CreatedAt,
           _ => card => card.CardId
       };

    private static bool IsValidEnum(object? value, Type type)
    {
        Enum.TryParse(type, value?.ToString(), true, out value);

        return Enum.IsDefined(type, value ?? string.Empty);
    }

    #endregion

}