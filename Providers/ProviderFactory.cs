namespace dynamicarecord.providers;

using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using dynamicarecord.models;

public class ProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, Type> Providers = new Dictionary<string, Type>
    {
        { "cloudflare", typeof(CloudflareProvider) },
        { "namecheap", typeof(NamecheapProvider) }
    };

    public ProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDNSProvider CreateProvider(Provider provider)
    {
        if (provider == null || provider.ApiKey == null || provider.IpAddress == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        string providerName = provider.Name.ToLower();
        if (!Providers.ContainsKey(providerName))
        {
            throw new NotSupportedException($"Provider {provider.Name} is not supported.");
        }

        Type providerType = Providers[providerName];
        return (IDNSProvider)ActivatorUtilities.CreateInstance(_serviceProvider, providerType, provider.ApiKey, provider.ApiUser, provider.IpAddress);
    }
}
