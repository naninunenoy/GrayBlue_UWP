using System;
using Windows.Devices.Bluetooth;

namespace IMUOberverCore.BLE {
    interface IIMUNotifyDevice : IDisposable {
        BluetoothLEDevice Divice { get; }
        string UUID { get; }
        IObservable<byte[]> ButtonUpdateObservable();
    }
}
