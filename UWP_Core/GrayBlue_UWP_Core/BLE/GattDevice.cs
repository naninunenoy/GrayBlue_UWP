using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace GrayBlueUWPCore.BLE {
    internal class GattDevice : IGattDevice {
        public string Name { private set; get; }
        public ulong Address { private set; get; }
        public string DeviceId { private set; get; }

        public GattDevice(BluetoothLEAdvertisementReceivedEventArgs advertise) {
            Address = advertise.BluetoothAddress;
            Name = advertise.Advertisement.LocalName;
            DeviceId = "";
        }

        public GattDevice(string name, ulong bluetoothAddress) {
            Name = name;
            Address = bluetoothAddress;
            DeviceId = "";
        }

        public async Task<string> GetDeviceIdAsync() {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(Address); // connect
            DeviceId = device.DeviceInformation.Id;
            device.Dispose(); // disconnect
            return DeviceId;
        }
    }
}
