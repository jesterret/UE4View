using FarNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UE4View.UE4.Pak;
using UE4View.UE4.UAsset;
using UE4View.UE4.VorbisBank.Xml;

namespace UE4View
{
    public class PakExplorer : Explorer
    {
        public static FPakFile InspectedFile;
        Panel PakViewPanel;

        Explorer Parent = null;
        List<FarFile> _files = new List<FarFile>();

        public PakExplorer(FPakFile Pak) : this(null, Pak.Index, Pak.OriginalFileName)
        {
            InspectedFile = Pak;
            //new UE4.Localization.LocalizationManager(Pak.ReadEntryByName("/*/Content/Localization/Game/en/*.locres"));
        }
        PakExplorer(Explorer Parent, FFileIndex Index, string Loc) : this(Parent, Loc)
        {
            Task.Run(() => ParseFileIndex(Index));
        }
        PakExplorer(Explorer parent, string location) : this()
        {
            Parent = parent;
            Location = Path.Combine(parent?.Location ?? string.Empty, location);
        }
        PakExplorer() : base(new Guid("dc1d3a1f-22da-4aae-954b-cf07f971cf01"))
        {
            CanOpenFile = true;
            CanGetContent = true;
            CanExportFiles = true;
            CanExploreLocation = true;
        }


        public override Explorer ExploreRoot(ExploreRootEventArgs args)
        {
            var exp = this.Parent as PakExplorer;
            while(exp != null && exp.Parent is PakExplorer Parent)
                exp = Parent;

            return exp;
        }

        public override void GetContent(GetContentEventArgs args)
        {
            var filedata = args.File.Data ?? GetFileData(args.File);
            var data = InspectedFile.ReadEntry(filedata);
            File.WriteAllBytes(args.FileName, data ?? new byte[0]);
        }

        public override Explorer OpenFile(OpenFileEventArgs args)
        {
            var filedata = args.File.Data ?? GetFileData(args.File);
            if (args.File.Name.EndsWith(".bnk") && filedata is FPakEntry entry)
            {
                var data = InspectedFile.ReadEntry(entry);
                var bankinfo = _files.Where(f => f.Name == "SoundbanksInfo.xml").SingleOrDefault();
                if (bankinfo != null)
                {
                    var bankData = InspectedFile.ReadEntry(bankinfo.Data);
                    var doc = new XmlDocument();
                    var xml = Encoding.UTF8.GetString(bankData);
                    doc.LoadXml(xml);
                    var info = new SoundBankInfo(doc);
                    return new UE4.VorbisBank.BankExplorer(new UE4.VorbisBank.BankFile(data, info));
                }
                else
                    return new UE4.VorbisBank.BankExplorer(new UE4.VorbisBank.BankFile(data));
            }
            else if (args.File.Name.EndsWith(".uasset") && filedata is FPakEntry asset)
            {
                var data = InspectedFile.ReadEntry(filedata);
                new UAsset(data, GetCookedAssetVersion());
                return null;
            }
            else
                return null;
        }

        public override Explorer ExploreLocation(ExploreLocationEventArgs args)
        {
            var dirs = args.Location.Replace(this.Location, string.Empty).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var exp = this;
            foreach(var dir in dirs)
            {
                var file = exp._files.Where(f => f.IsDirectory && f.Name == dir).SingleOrDefault();
                if (file != null && file.Data is PakExplorer found)
                    exp = found;
                else if (dir == "..")
                {
                    exp = exp.ExploreParent(new ExploreParentEventArgs(args.Mode)) as PakExplorer;
                    if (exp == null)
                        break;
                }
                else
                    break;

            }
            return exp == this ? null : exp;
        }

        public override Explorer ExploreParent(ExploreParentEventArgs args)
        {
            if (Parent != null)
                args.PostData = this;
            else
            {
                args.PostName = InspectedFile.OriginalFileName;
                InspectedFile.Close();
                PakViewPanel.Close();
            }
            return Parent;
        }

        public override void ExportFiles(ExportFilesEventArgs args)
        {
            foreach (var file in args.Files)
                ExportFileEntry(file, Path.Combine(args.DirectoryName, file.Name));
        }
        
        private void ParseFileIndex(FFileIndex Index)
        {
            _files.AddRange(Index.Directories.Select(dir =>
                new SetFile()
                {
                    Name = dir.Key,
                    Data = new PakExplorer(this, dir.Value, dir.Key),
                    IsDirectory = true,
                }));
            _files.AddRange(Index.Files.Select(entry =>
                new SetFile()
                {
                    Name = entry.Key,
                    Data = entry.Value,
                    Length = entry.Value.Size,
                }));

            // Should never happen, but who knows...
            _files.ForEach(f =>
            {
                if (FarNet.Works.Kit.IsInvalidFileName(f.Name))
                    f.Name = FarNet.Works.Kit.FixInvalidFileName(f.Name);
            });
        }

        private void ExportFileEntry(FarFile file, string Dir)
        {
            var filedata = file.Data ?? GetFileData(file);
            if (file.IsDirectory)
            {
                Directory.CreateDirectory(Dir);
                if (filedata is PakExplorer index)
                    index._files.ForEach(f => ExportFileEntry(f, Path.Combine(Dir, f.Name)));
            }
            else if (file.Name.EndsWith(".bnk") && filedata is FPakEntry entry)
            {
                UE4.VorbisBank.BankFile bank = null;
                var data = InspectedFile.ReadEntry(entry);
                var bankinfo = _files.Where(f => f.Name == "SoundbanksInfo.xml").SingleOrDefault();
                if (bankinfo != null)
                {
                    var bankData = InspectedFile.ReadEntry(bankinfo.Data);
                    var doc = new XmlDocument();
                    var xml = Encoding.UTF8.GetString(bankData);
                    doc.LoadXml(xml);
                    var info = new SoundBankInfo(doc);
                    bank = new UE4.VorbisBank.BankFile(data, info);
                }
                else
                    bank = new UE4.VorbisBank.BankFile(data);

                File.WriteAllBytes(Dir, data);
                var folder = Path.Combine(Path.GetDirectoryName(Dir), Path.GetFileNameWithoutExtension(Dir));
                Directory.CreateDirectory(folder);
                bank.Files.ForEach(track => File.WriteAllBytes(Path.Combine(folder, track.Name), bank.ReadTrack(track)));
            }
            else
            {
                var data = InspectedFile.ReadEntry(filedata);
                if (data != null)
                    File.WriteAllBytes(Dir, data);
            }
        }

        private int GetCookedAssetVersion()
        {
            var entry = InspectedFile.ReadEntryByName("*/CookedIniVersion.txt");
            if (entry != null)
            {
                var content = Encoding.UTF8.GetString(entry).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var line = content.Where(str => str.Contains("VersionedIniParams=PackageFileVersions")).SingleOrDefault();
                if (line != null)
                    return int.Parse(line.Split(':')[1]);
            }
            return 0;
        }

        public override Panel CreatePanel()
        {
            if (InspectedFile.Info.Magic == FPakInfo.PakFile_Magic)
            {
                PakViewPanel = new Panel(this)
                {
                    Title = InspectedFile.OriginalFileName,
                    SortMode = PanelSortMode.Name,
                    ViewMode = PanelViewMode.Wide,
                    DotsMode = PanelDotsMode.Dots,
                    UseSortGroups = true,
                };
                return PakViewPanel;
            }
            else
                Far.Api.Message("Not an UE4 PAK file format");

            return null;
        }

        private object GetFileData(FarFile file) => _files.Where(f => f.Name == file.Name).Single().Data;

        public override IList<FarFile> GetFiles(GetFilesEventArgs args) => _files;
    }
}
