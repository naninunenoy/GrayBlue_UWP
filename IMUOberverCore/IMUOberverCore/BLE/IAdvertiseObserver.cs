using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace IMUOberverCore.BLE {
    internal interface IAdvertiseObserver : IDisposable {
        Task<BluetoothLEDevice[]> ScanAdvertiseDevicesAsync();
    }
}
