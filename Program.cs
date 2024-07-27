namespace dynamicarecord;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

using dynamicarecord.config;
using dynamicarecord.models;
using dynamicarecord.services.publicipservice;
using dynamicarecord.providers;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            PrintHelp();
            return;
        }

        // Config and setup
        string configPath = args[0];
        string? ipAddressOverride = args.Length == 2 ? args[1] : null;
        var config = ConfigFile.LoadConfig(configPath);

        // Service Collection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        IPublicIpService? publicIpService = serviceProvider.GetService<IpInfoIoService>();
        if (publicIpService == null)
        {
            Console.WriteLine("Public IP service is available.");
            return;
        }

        await ConfigFile.ApplyEnvironmentOverridesAsync(config, publicIpService, ipAddressOverride);

        var providerFactory = serviceProvider.GetRequiredService<ProviderFactory>();

        foreach (var provider in config.Providers)
        {
            IDNSProvider dnsProvider = providerFactory.CreateProvider(provider);
            await UpdateDNSRecords(dnsProvider, provider.Domains, provider.IpAddress);
        }
    }

    static async Task UpdateDNSRecords(IDNSProvider provider, List<Domain> domains, string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
        {
            Console.WriteLine("No IP address provided. Skipping DNS record update.");
            return;
        }

        foreach (var domain in domains)
        {
            foreach (var subdomain in domain.Subdomains)
            {
                try
                {
                    await provider.UpdateDNSRecord(domain.FQDN, subdomain, ipAddress);
                    Console.WriteLine($"Updated DNS record for {subdomain}.{domain.FQDN} to {ipAddress}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update DNS record for {subdomain}.{domain.FQDN}: {ex.Message}");
                }
            }
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient<IpInfoIoService>();
        services.AddHttpClient<CloudflareProvider>();
        services.AddHttpClient<NamecheapProvider>();

        services.AddTransient<ProviderFactory>();
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: DNSUpdater <config_file_path> [ip_address]");
        return;
    }
}
