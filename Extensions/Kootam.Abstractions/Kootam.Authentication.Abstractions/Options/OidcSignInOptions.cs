namespace Kootam.Authentication.Abstractions.Options;

public class OidcSignInOptions
{
    public bool Enabled { get; set; }

    public string? ClientId { get; set; }

    public string[] ClientIds { get; set; } = [];

    public string? RedirectUri { get; set; }

    public IReadOnlyList<string> EffectiveClientIds()
    {
        var clientIds = new List<string>();
        Add(ClientId);
        if (ClientIds is not null)
        {
            foreach (var clientId in ClientIds)
                Add(clientId);
        }

        return clientIds;

        void Add(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var trimmed = value.Trim();
            if (clientIds.Exists(existing => string.Equals(existing, trimmed, StringComparison.Ordinal)))
                return;

            clientIds.Add(trimmed);
        }
    }

    public bool IsConfigured()
        => Enabled && EffectiveClientIds().Count > 0;
}
