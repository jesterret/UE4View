using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UE4View.UE4.UAsset.Export
{
    class UDataTable : UAsset
    {
        object RowStruct;
        FObjectResource TableType;
        public UDataTable(byte[] data) : this(data, 0) { }
        public UDataTable(byte[] data, int version) : base(data, version)
        {
            var table = ExportMap
                .Where(exp => ImpExp(exp.ClassIndex).ObjectName == "DataTable")
                .SingleOrDefault();
            if (table != null)
            {
                Seek((int)table.SerialOffset);
                RowStruct = FPropertyTag.ReadToEnd(this)
                    .Select(prop => prop.ToProperty(this))
                    .ToArray() // Resolve all lazy evaluations
                    .First(); // Interested only in Row Struct type, ignoring any other UDataTable property
                TableType = ImpExp(ToInt32()); // ObjectProperty Index for this DataTable type
            }
        }

        public void ReadRows(in TextWriter stream)
        {
            if (RowStruct != null)
            {
                var RowCount = ToInt32();
                foreach (var i in Enumerable.Range(0, RowCount))
                    ReadDataTableRow(stream);
            }
        }

        private void ReadDataTableRow(TextWriter stream)
        {
            var RowName = ToName();
            stream.WriteLine("\nRow {0}:", RowName);
            FPropertyTag.WriteAll(this, stream);
        }
    }
}
