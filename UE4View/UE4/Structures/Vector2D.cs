using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class Vector2D : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var x = reader.ToFloat();
            var y = reader.ToFloat();
            Value = $"Vector2D [X: {x}, Y: {y}]";
        }
    }
}
