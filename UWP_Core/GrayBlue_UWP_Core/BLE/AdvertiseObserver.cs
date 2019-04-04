using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace GrayBlueUWPCore.BLE {
    internal class AdvertiseObserver : IAdvertiseObserver {
        static readonly TimeSpan interval = TimeSpan.FromSeconds(1);
        static readonly TimeSpan scanLength = TimeSpan.FromSeconds(3);

        private readonly BluetoothLEAdvertisement advertise;
        private readonly BluetoothLEAdvertisementFilter advertiseFilter;
        private readonly BluetoothLEAdvertisementWatcher advertiseWatcher;
        private readonly Subject<BluetoothLEAdvertisementReceivedEventArgs> advertiseSubject;

        public AdvertiseObserver() {
            advertise = new BluetoothLEAdvertisement();
            advertise.ServiceUuids.Add(Profiles.Services.Button);
            advertiseFilter = new BluetoothLEAdvertisementFilter() { Advertisement = advertise };
            advertiseWatcher = new BluetoothLEAdvertisementWatcher() { AdvertisementFilter = advertiseFilter };
            advertiseWatcher.SignalStrengthFilter.SamplingInterval = interval;
            advertiseWatcher.Received += OnAdvertisementReceived;
            advertiseSubject = new Subject<BluetoothLEAdvertisementReceivedEventArgs>();
        }

        public void Dispose() {
            advertiseSubject.Dispose();
            advertiseWatcher.Stop();
            advertiseWatcher.Received -= OnAdvertisementReceived;
        }

        public async Task<IGattDevice[]> ScanAdvertiseDevicesAsync() {
            advertiseWatcher.Start();
            return await advertiseSubject
                .TakeUntil(DateTimeOffset.Now.Add(scanLength))
                .Finally(advertiseWatcher.Stop)
                .Select(arg => { return new GattDevice(arg); })
                .Distinct(x => x.Address)
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
