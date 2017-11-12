using System.Diagnostics;

namespace UE4View.UE4
{
    class FLocMetadataValue : USerializable
    {
        public override FArchive Serialize(FArchive reader)
        {
            int MetaDataTypeAsInt = reader.ToInt32();
            // E:\Source\UnrealEngine\Engine\Source\Runtime\Core\Private\Internationalization\InternationalizationMetadata.cpp:276
            Debugger.Break();
            return reader;
        }
    }
}
