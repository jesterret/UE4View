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
            var RowClass = ToInt32(); // ObjectProperty of RowStruct -> ImportMap
            var Separator = ToName(); // Probably other properties of UDataTable, meh
            var UnknownData = ToByteArray(0x4);
            return;
        }

        public void ReadRows(StreamWriter str)
        {
            var RowCount = ToInt32();

            foreach (var i in Enumerable.Range(0, RowCount))
            {
                ReadDataTableRow(str);
            }
        }

        private void ReadDataTableRow(StreamWriter str)
        {
            // TODO: Still need to change this to actual property system
            var RowName = ToName();
            str.WriteLine("\nRow {0}:", RowName);
            var Prop = ToObject<FPropertyTag>();
            while (Prop.Name != "None")
            {
                str.Write("{0}: ", Prop.Name);
                if (Prop.Type == "StrProperty" || Prop.Type == "NameProperty" || Prop.Type == "TextProperty" || Prop.Type == "IntProperty" || Prop.Type == "AssetObjectProperty")
                {
                    if (Prop.Type == "StrProperty" || Prop.Type == "AssetObjectProperty")
                        str.WriteLine(ToFString());
                    else if (Prop.Type == "NameProperty")
                        str.WriteLine(ToName());
                    else if (Prop.Type == "TextProperty")
                        str.WriteLine(ToFText());
                    else if (Prop.Type == "IntProperty")
                        str.WriteLine(ToInt32());
                }
                else if (Prop.Type == "ArrayProperty")
                {
                    if (Prop.InnerType == "StructProperty")
                    {
                        str.WriteLine("Skipped");
                        Skip(Prop.Size);
                        continue;
                    }
                    var ArrayCount = ToInt32();
                    str.Write(" [ ");
                    foreach (var i in Enumerable.Range(0, ArrayCount))
                    {
                        if (Prop.InnerType == "ByteProperty" || Prop.InnerType == "NameProperty")
                            str.Write(ToName());
                        else if (Prop.InnerType == "IntProperty")
                            str.Write(ToInt32());
                        else if (Prop.InnerType == "FloatProperty")
                            str.Write(ToFloat());
                        else if (Prop.InnerType == "StrProperty")
                            str.Write(ToFString());
                        else
                            break;

                        if (i < ArrayCount - 1)
                            str.Write(", ");
                    }
                    str.WriteLine(" ]");
                }
                else if (Prop.Type == "ByteProperty")
                {
                    var DataSize = ToInt64();
                    var EnumType = ToName();
                    ToByte();
                    var EnumValue = ToName();
                    str.WriteLine("{0}", EnumValue);
                }
                else if (Prop.Type == "FloatProperty")
                {
                    var Value = ToFloat();
                    str.WriteLine(Value);
                }
                else if (Prop.Type == "BoolProperty")
                {
                    str.WriteLine(Prop.BoolVal);
                }
                else if (Prop.Type == "StructProperty")
                {
                    Debugger.Break();
                    str.WriteLine();
                }
                else
                    throw new ArgumentException($"Unknown type \"{Prop.Type}\", implement handling for that shit.");
            }

        }
    }
}
