using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class ByteProperty : UProperty
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            // TODO: Figure out how to do it without throwing exceptions maybe?
            var off = reader.Tell();
            try
            {
                if (!tag.IsNative)
                    Value = reader.ToName();
                else
                    Value = reader.ToByte();
            }
            catch
            {
                tag.IsNative = true;
                reader.Seek(off);
                Value = reader.ToByte();
            }
        }
    }
}
