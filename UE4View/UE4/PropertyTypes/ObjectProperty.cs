using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class ObjectProperty : UProperty
    {
        private string ReadResource(FObjectResource res, FArchive reader)
        {
            var stringbuilder = new StringBuilder();
            {
                using (var stream = new StringWriter(stringbuilder))
                {
                    stream.WriteLine("{0}: {{\t", res.ObjectName);
                    FPropertyTag.WriteAll(reader, stream);
                    stringbuilder.Replace(stream.NewLine, stream.NewLine + "\t");
                    stream.Write("}");
                    return stream.ToString();
                }
            }
        }
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var index = reader.ToInt32();
            if (reader is UAsset.UAsset asset)
            {
                var resource = asset.ImpExp(index);
                if (resource is FObjectExport exp)
                {
                    var loc = reader.Tell();
                    reader.Seek((int)exp.SerialOffset);
                    Value = ReadResource(exp, reader);
                    reader.Seek(loc);
                }
                else if (resource is FObjectImport imp)
                {
                    if (imp.ClassName == "ScriptStruct")
                        Value = ReadResource(imp, reader);
                    else
                        Value = imp;
                }
            }
            else
                Value = index;

            return reader;
        }
    }
}
