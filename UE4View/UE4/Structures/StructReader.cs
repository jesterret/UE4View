using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    static class StructReader
    {
        public static object Read(FArchive reader, UStruct str)
        {
            str.Serialize(reader);
            return str.Value;
        }
    }
}
