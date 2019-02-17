using System;

namespace IMUObserverCore {
    public interface IConnectionDelegate {
        void OnConnectDone(string uuid);
        void OnConnectFail(string uuid);
        void OnConnectLost(string uuid);
    }
}
