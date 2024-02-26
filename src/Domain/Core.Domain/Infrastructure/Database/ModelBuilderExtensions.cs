using Microsoft.EntityFrameworkCore;

using Core.Domain.Enums;
using Core.Domain.Entities;

namespace Core.Domain.Infrastructure.Database;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        //Seed the time to a constant to avoid fresh migrations in every migration event
        DateTime seedTime = new(2024, 02, 24, 16, 45, 10, 100, DateTimeKind.Unspecified);


        #region Card Statuses

        modelBuilder.Entity<CardStatus>().HasData(
            new CardStatus { CardStatusId = (int)CardStates.ToDo, Status = "To Do", CreatedAt = seedTime, ModifiedAt = seedTime },
            new CardStatus { CardStatusId = (int)CardStates.InProgress, Status = "In Progress", CreatedAt = seedTime, ModifiedAt = seedTime },
            new CardStatus { CardStatusId = (int)CardStates.Done, Status = "Done", CreatedAt = seedTime, ModifiedAt = seedTime }          
        );

        #endregion
    }
}