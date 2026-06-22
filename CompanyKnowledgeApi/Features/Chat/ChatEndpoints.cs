using CompanyKnowledgeApi.Features.Chat.AskQuestion;

namespace CompanyKnowledgeApi.Features.Chat;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/chat")
            .WithTags("Chat");

        group.MapAskQuestionEndpoint();

        return app;
    }
}
