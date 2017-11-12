using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace UE4View.UE4.VorbisBank.Xml
{
    public class SoundBankInfo
    {
        public SoundBankInfo(XmlDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            // Not parsing not interesing stuff like original paths, events
            var files = doc.SelectNodes(".//StreamedFiles/*");
            StreamedFiles = new List<SoundFile>(files.Count);
            foreach (XmlNode file in files)
                StreamedFiles.Add(new SoundFile(file));

            var media = doc.SelectNodes(".//MediaFilesNotInAnyBank/*");
            if (media.Count > 0)
            {
                Debugger.Break(); // Break to verify whether it actually looks like that, DBD doesn't have it
                MediaFilesNotInAnyBank = new List<SoundFile>(media.Count);
                foreach (XmlNode file in media)
                    MediaFilesNotInAnyBank.Add(new SoundFile(file));
            }

            var banks = doc.SelectNodes(".//SoundBanks/*");
            SoundBanks = new List<SoundBank>(banks.Count);
            foreach (XmlNode bank in banks)
                SoundBanks.Add(new SoundBank(bank));
        }

        List<SoundFile> StreamedFiles;
        List<SoundFile> MediaFilesNotInAnyBank;
        List<SoundBank> SoundBanks;

        public string GetTrackName(uint bankId, uint trackId)
        {
            var Bank = SoundBanks.Where(bank => bank.Id == bankId).Single();
            return Bank.GetFile(trackId).GetExtractName();
        }
    }
}
