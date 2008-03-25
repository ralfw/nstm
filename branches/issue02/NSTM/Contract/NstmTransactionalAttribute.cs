using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Laos;
using PostSharp.Extensibility;

namespace NSTM
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    [MulticastAttributeUsage(MulticastTargets.Class)]
    public class NstmTransactionalAttribute : CompoundAspect
    {
        public override void ProvideAspects(object element, LaosReflectionAspectCollection collection)
        {
            Type targettype = (Type)element;

            collection.AddAspect(targettype, new NstmVersionableAspect());

            NstmTransactionalAspect txa = new NstmTransactionalAspect();

            foreach (System.Reflection.FieldInfo fi in targettype.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (!fi.IsStatic)
                    collection.AddAspect(fi, txa);
            }

        }
    }    
}
