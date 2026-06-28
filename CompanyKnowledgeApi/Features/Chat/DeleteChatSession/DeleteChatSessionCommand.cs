using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Chat.DeleteChatSession;

public sealed class DeleteChatSessionCommand(AppDbContext dbContext)
    : ICommand<DeleteChatSessionModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(DeleteChatSessionModel model, CancellationToken cancellationToken)
    {
        var session = await dbContext.ChatSessions
            .FirstOrDefaultAsync(session => session.Id == model.Id && !session.IsDeleted, cancellationToken);

        if (session is null)
        {
            return Results.NotFound();
        }

        var now = DateTimeOffset.UtcNow;
        session.IsDeleted = true;
        session.DeletedAt = now;
        session.UpdatedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
