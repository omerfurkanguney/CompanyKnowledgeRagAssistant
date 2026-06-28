namespace CompanyKnowledgeApi.Features.Chat.DeleteChatSession;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapDeleteChatSessionEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/sessions/{id:guid}", (
                Guid id,
                DeleteChatSessionCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new DeleteChatSessionModel(id), cancellationToken))
            .WithName("DeleteChatSession")
            .WithSummary("Soft deletes a chat session.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
