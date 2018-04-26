using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class MulticastDelegateProperty : UProperty
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var test = reader.ToInt32();
            if (test > 1)
                Debugger.Break();

            var test2 = reader.ToInt32();
            Value = reader.ToName();
            return;
        }
    }
}
