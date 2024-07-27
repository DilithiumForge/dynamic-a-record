namespace dynamicarecord.providers;

public interface IDNSProvider
{
    public Task<bool> UpdateDNSRecord(string domain, string subdomain, string ipAddress);
}
