namespace CompanyKnowledgeApi.Features.Chat.ListChatSessions;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapListChatSessionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions", (
                ListChatSessionsQuery query,
                CancellationToken cancellationToken) =>
            query.Handle(cancellationToken))
            .WithName("ListChatSessions")
            .WithSummary("Lists saved chat sessions.")
            .Produces<IReadOnlyList<ChatSessionSummaryResponse>>();

        return app;
    }
}
