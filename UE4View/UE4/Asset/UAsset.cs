using FarNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UE4View.UE4.UAsset.Export;

namespace UE4View.UE4.UAsset
{
    public class UAsset : FArchive
    {
        protected FPackageFileSummary Summary;
        List<string> NameMap;
        protected List<FObjectExport> ExportMap;
        protected List<FObjectImport> ImportMap;
        List<FGatherableTextData> GatherableTextDataMap;
        List<FString> StringAssetReferences;

        public FObjectResource ImpExp(int Index)
        {
            if (Index >= 0)
            {
                if (Index < ExportMap.Count)
                    return ExportMap[Index];
            }
            else
            {
                var i = -Index - 1;
                if (i < ImportMap.Count)
                    return ImportMap[-Index - 1];
            }
            return null;
        }

        public UObject GetObject()
        {
            var asset = GetAsset();
            if (asset == null)
                return null;

            var ObjectClass = ImpExp(asset.ClassIndex).ObjectName;            
            Seek((int)asset.SerialOffset);

            try
            {
                if (UObject.Classes.TryGetValue(ObjectClass, out Type type))
                {
                    var obj = Activator.CreateInstance(type) as UObject;
                    obj.Serialize(this);
                    return obj;
                }
                else
                    return new UObject(this);
            }
            catch
            {
                Debug.WriteLine(ObjectClass);
                return null;
            }
        }

        public UAsset(byte[] data, int version) : base(data)
        {
            Version = version;
            ReadSummary();

            Seek(Summary.NameOffset);
            foreach (var i in Enumerable.Range(0, Summary.NameCount))
                NameMap.Add(new FNameEntrySerialized(this).Name);

            Seek(Summary.GatherableTextDataOffset);
            foreach (var i in Enumerable.Range(0, Summary.GatherableTextDataCount))
            {
                Debugger.Break();
                // check whether it reads stuff properly
                GatherableTextDataMap.Add(new FGatherableTextData(this));
            }
            Seek(Summary.ImportOffset);
            foreach (var i in Enumerable.Range(0, Summary.ImportCount))
                ImportMap.Add(new FObjectImport(this));

            Seek(Summary.ExportOffset);
            foreach (var i in Enumerable.Range(0, Summary.ExportCount))
                ExportMap.Add(new FObjectExport(this));

            Seek(Summary.SoftPackageReferencesOffset);
            StringAssetReferences.AddRange(ToArray<FString>(Summary.SoftPackageReferencesCount));

            // TODO: Fixup imports & exports from indexes to names, match exports with exporter object
            foreach (var imp in ImportMap)
            {
                if(imp.ClassName == "Package")
                    imp.XObject = $"{imp.ClassName} {imp.ObjectName}";
                else
                    imp.XObject = string.Format("{0} {1}.{2}", imp.ClassName, ImpExp(imp.OuterIndex).ObjectName, imp.ObjectName);
            }
        }

        public UAsset(byte[] data) : this(data, 0)
        {
        }

        private void ReadSummary()
        {
            Summary = new FPackageFileSummary(this);
            Version = Summary.FileVersionUE4;
            NameMap = new List<string>(Summary.NameCount);
            ExportMap = new List<FObjectExport>(Summary.ExportCount);
            ImportMap = new List<FObjectImport>(Summary.ImportCount);
            GatherableTextDataMap = new List<FGatherableTextData>(Summary.GatherableTextDataCount);
            StringAssetReferences = new List<FString>(Summary.SoftPackageReferencesCount);
        }

        public override string ToName()
        {
            var NameIndex = ToInt32();
            var Number = ToInt32();
            if (NameMap.Count > NameIndex && NameIndex >= 0)
            {
                return NameMap[NameIndex];
            }
            if (NameIndex == -1)
                return base.ToName();

            throw new ArgumentOutOfRangeException(nameof(NameIndex));
        }

        public FObjectExport GetAsset()
        {
            if(Version < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_COOKED_ASSETS_IN_EDITOR_SUPPORT)
                return ExportMap.Where(ex => ex.ObjectFlags.HasFlag(FObjectExport.EObjectFlags.RF_Standalone)).SingleOrDefault();
            else
                return ExportMap.Where(ex => ex.bIsAsset).SingleOrDefault();
        }

        public FObjectExport GetCode()
        {
            var asset = GetAsset();
            var code = ExportMap.Where(exp => exp.ObjectName == asset.ObjectName + "_C").SingleOrDefault();
            return code;
        }
    }
}
