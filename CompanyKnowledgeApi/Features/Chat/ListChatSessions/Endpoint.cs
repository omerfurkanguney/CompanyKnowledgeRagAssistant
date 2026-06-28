namespace CompanyKnowledgeApi.Features.Chat.ListChatSessions;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapListChatSessionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions", (
                string? period,
                ListChatSessionsQuery query,
                CancellationToken cancellationToken) =>
            query.Handle(new ListChatSessionsModel(ParsePeriod(period)), cancellationToken))
            .WithName("ListChatSessions")
            .WithSummary("Lists saved chat sessions.")
            .Produces<IReadOnlyList<ChatSessionSummaryResponse>>();

        return app;
    }

    private static ListChatSessionsPeriod ParsePeriod(string? period)
    {
        return period?.Trim().ToLowerInvariant() switch
        {
            "today" => ListChatSessionsPeriod.Today,
            "week" => ListChatSessionsPeriod.Week,
            _ => ListChatSessionsPeriod.All
        };
    }
}
