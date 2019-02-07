using System;
using System.Collections.Generic;

namespace IMUObserverCore.BLE {
    internal interface IIMUNotifyDevice : IGattDevice {
        IObservable<byte[]> ButtonUpdateObservable();
    }
}
