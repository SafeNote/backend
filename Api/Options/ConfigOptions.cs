namespace SafeNote.Api.Options;

public sealed class ConfigOptions
{
    public const string Config = nameof(Config);

    public string RootDomain { get; set; } = default!;
    public AppUrl ApiUrl { get; set; } = default!;
    public AppUrl WebUrl { get; set; } = default!;
}

public sealed record AppUrl(string Scheme, string Domain)
{
    public override string ToString() => $"{Scheme}://{Domain}";
}