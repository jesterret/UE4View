using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class Vector2MaterialInput : MaterialInput
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            base.Serialize(reader, tag);
            var UseConst = reader.ToBoolean();
            var Const = StructReader.Read(reader, new Vector2D());

            if (UseConst)
                Value = Const;
        }
    }
}
