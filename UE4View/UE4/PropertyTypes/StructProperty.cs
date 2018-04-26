using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE4View.UE4.UAsset.Export;

namespace UE4View.UE4.PropertyTypes
{
    class StructProperty : UProperty
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            if (Structures.UStruct.NativeTypes.Where(t => t.Key == tag.StructName).Select(t => t.Value).SingleOrDefault() is Type StructType)
            {
                var obj = Activator.CreateInstance(StructType) as Structures.UStruct;
                obj.Serialize(reader, tag);
                Value = "{ " + obj.Value.ToString().Replace(Environment.NewLine, Environment.NewLine + "\t") + " }";
            }
            else
            {
                using (var wr = new StringWriter())
                {
                    wr.WriteLine("{");
                    try
                    {
                        using (var obj = new UObject(reader))
                            obj.Read(wr);
                    }
                    catch
                    {
                        wr.WriteLine("Structure {0} is expected to have a native deserialization", tag.StructName);
                    }
                    wr.Write("}");
                    Value = wr.ToString().Replace(Environment.NewLine, Environment.NewLine + "\t");
                }
            }
        }
    }
}
