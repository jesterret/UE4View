using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class RichCurveKey : UStruct
    {
        //bool FRichCurveKey::Serialize(FArchive& Ar)
        //{
        //    if (Ar.UE4Ver() < VER_UE4_SERIALIZE_RICH_CURVE_KEY)
        //    {
        //        return false;
        //    }

        //    // Serialization is handled manually to avoid the extra size overhead of UProperty tagging.
        //    // Otherwise with many keys in a rich curve the size can become quite large.
        //    Ar << InterpMode;
        //    Ar << TangentMode;
        //    Ar << TangentWeightMode;
        //    Ar << Time;
        //    Ar << Value;
        //    Ar << ArriveTangent;
        //    Ar << ArriveTangentWeight;
        //    Ar << LeaveTangent;
        //    Ar << LeaveTangentWeight;

        //    return true;
        //}

        byte InterpMode;
        byte TangentMode;
        byte TangentWeightMode;
        float Time;
        new float Value;
        float ArriveTangent;
        float ArriveTangentWeight;
        float LeaveTangent;
        float LeaveTangentWeight;

        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            if (reader.Version < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_SERIALIZE_RICH_CURVE_KEY)
                return;

            InterpMode = reader.ToByte();
            TangentMode = reader.ToByte();
            TangentWeightMode = reader.ToByte();
            Time = reader.ToFloat();
            Value = reader.ToFloat();
            ArriveTangent = reader.ToFloat();
            ArriveTangentWeight = reader.ToFloat();
            LeaveTangent = reader.ToFloat();
            LeaveTangentWeight = reader.ToFloat();

            using (var sw = new StringWriter(new StringBuilder()))
            {
                sw.WriteLine("{0}: {1}", nameof(InterpMode), InterpMode);
                sw.WriteLine("{0}: {1}", nameof(TangentMode), TangentMode);
                sw.WriteLine("{0}: {1}", nameof(TangentWeightMode), TangentWeightMode);
                sw.WriteLine("{0}: {1}", nameof(Time), Time);
                sw.WriteLine("{0}: {1}", nameof(Value), Value);
                sw.WriteLine("{0}: {1}", nameof(ArriveTangent), ArriveTangent);
                sw.WriteLine("{0}: {1}", nameof(ArriveTangentWeight), ArriveTangentWeight);
                sw.WriteLine("{0}: {1}", nameof(LeaveTangent), LeaveTangent);
                sw.WriteLine("{0}: {1}", nameof(LeaveTangentWeight), LeaveTangentWeight);
                base.Value = sw.ToString();
            }
        }
    }
}
