using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UE4View.UE4.UAsset.Export
{
    class UDataTable : UAsset
    {
        public UDataTable(byte[] data, int version) : base(data, version)
        {
            Seek((int)ExportMap[0].SerialOffset);

            var RowStruct = ToObject<FPropertyTag>();
            var RowClass = RowStruct.ToProperty(this); // ObjectProperty of RowStruct -> ImportMap
            var Separator = ToName(); // Probably other properties of UDataTable, meh
            var UnknownData = ToInt32();
            if (UnknownData != 0)
                Debugger.Break();

            return;
        }

        public void ReadRows(TextWriter str)
        {
            var RowCount = ToInt32();
            foreach (var i in Enumerable.Range(0, RowCount))
                ReadDataTableRow(str);
        }

        private void ReadDataTableRow(TextWriter str)
        {
            var RowName = ToName();
            str.WriteLine("\nRow {0}:", RowName);
            FPropertyTag.WriteAll(this, str);
        }
    }
}
