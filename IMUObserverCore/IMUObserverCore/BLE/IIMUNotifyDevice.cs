using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reactive;

namespace IMUObserverCore.BLE {
    internal interface IIMUNotifyDevice : IGattDevice {
        Task<IIMUNotifyDevice> ConnectionAsync();
        void Disconnect();
        IObservable<byte[]> ButtonUpdateObservable();
        IObservable<Unit> ConnectionLostObservable();
    }
}
