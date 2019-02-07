using System;
using System.Threading.Tasks;

namespace IMUObserverCore.BLE {
    internal interface IAdvertiseObserver : IDisposable {
        Task<IGattDevice[]> ScanAdvertiseDevicesAsync();
    }
}
