namespace SafeNote.Api.Persistence.Entities;

public sealed record DataBundle(string KeyHash, string Nonce, string EncryptedData);