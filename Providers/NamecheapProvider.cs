using System.Web;

namespace dynamicarecord.providers;

public class NamecheapProvider : IDNSProvider
{
    private readonly string _apiUser;
    private readonly string _apiKey;
    private readonly string _clientIp;
    private readonly HttpClient _httpClient;

    public NamecheapProvider(string apiUser, string apiKey, string clientIp)
    {
        _apiUser = apiUser;
        _apiKey = apiKey;
        _clientIp = clientIp;
        _httpClient = new HttpClient();
    }

    public async Task<bool> UpdateDNSRecord(string domain, string subdomain, string ipAddress)
    {
        var baseUrl = "https://api.namecheap.com/xml.response";
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["ApiUser"] = _apiUser;
        query["ApiKey"] = _apiKey;
        query["UserName"] = _apiUser;
        query["ClientIp"] = _clientIp;
        query["Command"] = "namecheap.domains.dns.setHosts";
        query["SLD"] = domain.Split('.')[0]; // Assuming domain is something like "example.com"
        query["TLD"] = domain.Split('.')[1];
        query["HostName1"] = subdomain;
        query["RecordType1"] = "A";
        query["Address1"] = ipAddress;
    
        var url = $"{baseUrl}?{query}";
    
        var response = await _httpClient.PostAsync(url, null);
    
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Namecheap: Updated {subdomain}.{domain} to {ipAddress}");
            return true;
        }
        else
        {
            Console.WriteLine($"Namecheap: Failed to update {subdomain}.{domain} to {ipAddress}");
            return false;
        }
    }
}
