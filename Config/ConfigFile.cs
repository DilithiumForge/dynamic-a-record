
namespace dynamicarecord.config;

using dynamicarecord.models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;
using System.IO;
using dynamicarecord.services.publicipservice;

class ConfigFile {
    public static Config LoadConfig(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using (var reader = new StreamReader(path))
        {
            return deserializer.Deserialize<Config>(reader);
        }
    }

    public static async Task ApplyEnvironmentOverridesAsync(Config config, IPublicIpService publicIpService, string? ipAddressOverride)
    {
        string? globalIpAddress = null;

        if (!string.IsNullOrEmpty(ipAddressOverride))
        {
            globalIpAddress = ipAddressOverride;
        }
        else
        {
            string? envIpAddress = Environment.GetEnvironmentVariable("IP_ADDRESS");
            if (!string.IsNullOrEmpty(envIpAddress))
            {
                globalIpAddress = envIpAddress;
            }
            else
            {
                IpInfo ipInfo = await publicIpService.GetIpInfo();
                globalIpAddress = ipInfo.Ip;
            }
        }

        foreach (var provider in config.Providers)
        {
            string? envApiKey = Environment.GetEnvironmentVariable($"{provider.Name.ToUpper()}_API_KEY");
            if (!string.IsNullOrEmpty(envApiKey))
            {
                provider.ApiKey = envApiKey;
            }

            provider.IpAddress = globalIpAddress;
        }

        Console.WriteLine($"Using IP address: {globalIpAddress}");
    }
}
