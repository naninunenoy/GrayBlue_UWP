using System;

namespace IMUOberverCore {
    interface IConnectionDelegate {
        void OnConnectDone(string uuid);
        void OnConnectTimeout(string uuid);
        void OnConnectLost(string uuid);
    }
}
