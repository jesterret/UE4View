using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UE4View.UE4.UAsset.Export
{
    class DataTable : UObject
    {
        object RowStruct;
        FObjectResource TableType;

        List<KeyValuePair<string, UObject>> Table { get; } = new List<KeyValuePair<string, UObject>>();

        public DataTable(UAsset reader) : base(reader)
        {
            RowStruct = TaggedVars.Find(idx => idx.Key == "RowStruct").Value;
            // ObjectProperty Index for this DataTable type
            TableType = reader.ImpExp(reader.ToInt32());

            ReadRows(reader);
        }

        public override void Read(TextWriter wr)
        {
            base.Read(wr);
            foreach(var row in Table)
            {
                wr.WriteLine("\nRow {0}:", row.Key);
                row.Value.Read(wr);
            }
        }

        public void ReadRows(UAsset reader)
        {
            if (RowStruct != null)
            {
                var RowCount = reader.ToInt32();
                foreach (var i in Enumerable.Range(0, RowCount))
                    ReadDataTableRow(reader);
            }
        }

        private void ReadDataTableRow(UAsset reader)
        {
            var RowName = reader.ToName();
            Table.Add(new KeyValuePair<string, UObject>(RowName, new UObject(reader)));
        }
    }
}
