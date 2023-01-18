using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SafeNote.Api.Persistence;
using SafeNote.Api.Persistence.Entities;

namespace SafeNote.Api.Features.Note.Endpoints;

public static class Get
{
    public static IEndpointRouteBuilder MapGetEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/get/{id}/{hash}", GetNoteHandler);
        return routes;
    }

    private static async Task<Results<Ok<GetNoteResponse>, ValidationProblem>> GetNoteHandler(string id, string hash,
        AppDbContext dbContext, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(hash))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "errors", new[] { "Id and/or hash can not be empty." } }
            });
        }

        var note = await dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id && x.DataBundle.KeyHash == hash, ct);
        if (note is null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "errors", new[] { "No note found." } }
            });
        }

        return TypedResults.Ok(new GetNoteResponse(note.Id, note.DataBundle));
    }
}

public sealed record GetNoteResponse(string Id, DataBundle DataBundle);