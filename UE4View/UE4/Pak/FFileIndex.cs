using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UE4View.UE4.Pak
{
    public class FFileIndex
    {
        public FFileIndex Add(string Dir, FPakEntry Entry)
        {
            var paths = Dir.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length > 1)
            {
                if (!Directories.ContainsKey(paths[0]))
                    Directories.Add(paths[0], new FFileIndex());

                Directories[paths[0]].Add(Path.Combine(paths.Skip(1).ToArray()), Entry);
            }
            else
                Files.Add(Dir, Entry);
            return this;
        }

        public SortedDictionary<string, FFileIndex> Directories { get; } = new SortedDictionary<string, FFileIndex>();

        public SortedDictionary<string, FPakEntry> Files { get; } = new SortedDictionary<string, FPakEntry>();
    }
}
