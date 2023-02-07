namespace SafeNote.Api.Persistence.Entities;

public sealed class Note
{
    public string Id { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DataBundle DataBundle { get; set; } = default!;
}