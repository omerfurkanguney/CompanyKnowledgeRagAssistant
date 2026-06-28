using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Chat.ListChatSessions;

public sealed class ListChatSessionsQuery(AppDbContext dbContext)
    : IQuery<IReadOnlyList<ChatSessionSummaryResponse>>, IScopedService
{
    public async Task<IReadOnlyList<ChatSessionSummaryResponse>> Handle(CancellationToken cancellationToken)
    {
        return await dbContext.ChatSessions
            .AsNoTracking()
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
