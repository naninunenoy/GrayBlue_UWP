using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMUObserverCore {
    public class Plugin : IPlugin {
        static Plugin instance;
        static readonly object lockObj = new object();

        BLE.IAdvertiseObserver advertiseObserver;
        IDictionary<string, BLE.IIMUNotifyDevice> deviceDict;

        private Plugin() {
            advertiseObserver = new BLE.AdvertiseObserver();
            deviceDict = new Dictionary<string, BLE.IIMUNotifyDevice>();
        }

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

        public async Task<bool> CanUseBle() {
            return await BLE.BLEAvailable.ChackAsync();
        }

        public async Task<string[]> Scan() {
            // scan devices
            Debug.WriteLine("scanning..");
            var devices = await advertiseObserver.ScanAdvertiseDevicesAsync();
            if (devices == null || devices.Length == 0) {
                Debug.WriteLine("no device found..");
                return new string[0]; //empty
            }
            Debug.WriteLine($"found {devices.Length} devices");
            // read deviceId
            var deviceIds = await Task.WhenAll(devices.Select(async x => await x.GetDeviceIdAsync()));
            Debug.WriteLine($"DeviceId: {string.Join("/", deviceIds)}");
            return deviceIds.ToArray();
        }

        public async Task<bool> ConnectTo(string deviceId, IConnectionDelegate connectionDelegate, INotifyDelegate notifyDelegate) {
            BLE.IIMUNotifyDevice device = new BLE.IMUDevice(deviceId);
            try {
                device = await device.ConnectionAsync();
            } catch (BLE.BLEException e) {
                Debug.WriteLine($"{deviceId} connect failed. {e.Message}");
                connectionDelegate?.OnConnectFail(deviceId);
                device.Dispose();
                return false;
            }
            device.ConnectionLostObservable()
                  .Subscribe(_ => { connectionDelegate?.OnConnectLost(deviceId); });
            device.ButtonUpdateObservable()
                  .Subscribe(data => {
                      bool press = BitConverter.ToBoolean(data, 0);
                      char name = (char)data[1];
                      short ms = BitConverter.ToInt16(data, 2);
                      float time = ms / 1000.0F;
                      if (press) {
                          notifyDelegate?.OnButtonPush(deviceId, name.ToString());
                      } else {
                          notifyDelegate?.OnButtonRelease(deviceId, name.ToString(), time);
                      }
                  });
            device.IMUUpdateObservable()
                  .Subscribe(data => {
                      var acc = new float[3] {
                          BitConverter.ToSingle(data, 0),
                          BitConverter.ToSingle(data, 4),
                          BitConverter.ToSingle(data, 8)
                      };
                      var gyro = new float[3] {
                          BitConverter.ToSingle(data, 12),
                          BitConverter.ToSingle(data, 16),
                          BitConverter.ToSingle(data, 20)
                      };
                      var mag = new float[3] {
                          BitConverter.ToSingle(data, 24),
                          BitConverter.ToSingle(data, 28),
                          BitConverter.ToSingle(data, 32)
                      };
                      var quat = new float[4] {
                          BitConverter.ToSingle(data, 36),
                          BitConverter.ToSingle(data, 40),
                          BitConverter.ToSingle(data, 44),
                          BitConverter.ToSingle(data, 48)
                      };
                      notifyDelegate?.OnIMUDataUpdate(deviceId, acc, gyro, mag, quat);
                  });
            connectionDelegate?.OnConnectDone(deviceId);
            if (deviceDict.ContainsKey(deviceId)) {
                // overwrite
                deviceDict[deviceId].Dispose();
                deviceDict[deviceId] = device;
            } else {
                deviceDict.Add(deviceId, device);
            }
            return true;
        }

        public void DisconnectTo(string deviceId) {
            if (!deviceDict.ContainsKey(deviceId)) {
                Debug.Fail($"{deviceId} is not exist");
                return;
            }
            deviceDict[deviceId].Disconnect();
            deviceDict.Remove(deviceId);
        }

        public void DisconnectAllDevices() {
            foreach(var device in deviceDict.Values) {
                device.Disconnect();
            }
            deviceDict.Clear();
        }

        public void Dispose() {
            foreach (var device in deviceDict.Values) {
                device.Dispose();
            }
            advertiseObserver.Dispose();
            deviceDict.Clear();
        }
    }
}
