using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace IMUObserverCore.BLE {
    internal interface IGattDevice : IDisposable {
        BluetoothLEDevice Device { get; }
        string Name { get; }
        string UUID { get; }
        ulong Address { get; }
        GattDeviceServicesResult GattServices { get; }
    }
}
