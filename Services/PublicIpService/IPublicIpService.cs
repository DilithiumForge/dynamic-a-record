namespace dynamicarecord.services.publicipservice;

using System.Threading.Tasks;
using dynamicarecord.models;

public interface IPublicIpService
{
    Task<IpInfo> GetIpInfo();
}
