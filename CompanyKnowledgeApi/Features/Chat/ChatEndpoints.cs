using CompanyKnowledgeApi.Features.Chat.AskQuestion;
using CompanyKnowledgeApi.Features.Chat.DeleteChatSession;
using CompanyKnowledgeApi.Features.Chat.GetChatSession;
using CompanyKnowledgeApi.Features.Chat.ListChatSessions;

namespace CompanyKnowledgeApi.Features.Chat;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/chat")
            .WithTags("Chat");

        group.MapAskQuestionEndpoint();
        group.MapListChatSessionsEndpoint();
        group.MapGetChatSessionEndpoint();
        group.MapDeleteChatSessionEndpoint();

        return app;
    }
}
