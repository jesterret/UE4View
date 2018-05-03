using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    [DebuggerDisplay("{Value}")]
    public abstract class UProperty
    {
        public abstract void Serialize(FArchive reader, FPropertyTag tag = null);
        
        public object Value { get; protected set; }

        public override string ToString() => Value?.ToString();

        public static Dictionary<string, Type> PropertyTypes { get; } = typeof(UProperty).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(UProperty))).ToDictionary(t => t.Name);
    }
}
