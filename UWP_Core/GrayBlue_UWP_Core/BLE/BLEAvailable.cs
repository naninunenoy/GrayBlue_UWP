using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace IMUObserverCore.BLE {
    internal static class BLEAvailable {
        public static async Task<bool> ChackAsync() {
            var radios = await Radio.GetRadiosAsync();
            var bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }
    }
}
