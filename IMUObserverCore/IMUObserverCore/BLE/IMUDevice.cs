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
        public string Name { get => Device?.Name ?? "???"; }
        public string DeviceId { private set; get; }

        private BluetoothLEDevice Device { set; get; }
        private bool connected;
        private Subject<Unit> deviceLostSubject;
        private Subject<byte[]> buttonSubject;

        public IMUDevice(string deviceId) {
            DeviceId = deviceId;
            connected = false;
        }

        public async Task<IIMUNotifyDevice> ConnectionAsync() {
            if (connected) {
                throw new BLEException($"device({DeviceId}) already connected");
            }
            // try connect
            Device = await BluetoothLEDevice.FromIdAsync(DeviceId);
            if (Device == null) {
                throw new BLEException($"device({DeviceId}) not found");
            }
            // connect done
            connected = true;
            // monitor connection status
            Device.ConnectionStatusChanged += (device, _) => {
                if (connected && device.ConnectionStatus == BluetoothConnectionStatus.Disconnected) {
                    connected = false;
                    deviceLostSubject?.OnNext(Unit.Default);
                    deviceLostSubject?.OnCompleted();
                }
            };
            // monitor original services
            var gatt = await Device.GetGattServicesAsync();
            var allServices = gatt.Services.ToDictionary(x => x.Uuid);
            // button operation service
            if (allServices.ContainsKey(Profiles.Services.Button)) {
                var service = allServices[Profiles.Services.Button];
                var characteristics = await service.GetCharacteristicsForUuidAsync(Profiles.Characteristics.ButtonOperation);
                var characteristic = characteristics.Characteristics.FirstOrDefault();
                if (characteristic == null) {
                    throw new BLEException($"characteristic({Profiles.Characteristics.ButtonOperation}) not found");
                }
                if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) {
                    throw new BLEException($"characteristic({Profiles.Characteristics.ButtonOperation}) has no Notify flag");
                }
                var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status != GattCommunicationStatus.Success) {
                    throw new GattConnectionException("Button Operation GattCommunicationStatus is failed");
                }
                characteristic.ValueChanged += (c, arg) => {
                    if (c.Uuid == Profiles.Characteristics.ButtonOperation) {
                        var data = new byte[arg.CharacteristicValue.Length];
                        DataReader.FromBuffer(arg.CharacteristicValue).ReadBytes(data);
                        buttonSubject.OnNext(data);
                    }
                };
            }
            // connection complete
            deviceLostSubject = new Subject<Unit>();
            buttonSubject = new Subject<byte[]>();
            return this;
        }

        public void Disconnect() {
            if (connected) {
                connected = false;
                Dispose();
            }
        }

        public IObservable<Unit> ConnectionLostObservable() { return deviceLostSubject?.AsObservable(); }
        public IObservable<byte[]> ButtonUpdateObservable() { return buttonSubject?.AsObservable(); }

        public void Dispose() {
            deviceLostSubject?.Dispose();
            buttonSubject?.Dispose();
            Device?.Dispose();
        }
    }
}
