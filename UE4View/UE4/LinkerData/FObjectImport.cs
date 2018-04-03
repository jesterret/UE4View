using System.Diagnostics;

namespace UE4View.UE4
{
    [DebuggerDisplay("ClassName: {ClassName} ObjectName: {ObjectName}")]
    public class FObjectImport : FObjectResource
    {
        public string ClassPackage;
        public string ClassName;
        public object XObject;
        public object SourceLinker;
        public int SourceIndex;
        public override FArchive Serialize(FArchive reader)
        {
            ClassPackage = reader.ToName();
            ClassName = reader.ToName();
            OuterIndex = reader.ToInt32();
            ObjectName = reader.ToName();

            XObject = SourceLinker = null;
            SourceIndex = 0;
            return reader;
        }
    }
}
