using System;
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
            var len = ToInt32();
            if (len != 0)
            {
                var str = Encoding.UTF8.GetString(Buffer, offset, len - 1);
                offset += len;
                return str;
            }
            return string.Empty;
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
            ToByteArray(0x9);
            var LocalizationGuid = ToFString();
            var result = ToFString();
            return result;

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
            //var Flags = ToUInt32();

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
        public T[] ToArray<T>() where T : USerializable
        {
            var len = ToInt32();
            if (len > 0)
            {
                var items = new T[len];
                foreach (var i in Enumerable.Range(0, len))
                    items[i].Serialize(this);

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
