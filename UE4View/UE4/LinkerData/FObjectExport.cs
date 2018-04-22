using System;
using UE4View.UE4.UAsset;

namespace UE4View.UE4
{
    public class FObjectExport : FObjectResource
    {
        public int ClassIndex;
        public int SuperIndex;
        public int ObjectFlags;
        public long SerialSize;
        public long SerialOffset;
        public bool bForcedExport;
        public bool bNotForClient;
        public bool bNotForServer;
        public Guid PackageGuid;
        public uint PackageFlags;
        public bool bNotAlwaysLoadedForEditorGame;
        public bool bIsAsset;
        public int FirstExportDependency;
        public int SerializationBeforeSerializationDependencies;
        public int CreateBeforeSerializationDependencies;
        public int SerializationBeforeCreateDependencies;
        public int CreateBeforeCreateDependencies;

        public FObjectExport(FArchive reader) : base(reader)
        {
        }

        public override void Serialize(FArchive reader)
        {
            ClassIndex = reader.ToInt32();
            SuperIndex = reader.ToInt32();

            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_TemplateIndex_IN_COOKED_EXPORTS)
            {
                var TemplateIndex = reader.ToInt32();
            }
            OuterIndex = reader.ToInt32();
            ObjectName = reader.ToName();

            ObjectFlags = reader.ToInt32();

            if(reader.Version < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_64BIT_EXPORTMAP_SERIALSIZES)
            {
                SerialSize = reader.ToInt32();
                SerialOffset = reader.ToInt32();
            }
            else
            {
                SerialSize = reader.ToInt64();
                SerialOffset = reader.ToInt64();
            }
            
            bForcedExport = reader.ToBoolean();
            bNotForClient = reader.ToBoolean();
            bNotForServer = reader.ToBoolean();
            PackageGuid = reader.ToGuid();
            PackageFlags = reader.ToUInt32();

            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_LOAD_FOR_EDITOR_GAME)
                bNotAlwaysLoadedForEditorGame = reader.ToBoolean();

            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_COOKED_ASSETS_IN_EDITOR_SUPPORT)
                bIsAsset = reader.ToBoolean();

            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_PRELOAD_DEPENDENCIES_IN_COOKED_EXPORTS)
            {
                FirstExportDependency = reader.ToInt32();
                SerializationBeforeSerializationDependencies = reader.ToInt32();
                CreateBeforeSerializationDependencies = reader.ToInt32();
                SerializationBeforeCreateDependencies = reader.ToInt32();
                CreateBeforeCreateDependencies = reader.ToInt32();
            }
            
        }
    }
}
