using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UE4View.UE4
{
    public class FPackageFileSummary : USerializable
    {
        public FPackageFileSummary(FArchive reader) : base(reader)
        {
        }

        const long PACKAGE_TAG_MAGIC = 0x9E2A83C1;
        const long PACKAGE_TAG_MAGIC_SWAPPED = 0xC1832A9E;

        public uint Tag;
        public int FileVersionUE4;
        public int FileVersionLicenseeUE4;
        // public FCustomVersionContainer CustomVersionContainer;
        public int TotalHeaderSize;
        public uint PackageFlags;
        public string FolderName;
        public int NameCount;
        public int NameOffset;
        public int GatherableTextDataCount;
        public int GatherableTextDataOffset;
        public int ExportCount;
        public int ExportOffset;
        public int ImportCount;
        public int ImportOffset;
        public int DependsOffset;
        public int SoftPackageReferencesCount;
        public int SoftPackageReferencesOffset;
        public int SearchableNamesOffset;
        public int ThumbnailTableOffset;
        public Guid Guid;
        public List<FGenerationInfo> Generations = new List<FGenerationInfo>();
        public FEngineVersion SavedByEngineVersion;
        public FEngineVersion CompatibleWithEngineVersion;
        public uint CompressionFlags;
        public uint PackageSource;
        // public TArray<FCompressedChunk> CompressedChunks;
        // public TArray<FString> AdditionalPackagesToCook;
        public bool bUnversioned;
        // public TArray<int> TextureAllocations;
        public int AssetRegistryDataOffset;
        public long BulkDataStartOffset;
        public int WorldTileInfoDataOffset;

        // public TArray<int> ChunkIDs;

        public override void Serialize(FArchive reader)
        {
            const long MinimumPackageSize = 32;
            if (reader.Length() < MinimumPackageSize)
                return;

            Tag = reader.ToUInt32();
            if (Tag != PACKAGE_TAG_MAGIC)
                return;

            const int CurrentLegacyFileVersion = -7;
            int LegacyFileVersion = reader.ToInt32();
            if (LegacyFileVersion < 0)
            {
                if (LegacyFileVersion < CurrentLegacyFileVersion)
                {
                    FileVersionUE4 = 0;
                    FileVersionLicenseeUE4 = 0;
                    return;
                }

                if (LegacyFileVersion != -4)
                {
                    // skip legacy ue3 version
                    reader.ToInt32();
                }

                FileVersionUE4 = reader.ToInt32();
                FileVersionLicenseeUE4 = reader.ToInt32();

                if (LegacyFileVersion <= -2)
                {
                    // custom version container
                    var count = reader.ToInt32();
                    // it's TSet, count + new elements[count]
                    if (count > 0)
                        Debugger.Break();
                }

                if (FileVersionUE4 == 0 && FileVersionLicenseeUE4 == 0 && reader.Version == 0)
                {
#if DEBUG
                    // no version information
                    var input = FarNet.Far.Api.CreateListMenu();
                    input.Title = "This file is unversioned, select version";
                    input.NoInfo = false;
                    input.ShowAmpersands = true;

                    var values = ObjectVersion.VersionDictionary.Reverse();
                    foreach (var name in values)
                        input.Add(name.Key);

                    input.Show();
                    
                    if (input.Selected != -1)
                        FileVersionUE4 = (int)values.ElementAt(input.Selected).Value;
                    else
                        FileVersionUE4 = (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_AUTOMATIC_VERSION;
#else
                    
                    FileVersionUE4 = (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_COMPRESSED_SHADER_RESOURCES; // 4.13 for now
#endif
                    FileVersionLicenseeUE4 = 0;
                    bUnversioned = true;
                }
                else
                    FileVersionUE4 = reader.Version; // version read from config file
            }
            else
            {
                FileVersionUE4 = FileVersionLicenseeUE4 = 0;
            }

            TotalHeaderSize = reader.ToInt32();
            FolderName = reader.ToFString();
            PackageFlags = reader.ToUInt32();

            NameCount = reader.ToInt32();
            NameOffset = reader.ToInt32();

            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_SERIALIZE_TEXT_IN_PACKAGES)
            {
                GatherableTextDataCount = reader.ToInt32();
                GatherableTextDataOffset = reader.ToInt32();
            }

            ExportCount = reader.ToInt32();
            ExportOffset = reader.ToInt32();
            ImportCount = reader.ToInt32();
            ImportOffset = reader.ToInt32();
            DependsOffset = reader.ToInt32();

            if (FileVersionUE4 < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_OLDEST_LOADABLE_PACKAGE)
                return;

            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_ADD_STRING_ASSET_REFERENCES_MAP)
            {
                SoftPackageReferencesCount = reader.ToInt32();
                SoftPackageReferencesOffset = reader.ToInt32();
            }
            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_ADDED_SEARCHABLE_NAMES)
            {
                SearchableNamesOffset = reader.ToInt32();
            }

            ThumbnailTableOffset = reader.ToInt32();
            Guid = reader.ToGuid();

            var GenerationCount = reader.ToInt32();
            foreach (var i in Enumerable.Range(0, GenerationCount))
            {
                var gen = new FGenerationInfo(reader);
                Generations.Add(gen);
            }

            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_ENGINE_VERSION_OBJECT)
                SavedByEngineVersion = new FEngineVersion(reader);
            else
            {
                var EngineChangelist = reader.ToUInt32();
                SavedByEngineVersion = new FEngineVersion(4, 0, 0, EngineChangelist, string.Empty);
            }

            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_PACKAGE_SUMMARY_HAS_COMPATIBLE_ENGINE_VERSION)
                CompatibleWithEngineVersion = new FEngineVersion(reader);
            else
                CompatibleWithEngineVersion = SavedByEngineVersion;

            CompressionFlags = reader.ToUInt32();

            var CompressedChunks = reader.ToInt32();
            if (CompressedChunks != 0)
                return;

            PackageSource = reader.ToUInt32();

            var packages = reader.ToInt32();

            if(LegacyFileVersion > -7)
            {
                var NumTextureAllocations = reader.ToInt32();
                if (NumTextureAllocations != 0) // Not supported
                    return;
            }

            AssetRegistryDataOffset = reader.ToInt32();
            BulkDataStartOffset = reader.ToInt64();

            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_WORLD_LEVEL_INFO)
                WorldTileInfoDataOffset = reader.ToInt32();

            if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_CHANGED_CHUNKID_TO_BE_AN_ARRAY_OF_CHUNKIDS)
            {
                var ChunkIDs = reader.ToInt32(); // Hopefully 0, if not I'll need to implement ToArray<primitive>()
                if (ChunkIDs != 0)
                    Debugger.Break();
            }
            else if (FileVersionUE4 >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_ADDED_CHUNKID_TO_ASSETDATA_AND_UPACKAGE)
            {
                // Single Element Array;
            }

            // VER_UE4_PRELOAD_DEPENDENCIES_IN_COOKED_EXPORTS stuff here
            //if (Ar.IsSaving() || Sum.FileVersionUE4 >= VER_UE4_PRELOAD_DEPENDENCIES_IN_COOKED_EXPORTS)
            //{
            //    Ar << Sum.PreloadDependencyCount << Sum.PreloadDependencyOffset;
            //}
            //else
            //{
            //    Sum.PreloadDependencyCount = -1;
            //    Sum.PreloadDependencyOffset = 0;
            //}
        }
    }
}
