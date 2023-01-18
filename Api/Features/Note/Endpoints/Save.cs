using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SafeNote.Api.Persistence;
using SafeNote.Api.Persistence.Entities;

namespace SafeNote.Api.Features.Note.Endpoints;

public static class Save
{
    public static IEndpointRouteBuilder MapSaveEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPut("/save", SaveNoteHandler);
        return routes;
    }

    private static async Task<Results<Ok, ValidationProblem>> SaveNoteHandler(
        SaveNoteRequest req, IValidator<SaveNoteRequest> validator, AppDbContext dbContext, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(req, ct);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var note = await dbContext.Notes.FirstOrDefaultAsync(
            x => x.Id == req.Id && x.DataBundle.KeyHash == req.DataBundle.KeyHash, ct);
        if (note is null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "errors", new[] { "No note found." } }
            });
        }

        note.DataBundle = req.DataBundle;
        await dbContext.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}

public sealed record SaveNoteRequest(string Id, DataBundle DataBundle);

public sealed class SaveNoteRequestValidator : AbstractValidator<SaveNoteRequest>
{
    public SaveNoteRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DataBundle).NotNull();
        RuleFor(x => x.DataBundle.KeyHash).NotEmpty();
    }
}