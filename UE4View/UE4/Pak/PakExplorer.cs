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
        public static FPakFile _internalPakFile;
        Explorer _parent = null;
        List<FarFile> _files = new List<FarFile>();

        public PakExplorer(FPakFile pak) : this(null, pak.Index, pak.OriginalFileName)
        {
            _internalPakFile = pak;
            //new UE4.Localization.LocalizationManager(Pak.ReadEntryByName("/*/Content/Localization/Game/en/*.locres"));
        }
        PakExplorer(Explorer parent, FFileIndex index, string location) : this(parent, location)
        {
            Task.Run(() => ParseFileIndex(index));
        }
        PakExplorer(Explorer parent, string location) : this()
        {
            _parent = parent;
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
            var exp = this._parent as PakExplorer;
            while(exp != null && exp._parent is PakExplorer Parent)
                exp = Parent;

            return exp;
        }

        public override void GetContent(GetContentEventArgs args)
        {
            var filedata = args.File.Data ?? GetFileData(args.File);
            var data = _internalPakFile.ReadEntry(filedata);
            File.WriteAllBytes(args.FileName, data ?? new byte[0]);
        }

        public override Explorer OpenFile(OpenFileEventArgs args)
        {
            var filedata = args.File.Data ?? GetFileData(args.File);
            var filename = args.File.Name;
            if (filedata is FPakEntry entry)
            {
                var data = _internalPakFile.ReadEntry(entry);
                if (filename.EndsWith(".bnk"))
                {
                    var bankinfo = GetFileData("SoundbanksInfo.xml");
                    if (bankinfo != null)
                    {
                        var bankData = _internalPakFile.ReadEntry(bankinfo);
                        var doc = new XmlDocument();
                        var xml = Encoding.UTF8.GetString(bankData);
                        doc.LoadXml(xml);
                        var info = new SoundBankInfo(doc);
                        return new UE4.VorbisBank.BankExplorer(new UE4.VorbisBank.BankFile(data, info));
                    }
                    else
                        return new UE4.VorbisBank.BankExplorer(new UE4.VorbisBank.BankFile(data));
                }
                else if (filename.EndsWith(".uasset") )
                {
                    var uexps = Path.GetFileNameWithoutExtension(filename) + ".uexp";
                    var expData = GetFileData(uexps);
                    if (expData != null)
                        data = data.Concat(_internalPakFile.ReadEntry(expData)).ToArray();

                    new UAsset(data, GetCookedAssetVersion());
                }
            }
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
            if (_parent != null)
                args.PostData = this;
            else
            {
                args.PostName = _internalPakFile.OriginalFileName;
                _internalPakFile.Close();
                Far.Api.Panel.Close(); // Cleanup our panel after
            }
            return _parent;
        }

        public override void ExportFiles(ExportFilesEventArgs args)
        {
            foreach (var file in args.Files)
                ExportFileEntry(file, Path.Combine(args.DirectoryName, file.Name));
        }
        
        private void ParseFileIndex(FFileIndex index)
        {
            _files.AddRange(index.Directories.Select(dir =>
                new SetFile()
                {
                    Name = dir.Key,
                    Data = new PakExplorer(this, dir.Value, dir.Key),
                    IsDirectory = true,
                }));
            _files.AddRange(index.Files);

            // Should never happen, but who knows...
            _files.ForEach(f =>
            {
                if (FarNet.Works.Kit.IsInvalidFileName(f.Name))
                    f.Name = FarNet.Works.Kit.FixInvalidFileName(f.Name);
            });
        }

        private void ExportFileEntry(FarFile file, string dir)
        {
            var filedata = file.Data ?? GetFileData(file);
            if (file.IsDirectory)
            {
                Directory.CreateDirectory(dir);
                if (filedata is PakExplorer index)
                    index._files.ForEach(f => ExportFileEntry(f, Path.Combine(dir, f.Name)));
            }
            else if (file.Name.EndsWith(".bnk") && filedata is FPakEntry entry)
            {
                UE4.VorbisBank.BankFile bank = null;
                var data = _internalPakFile.ReadEntry(entry);
                var bankinfo = _files.Where(f => f.Name == "SoundbanksInfo.xml").SingleOrDefault();
                if (bankinfo != null)
                {
                    var bankData = _internalPakFile.ReadEntry(bankinfo.Data);
                    var doc = new XmlDocument();
                    var xml = Encoding.UTF8.GetString(bankData);
                    doc.LoadXml(xml);
                    var info = new SoundBankInfo(doc);
                    bank = new UE4.VorbisBank.BankFile(data, info);
                }
                else
                    bank = new UE4.VorbisBank.BankFile(data);

                File.WriteAllBytes(dir, data);
                var folder = Path.Combine(Path.GetDirectoryName(dir), Path.GetFileNameWithoutExtension(dir));
                Directory.CreateDirectory(folder);
                bank.Files.ForEach(track => File.WriteAllBytes(Path.Combine(folder, track.Name), bank.ReadTrack(track)));
            }
            else
            {
                var data = _internalPakFile.ReadEntry(filedata);
                if (data != null)
                    File.WriteAllBytes(dir, data);
            }
        }

        private int GetCookedAssetVersion()
        {
            var entry = _internalPakFile.ReadEntryByName("*/CookedIniVersion.txt");
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
            if (_internalPakFile.IsPakMagic())
                return new Panel(this)
                {
                    Title = _internalPakFile.OriginalFileName,
                    SortMode = PanelSortMode.Name,
                    ViewMode = PanelViewMode.Wide,
                    DotsMode = PanelDotsMode.Dots,
                    UseSortGroups = true,
                };
            else
                Far.Api.Message("Not an UE4 PAK file format");

            return null;
        }
        
        private object GetFileData(FarFile file) => GetFileData(file.Name);
        private object GetFileData(string name) => _files.Where(f => f.Name == name).SingleOrDefault()?.Data;

        public override IList<FarFile> GetFiles(GetFilesEventArgs args) => _files;
    }
}
