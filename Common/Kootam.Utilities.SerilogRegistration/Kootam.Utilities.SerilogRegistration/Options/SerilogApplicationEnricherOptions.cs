namespace Kootam.Utilities.SerilogRegistration.Options;

public class SerilogApplicationEnricherOptions
{
    public required string  ApplicationName { get; set; }
    public required string ServiceName { get; set; }
    public required string ServiceVersion { get; set; }
    public required string ServiceId { get; set; }
}