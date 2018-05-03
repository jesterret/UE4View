using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class MaterialInput : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var InputName = reader.ToName();
            var Mask = reader.ToInt32();
            var MaskR = reader.ToInt32();
            var MaskG = reader.ToInt32();
            var MaskB = reader.ToInt32();
            var MaskA = reader.ToInt32();
            var ExpressionName = reader.ToName();

            Value = InputName;
        }
    }
}
