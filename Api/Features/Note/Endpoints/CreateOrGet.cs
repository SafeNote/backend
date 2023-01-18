using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SafeNote.Api.Persistence;
using SafeNote.Api.Persistence.Entities;

namespace SafeNote.Api.Features.Note.Endpoints;

public static class CreateOrGet
{
    public static IEndpointRouteBuilder MapCreateOrGetEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/create-or-get", CreateOrGetNoteHandler);
        return routes;
    }

    private static async Task<Results<Ok<CreateOrGetNoteResponse>, ValidationProblem>> CreateOrGetNoteHandler(
        CreateOrGetNoteRequest req, IValidator<CreateOrGetNoteRequest> validator, AppDbContext dbContext,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(req, ct);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var note = await dbContext.Notes.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (note is null)
        {
            note = new Persistence.Entities.Note
            {
                Id = req.Id,
                DataBundle = req.DataBundle
            };

            await dbContext.Notes.AddAsync(note, ct);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok(new CreateOrGetNoteResponse(note.Id, note.DataBundle, "CREATE"));
        }

        if (note.DataBundle.KeyHash != req.DataBundle.KeyHash)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "errors", new[] { "No note found." } }
            });
        }

        return TypedResults.Ok(new CreateOrGetNoteResponse(note.Id, note.DataBundle, "GET"));
    }
}

public sealed record CreateOrGetNoteRequest(string Id, DataBundle DataBundle);

public sealed class CreateOrGetNoteRequestValidator : AbstractValidator<CreateOrGetNoteRequest>
{
    public CreateOrGetNoteRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DataBundle).NotNull();
        RuleFor(x => x.DataBundle.KeyHash).NotEmpty();
    }
}

public sealed record CreateOrGetNoteResponse(string Id, DataBundle DataBundle, string Action);