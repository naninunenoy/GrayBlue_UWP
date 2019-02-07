using System;

namespace IMUObserverCore {
    interface IConnectionDelegate {
        void OnConnectDone(string uuid);
        void OnConnectTimeout(string uuid);
        void OnConnectLost(string uuid);
    }
}
