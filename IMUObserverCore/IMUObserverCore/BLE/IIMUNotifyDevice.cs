using System;

namespace IMUObserverCore.BLE {
    internal interface IIMUNotifyDevice : IGattDevice {
        IObservable<byte[]> ButtonUpdateObservable();
    }
}
