using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UE4View.UE4.Localization;
using UE4View.UE4.Pak;
using UE4View.UE4.UAsset;
using UE4View.UE4.VorbisBank;
using UE4View.UE4.VorbisBank.Xml;

namespace UE4View
{
    class AssetResolver
    {
        public delegate byte[] GetFileBytes(string name);

        private int GetCookedAssetVersion()
        {
            var dir = Path.Combine(FarNet.Far.Api.CurrentDirectory, "CookedIniVersion.txt");
            if (File.Exists(dir))
            {
                var FileContent = File.ReadAllLines(dir);
                var verline = FileContent?.Where(line => line.Contains("VersionedIniParams=PackageFileVersions")).SingleOrDefault();
                if (verline != null)
                    return int.Parse(verline.Split(':')[1]);
            }
            return 0;
        }

        public static FarNet.Explorer Explorer(string file, GetFileBytes getFileBytes)
        {
            var ext = Path.GetExtension(file);
            if (ext == ".pak")
            {
                var dir = Path.GetDirectoryName(file);
                //using (var reader = new PakReader(dir)) ;
                return new PakExplorer(new FPakFile(file));
            }
            var data = getFileBytes(file);
            switch (ext)
            {
                case ".bnk":
                    {
                        try
                        {
                            SoundBankInfo info = null;
                            var infoPath = Path.Combine(Path.GetDirectoryName(file), "SoundbanksInfo.xml");
                            var bankinfo = getFileBytes(infoPath);
                            if (bankinfo != null)
                            {
                                var doc = new XmlDocument();
                                var xml = Encoding.UTF8.GetString(bankinfo);
                                doc.LoadXml(xml);
                                info = new SoundBankInfo(doc);
                            }
                            return new BankExplorer(new BankFile(data, info));
                        }
                        catch (FileNotFoundException)
                        {
                            return new BankExplorer(new BankFile(data));
                        }
                    }
                case ".uasset":
                    {
                        // check whether we can find a localization in this directory
                        //if (Directory.EnumerateFiles(FarNet.Far.Api.CurrentDirectory, "*.locres").FirstOrDefault() is string found)
                        //    LocalizationManager.Load(File.ReadAllBytes(found));

                        //var uexps = file.Replace(".uasset", ".uexp");
                        //try
                        //{
                        //    data = data.Concat(getFileBytes(uexps)).ToArray();
                        //}
                        //catch (FileNotFoundException)
                        //{
                        //}

                        //using (var db = File.CreateText(file + ".log"))
                        //    new UAsset(data).GetObject().Read(db);
                    }
                    break;

                default:
                    break;
            }

            return null;
        }

        public static void Exporter(string outPath, string file, GetFileBytes getFileBytes)
        {
            (var pak, var pathFileName) = PakReader.SplitPakName(file);
            var outFile = Path.Combine(outPath, pathFileName);
            var outDir = Path.GetDirectoryName(outFile);
            var ext = Path.GetExtension(pathFileName);
            switch(ext)
            {
                case ".h":
                case ".txt":
                case ".ini":
                case ".json":
                case ".uplugin":
                case ".uproject":
                case ".upluginmanifest":
                    var bytes = getFileBytes(file);
                    if (bytes != null)
                    {
                        Directory.CreateDirectory(outDir);
                        File.WriteAllBytes(outFile, getFileBytes(file));
                    }
                    // dump text to file
                    break;
                case ".locmeta":
                    LocalizationManager.LoadMeta(getFileBytes(file)); // nothing interesting there tbh
                    break;
                case ".locres":
                    // parse localization
                    if (file.Contains("/en/")) // tbh don't give a fuck about other localizations, too much problems for git...
                    {
                        LocalizationManager.Load(getFileBytes(file));
                        Directory.CreateDirectory(outDir);
                        using (var sw = File.CreateText(outFile))
                        {
                            foreach (var ns in LocalizationManager.Get().Namespaces)
                            {
                                sw.WriteLine("NAMESPACE {0}", ns.Key);
                                foreach (var entry in ns.Value)
                                {
                                    sw.WriteLine("    {0}", entry.Value.LocalizedString);
                                }
                                sw.WriteLine("NAMESPACE END {0}", ns.Key);
                            }
                        }
                    }
                    break;
                case ".uasset":
                    Directory.CreateDirectory(outDir);

                    var data = getFileBytes(file);
                    var uexps = file.Replace(".uasset", ".uexp");
                    try
                    {
                        data = data.Concat(getFileBytes(uexps)).ToArray();
                    }
                    catch (FileNotFoundException)
                    {
                    }
                    catch(ArgumentNullException)
                    {
                    }

                    using (var asset = new UAsset(data))
                    {
                        using (var obj = asset.GetObject())
                        {
                            if (obj != null)
                                using (var wr = File.CreateText(outFile))
                                    obj.Read(wr);
                        }
                    }
                    break;

                default: // skip anything else
                    break;
            }
        }
    }
}
