using System;
using System.Collections.Generic;
using System.Linq;
using UE4View.UE4.PropertyTypes;

namespace UE4View.UE4.NativeStructs
{
    public abstract class UStruct : UProperty
    {
        public static Dictionary<string, Type> Structures { get; } = typeof(UStruct).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(UStruct))).ToDictionary(t => t.Name);
    }
}
