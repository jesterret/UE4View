using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4
{
    public class FString : USerializable
    {
        string Value { get; set; }

        public static implicit operator string(FString str)
        {
            return str.Value;
        }
        public static implicit operator FString(string str)
        {
            return new FString()
            {
                Value = str
            };
        }

        public override void Serialize(FArchive reader)
        {
            Value = reader.ToFString();
        }

        public override string ToString() => Value;
    }
}
