using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    public abstract class UPropertyBase
    {
        public abstract FArchive Serialize(FArchive reader, FPropertyTag tag = null);
    }
}
