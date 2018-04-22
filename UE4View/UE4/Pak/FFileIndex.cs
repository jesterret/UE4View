using FarNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UE4View.UE4.Pak
{
    [DebuggerDisplay("{Directories.Count} dirs, {Files.Count} files")]
    public class FFileIndex
    {
        public void Add(string path, in FPakEntry entry)
        {
            var parts = path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            Add(parts, entry);
        }
        void Add(IEnumerable<string> parts, FPakEntry entry)
        {
            var first = parts.First();
            if (parts.Count() > 1)
            {
                if (!Directories.ContainsKey(first))
                    Directories.Add(first, new FFileIndex());

                Directories[first].Add(parts.Skip(1), entry);
            }
            else
                Files.Add(new SetFile()
                {
                    Name = first,
                    Data = entry,
                    Length = entry.Size,
                });
        }

        internal void Clear()
        {
            foreach(var dir in Directories)
                dir.Value.Clear();
            Directories.Clear();
            Files.Clear();
        }

        public Dictionary<string, FFileIndex> Directories { get; } = new Dictionary<string, FFileIndex>();

        public List<FarFile> Files { get; } = new List<FarFile>();
    }
}
