namespace MediSearch.Presentation.WebApi.Shared.Endpoints;

internal interface IEndpoint
{
    string? RoutePrefix => null;

    string? Tag => null;

    void Map(RouteGroupBuilder groupBuilder);
}
