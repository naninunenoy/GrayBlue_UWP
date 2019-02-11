using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMUObserverCore.BLE {
    class BLEException : Exception {
        public BLEException() { }
        public BLEException(string message) : base(message) { }
        public BLEException(string message, Exception inner) : base(message, inner) { }
    }

    class GattConnectionException : BLEException {
        public GattConnectionException() { }
        public GattConnectionException(string message) : base(message) { }
        public GattConnectionException(string message, Exception inner) : base(message, inner) { }
    }
}
