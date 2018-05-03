using System;
using UE4View.UE4.UAsset;

namespace UE4View.UE4
{
    public class FObjectExport : FObjectResource
    {
        public enum EObjectFlags
        {
            RF_NoFlags = 0x00000000,
            RF_Public = 0x00000001,
            RF_Standalone = 0x00000002,
            RF_MarkAsNative = 0x00000004,
            RF_Transactional = 0x00000008,
            RF_ClassDefaultObject = 0x00000010,
            RF_ArchetypeObject = 0x00000020,
            RF_Transient = 0x00000040,
            RF_MarkAsRootSet = 0x00000080,
            RF_TagGarbageTemp = 0x00000100,
            RF_NeedInitialization = 0x00000200,
            RF_NeedLoad = 0x00000400,
            RF_KeepForCooker = 0x00000800,
            RF_NeedPostLoad = 0x00001000,
            RF_NeedPostLoadSubobjects = 0x00002000,
            RF_NewerVersionExists = 0x00004000,
            RF_BeginDestroyed = 0x00008000,
            RF_FinishDestroyed = 0x00010000,
            RF_BeingRegenerated = 0x00020000,
            RF_DefaultSubObject = 0x00040000,
            RF_WasLoaded = 0x00080000,
            RF_TextExportTransient = 0x00100000,
            RF_LoadCompleted = 0x00200000,
            RF_InheritableComponentTemplate = 0x00400000,
            RF_DuplicateTransient = 0x00800000,
            RF_StrongRefOnFrame = 0x01000000,
            RF_NonPIEDuplicateTransient = 0x02000000,
            RF_Dynamic = 0x04000000,
            RF_WillBeLoaded = 0x08000000,
        }

        public int ClassIndex;
        public int SuperIndex;
        public int TemplateIndex;
        public EObjectFlags ObjectFlags;
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
                TemplateIndex = reader.ToInt32();

            OuterIndex = reader.ToInt32();
            ObjectName = reader.ToName();

            ObjectFlags = (EObjectFlags)reader.ToInt32();

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
