using System;
using System.Threading.Tasks;

namespace GrayBlueUWPCore {
    public interface IPlugin {
        Task<bool> CanUseBle();
        Task<string[]> Scan();
        Task<bool> ConnectTo(string deviceId, IConnectionDelegate connectionDelegate, INotifyDelegate notifyDelegate);
        void DisconnectTo(string deviceId);
        void DisconnectAllDevices();
        void Dispose();
    }
}
