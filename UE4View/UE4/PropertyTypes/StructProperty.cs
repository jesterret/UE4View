using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class StructProperty : UProperty<string>
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            using (var wr = new StringWriter())
            {
                wr.WriteLine("{");
                FPropertyTag.WriteAll(reader, wr);
                wr.Write("}");
                Value = wr.ToString().Replace(Environment.NewLine, Environment.NewLine + "\t");
            }
            return reader;
        }
    }
}
