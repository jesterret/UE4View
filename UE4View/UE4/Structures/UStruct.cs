using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UE4View.UE4.PropertyTypes;

namespace UE4View.UE4.Structures
{
    [DebuggerDisplay("{Value}")]
    public class UStruct
    {
        public virtual void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            reader.Seek((int)tag.PropertyEnd);
            Value = "[Skipped]";
        }

        public object Value { get; protected set; }

        public override string ToString() => Value.ToString();
        public static Dictionary<string, Type> NativeTypes { get; } = typeof(UStruct).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(UStruct))).ToDictionary(t => t.Name);
    }
}
