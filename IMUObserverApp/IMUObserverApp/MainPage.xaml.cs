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
                    var uuid = result[0];
                    Core.Plugin.ConnectTo(uuid, this, this);
                }
            });
        }

        public void OnButtonPush(string uuid, string buttonName) {
            Debug.WriteLine($"push {buttonName} {uuid}");
        }

        public void OnButtonRelease(string uuid, string buttonName, float pressTime) {
            Debug.WriteLine($"release {buttonName} {pressTime} {uuid}");
        }

        public void OnConnectDone(string uuid) {
            //throw new NotImplementedException();
        }

        public void OnConnectLost(string uuid) {
            //throw new NotImplementedException();
        }

        public void OnConnectTimeout(string uuid) {
            //throw new NotImplementedException();
        }
    }
}
