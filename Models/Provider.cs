namespace dynamicarecord.models;

public class Provider
{
    public required string Name { get; set; }
    public string? ApiKey { get; set; }
    public required string ApiUser { get; set; }
    public string? IpAddress { get; set; }
    public required List<Domain> Domains { get; set; }
}
