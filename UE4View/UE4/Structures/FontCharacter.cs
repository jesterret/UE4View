using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class FontCharacter : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var StartU = reader.ToInt32();
            var StartV = reader.ToInt32();
            var USize = reader.ToInt32();
            var VSize = reader.ToInt32();
            var TextureIndex = reader.ToByte();
            var VerticalOffset = reader.ToInt32();
            Value = $"FontCharacter [StartU={StartU}, StartV={StartV}, USize={USize}, VSize={VSize}, TextureIndex={TextureIndex}, VerticalOffset={VerticalOffset}]";
        }
    }
}
