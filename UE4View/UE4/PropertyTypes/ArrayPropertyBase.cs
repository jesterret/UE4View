using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    abstract class ArrayPropertyBase : UProperty
    {
        public object this[int i] => ((List<object>)Value)[i];
        public int Count => ((List<object>)Value).Count;
    }
}
