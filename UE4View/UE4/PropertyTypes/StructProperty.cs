using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class StructProperty : UProperty
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            if (NativeStructs.UStruct.Structures.Where(t => t.Key == tag.StructName).Select(t => t.Value).SingleOrDefault() is Type StructType)
            {
                var obj = Activator.CreateInstance(StructType) as NativeStructs.UStruct;
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
                        FPropertyTag.WriteAll(reader, wr);
                    }
                    catch
                    {
                        wr.WriteLine("Structure {0} is expected to have a native deserialization", tag.StructName);
                    }
                    wr.Write("}");
                    Value = wr.ToString().Replace(Environment.NewLine, Environment.NewLine + "\t");
                }
            }
            return reader;
        }
    }
}
