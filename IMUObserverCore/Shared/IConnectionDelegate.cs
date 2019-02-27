using System;

namespace IMUObserverCore {
    public interface IConnectionDelegate {
        void OnConnectDone(string deviceId);
        void OnConnectFail(string deviceId);
        void OnConnectLost(string deviceId);
    }
}
