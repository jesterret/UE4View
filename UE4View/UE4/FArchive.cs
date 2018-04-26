using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UE4View.UE4
{
    public class FArchive : IDisposable
    {
        int offset;
        byte[] buffer;

        public static implicit operator FArchive(byte[] data)
        {
            return new FArchive(data);
        }

        public FArchive(byte[] data)
        {
            buffer = data ?? new byte[0];
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
                var str = enc.GetString(buffer, offset, len - enc.GetByteCount(" "));
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
            else
            {
                var HistoryType = ToByte();
                switch (HistoryType)
                {
                    case 0: // case ETextHistoryType::Base:

                        var Namespace = ToFString();
                        var Key = ToFString();
                        SourceString = ToFString();
                        if (!Localization.LocalizationManager.Empty() && Localization.LocalizationManager.TryGetValue(Namespace, out var KeyTable))
                        {
                            if (KeyTable.TryGetValue(Key, out var Entry))
                            {
                                SourceString = Entry.LocalizedString;
                            }
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
                        break;
                }
            }
            return SourceString;
        }
        public byte[] ToByteArray(int arraySize)
        {
            BoundCheck(arraySize);

            var data = new byte[arraySize];
            Array.Copy(buffer, offset, data, 0, arraySize);
            offset += arraySize;
            return data;
        }
        public List<T> ToArray<T>() where T : USerializable, new()
        {
            BoundCheck(sizeof(int)); // check if atleast able to read the array size

            var len = ToInt32();
            return ToArray<T>(len);
        }
        public List<T> ToArray<T>(int len) where T : USerializable, new()
        {
            var items = new List<T>(len);
            if (len > 0)
                items.AddRange(Enumerable.Range(0, len).Select(i => ToObject<T>()));

            return items;
        }
        public T ToObject<T>() where T : USerializable, new()
        {
            var obj = new T();
            obj.Serialize(this);
            return obj;
        }
        public Guid ToGuid()
        {
            return new Guid(ToByteArray(16));
        }
        public long ToInt64()
        {
            BoundCheck(sizeof(long));

            var val = BitConverter.ToInt64(buffer, offset);
            offset += sizeof(long);
            return val;
        }
        public ulong ToUInt64()
        {
            BoundCheck(sizeof(ulong));

            var val = BitConverter.ToUInt64(buffer, offset);
            offset += sizeof(ulong);
            return val;
        }
        public int ToInt32()
        {
            BoundCheck(sizeof(int));

            var val = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            return val;
        }
        public uint ToUInt32()
        {
            BoundCheck(sizeof(uint));

            var val = BitConverter.ToUInt32(buffer, offset);
            offset += sizeof(uint);
            return val;
        }
        public short ToInt16()
        {
            BoundCheck(sizeof(short));

            var val = BitConverter.ToInt16(buffer, offset);
            offset += sizeof(short);
            return val;
        }
        public ushort ToUInt16()
        {
            BoundCheck(sizeof(ushort));

            var val = BitConverter.ToUInt16(buffer, offset);
            offset += sizeof(ushort);
            return val;
        }
        public byte ToByte()
        {
            BoundCheck(sizeof(byte));

            return buffer[offset++];
        }
        public bool ToBoolean()
        {
            return ToInt32() != 0;
        }
        public float ToFloat()
        {
            BoundCheck(sizeof(float));

            var val = BitConverter.ToSingle(buffer, offset);
            offset += sizeof(float);
            return val;
        }
        public double ToDouble()
        {
            BoundCheck(sizeof(double));

            var val = BitConverter.ToDouble(buffer, offset);
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
            return buffer.Length;
        }

        public int Version { get; set; }

        [System.Diagnostics.Conditional("DEBUG")]
        protected void BoundCheck(int size) => BoundCheck(offset, size);

        [System.Diagnostics.Conditional("DEBUG")]
        protected void BoundCheck(int off, int size)
        {
            if (off + size > buffer.Length)
                throw new IndexOutOfRangeException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    buffer = null;

                disposedValue = true;
            }
        }
        
        public void Dispose() => Dispose(true);
        #endregion
    }
}
