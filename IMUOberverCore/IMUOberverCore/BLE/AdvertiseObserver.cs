using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using Windows.Devices.Bluetooth.Advertisement;

namespace IMUOberverCore.BLE {
    class AdvertiseObserver : IDisposable {
        const int advertiseImtervalMS = 3000;
        private BluetoothLEAdvertisementWatcher advertiseWatcher;
        private Subject<BluetoothLEAdvertisement> advertiseSubject;

        public AdvertiseObserver() {
            advertiseWatcher = new BluetoothLEAdvertisementWatcher();
            advertiseSubject = new Subject<BluetoothLEAdvertisement>();
            advertiseWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(advertiseImtervalMS);
            advertiseWatcher.Received += OnAdvertisementReceived;
        }

        public void Dispose() {
            advertiseWatcher.Stop();
            advertiseWatcher.Received -= OnAdvertisementReceived;
            advertiseSubject.Dispose();
        }

        public IObservable<BluetoothLEAdvertisement> AdvertiseAsync() {
            advertiseWatcher.Stop();
            advertiseSubject.Dispose();
            advertiseWatcher.Start();
            return advertiseSubject.AsObservable();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) {
            advertiseSubject.OnNext(args.Advertisement);
        }
    }
}
