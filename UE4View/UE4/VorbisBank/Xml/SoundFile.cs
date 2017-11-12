using System;
using System.IO;
using System.Xml;

namespace UE4View.UE4.VorbisBank.Xml
{
    public class SoundFile
    {
        public SoundFile(XmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Id = uint.Parse(node.Attributes["Id"].Value);
            ShortName = node["ShortName"].InnerText;
            FilePath = node["Path"].InnerText;
        }

        public uint Id;
        public string ShortName;
        public string FilePath;

        public string GetExtractName()
        {
            return Path.GetFileNameWithoutExtension(ShortName) + Path.GetExtension(FilePath);
        }
    }
}
