using System;
using System.Collections.Generic;
using System.Text;

using NSTM;

namespace NSTM.Collections
{
    //TODO: implement stack using INstmObject to see if the problem of the parallel test is caused by NstmTransactional
    [NstmTransactional]
    public class NstmStack<T>
    {
        [NstmTransactional]
        public class Entry
        {
            private T value;
            private Entry next;

            public Entry(T value)
            {
                this.value = value;
            }

            public T Value
            {
                get { return this.value; }
                set { this.value = value; }
            }

            public Entry Next
            {
                get { return this.next; }
                set { this.next = value; }
            }
        }


        private Entry top;
        private int count;


public Entry Top
{
    get { return this.top; }
}


        public NstmStack()
        {
            this.top = null;
            this.count = 0;
        }


        public void Push(T value)
        {
            Entry newEntry = new Entry(value);
            newEntry.Next = this.top;
            this.top = newEntry;
            this.count++;
        }


        public T Pop()
        {
            if (this.top != null)
            {
                T topValue = this.top.Value;

                this.top = this.top.Next;
                this.count--;

                return topValue;
            }
            else
                throw new InvalidOperationException("Stack empty!");
        }


        public T Peek()
        {
            if (this.top != null)
            {
                return this.top.Value;
            }
            else
                throw new InvalidOperationException("Stack empty!");
        }


        public int Count
        {
            get
            {
                return this.count;
            }
        }
    }
}
