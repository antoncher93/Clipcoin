using System;
using System.Collections.Generic;
using System.Text;

namespace Clipcoin.Smartphone.SignalManagement.Signals
{
    public struct GroupID
    {
        public static GroupID FromValues(int minor, int major) 
            => new GroupID { Minor = minor, Major = major };
        public int Minor { get; private set; }
        public int Major { get; private set; }
        public override string ToString()
        {
            return $"[{Minor}, {Major}]";
        }
    }
}
