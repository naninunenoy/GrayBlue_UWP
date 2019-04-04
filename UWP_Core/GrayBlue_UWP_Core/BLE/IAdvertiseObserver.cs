using System;
using System.Threading.Tasks;

namespace GrayBlueUWPCore.BLE {
    internal interface IAdvertiseObserver : IDisposable {
        Task<IGattDevice[]> ScanAdvertiseDevicesAsync();
    }
}
