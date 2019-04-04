using System;

namespace GrayBlueUWPCore {
    public interface IConnectionDelegate {
        void OnConnectDone(string deviceId);
        void OnConnectFail(string deviceId);
        void OnConnectLost(string deviceId);
    }
}
