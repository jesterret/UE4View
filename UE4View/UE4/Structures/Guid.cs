using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class Guid : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            Value = reader.ToGuid();
        }
    }
}
