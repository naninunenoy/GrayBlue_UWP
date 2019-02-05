using System;
using Windows.Devices.Bluetooth.Advertisement;

namespace IMUOberverCore.BLE {
    interface IAdvertiseObserver : IDisposable {
        IObservable<BluetoothLEAdvertisementReceivedEventArgs> ScanAdvertiseDevicesAsync();
    }
}
