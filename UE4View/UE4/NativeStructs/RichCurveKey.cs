using System;
using System.Collections.Generic;
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

        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            if (reader.Version < (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_SERIALIZE_RICH_CURVE_KEY)
                return reader;

            InterpMode = reader.ToByte();
            TangentMode = reader.ToByte();
            TangentWeightMode = reader.ToByte();
            Time = reader.ToFloat();
            Value = reader.ToFloat();
            ArriveTangent = reader.ToFloat();
            ArriveTangentWeight = reader.ToFloat();
            LeaveTangent = reader.ToFloat();
            LeaveTangentWeight = reader.ToFloat();

            base.Value = string.Empty;
            base.Value += string.Format("{0}: {1}{2}", nameof(InterpMode), InterpMode, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(TangentMode), TangentMode, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(TangentWeightMode), TangentWeightMode, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(Time), Time, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(Value), Value, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(ArriveTangent), ArriveTangent, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(ArriveTangentWeight), ArriveTangentWeight, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(LeaveTangent), LeaveTangent, Environment.NewLine);
            base.Value += string.Format("{0}: {1}{2}", nameof(LeaveTangentWeight), LeaveTangentWeight, Environment.NewLine);
            return reader;
        }
    }
}
