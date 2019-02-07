using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMUOberverCore {
    public static class Plugin {
        static BLE.IAdvertiseObserver advertiseObserver;

        static Plugin() {
            advertiseObserver = new BLE.AdvertiseObserver();
        }

        public static async Task<string[]> Scan() {
            var devices = await advertiseObserver.ScanAdvertiseDevicesAsync();
            if (devices == null || devices.Length == 0) {
                Debug.WriteLine("no device found..");
                return new string[0]; //empty
            }
            Debug.WriteLine($"found {devices.Length} devices");
            Debug.WriteLine($"DeviceId: {string.Join("/", devices.Select(x => x.DeviceId.ToString()))}");
            Debug.WriteLine($"BluetoothDeviceId: {string.Join("/", devices.Select(x => x.BluetoothDeviceId.Id.ToUpper()))}");
            Debug.WriteLine($"BluetoothAddress :{string.Join("/", devices.Select(x => x.BluetoothAddress))}");
            return devices.Select(x => x.DeviceId).ToArray();
        }
    }
}
