using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace IMUObserverCore.BLE {
    internal class IMUDevice : IIMUNotifyDevice {
        public BluetoothLEDevice Device { private set; get; }
        public string Name { get { return Device.Name; } }
        public string UUID { get { return Device.DeviceId; } }
        public ulong Address { private set; get; }
        public GattDeviceServicesResult GattServices { private set; get; }
        public Dictionary<string, GattDeviceService> GattServiceDict { private set; get; }

        private GattCharacteristic buttonCharacteristic;
        private Subject<byte[]> buttonSubject;

        public IMUDevice(IGattDevice gattDevice) {
            Device = gattDevice.Device;
            Address = gattDevice.Address;
            GattServices = gattDevice.GattServices;
            GattServiceDict = gattDevice.GattServiceDict;
            buttonSubject = new Subject<byte[]>();
        }

        public async Task<IMUDevice> LoadAsync() {
            // button operation
            if (GattServiceDict.ContainsKey(Profiles.Services.Button)) {
                var s = GattServiceDict[Profiles.Services.Button];
                var cs = await s.GetCharacteristicsForUuidAsync(Profiles.Characteristics.ButtonOperation);
                buttonCharacteristic = cs.Characteristics.FirstOrDefault();
                buttonCharacteristic.ValueChanged += OnButtonNotify;
            }
            return this;
        }

        private void OnButtonNotify(GattCharacteristic c, GattValueChangedEventArgs value) {

        }
        public IObservable<byte[]> ButtonUpdateObservable() { return buttonSubject.AsObservable(); }

        public void Dispose() {
            buttonCharacteristic.ValueChanged -= OnButtonNotify;
            buttonSubject.Dispose();
            Device.Dispose();
        }
    }
}
