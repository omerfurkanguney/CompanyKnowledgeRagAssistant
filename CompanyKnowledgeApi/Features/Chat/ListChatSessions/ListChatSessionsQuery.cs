using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Chat.ListChatSessions;

public sealed class ListChatSessionsQuery(AppDbContext dbContext)
    : IQuery<ListChatSessionsModel, IReadOnlyList<ChatSessionSummaryResponse>>, IScopedService
{
    public async Task<IReadOnlyList<ChatSessionSummaryResponse>> Handle(
        ListChatSessionsModel model,
        CancellationToken cancellationToken)
    {
        var query = dbContext.ChatSessions
            .AsNoTracking()
            .Where(session => !session.IsDeleted)
            .AsQueryable();

        query = model.Period switch
        {
            ListChatSessionsPeriod.Today => query.Where(session => session.UpdatedAt >= DateTimeOffset.UtcNow.Date),
            ListChatSessionsPeriod.Week => query.Where(session => session.UpdatedAt >= DateTimeOffset.UtcNow.AddDays(-7)),
            _ => query
        };

        return await query
            .OrderByDescending(session => session.UpdatedAt)
            .Select(session => new ChatSessionSummaryResponse(
                session.Id,
                session.Title ?? "Yeni Sohbet",
                session.CreatedAt,
                session.UpdatedAt,
                session.Messages.Count))
            .ToListAsync(cancellationToken);
    }
}
