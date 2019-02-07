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
            foreach(var device in devices) {
                if (!DeviceDict.ContainsKey(device.UUID)) {
                    DeviceDict.Add(device.UUID, device);
                }
            }
            return devices.Select(x => x.UUID).ToArray();
        }

        public static void Dispose() {
            advertiseObserver.Dispose();
            DeviceDict.Clear();
        }
    }
}
