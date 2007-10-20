using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Transactions;
using NSTM;

namespace SimpleSample
{
    [NstmTransactional]
    public class Account
    {
        private int amount;
        private int maxOverdraftAmount;

        public Account()
        {
            this.amount = 0;
            this.maxOverdraftAmount = 0;
        }

        public Account(int initialAmount, int maxOverdraftAmount)
        {
            this.amount = initialAmount;
            this.maxOverdraftAmount = maxOverdraftAmount;
        }

        public int Amount
        {
            get
            {
                return this.amount;
            }
            set
            {
                if (value < maxOverdraftAmount)
                    throw new ApplicationException(string.Format("Cannot overdraw account beyond {0} Euros!", this.maxOverdraftAmount));
                this.amount = value;
            }
        }
    }

    class Program
    {
        [NstmTransactional]
        class ContactWithManyAddresses
        {
            private string name;
            private List<Address> addresses;

            public ContactWithManyAddresses()
            {
                this.addresses = new List<Address>();
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public List<Address> Addresses
            {
                get { return this.addresses; }
            }
        }


        [NstmTransactional]
        class Contact
        {
            private string name;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            private Address location;

            public Address Location
            {
                get { return location; }
                set { location = value; }
            }
        }

        [NstmTransactional]
        class Address
        {
            private string city;

            public string City
            {
                get { return city; }
                set { city = value; }
            }
        }


        static void Main()
        {
            ContactWithManyAddresses c = new ContactWithManyAddresses();
            c.Name = "John Doe";
            Address a = new Address();
            a.City = "Hamburg";
            c.Addresses.Add(a);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                c.Name = "Peter Doe";
                c.Addresses[0].City = "Berlin";

                a = new Address();
                a.City = "Munich";
                c.Addresses.Add(a);
            }

            Console.WriteLine(c.Name);
            foreach (Address b in c.Addresses)
                Console.WriteLine(b.City);
        }


        static void MainNestedObjects()
        {
            Contact c = new Contact();
            c.Name = "John Doe";
            c.Location = new Address();
            c.Location.City = "Hamburg";

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                c.Name = "Peter Doe";
                c.Location.City = "Berlin";
            }

            Console.WriteLine(c.Name);
            Console.WriteLine(c.Location.City);
        }


        static NstmTransactional<int> myAccount, yourAccount;

        static void MainNstmTx()
        {
            myAccount = 1000;
            yourAccount = 500;

            try
            {
                TransferMoney(1001);
            }
            catch (Exception ex)
            {
                Console.WriteLine("*** {0}", ex.Message);
            }

            Console.WriteLine("my account: {0}", myAccount.Value);
            Console.WriteLine("your account: {0}", yourAccount.Value);
        }


        [NstmAtomic()]
        static void TransferMoney(int amountToTransfer)
        {
            myAccount.Value -= amountToTransfer;
            yourAccount.Value += amountToTransfer;

            if (myAccount < 0)
                throw new ApplicationException("No overdraft allowed!");
        }


        //static Account myAccount, yourAccount;

        //static void Main()
        //{
        //    myAccount = new Account(1000, 0);
        //    yourAccount = new Account(500, 0);

        //    try
        //    {
        //        TransferMoney(1001);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("*** {0}", ex.Message);
        //    }

        //    Console.WriteLine("my account: {0}", myAccount.Amount);
        //    Console.WriteLine("your account: {0}", yourAccount.Amount);
        //}


        //[NstmAtomic()]
        //static void TransferMoney(int amountToTransfer)
        //{
        //    myAccount.Amount -= amountToTransfer;
        //    yourAccount.Amount += amountToTransfer;
        //}


        //static INstmObject<int> myAccount, yourAccount;

        //static void MainTransferWithINstmObject()
        //{
        //    myAccount = NstmMemory.CreateObject<int>(1000);
        //    yourAccount = NstmMemory.CreateObject<int>(500);

        //    try
        //    {
        //        TransferMoney(350);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("*** {0}", ex.Message);
        //    }

        //    Console.WriteLine("my account: {0}", myAccount.Read());
        //    Console.WriteLine("your account: {0}", yourAccount.Read());
        //}

        //[NstmAtomic()]
        //static void TransferMoney(int amountToTransfer)
        //{
        //    myAccount.Write(myAccount.Read() - amountToTransfer);
        //    yourAccount.Write(yourAccount.Read() + amountToTransfer);
        //    throw new ApplicationException("Money transaction failed!");
        //}

        static void MainSystemTx()
        {
            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnAccess;

            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            using (TransactionScope tx = new TransactionScope())
            {
                o.Write(2);
                tx.Complete();
            }
            Console.WriteLine(o.Read());

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnBeginTransaction;
            using (TransactionScope tx = new TransactionScope())
            {
                using (INstmTransaction txNSTM = NstmMemory.BeginTransaction())
                {
                    o.Write(3);
                    txNSTM.Commit();
                }
                tx.Complete();
            }
            Console.WriteLine(o.Read());

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnAccess;
            using (TransactionScope tx = new TransactionScope())
            {
                System.Threading.AutoResetEvent are = new System.Threading.AutoResetEvent(false);
                System.Threading.ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        o.Write(4);
                        are.Set();
                    }
                    );
                are.WaitOne();
                //tx.Complete();
            }
            Console.WriteLine(o.Read());
        }


        struct MyStruct
        {
            public int j;

            public MyStruct(int value)
            {
                this.j = value;
            }
        }

        class MyClass : ICloneable
        {
            public int i;

            public MyClass(int value)
            {
                this.i = value;
            }

            #region ICloneable Members

            public object Clone()
            {
                return new MyClass(this.i);
            }

            #endregion
        }


        
        static void MainTypes()
        {
            INstmObject<int> i;
            INstmObject<string> s;
            INstmObject<MyStruct> r;
            INstmObject<MyClass> c;

            i = NstmMemory.CreateObject<int>(1);
            s = NstmMemory.CreateObject<string>("hello");
            r = NstmMemory.CreateObject<MyStruct>(new MyStruct(2));
            c = NstmMemory.CreateObject<MyClass>(new MyClass(3));

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                i.Write(10);
                s.Write("world!");
                r.Write(new MyStruct(20));
                c.Write(new MyClass(30));
            }

            Console.WriteLine(i.Read());
            Console.WriteLine(s.Read());
            Console.WriteLine(r.Read().j);
            Console.WriteLine(c.Read().i);
        }


        static void MainSysTx(string[] args)
        {
            INstmObject<double> myAccount;
            INstmObject<double> yourAccount;

            myAccount = NstmMemory.CreateObject<double>(1000);
            yourAccount = NstmMemory.CreateObject<double>(500);

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnAccess;
            using (TransactionScope tx = new TransactionScope())
            {
                double amountToTransfer = 150;

                myAccount.Write(myAccount.Read() - amountToTransfer);
                yourAccount.Write(yourAccount.Read() + amountToTransfer);

                tx.Complete();
            }

            Console.WriteLine("My account balance: {0}", myAccount.Read());
            Console.WriteLine("Your account balance: {0}", yourAccount.Read());
        }


        static void MainException(string[] args)
        {
            INstmObject<double> myAccount;
            INstmObject<double> yourAccount;

            myAccount = NstmMemory.CreateObject<double>(1000);
            yourAccount = NstmMemory.CreateObject<double>(500);

            try
            {

                using (INstmTransaction tx = NstmMemory.BeginTransaction())
                {
                    double amountToTransfer = 150;

                    myAccount.Write(myAccount.Read() - amountToTransfer);

                    throw new ApplicationException("Error during processing transfer!");

                    yourAccount.Write(yourAccount.Read() + amountToTransfer);

                    tx.Commit();
                }
            }

            catch(Exception ex)
            {
                Console.WriteLine("*** {0}", ex.Message);
            }

            Console.WriteLine("My account balance: {0}", myAccount.Read());
            Console.WriteLine("Your account balance: {0}", yourAccount.Read());
        }


        static void MainNoException(string[] args)
        {
            INstmObject<double> myAccount;
            INstmObject<double> yourAccount;

            myAccount = NstmMemory.CreateObject<double>(1000);
            yourAccount = NstmMemory.CreateObject<double>(500);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                double amountToTransfer = 150;

                myAccount.Write(myAccount.Read() - amountToTransfer);
                yourAccount.Write(yourAccount.Read() + amountToTransfer);

                tx.Commit();
            }

            Console.WriteLine("My account balance: {0}", myAccount.Read());
            Console.WriteLine("Your account balance: {0}", yourAccount.Read());
        }


        static void MainNoTx()
        {
            double myAccount;
            double yourAccount;

            myAccount = 1000;
            yourAccount = 500;

            double amountToTransfer = 150;
            myAccount -= amountToTransfer;
            yourAccount += amountToTransfer;
        }
    }
}
