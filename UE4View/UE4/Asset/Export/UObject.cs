using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE4View.UE4.PropertyTypes;

namespace UE4View.UE4.UAsset.Export
{
    public class UObject : IDisposable
    {
        protected List<KeyValuePair<string, object>> TaggedVars { get; } = new List<KeyValuePair<string, object>>();
        public UObject() { }
        public UObject(FArchive reader) => Serialize(reader);

        public virtual void Serialize(FArchive reader)
        {
            foreach (var prop in FPropertyTag.ReadToEnd(reader))
                AddVar(prop.Name, prop.ToProperty(reader));
        }

        public virtual void Read(TextWriter wr)
        {
            foreach(var kv in TaggedVars)
            {
                wr.Write("{0}: ", kv.Key);
                WriteProperty(wr, kv.Value);
            }
        }

        public void AddVar(string name, object prop)
        {
            TaggedVars.Add(new KeyValuePair<string, object>(name, prop));
        }

        protected void WriteProperty(TextWriter wr, object prop)
        {
            if (prop is ArrayPropertyBase array)
            {
                var count = array.Count - 1;
                wr.Write("[ ");
                foreach (var i in Enumerable.Range(0, count + 1))
                {
                    wr.Write(array[i]);
                    if (i < count)
                        wr.Write(", ");
                }
                wr.WriteLine(" ]");
            }
            else
                wr.WriteLine(prop);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                sw.WriteLine("{{ ");
                Read(sw);
                sw.WriteLine(" }}");
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            TaggedVars.Clear();
        }

        public static Dictionary<string, Type> Classes { get; } = typeof(UObject).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(UObject))).ToDictionary(t => t.Name);
    }
}
