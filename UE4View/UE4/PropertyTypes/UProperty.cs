using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    public abstract class UProperty<T> : UPropertyBase
    {
        public T Value { get; protected set; }
        
        public override string ToString() => Value.ToString();
    }
}
