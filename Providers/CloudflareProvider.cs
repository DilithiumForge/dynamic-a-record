using CloudFlare.Client;
using CloudFlare.Client.Api.Zones;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace dynamicarecord.providers;
public class CloudflareProvider : IDNSProvider
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly CloudFlareClient _cloudFlareClient;

    public CloudflareProvider(string apiKey, HttpClient httpClient)
    {
        _apiKey = apiKey;
        _httpClient = httpClient;
        _cloudFlareClient = new CloudFlareClient(apiKey);
    }

    public async Task<bool> UpdateDNSRecord(string domain, string subdomain, string ipAddress)
    {
        try
        {
            var zoneId = await GetZoneId(domain);
            if (zoneId == null)
            {
                Console.WriteLine($"Cloudflare: Zone ID for {domain} not found.");
                return false;
            }

            var dnsRecord = new NewDnsRecord
            {
                Type = DnsRecordType.A,
                Name = $"{subdomain}.{domain}",
                Content = ipAddress,
                Ttl = 120
            };

            var result = await _cloudFlareClient.Zones.DnsRecords.AddAsync(zoneId, dnsRecord);

            if (result.Success)
            {
                Console.WriteLine($"Cloudflare: Updated {subdomain}.{domain} to {ipAddress}");
            }
            else
            {
                Console.WriteLine($"Cloudflare: Failed to update {subdomain}.{domain} to {ipAddress}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cloudflare: Exception occurred while updating {subdomain}.{domain} to {ipAddress}: {ex.Message}");
            return false;
        }
    }

    private async Task<string?> GetZoneId(string domain)
    {
        var zones = await _cloudFlareClient.Zones.GetAsync(new ZoneFilter { Name = domain });
        return zones.Result.Count > 0 ? zones.Result[0].Id : null;
    }
}