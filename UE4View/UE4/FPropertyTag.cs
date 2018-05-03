﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UE4View.UE4.PropertyTypes;

namespace UE4View.UE4
{

    [DebuggerDisplay("{Type} {Name}")]
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
        public long PropertyEnd;
        public bool IsNative = false;

        public static IEnumerable<FPropertyTag> ReadToEnd(FArchive reader)
        {
            FPropertyTag tag = null;
            while((tag = reader.ToObject<FPropertyTag>()).Name != "None")
            {
                yield return tag;
            }
        }

        public override void Serialize(FArchive reader)
        {
            Name = reader.ToName();
            if (Name == "None")
                return;

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

            PropertyEnd = reader.Tell() + Size;
        }

        private Type GetPropertyType()
        {
            if (UProperty.PropertyTypes.Any(kv => kv.Key == Type))
                return UProperty.PropertyTypes.Single(kv => kv.Key == Type).Value;
            else
                return UProperty.PropertyTypes.Where(t => t.Key.StartsWith(Type)).Select(t => t.Value).SingleOrDefault();
        }
        public UProperty ToProperty(FArchive reader)
        {
            if (GetPropertyType() is Type type)
            {
                if (type.ContainsGenericParameters)
                    type = type.MakeGenericType(UProperty.PropertyTypes[InnerType]);

                var prop = Activator.CreateInstance(type) as UProperty;
                prop.Serialize(reader, this);
                return prop;
            }
            else
                throw new ArgumentException($"Unknown type \"{Type}\", implement handling for that shit.");
        }
    }
}
