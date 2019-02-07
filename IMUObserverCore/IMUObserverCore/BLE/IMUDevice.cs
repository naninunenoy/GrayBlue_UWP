using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace IMUObserverCore.BLE {
    internal class IMUDevice : IIMUNotifyDevice {
        public BluetoothLEDevice Device { private set; get; }
        public string Name { get { return Device.Name; } }
        public string UUID { get { return Device.DeviceId; } }
        public ulong Address { private set; get; }
        public GattDeviceServicesResult GattServices { private set; get; }

        public IMUDevice(IGattDevice gattDevice) {
            Device = gattDevice.Device;
            Address = gattDevice.Address;
            GattServices = gattDevice.GattServices;
        }

        public IObservable<byte[]> ButtonUpdateObservable() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            Device.Dispose();
        }
    }
}
