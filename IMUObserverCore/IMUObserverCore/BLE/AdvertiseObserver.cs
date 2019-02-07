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

namespace IMUObserverCore.BLE {
    internal class AdvertiseObserver : IAdvertiseObserver {
        static readonly TimeSpan interval = TimeSpan.FromSeconds(1);
        static readonly TimeSpan scanLength = TimeSpan.FromSeconds(3);

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

        public async Task<IIMUNotifyDevice[]> ScanAdvertiseDevicesAsync() {
            Debug.WriteLine("ScanAdvertiseDevicesAsync");
            advertiseWatcher.Start();
            return await advertiseSubject
                .TakeUntil(DateTimeOffset.Now.Add(scanLength))
                .Finally(advertiseWatcher.Stop)
                .Select(async arg => {
                    var device = await new GattDevice(arg.BluetoothAddress).LoadAsync();
                    var isMyService = device.GattServices.ContainsServiceUuid(Profiles.Services.Button);
                    Debug.WriteLine($"{device.Name}'s services are {device.GattServices.ServiceUuids()}");
                    return new { isMyService, device };
                })
                .Select(task => task.Result)
                .Where(x => x.isMyService)
                .Select(x => x.device)
                .Distinct(x => x.UUID)
                .Select(async x => await new IMUDevice(x).LoadAsync())
                .Select(task => task.Result)
                .ToArray()
                .ToTask();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) {
            Debug.WriteLine($"OnAdvertisementReceived {args.BluetoothAddress}");
            if (sender == advertiseWatcher) {
                advertiseSubject.OnNext(args);
            }
        }
    }
}
