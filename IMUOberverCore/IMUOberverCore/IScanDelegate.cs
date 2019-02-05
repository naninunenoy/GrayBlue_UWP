using System;

namespace IMUOberverCore {
    interface IScanDelegate {
        void OnScanFinish(string[] foundUUIDs);
    }
}
