using System;
using System.Collections.Generic;
using System.Linq;

namespace UE4View.UE4.UAsset
{
    class ExportTypes
    {
        static List<Type> Types = typeof(ExportTypes).Assembly.GetTypes()
                                                       .Where(t => t.IsSubclassOf(typeof(USerializable))) // TODO: Change which type I'm gonna use? Or should I just use namespace?
                                                       .ToList();
    }
}
