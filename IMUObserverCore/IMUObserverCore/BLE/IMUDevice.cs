using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Foundation;
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
        private Subject<byte[]> imuSubject;

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
                IObservable<byte[]> notify;
                try {
                    notify = await allServices[Profiles.Services.Button].GetCharacteristicsNotifyOf(Profiles.Characteristics.ButtonOperation);
                } catch (BLEException e) {
                    throw new BLEException($"{e.Message} (Button)", e);
                }
                notify?.Subscribe(x => { buttonSubject?.OnNext(x); });
            }
            // IMU data service
            if (allServices.ContainsKey(Profiles.Services.NineAxis)) {
                IObservable<byte[]> notify;
                try {
                    notify = await allServices[Profiles.Services.NineAxis].GetCharacteristicsNotifyOf(Profiles.Characteristics.NineAxisIMUData);
                } catch (BLEException e) {
                    throw new BLEException($"{e.Message} (IMU)", e);
                }
                notify?.Subscribe(x => { imuSubject?.OnNext(x); });
            }
            // connection complete
            deviceLostSubject = new Subject<Unit>();
            buttonSubject = new Subject<byte[]>();
            imuSubject = new Subject<byte[]>();
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
        public IObservable<byte[]> IMUUpdateObservable() { return imuSubject?.AsObservable(); }

        public void Dispose() {
            deviceLostSubject?.Dispose();
            buttonSubject?.Dispose();
            imuSubject?.Dispose();
            Device?.Dispose();
        }
    }
    
    internal static class BleServiceGuidExtension {
        public static async Task<IObservable<byte[]>> GetCharacteristicsNotifyOf(this GattDeviceService service, Guid of) {
            var characteristics = await service.GetCharacteristicsForUuidAsync(of);
            var characteristic = characteristics.Characteristics.FirstOrDefault();
            if (characteristic == null) {
                throw new BLEException("Characteristic not found.");
            }
            if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) {
                throw new BLEException("characteristic has no Notify flag.");
            }
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (status != GattCommunicationStatus.Success) {
                throw new BLEException("GattCommunicationStatus is failed.");
            }
            return Observable.FromEventPattern<
                TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs>,
                GattCharacteristic, GattValueChangedEventArgs>(
                    h => characteristic.ValueChanged += h,
                    h => characteristic.ValueChanged -= h)
                .Select(x => x.EventArgs)
                .Select(x => {
                    var data = new byte[x.CharacteristicValue.Length];
                    DataReader.FromBuffer(x.CharacteristicValue).ReadBytes(data);
                    return data;
                });
        }
    }
}
