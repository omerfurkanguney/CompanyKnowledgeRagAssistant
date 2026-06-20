namespace CompanyKnowledgeApi.Common.Abstractions;

public interface ICommand<in TModel, TResponse>
{
    Task<TResponse> Handle(TModel model, CancellationToken cancellationToken);
}
