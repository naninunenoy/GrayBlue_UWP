using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMUObserverCore.BLE {
    internal static class Profiles {
        public static class Services {
            public const string Button = "de4c3b20-26ea-11e9-ab14-d663bd873d93";
        }

        public static class Characteristics {
            const string buttonOperation = "de4c4016-26ea-11e9-ab14-d663bd873d93";
            public static readonly Guid ButtonOperation = new Guid(buttonOperation);
        }
    }
}
