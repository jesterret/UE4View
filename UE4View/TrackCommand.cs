using FarNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE4View.UE4.Pak;

namespace UE4View
{
    [System.Runtime.InteropServices.Guid("dc1d3a1f-22da-4aae-954b-cf07f971ceff")]
    [ModuleCommand(Name = "UE4 Tracker", Prefix = "UE4Track")]
    public class TrackCommand : ModuleCommand
    {
        public override void Invoke(object sender, ModuleCommandEventArgs e)
        {
            Task.Run(() =>
            {
                var outDir = Directory.GetDirectories(e.Command).Where(dir => dir.ToLowerInvariant().Contains("track")).FirstOrDefault() ?? Directory.CreateDirectory(Path.Combine(e.Command, "UE4Track")).FullName;
                using (var reader = new PakReader(e.Command))
                {
                    using (var summary = File.CreateText(Path.Combine(outDir, "summary.txt")))
                    {
                        foreach (var file in reader.AbsoluteIndex.OrderBy(kv => kv.Key))
                        {
                            AssetResolver.Exporter(outDir, file.Key, (string f) => reader.ReadData(f));
                            summary.WriteLine("{0} - sha1: {1}", PakReader.SplitPakName(file.Key).PakPath, file.Value.GetSha1());
                        }
                    }
                }
                // Temporary hack to cleanup after
                Task.Run(() =>
                {
                    GC.Collect();
                });
            });
        }
    }
}
