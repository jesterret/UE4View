namespace UE4View.UE4
{
    public abstract class USerializable
    {
        public USerializable() { }
        protected USerializable(FArchive reader) => Serialize(reader);
        public abstract void Serialize(FArchive reader);
    }
}
