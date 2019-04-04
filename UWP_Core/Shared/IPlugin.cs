using System;
using System.Threading.Tasks;

namespace IMUObserverCore {
    public interface IPlugin {
        Task<bool> CanUseBle();
        Task<string[]> Scan();
        Task<bool> ConnectTo(string deviceId, IConnectionDelegate connectionDelegate, INotifyDelegate notifyDelegate);
        void DisconnectTo(string deviceId);
        void DisconnectAllDevices();
        void Dispose();
    }
}
