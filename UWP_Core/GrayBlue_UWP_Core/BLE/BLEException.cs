using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayBlueUWPCore.BLE {
    class BLEException : Exception {
        public BLEException() { }
        public BLEException(string message) : base(message) { }
        public BLEException(string message, Exception inner) : base(message, inner) { }
    }
}
