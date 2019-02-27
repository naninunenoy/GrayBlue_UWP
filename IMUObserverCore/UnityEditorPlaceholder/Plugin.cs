using System;
using System.Threading.Tasks;

namespace IMUObserverCore {
    public class Plugin : IPlugin {
        static Plugin instance;
        static readonly object lockObj = new object();

        private Plugin() { /* Do Nothing*/　}

        public static Plugin Instance {
            get {
                if (instance == null) {
                    lock (lockObj) {
                        if (instance == null) {
                            instance = new Plugin();
                        }
                    }
                }
                return instance;
            }
        }

        public Task<bool> CanUseBle() { return Task.FromResult(false); }
        public Task<string[]> Scan() { return Task.FromResult(new string[0]); }
        public Task<bool> ConnectTo(string deviceId, IConnectionDelegate connectionDelegate, INotifyDelegate notifyDelegate) {
            return Task.FromResult(false);
        }
        public void DisconnectAllDevices() { /* Do Nothing*/　}
        public void DisconnectTo(string deviceId) { /* Do Nothing*/　}
        public void Dispose() { /* Do Nothing*/　}
    }
}
