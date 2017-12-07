using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    public abstract class UStruct
    {
        public object Value;
        public abstract FArchive Serialize(FArchive reader, FPropertyTag tag);

        public static Dictionary<string, Type> Structures { get; } = typeof(UStruct).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(UStruct))).ToDictionary(t => t.Name);
    }
}
