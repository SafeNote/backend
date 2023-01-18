using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SafeNote.Api.Persistence;

namespace SafeNote.Api.Features.Note.Endpoints;

public static class Delete
{
    public static IEndpointRouteBuilder MapDeleteEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapDelete("/delete/{id}/{hash}", DeleteNoteHandler);
        return routes;
    }

    private static async Task<Results<Ok<DeleteNoteResponse>, ValidationProblem>> DeleteNoteHandler(
        string id, string hash, AppDbContext dbContext, CancellationToken ct)
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

        dbContext.Notes.Remove(note);
        await dbContext.SaveChangesAsync(ct);

        return TypedResults.Ok(new DeleteNoteResponse(note.Id));
    }
}

public sealed record DeleteNoteResponse(string Id);