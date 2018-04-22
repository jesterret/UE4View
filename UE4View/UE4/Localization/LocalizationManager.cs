using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FKeysTable = System.Collections.Generic.Dictionary<string, UE4View.UE4.Localization.FEntry>;

namespace UE4View.UE4.Localization
{
    public class LocalizationManager : USerializable
    {
        private enum ELocResVersion
        {
            /** Legacy format file - will be missing the magic number. */
            Legacy = 0,
            /** Compact format file - strings are stored in a LUT to avoid duplication. */
            Compact = 1,

            LatestPlusOne,
            Latest = LatestPlusOne - 1
        }
        private enum ELocMetaVersion
        {
            /** Initial format. */
            Initial = 0,

            LatestPlusOne,
            Latest = LatestPlusOne - 1
        }

        public static Guid LocMetaMagic = new Guid(0xA14CEE4F, 0x4868, 0x8355, 0x6C, 0x4C, 0x46, 0xBD, 0x70, 0xDA, 0x50, 0x7C);
        public static Guid LocResMagic = new Guid(0x7574140E, 0x4A67, 0xFC03, 0x4A, 0x15, 0x90, 0x9D, 0xC3, 0x37, 0x7F, 0x1B);


        public static LocalizationManager Get() => _manager;
        public static bool TryGetValue(string key, out FKeysTable value) => Get().Namespaces.TryGetValue(key, out value);
        public static void Load(FArchive reader) => Get().Serialize(reader);
        public static void LoadMeta(FArchive reader)
        {
            var loc = Get();
            ELocMetaVersion VersionNumber = ELocMetaVersion.Initial;
            Guid MagicNumber = reader.ToGuid();
            if (MagicNumber != LocMetaMagic)
                return;

            VersionNumber = (ELocMetaVersion)reader.ToByte();

            if (VersionNumber > ELocMetaVersion.Latest)
                return;

            loc.NativeCulture = reader.ToFString();
            loc.NativeLocRes = reader.ToFString();
        }
        public static bool Empty() => Get().Namespaces.Count == 0;
        public override void Serialize(FArchive reader)
        {
            if (Empty())
            {
                Guid MagicNumber = reader.ToGuid();
                ELocResVersion VersionNumber = ELocResVersion.Legacy;
                if (MagicNumber == LocResMagic)
                {
                    VersionNumber = (ELocResVersion)reader.ToByte();
                }
                else
                    reader.Seek(0); // legacy LocRes

                if(VersionNumber > ELocResVersion.Latest)
                {
                    // Check for changes in the new version and implement them...
                    Debugger.Break();
                    return;
                }

                List<FString> LocalizedStringArray = null;
                if (VersionNumber >= ELocResVersion.Compact)
                {
                    long LocalizedStringArrayOffset = reader.ToInt64();
                    if(LocalizedStringArrayOffset != -1)
                    {
                        var offset = reader.Tell();
                        reader.Seek((int)LocalizedStringArrayOffset);
                        LocalizedStringArray = reader.ToArray<FString>();
                        reader.Seek(offset);
                    }
                }

                var NamespaceCount = reader.ToUInt32();
                foreach (var i in Enumerable.Range(0, (int)NamespaceCount))
                {
                    var NamespaceName = reader.ToFString();
                    var KeyCount = reader.ToUInt32();
                    var KeyTable = Namespaces.FindOrAdd(NamespaceName);
                    foreach (var j in Enumerable.Range(0, (int)KeyCount))
                    {
                        var Key = reader.ToFString();
                        var Entry = KeyTable.FindOrAdd(Key);
                        
                        Entry.SourceStringHash = reader.ToInt32();
                        if (VersionNumber >= ELocResVersion.Compact)
                        {
                            var LocalizedStringIndex = reader.ToInt32();
                            if(LocalizedStringIndex > -1 && LocalizedStringIndex < LocalizedStringArray.Count)
                            {
                                Entry.LocalizedString = LocalizedStringArray[LocalizedStringIndex];
                            }
                        }
                        else
                        { 
                            Entry.LocalizedString = reader.ToFString();
                        }
                    }
                }
            }
        }

        private static readonly LocalizationManager _manager = new LocalizationManager();

        public Dictionary<string, FKeysTable> Namespaces { get; } = new Dictionary<string, FKeysTable>();

        public string NativeCulture { get; private set; }
        public string NativeLocRes { get; private set; }
    }
}
