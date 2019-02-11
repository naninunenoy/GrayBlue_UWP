using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMUObserverCore {
    public static class Plugin {
        static BLE.IAdvertiseObserver advertiseObserver;
        static Dictionary<string, BLE.IIMUNotifyDevice> DeviceDict;

        static Plugin() {
            advertiseObserver = new BLE.AdvertiseObserver();
            DeviceDict = new Dictionary<string, BLE.IIMUNotifyDevice>();
        }

        public static async Task<string[]> Scan() {
            var devices = await advertiseObserver.ScanAdvertiseDevicesAsync();
            if (devices == null || devices.Length == 0) {
                Debug.WriteLine("no device found..");
                return new string[0]; //empty
            }
            Debug.WriteLine($"found {devices.Length} devices");
            Debug.WriteLine($"DeviceId: {string.Join("/", devices.Select(x => x.UUID.ToString()))}");
            Debug.WriteLine($"BluetoothAddress :{string.Join("/", devices.Select(x => x.Address))}");
            foreach (var device in devices) {
                if (!DeviceDict.ContainsKey(device.UUID)) {
                    DeviceDict.Add(device.UUID, device);
                }
            }
            return devices.Select(x => x.UUID).ToArray();
        }

        public static void ConnectTo(string uuid, IConnectionDelegate connectionDelegate, INotifyDelegate notifyDelegate) {
            if (!DeviceDict.ContainsKey(uuid)) {
                Debug.Fail($"{uuid} is not exist");
                return;
            }
            var device = DeviceDict[uuid];
            device.ButtonUpdateObservable()
                  .Subscribe(data => {
                      bool press = (data[0] != 0);
                      char name = (char)data[1];
                      short ms = (short)((data[3] << 8) + data[2]);
                      float time = ms / 1000.0F;
                      Debug.WriteLine($"p={press},c={name},t={time}");
                      if (press) {
                          notifyDelegate.OnButtonPush(uuid, name.ToString());
                      } else {
                          notifyDelegate.OnButtonRelease(uuid, name.ToString(), time);
                      }
                  });
        }

        public static void Dispose() {
            foreach (var device in DeviceDict.Values) {
                device.Dispose();
            }
            advertiseObserver.Dispose();
            DeviceDict.Clear();
        }
    }
}
