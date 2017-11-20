using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UE4View.UE4
{
    public class FPropertyTag : USerializable
    {
        public string Name;        // Name of property
        public string Type;        // Type of property
        public byte BoolVal;       // a boolean property's value (never need to serialize data for bool properties except here)
        public string StructName;  // Struct name if UStructProperty
        public string EnumName;    // Enum name if UByteProperty or UEnumProperty
        public string InnerType;   // Inner type if UArrayProperty, USetProperty, or UMapProperty
        public string ValueType;   // Value type if UMapPropery
        public int Size;           // Property size.
        public int ArrayIndex;     // Index if an array; else 0
        public long SizeOffset;    // location in stream of tag size member
        public Guid StructGuid;
        public byte HasPropertyGuid;
        public Guid PropertyGuid;

        public int PropertyStartOffset;

        public static void WriteAll(FArchive source, TextWriter destination)
        {
            foreach (var prop in ReadToEnd(source))
            {
                destination.Write("{0}: ", prop.Name);
                var PropValue = prop.ToProperty(source);
                if (PropValue is List<object> array)
                {
                    destination.Write("[ ");
                    foreach (var i in Enumerable.Range(0, array.Count))
                    {
                        destination.Write(array[i]);
                        if (i < array.Count - 1)
                            destination.Write(", ");
                    }
                    destination.WriteLine(" ]");
                }
                else
                    destination.WriteLine(PropValue);
            }
        }
        public static IEnumerable<FPropertyTag> ReadToEnd(FArchive reader)
        {
            FPropertyTag tag = null;
            while((tag = reader.ToObject<FPropertyTag>()).Name != "None")
            {
                yield return tag;
            }
        }

        public override FArchive Serialize(FArchive reader)
        {
            Name = reader.ToName();
            if (Name == "None")
                return reader;

            SizeOffset = reader.Tell();
            Type = reader.ToName();
            Size = reader.ToInt32();
            ArrayIndex = reader.ToInt32();

            PropertyStartOffset = reader.Tell();

            if(Type == "StructProperty")
            {
                StructName = reader.ToName();
                if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_STRUCT_GUID_IN_PROPERTY_TAG)
                {
                    StructGuid = reader.ToGuid();
                }
            }
            else if (Type == "BoolProperty")
            {
                BoolVal = reader.ToByte();
            }
            else if (Type == "ByteProperty" || Type == "EnumProperty")
            {
                EnumName = reader.ToName();
            }
            else if (Type == "ArrayProperty")
            {
                if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VAR_UE4_ARRAY_PROPERTY_INNER_TAGS)
                {
                    InnerType = reader.ToName();
                }
            }

            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_PROPERTY_TAG_SET_MAP_SUPPORT)
            {
                if (Type == "SetProperty")
                {
                    InnerType = reader.ToName();
                }
                else if (Type == "MapProperty")
                {
                    InnerType = reader.ToName();
                    ValueType = reader.ToName();
                }
            }

            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_PROPERTY_GUID_IN_PROPERTY_TAG)
            {
                HasPropertyGuid = reader.ToByte();
                if(HasPropertyGuid != 0)
                {
                    PropertyGuid = reader.ToGuid();
                }
            }

            return reader;
        }
        public object ToProperty(FArchive reader)
        {
            if (!string.IsNullOrEmpty(Type))
            {
                if (Types.Where(t => t.Key.StartsWith(Type)).Select(t => t.Value).FirstOrDefault() is Type type)
                {
                    if (type.ContainsGenericParameters)
                    {
                        var t = type.MakeGenericType(Types[InnerType]);
                        dynamic prop = Activator.CreateInstance(t);
                        prop.Serialize(reader, this);
                        return prop.Value;
                    }
                    else
                    {
                        dynamic prop = Activator.CreateInstance(type);
                        prop.Serialize(reader, this);
                        return prop.Value;
                    }
                }
                else
                    throw new ArgumentException($"Unknown type \"{Type}\", implement handling for that shit.");
            }
            return null;
        }

        private static Dictionary<string, Type> Types { get; } = typeof(PropertyTypes.UPropertyBase).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PropertyTypes.UPropertyBase))).ToDictionary(t => t.Name);
    }
}
