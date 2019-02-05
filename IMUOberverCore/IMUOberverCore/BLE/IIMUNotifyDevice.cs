using System;
using Windows.Devices.Bluetooth;

namespace IMUOberverCore.BLE {
    internal interface IIMUNotifyDevice : IDisposable {
        BluetoothLEDevice Divice { get; }
        string UUID { get; }
        IObservable<byte[]> ButtonUpdateObservable();
    }
}
