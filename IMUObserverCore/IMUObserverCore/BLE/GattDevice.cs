using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace IMUObserverCore.BLE {
    internal class GattDevice : IGattDevice {
        public BluetoothLEDevice Device { private set; get; }
        public string Name { get { return Device?.Name; } }
        public string UUID { get { return Device?.DeviceId; } }
        public ulong Address { private set; get; }
        public GattDeviceServicesResult GattServices { private set; get; }

        public GattDevice(ulong bluetoothAddress) {
            Address = bluetoothAddress;
        }

        public async Task LoadAsync() {
            Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Address);
            GattServices = await Device.GetGattServicesAsync();
        }

        public void Dispose() {
            Device?.Dispose();
        }
    }

    internal static class GattDeviceServiceExtension {
        public static bool ContainsServiceUuid(this GattDeviceServicesResult gatt, string uuid) {
            return gatt.Services.Any(x => uuid == x.Uuid.ToString());
        }
        public static string ServiceUuids(this GattDeviceServicesResult gatt) {
            if (gatt == null || gatt.Services == null)
                return "<NULL>";
            if (!gatt.Services.Any())
                return "<EMPTY>";
            return $"<{string.Join(",", gatt.Services.Select(x => x.Uuid.ToString()))}>";
        }
    }
}
