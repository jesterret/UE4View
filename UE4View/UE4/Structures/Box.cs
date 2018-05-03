using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE4View.UE4.UAsset.Export;

namespace UE4View.UE4.Structures
{
    class Box : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var obj = new UObject();
            obj.AddVar("Min", StructReader.Read(reader, new Vector()));
            obj.AddVar("Max", StructReader.Read(reader, new Vector()));
            obj.AddVar("IsValid", reader.ToByte());
            Value = obj;
        }
    }
}
