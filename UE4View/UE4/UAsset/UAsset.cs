using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace UE4View.UE4.UAsset
{
    public class UAsset : FArchive
    {
        protected FPackageFileSummary Summary;
        List<string> NameMap;
        protected List<FObjectExport> ExportMap;
        protected List<FObjectImport> ImportMap;
        List<FGatherableTextData> GatherableTextDataMap;
        List<string> StringAssetReferences;

        public UAsset(byte[] data, int version) : base(data)
        {
            Version = version;
            ReadSummary();

            Seek(Summary.NameOffset);
            foreach (var i in Enumerable.Range(0, Summary.NameCount))
            {
                var entry = new FNameEntrySerialized();
                entry.Serialize(this);
                NameMap.Add(entry.Name);
            }
            Seek(Summary.GatherableTextDataOffset);
            foreach (var i in Enumerable.Range(0, Summary.GatherableTextDataCount))
            {
                Debugger.Break();
                // check whether it reads stuff properly
                var gatherable = new FGatherableTextData();
                gatherable.Serialize(this);
                GatherableTextDataMap.Add(gatherable);
            }
            Seek(Summary.ImportOffset);
            foreach (var i in Enumerable.Range(0, Summary.ImportCount))
            {
                var imp = new FObjectImport();
                imp.Serialize(this);
                ImportMap.Add(imp);
            }
            Seek(Summary.ExportOffset);
            foreach (var i in Enumerable.Range(0, Summary.ExportCount))
            {
                var exp = new FObjectExport();
                exp.Serialize(this);
                ExportMap.Add(exp);
            }
            Seek(Summary.StringAssetReferencesOffset);
            foreach (var i in Enumerable.Range(0, Summary.StringAssetReferencesCount))
            {
                StringAssetReferences.Add(ToFString());
            }
            
            // TODO: Fixup imports & exports from indexes to names, match exports with exporter object
        }

        public UAsset(byte[] data) : this(data, 0)
        {
        }

        private void ReadSummary()
        {
            Summary = new FPackageFileSummary();
            Summary.Serialize(this);
            Version = Summary.FileVersionUE4;
            NameMap = new List<string>(Summary.NameCount);
            ExportMap = new List<FObjectExport>(Summary.ExportCount);
            ImportMap = new List<FObjectImport>(Summary.ImportCount);
            GatherableTextDataMap = new List<FGatherableTextData>(Summary.GatherableTextDataCount);
            StringAssetReferences = new List<string>(Summary.StringAssetReferencesCount);
        }

        public override string ToName()
        {
            var NameIndex = ToInt32();
            if (NameMap.Count > NameIndex && NameIndex >= 0)
            {
                var Number = ToInt32();
                return NameMap[NameIndex];
            }
            throw new ArgumentOutOfRangeException(nameof(NameIndex));
            //return base.ToName();
        }
    }
}
