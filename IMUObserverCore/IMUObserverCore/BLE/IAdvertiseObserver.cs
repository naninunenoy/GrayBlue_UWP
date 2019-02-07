using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace IMUObserverCore.BLE {
    internal interface IAdvertiseObserver : IDisposable {
        Task<BluetoothLEDevice[]> ScanAdvertiseDevicesAsync();
    }
}
