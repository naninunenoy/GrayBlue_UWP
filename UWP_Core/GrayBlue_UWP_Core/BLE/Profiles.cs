using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayBlueUWPCore.BLE {
    internal static class Profiles {
        public static class Services {
            const string button = "de4c3b20-26ea-11e9-ab14-d663bd873d93";
            const string nineAxis = "c87ace96-3523-11e9-b210-d663bd873d93";
            public static readonly Guid Button = new Guid(button);
            public static readonly Guid NineAxis = new Guid(nineAxis);
        }

        public static class Characteristics {
            const string buttonOperation = "de4c4016-26ea-11e9-ab14-d663bd873d93";
            const string nineAxisIMUData = "c87ad148-3523-11e9-b210-d663bd873d93";
            public static readonly Guid ButtonOperation = new Guid(buttonOperation);
            public static readonly Guid NineAxisIMUData = new Guid(nineAxisIMUData);
        }
    }
}
