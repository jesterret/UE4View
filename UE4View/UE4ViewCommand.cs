using FarNet;
using System;
using System.IO;
using System.Linq;
using UE4View.UE4.Localization;

namespace UE4View
{
    [System.Runtime.InteropServices.Guid("dc1d3a1f-22da-4aae-954b-cf07f971cf00")]
    [ModuleCommand(Name = "UE4 Viewer", Prefix = "UE4View")]
    public class UE4ViewCommand : ModuleCommand
    {
        public override void Invoke(object sender, ModuleCommandEventArgs e)
        {
            var exp = AssetResolver.Explorer(e.Command, File.ReadAllBytes);
            if (exp != null)
                exp.OpenPanel();
            else
                e.Ignore = true;
        }
    }
}
