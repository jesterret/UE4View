namespace UE4View.UE4
{
    public class FObjectImport : USerializable
    {
        public string ClassPackage;
        public string ClassName;
        public int OuterIndex;
        public string ObjectName;
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
