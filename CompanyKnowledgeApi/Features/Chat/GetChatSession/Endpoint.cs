namespace CompanyKnowledgeApi.Features.Chat.GetChatSession;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetChatSessionEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions/{id:guid}", (
                Guid id,
                GetChatSessionQuery query,
                CancellationToken cancellationToken) =>
            query.Handle(id, cancellationToken))
            .WithName("GetChatSession")
            .WithSummary("Gets a saved chat session with messages.")
            .Produces<ChatSessionDetailResponse>()
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
