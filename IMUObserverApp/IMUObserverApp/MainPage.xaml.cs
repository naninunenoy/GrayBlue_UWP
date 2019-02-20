using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Diagnostics;
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


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace IMUObserverApp {
    using Core = IMUObserverCore;
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page, Core.IConnectionDelegate, Core.INotifyDelegate {

        public MainPage() {
            this.InitializeComponent();
            Debug.WriteLine("MainPage");

            var blescan = Core.Plugin.Scan();

            Task.Run(async () => {
                var result = await blescan;
                Debug.WriteLine($"found {result.Length} devices. {string.Join(",", result)}");
                if (result.Length > 0) {
                    var deviceId = result[0];
                    Core.Plugin.ConnectTo(deviceId, this, this);
                }
            });
        }

        public void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            // Do Nothing
        }

        public void OnButtonPush(string deviceId, string buttonName) {
            Debug.WriteLine($"push {buttonName} {deviceId}");
        }

        public void OnButtonRelease(string deviceId, string buttonName, float pressTime) {
            Debug.WriteLine($"release {buttonName} {pressTime} {deviceId}");
        }

        public void OnConnectDone(string deviceId) {
            Debug.WriteLine($"OnConnectDone {deviceId}");
        }

        public void OnConnectFail(string deviceId) {
            Debug.WriteLine($"OnConnectFail {deviceId}");
        }

        public void OnConnectLost(string deviceId) {
            Debug.WriteLine($"OnConnectLost {deviceId}");
        }
    }
}
