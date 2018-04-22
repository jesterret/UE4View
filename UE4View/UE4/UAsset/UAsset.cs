using FarNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
                return ExportMap[Index];
            else
                return ImportMap[-Index - 1];
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
            //var ufunc = ExportMap.Where(exp => ImpExp(exp.ClassIndex).ObjectName == "Function").ToArray();
            //foreach(var func in ufunc)
            //{
            //    Seek((int)func.SerialOffset);
            //    var tags = FPropertyTag.ReadToEnd(this).ToArray();
            //    if(tags.Length > 0)
            //    {
            //        Debugger.Break();
            //    }
            //    var FunctionFlags = ToUInt32();
            //    if(Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_SERIALIZE_BLUEPRINT_EVENTGRAPH_FASTCALLS_IN_UFUNCTION)
            //    {
            //        var test = FPropertyTag.ReadToEnd(this).ToArray();
            //        return;
            //    }
            //}

            var ExportInfo = GetAsset();
            if (ExportInfo != null)
            {
                Seek((int)ExportInfo.SerialOffset);
                using (var wr = File.CreateText(Path.Combine(Far.Api.CurrentDirectory, ExportInfo.ObjectName + ".log")))
                {
                    try
                    {
                        FPropertyTag.WriteAll(this, wr);
                        var nativeSize = ExportInfo.SerialOffset + ExportInfo.SerialSize - Tell();
                        if (nativeSize > 0)
                        {
                            wr.WriteLine("Found {0} bytes of native data.", nativeSize);
                            wr.Write(BitConverter.ToString(ToByteArray((int)nativeSize)).Replace("-", string.Empty));
                        }
                    }
                    catch
                    {
                        wr.Flush();
                    }
                }
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
            if (NameMap.Count > NameIndex && NameIndex >= 0)
            {
                var Number = ToInt32();
                return NameMap[NameIndex];
            }

            throw new ArgumentOutOfRangeException(nameof(NameIndex));
        }

        public FObjectExport GetAsset()
        {
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
