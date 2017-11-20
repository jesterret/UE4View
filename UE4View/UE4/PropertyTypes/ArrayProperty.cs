using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class ArrayProperty<T> : UProperty<List<object>>
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            var Count = reader.ToInt32();
            var Info = tag;
            Value = new List<object>(Count);
            if (tag.InnerType == "StructProperty")
                Info = reader.ToObject<FPropertyTag>();

            foreach(var i in Enumerable.Range(0, Count))
            {
                dynamic t = Activator.CreateInstance<T>();
                t.Serialize(reader, Info);
                Value.Add(t.Value);
            }
            return reader;
        }
    }
}
