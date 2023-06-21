using Music.Identity.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Shared.Identity.Common.Models;

namespace Music.Identity.Service.Services;

public interface IDevicesService
{
    Task<Device> GetDevice(DeviceInfo deviceInfo);
}

public class DevicesService : IDevicesService
{
    private readonly IRepository<Device> _devicesRepository;
    
    public DevicesService(IRepository<Device> devicesRepository)
    {
        _devicesRepository = devicesRepository;
    }
    
    public async Task<Device> GetDevice(DeviceInfo deviceInfo)
    {
        var device = await _devicesRepository.GetAsync(x => x.Hash == deviceInfo.DeviceHash);
        if (device != null) return device;
        
        device = new Device()
        {
            DbAddingDate = DateTimeOffset.UtcNow,
            Hash = deviceInfo.DeviceHash
        };
        await _devicesRepository.CreateAsync(device);

        return device;
    }
}