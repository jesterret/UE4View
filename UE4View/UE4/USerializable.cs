namespace UE4View.UE4
{
    public abstract class USerializable
    {
        public abstract FArchive Serialize(FArchive reader);
    }
}
