namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapAskQuestionEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/ask", (
                AskQuestionModel model,
                AskQuestionCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(model, cancellationToken))
            .WithName("AskQuestion")
            .WithSummary("Answers a question using the most relevant document chunks.")
            .Produces<AskQuestionResponse>()
            .ProducesValidationProblem();

        return app;
    }
}
