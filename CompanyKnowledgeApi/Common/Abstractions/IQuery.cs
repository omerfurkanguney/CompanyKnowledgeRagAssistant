namespace CompanyKnowledgeApi.Common.Abstractions;

public interface IQuery<TResponse>
{
    Task<TResponse> Handle(CancellationToken cancellationToken);
}

public interface IQuery<in TModel, TResponse>
{
    Task<TResponse> Handle(TModel model, CancellationToken cancellationToken);
}
