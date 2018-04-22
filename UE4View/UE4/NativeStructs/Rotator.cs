using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class Rotator : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            var Pitch = reader.ToInt32();
            var Roll = reader.ToInt32();
            var Yaw = reader.ToInt32();
            Value = $"Rotator [Pitch={Pitch}, Roll={Roll}, Yaw={Yaw}]";
        }
    }
}
