using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Storage.Streams;
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
                var sevice = GattServiceDict[Profiles.Services.Button];
                var characteristics = await sevice.GetCharacteristicsForUuidAsync(Profiles.Characteristics.ButtonOperation);
                var characteristic = characteristics.Characteristics.FirstOrDefault();
                if (characteristic == null) {
                    throw new BLEException($"characteristic({Profiles.Characteristics.ButtonOperation}) not found");
                }
                if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) {
                    throw new BLEException($"characteristic({Profiles.Characteristics.ButtonOperation}) has no Notify flag");
                }
                var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                Debug.WriteLine(status);
                if (status != GattCommunicationStatus.Success) {
                    throw new GattConnectionException("Button Operation GattCommunicationStatus is failed");
                }
                characteristic.ValueChanged += OnButtonNotify;
            }
            return this;
        }

        private void OnButtonNotify(GattCharacteristic c, GattValueChangedEventArgs arg) {
            if (c.Uuid == Profiles.Characteristics.ButtonOperation) {
                var data = new byte[arg.CharacteristicValue.Length];
                DataReader.FromBuffer(arg.CharacteristicValue).ReadBytes(data);
                buttonSubject.OnNext(data);
            }
        }

        public IObservable<byte[]> ButtonUpdateObservable() { return buttonSubject.AsObservable(); }

        public void Dispose() {
            buttonSubject.Dispose();
            Device.Dispose();
        }
    }
}