using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Diagnostics;
using IMUOberverCore;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using Windows.Devices.Bluetooth;

using Windows.Devices.Bluetooth.Advertisement;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace IMUObserverApp {
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page {

        public MainPage() {
            this.InitializeComponent();
            Debug.WriteLine("MainPage");

            var core = new IMUOberverCore.IMUOberverCore();
            core.AdvertiseAsync()
                .Subscribe(x => {
                    Debug.WriteLine(x.LocalName + " " + x.ServiceUuids[0]);
                });
            var BleWatcher = new BluetoothLEAdvertisementWatcher {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            BleWatcher.Received += async (w, btAdv) => {
                var device = await BluetoothLEDevice.FromBluetoothAddressAsync(btAdv.BluetoothAddress);
                Debug.WriteLine($"BLEWATCHER Found: {device.Name}");

                // SERVICES!!
                var gatt = await device.GetGattServicesAsync();
                Debug.WriteLine($"{device.Name} Services: {gatt.Services.Count}, {gatt.Status}, {gatt.ProtocolError}");

                // CHARACTERISTICS!!
                //var characs = await gatt.Services.Single(s => s.Uuid == SAMPLESERVICEUUID).GetCharacteristicsAsync();
                //var charac = characs.Single(c => c.Uuid == SAMPLECHARACUUID);
                //await charac.WriteValueAsync(SOMEDATA);
            };

            BleWatcher.Start();

        }

    }
}
