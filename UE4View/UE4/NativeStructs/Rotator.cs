using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class Rotator : UStruct
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            int Pitch, Roll, Yaw;
            Pitch = reader.ToInt32();
            Roll = reader.ToInt32();
            Yaw = reader.ToInt32();
            Value = string.Format("{{ {0}, {1}, {2} }}", Pitch, Roll, Yaw);
            return reader;
        }
    }
}
