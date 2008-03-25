using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Laos;

namespace NSTM
{
    [Serializable]
    internal class NstmTransactionalAspect : OnFieldAccessAspect
    {
        public override void OnGetValue(FieldAccessEventArgs eventArgs)
        {
            NstmTransaction tx = (NstmTransaction)NstmMemory.Current;
            if (tx != null)
                eventArgs.StoredFieldValue = tx.LogRead((INstmVersioned)eventArgs.Instance, eventArgs.FieldInfo.Name, eventArgs.StoredFieldValue);

            base.OnGetValue(eventArgs);
        }

        public override void OnSetValue(FieldAccessEventArgs eventArgs)
        {
            NstmTransaction tx = (NstmTransaction)NstmMemory.Current;
            if (tx != null)
            {
                lock (eventArgs.Instance)
                {
                    tx.LogWrite((INstmVersioned)eventArgs.Instance, eventArgs.FieldInfo.Name, eventArgs.ExposedFieldValue);
                    eventArgs.ExposedFieldValue = eventArgs.StoredFieldValue;
                    base.OnSetValue(eventArgs);
                }
            }
            else
            {
                lock (eventArgs.Instance)
                {
                    ((INstmVersioned)eventArgs.Instance).IncrementVersion();
                    base.OnSetValue(eventArgs);
                }

                Infrastructure.RetryTriggerList.Instance.NotifyRetriesForTrigger(eventArgs.Instance);
            }
        }
    }
}
