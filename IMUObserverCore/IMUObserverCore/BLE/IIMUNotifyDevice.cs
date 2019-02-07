using System;
using Windows.Devices.Bluetooth;

namespace IMUObserverCore.BLE {
    internal interface IIMUNotifyDevice : IDisposable {
        BluetoothLEDevice Divice { get; }
        string UUID { get; }
        IObservable<byte[]> ButtonUpdateObservable();
    }
}
