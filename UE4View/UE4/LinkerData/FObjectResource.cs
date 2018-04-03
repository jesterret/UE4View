using System.Diagnostics;

namespace UE4View.UE4
{
    [DebuggerDisplay("ObjectName: {ObjectName}")]
    public abstract class FObjectResource : USerializable
    {
        public string ObjectName;
        public int OuterIndex;
    }
}
