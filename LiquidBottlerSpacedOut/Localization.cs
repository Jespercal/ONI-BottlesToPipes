using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STRINGSS = STRINGS;

namespace Alesseon.LiquidBottler
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class ALESSEON
                {
                    public class LIQUIDBOTTLER
                    {
                        public static LocString NAME = "Liquid Bottle Filler";
                        public static LocString DESC = "Allow Duplicants to fetch bottled liquids for delivery to buildings.";
                        public static LocString EFFECT = "Automatically stores piped <link=\"ELEMENTSLIQUID\">Liquid</link> into bottles for manual transport.";
                    }
                    public class LIQUIDBOTTLEEMPTIER
                    {
                        public static LocString NAME = "Liquid bottle emptier";
                        public static LocString DESC = "Allows emptying bottles directly to the pipe system.";
                        public static LocString EFFECT = "Automatically empties <link=\"ELEMENTSLIQUID\">Liquid</link> from bottles for pipe transport.";
                    }
                    public class GASCANISTEREMPTIER
                    {
                        public static LocString NAME = "Gas Canister emptier";
                        public static LocString DESC = "Allows emptying canisters directly to the vent system.";
                        public static LocString EFFECT = "Automatically empties <link=\"ELEMENTSGAS\">Gas</link> from canisters for vent transport.";
                    }
                }
            }
            public class STATUSITEMS
            {
                public class GAS_EMPTIER
                {
                    public static class ALLOWED
                    {
                        public static LocString NAME = "Auto-Canister: On";

                        public static LocString TOOLTIP = string.Concat("Duplicants may specifically fetch ", STRINGSS.UI.PRE_KEYWORD, "Gas", STRINGSS.UI.PST_KEYWORD, " from a ", STRINGSS.BUILDINGS.PREFABS.GASBOTTLER.NAME, " to bring to this location");
                    }

                    public static class DENIED
                    {
                        public static LocString NAME = "Auto-Bottle: Off";

                        public static LocString TOOLTIP = string.Concat("Duplicants may not specifically fetch ", STRINGSS.UI.PRE_KEYWORD, "Gas", STRINGSS.UI.PST_KEYWORD, " from a ", STRINGSS.BUILDINGS.PREFABS.GASBOTTLER.NAME, " to bring to this location");
                    }
                }
            }
        }
        public class UI
        {
            public class USERMENUACTIONS
            {
                public class AUTO_PUMP_DROP
                {
                    public class DENIED
                    {
                        public static LocString NAME = "Enable auto drop";
                        public static LocString TOOLTIP = "Drop the fluid";
                        public static LocString TOOLTIP2 = "Drop the gas";
                    }
                    public class ALLOWED
                    {
                        public static LocString NAME = "Disable auto drop";
                        public static LocString TOOLTIP = "Auto drop disabled";
                    }
                }
            }
        }
    }
}
