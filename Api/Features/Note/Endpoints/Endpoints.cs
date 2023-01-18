using SafeNote.Api.Constants;

namespace SafeNote.Api.Features.Note.Endpoints;

public static class Endpoints
{
    public static RouteGroupBuilder MapNoteEndpoints(this IEndpointRouteBuilder routes)
    {
        var noteRoutes = routes.MapGroup("/note");
        noteRoutes.WithTags("Note");
        noteRoutes.RequireRateLimiting(AppConstants.IpRateLimit);

        noteRoutes.MapGetEndpoints();
        noteRoutes.MapCreateOrGetEndpoints();
        noteRoutes.MapDeleteEndpoints();
        noteRoutes.MapSaveEndpoints();

        return noteRoutes;
    }
}