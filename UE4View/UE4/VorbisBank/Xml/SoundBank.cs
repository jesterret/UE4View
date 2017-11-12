using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace UE4View.UE4.VorbisBank.Xml
{
    public class SoundBank : SoundFile
    {
        public SoundBank(XmlNode node) : base(node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var files = node.SelectNodes(".//IncludedMemoryFiles/*");
            MemoryFiles = new List<SoundFile>(files.Count);
            foreach (XmlNode file in files)
                MemoryFiles.Add(new SoundFile(file));
        }

        public SoundFile GetFile(uint trackId)
        {
            return MemoryFiles.Where(file => file.Id == trackId).Single();
        }

        List<SoundFile> MemoryFiles;
    }
}
