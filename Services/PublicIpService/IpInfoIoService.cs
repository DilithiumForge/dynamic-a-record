namespace dynamicarecord.services.publicipservice;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using dynamicarecord.models;
using Newtonsoft.Json;

public class IpInfoIoService : IPublicIpService
{
    private readonly HttpClient _httpClient;

    public IpInfoIoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IpInfo> GetIpInfo()
    {
        var response = await _httpClient.GetAsync("https://ipinfo.io");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get IP info");
        }

        var content = await response.Content.ReadAsStringAsync();
        var deserializedObject = JsonConvert.DeserializeObject<IpInfo>(content);
        if (deserializedObject == null)
        {
            throw new Exception("Failed to deserialize IP info");
        }

        return deserializedObject;
    }
}