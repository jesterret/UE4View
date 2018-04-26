namespace UE4View.UE4.Structures
{
    class Vector4 : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            float X = reader.ToFloat();
            float Y = reader.ToFloat();
            float Z = reader.ToFloat();
            float W = reader.ToFloat();
            Value = $"Vector4 [X={X}, Y={Y}, Z={Z}, W={W}]";
        }
    }
}
