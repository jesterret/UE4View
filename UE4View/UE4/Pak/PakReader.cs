using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Pak
{
    class PakReader : IDisposable
    {
        const char Splitter = ']';
        public PakReader(string path)
        {
            ParentDirectory = path;
            var game = Directory.GetParent(path).Parent.Name;
            var files = Directory.GetFiles(path, "*.pak");
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                var pak = new FPakFile(file);
                {
                    foreach (var kv in pak.AbsoluteIndex)
                    {
                        var pakEntry = kv.Value;
                        //Index.Add(kv.Key, pakEntry);
                        AbsoluteIndex.Add(filename + Splitter + kv.Key, pakEntry);
                    }
                    Paks.Add(pak);
                }
            }
            //Index.ToString();
        }
        
        static public (string PakFile, string PakPath) SplitPakName(string name)
        {
            var spl = name.Split(Splitter);
            return (spl[0], spl[1]);
        }
        public byte[] ReadData(string file)
        {
            (var pak, var name) = SplitPakName(file);
            return Paks.Where(p => p.OriginalFileName == pak).Single().ReadEntryByAbsoluteName(name);
        }
        public string ParentDirectory { get; private set; }
        public List<FPakFile> Paks { get; } = new List<FPakFile>();
        public Dictionary<string, FPakEntry> AbsoluteIndex { get; } = new Dictionary<string, FPakEntry>();
        FFileIndex Index
        {
            get
            {
                //if (_fileIndex == null)
                //{
                //    _fileIndex = new FFileIndex();
                //    foreach (var kv in AbsoluteIndex)
                //        _fileIndex.Add(kv.Key, kv.Value);
                //}

                return _fileIndex;
            }
        }
        FFileIndex _fileIndex = null;

        public void Dispose()
        {
            Paks.ForEach(pak => pak.Dispose());
            Paks.Clear();
            AbsoluteIndex.Clear();
            _fileIndex?.Clear();
        }
    }
}
