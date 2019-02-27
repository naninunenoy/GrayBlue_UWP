using System;

namespace IMUObserverCore {
    public interface INotifyDelegate {
        void OnButtonPush(string deviceId, string buttonName);
        void OnButtonRelease(string deviceId, string buttonName, float pressTime);
        void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat);
    }
}
