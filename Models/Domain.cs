namespace dynamicarecord.models;

public class Domain
{
    public required string FQDN { get; set; }
    public required List<string> Subdomains { get; set; }
}