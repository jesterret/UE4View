using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE4View.UE4.PropertyTypes;

namespace UE4View.UE4.UAsset.Export
{
    class UObject
    {
        protected Dictionary<string, UProperty> TaggedVars { get; } = new Dictionary<string, UProperty>();
        public UObject(FObjectExport exp)
        {
            foreach(var prop in FPropertyTag.ReadToEnd(null))
            {
                TaggedVars.Add(prop.Name, prop.ToProperty(null));
            }

            //using (var wr = File.CreateText(Path.Combine(Far.Api.CurrentDirectory, exp.ObjectName + ".log")))
            //{
            //    try
            //    {
            //        FPropertyTag.WriteAll(this, wr);
            //        var nativeSize = exp.SerialOffset + exp.SerialSize - Tell();
            //        if (nativeSize > 0)
            //        {
            //            wr.WriteLine("Found {0} bytes of native data.", nativeSize);
            //            wr.Write(BitConverter.ToString(ToByteArray((int)nativeSize)).Replace("-", string.Empty));
            //        }
            //    }
            //    catch
            //    {
            //        wr.Flush();
            //    }
            //}
        }


    }
}
