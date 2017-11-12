using FarNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UE4View.UE4.VorbisBank
{
    class BankExplorer : Explorer
    {
        BankFile InspectedBank;
        List<FarFile> _files = new List<FarFile>();

        public BankExplorer(BankFile bank) : base(new Guid("dc1d3a1f-22da-4aae-954b-cf07f971cf02"))
        {
            InspectedBank = bank;
            CanGetContent = true;
            Location = "SoundBank";
            _files.AddRange(bank.Files.Select(f => new SetFile()
            {
                Name = f.Name,
                Length = f.Length,
                Data = f
            }));
        }

        public override void EnterPanel(Panel panel)
        {
            panel.Title = "SoundBank View";
            panel.CurrentLocation = this.Location;
            base.EnterPanel(panel);
        }

        public override void GetContent(GetContentEventArgs args)
        {
            File.WriteAllBytes(args.FileName, InspectedBank.ReadTrack(args.File.Data as BankFile.BankTrack));
        }

        public override IList<FarFile> GetFiles(GetFilesEventArgs args)
        {
            return _files;
        }
    }
}
