using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4
{
    public abstract class FObjectResource : USerializable
    {
        public string ObjectName;
        public int OuterIndex;

        public override string ToString() => ObjectName;
    }
}
