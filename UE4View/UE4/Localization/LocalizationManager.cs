using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FKeysTable = System.Collections.Generic.Dictionary<string, UE4View.UE4.Localization.FEntry>;

namespace UE4View.UE4.Localization
{
    public class LocalizationManager : USerializable
    {
        public LocalizationManager(byte[] data)
        {
            if(data != null)
                Serialize(data);
        }

        public override FArchive Serialize(FArchive reader)
        {
            if (NamespaceTable.Count == 0)
            {
                var NamespaceCount = reader.ToUInt32();
                foreach (var i in Enumerable.Range(0, (int)NamespaceCount))
                {
                    var NamespaceName = reader.ToFString();
                    var KeyCount = reader.ToUInt32();
                    var KeyTable = NamespaceTable.FindOrAdd(NamespaceName);
                    foreach (var j in Enumerable.Range(0, (int)KeyCount))
                    {
                        var Key = reader.ToFString();
                        var Entry = KeyTable.FindOrAdd(Key);
                        
                        Entry.SourceStringHash = reader.ToInt32();
                        Entry.LocalizedString = reader.ToFString();
                    }
                }
            }
            return reader;
        }

        public static Dictionary<string, FKeysTable> NamespaceTable { get; } = new Dictionary<string, FKeysTable>();
    }
}
