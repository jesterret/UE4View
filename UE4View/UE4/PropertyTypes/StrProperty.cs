﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class StrProperty : UProperty
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            Value = reader.ToFString();
            return reader;
        }
    }
}
