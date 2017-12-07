using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UE4View.UE4
{
    public class FArchive
    {
        int offset;
        byte[] Buffer;
        public FArchive(byte[] Data)
        {
            Buffer = Data;
            offset = 0;
        }
        public virtual string ToName()
        {
            return "INDEX_NONE";
        }
        public string ToFString()
        {
            string Str = string.Empty;

            var len = ToInt32();
            // Negative length denotes UTF16
            if (len < 0)
                Str = ToFString(Encoding.Unicode, -len * 2);
            else
                Str = ToFString(Encoding.UTF8, len);

            return Str;
        }
        public string ToFString(Encoding enc, int? Length = null)
        {
            var len = Length != null ? Length.Value : ToInt32();
            if (len != 0)
            {
                var str = enc.GetString(Buffer, offset, len - enc.GetByteCount(" "));
                offset += len;
                return str;
            }
            return string.Empty;
        }
        public string ToFText()
        {
            string SourceString = string.Empty;
            var Flags = ToUInt32();
            if (Version < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_FTEXT_HISTORY)
            {
                var str = ToFString();
                if (Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_ADDED_NAMESPACE_AND_KEY_DATA_TO_FTEXT)
                {
                    var Namespace = ToFString();
                    var Key = ToFString();
                }
                return str;
            }

            if (Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_FTEXT_HISTORY)
            {
                var HistoryType = ToByte();
                bool bSerializeHistory = true;
                switch (HistoryType)
                {
                    case 0: // case ETextHistoryType::Base:
                        if (Localization.LocalizationManager.NamespaceTable.Count > 0)
                        {
                            var Namespace = ToFString();
                            if (Localization.LocalizationManager.NamespaceTable.TryGetValue(Namespace, out var KeyTable))
                            {
                                var Key = ToFString();
                                SourceString = ToFString();
                                if (KeyTable.TryGetValue(Key, out var LocString))
                                {
                                    SourceString = LocString[0].LocalizedString;
                                }
                            }
                        }
                        else
                        {
                            var Namespace = ToFString();
                            var Key = ToFString();
                            SourceString = ToFString();
                        }
                        break;
                    case 1: // case ETextHistoryType::NamedFormat:
                        Debugger.Break();
                        var FormatText = ToFText();

                        break;
                    case 2: // case ETextHistoryType::OrderedFormat:
                        Debugger.Break();
                        break;
                    case 3: // case ETextHistoryType::ArgumentFormat:
                        Debugger.Break();
                        break;
                    case 4: // case ETextHistoryType::AsNumber:
                        Debugger.Break();
                        break;
                    case 5: // case ETextHistoryType::AsPercent:
                        Debugger.Break();
                        break;
                    case 6: // case ETextHistoryType::AsCurrency:
                        Debugger.Break();
                        break;
                    case 7: // case ETextHistoryType::AsDate:
                        Debugger.Break();
                        break;
                    case 8: // case ETextHistoryType::AsTime:
                        Debugger.Break();
                        break;
                    case 9: // case ETextHistoryType::AsDateTime:
                        Debugger.Break();
                        break;
                    case 10: // case ETextHistoryType::Transform:
                        Debugger.Break();
                        break;
                    case 11: // case ETextHistoryType::StringTableEntry:
                        Debugger.Break();
                        break;

                    default:
                        bSerializeHistory = false;
                        break;
                }

                if (bSerializeHistory)
                {
                    // HistoryType object -> serialize
                }
            }
            return SourceString;

            //if (Version < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_FTEXT_HISTORY)
            //{
            //    var str = ToFString();
            //    if (Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_ADDED_NAMESPACE_AND_KEY_DATA_TO_FTEXT)
            //    {
            //        var Namespace = ToFString();
            //        var Key = ToFString();
            //    }
            //    return str;
            //}

            //if (Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_FTEXT_HISTORY)
            //{
            //    var HistoryType = ToByte();
            //    bool bSerializeHistory = true;
            //    if(HistoryType > 11) // FTextHistoryType determining
            //    {
            //        bSerializeHistory = false;
            //    }

            //    if(bSerializeHistory)
            //    {
            //        // HistoryType object -> serialize
            //    }
            //}

            //// Value.Rebuild()

            //// check localization?

            //return string.Empty;
        }
        public byte[] ToByteArray(int ArraySize)
        {
            var data = new byte[ArraySize];
            Array.Copy(Buffer, offset, data, 0, ArraySize);
            offset += ArraySize;
            return data;
        }
        public List<T> ToArray<T>() where T : USerializable, new()
        {
            var len = ToInt32();
            if (len > 0)
            {
                var items = new List<T>(len);
                foreach (var i in Enumerable.Range(0, len))
                    items.Add(ToObject<T>());

                return items;
            }
            return null;
        }
        public T ToObject<T>() where T : USerializable, new()
        {
            var obj = new T();
            obj.Serialize(this);
            return obj;
        }
        public Guid ToGuid()
        {
            var data = new byte[16];
            Array.Copy(Buffer, offset, data, 0, 16);
            offset += 16;
            return new Guid(data);
        }
        public long ToInt64()
        {
            var val = BitConverter.ToInt64(Buffer, offset);
            offset += sizeof(long);
            return val;
        }
        public ulong ToUInt64()
        {
            var val = BitConverter.ToUInt64(Buffer, offset);
            offset += sizeof(ulong);
            return val;
        }
        public int ToInt32()
        {
            var val = BitConverter.ToInt32(Buffer, offset);
            offset += sizeof(int);
            return val;
        }
        public uint ToUInt32()
        {
            var val = BitConverter.ToUInt32(Buffer, offset);
            offset += sizeof(uint);
            return val;
        }
        public short ToInt16()
        {
            var val = BitConverter.ToInt16(Buffer, offset);
            offset += sizeof(short);
            return val;
        }
        public ushort ToUInt16()
        {
            var val = BitConverter.ToUInt16(Buffer, offset);
            offset += sizeof(ushort);
            return val;
        }
        public byte ToByte()
        {
            return Buffer[offset++];
        }
        public bool ToBoolean()
        {
            return ToInt32() != 0;
        }
        public float ToFloat()
        {
            var val = BitConverter.ToSingle(Buffer, offset);
            offset += sizeof(float);
            return val;
        }
        public double ToDouble()
        {
            var val = BitConverter.ToDouble(Buffer, offset);
            offset += sizeof(double);
            return val;
        }


        public FArchive Skip(int len)
        {
            offset += len;
            return this;
        }
        public FArchive Seek(int off)
        {
            offset = off;
            return this;
        }
        public int Tell()
        {
            return offset;
        }
        public int Length()
        {
            return Buffer.Length;
        }

        public int Version { get; set; }
    }
}
