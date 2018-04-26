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
    
    class ObjectProperty : UProperty
    {
        string ExportName { get; set; }
        private string ReadResource()
        {
            var stringbuilder = new StringBuilder();
            {
                using (var stream = new StringWriter(stringbuilder))
                {
                    stream.WriteLine("{0}: {{\t", ExportName);
                    ((UObject)Value).Read(stream);
                    stringbuilder.Replace(stream.NewLine, stream.NewLine + "\t");
                    stream.Write("}");
                    return stream.ToString();
                }
            }
        }
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var index = reader.ToInt32();
            Value = index;
            if (reader is UAsset.UAsset asset)
            {
                var resource = asset.ImpExp(index);
                if (resource is FObjectExport exp)
                {
                    var loc = reader.Tell();
                    if (loc > exp.SerialOffset && loc < exp.SerialOffset + exp.SerialSize)
                        Value = "this";
                    else
                    {
                        // Preventing recursive deserialization by marking asset as WIP
                        if ((exp.ObjectFlags & FObjectExport.EObjectFlags.RF_WillBeLoaded) != FObjectExport.EObjectFlags.RF_WillBeLoaded)
                        {
                            exp.ObjectFlags |= FObjectExport.EObjectFlags.RF_WillBeLoaded;
                            reader.Seek((int)exp.SerialOffset);
                            ExportName = exp.ObjectName;
                            Value = new UObject(reader);
                            reader.Seek(loc);
                        }
                    }
                }
                else if (resource is FObjectImport imp)
                    Value = imp.XObject;
            }
            
        }
        public override string ToString()
        {
            if (ExportName != null)
                return ReadResource();
            else
                return base.ToString();
        }
    }
}
