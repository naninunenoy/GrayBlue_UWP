using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrayBlueUWPCore.BLE {
    internal interface IGattDevice {
        string Name { get; }
        ulong Address { get; }
        string DeviceId { get; }
        Task<string> GetDeviceIdAsync();
    }
}
