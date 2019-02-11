using System;

namespace IMUObserverCore {
    public interface IConnectionDelegate {
        void OnConnectDone(string uuid);
        void OnConnectTimeout(string uuid);
        void OnConnectLost(string uuid);
    }
}
