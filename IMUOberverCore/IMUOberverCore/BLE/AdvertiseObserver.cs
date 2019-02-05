using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace IMUOberverCore.BLE {
    internal class AdvertiseObserver : IAdvertiseObserver {
        static readonly TimeSpan interval = TimeSpan.FromMilliseconds(500);
        static readonly TimeSpan scanLength = TimeSpan.FromSeconds(5);

        private readonly BluetoothLEAdvertisementWatcher advertiseWatcher;
        private readonly Subject<BluetoothLEAdvertisementReceivedEventArgs> advertiseSubject;

        public AdvertiseObserver() {
            advertiseWatcher = new BluetoothLEAdvertisementWatcher();
            advertiseSubject = new Subject<BluetoothLEAdvertisementReceivedEventArgs>();
            advertiseWatcher.SignalStrengthFilter.SamplingInterval = interval;
            advertiseWatcher.Received += OnAdvertisementReceived;
        }

        public void Dispose() {
            advertiseWatcher.Stop();
            advertiseWatcher.Received -= OnAdvertisementReceived;
            advertiseSubject.Dispose();
        }

        public async Task<BluetoothLEDevice[]> ScanAdvertiseDevicesAsync() {
            Debug.WriteLine("ScanAdvertiseDevicesAsync");
            return await advertiseSubject
                .TakeUntil(new DateTimeOffset(DateTime.Now, scanLength))
                .Select(async arg => {
                    var device = await BluetoothLEDevice.FromBluetoothAddressAsync(arg.BluetoothAddress);
                    var gatt = await device.GetGattServicesAsync();
                    var isMyService = gatt.ContainsServiceUuid(Profiles.Services.Button);
                    Debug.WriteLine($"{device.Name}'s services are {gatt.ServiceUuids()}");
                    return new { isMyService, device };
                })
                .Select(task => task.Result)
                .Where(x => x.isMyService)
                .Select(x => x.device)
                .ToArray()
                .ToTask();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) {
            if (sender == advertiseWatcher) {
                advertiseSubject.OnNext(args);
            }
        }
    }

    internal static class GattDeviceServiceExtension {
        public static bool ContainsServiceUuid(this GattDeviceServicesResult gatt, string uuid) {
            return gatt.Services.Any(x => uuid == x.Uuid.ToString());
        }
        public static string ServiceUuids(this GattDeviceServicesResult gatt) {
            if (gatt == null || gatt.Services == null) return "<NULL>";
            if (!gatt.Services.Any()) return "<EMPTY>";
            return $"<{string.Join(",", gatt.Services.Select(x => x.Uuid.ToString()))}>";
        }
    }
}
