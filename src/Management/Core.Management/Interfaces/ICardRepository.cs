using Core.Domain.Entities;
using Core.Management.Common.Projections;

namespace Core.Management.Interfaces;

public interface ICardRepository
{
    Task<Card> CreateCard(string name, string? description, string? color);        
    Task<Card> EditCard(long cardId, string? name, string? description, string? color, int? cardStatusId);
    Task<Card> GetCardById(long cardId);
    Task<Card> DeleteCard(long cardId);
    Task<PaginatedList<Card>> SearchCards(string? searchFilter, string? sortColumn, string? sortOrder, int page, int pageSize);
    Task<List<CardStatus>> GetCardStatus();
}