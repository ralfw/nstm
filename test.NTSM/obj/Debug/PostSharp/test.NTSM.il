// Generated by PostSharp 1.0.5.218 at 21.07.2007 15:21:20.
.typelist
{
  test.testit
  test.testit/'<>c__DisplayClass4'
}
.assembly extern mscorlib
{
  .publickeytoken = (  B7 7A 5C 56 19 34 E0 89)
  .ver 2:0:0:0
}
.assembly extern nunit.framework
{
  .publickeytoken = (  96 D0 9A 1E B7 F4 4A 77)
  .ver 2:2:8:0
}
.assembly extern NSTM.Collections
{
  .ver 1:0:0:0
}
.assembly extern NSTM
{
  .ver 1:0:0:202
}
.assembly extern System
{
  .publickeytoken = (  B7 7A 5C 56 19 34 E0 89)
  .ver 2:0:0:0
}
.assembly test.NTSM
{
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = (  01 00 24 43 6F 70 79 72 69 67 68 74 20 C2 A9 20
                                                                                                  4F 6E 65 20 4D 61 6E 20 54 68 69 6E 6B 20 54 61
                                                                                                  6E 6B 20 32 30 30 37 00 00)
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = (  01 00 09 74 65 73 74 2E 4E 54 53 4D 00 00)
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (  01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01)
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = (  01 00 07 31 2E 30 2E 30 2E 30 00 00)
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = (  01 00 24 34 36 63 66 34 31 31 62 2D 62 33 65 61
                                                                                                  2D 34 31 36 30 2D 38 61 65 36 2D 31 34 30 38 39
                                                                                                  32 33 39 35 30 63 36 00 00)
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = (  01 00 00 00 00)
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = (  01 00 00 00 00)
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = (  01 00 12 4F 6E 65 20 4D 61 6E 20 54 68 69 6E 6B
                                                                                                20 54 61 6E 6B 00 00)
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = (  01 00 00 00 00)
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = (  01 00 00 00 00)
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = (  01 00 09 74 65 73 74 2E 4E 54 53 4D 00 00)
  // Intentionally skipped: System.Diagnostics.DebuggableAttribute
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (  01 00 08 00 00 00 00 00)
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module test.NTSM.dll
.imagebase 0x400000
.file alignment 0x00001000
.stackreserve 0x100000
.subsystem 0x0003
.corflags 0x00000001
.class public auto ansi beforefieldinit test.testit
      extends [mscorlib]System.Object
{
.class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4'
      extends [mscorlib]System.Object
{
}
// End of type: test.testit::<>c__DisplayClass4



}
// End of type: test.testit


.class public auto ansi beforefieldinit test.testit
      extends [mscorlib]System.Object
{
  .custom instance void [nunit.framework]NUnit.Framework.TestFixtureAttribute::.ctor() = (  01 00 00 00)
.class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4'
      extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (  01 00 00 00)
  .field public class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> ares

  .field public class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> q

  .field public int32 nProduced

  .field public int32 nConsumed

  .field public int32 nDequeueRetries

  .field public class test.testit  '<>4__this'

  .method public hidebysig specialname rtspecialname
          instance void .ctor() cil managed
  {
    .maxstack 8
    // InstructionBlock 0 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 0-7

  IL_0000:
    // {InstructionSequence 0 Original, Range=0-7 }
    ldarg.0   
    call       instance void [mscorlib]System.Object::.ctor()
    ret       
  }
  // End of method: test.testit::<>c__DisplayClass4/.ctor() : void

  .method public hidebysig instance void
'<TestQueueMultithreaded>b__0'(object state) cil managed
  {
    .maxstack 4
    .language  '{3f5162f8-07c6-11d3-9053-00c04fa302a1}',  '{994b45c4-e6e9-11d2-903f-00c04fa302a1}',  '{5a869d0b-6611-11d3-bd2a-0000f80849bd}'

    {
      // InstructionBlock 0 [], 5 local(s), 0 handler(s), 4 child(ren), 0 sequence(s), 0-d4
      .locals init ( [0]  int32 thIndex,
                     [1]  int32 n,
                     [2]  class test.testit CS$2$0000,
                     [3]  bool CS$4$0001,
                     [4]  object[] CS$0$0002)
      // InstructionBlock 1 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), 0-45

    IL_0000:
      // {InstructionSequence 0 Original, Range=0-1d }
      .line 39,39 : 21,22  'C:\\My Software Projects\\XVSM\\SoftwareTransactionalMemory Prototype\\test.NTSM\\testit.cs'
      nop       
      .line 40,40 : 25,50
      ldarg.1   
      unbox.any  [mscorlib]System.Int32
      stloc.0   
      .line 41,41 : 25,75
      ldstr       "prod {0} started..."
      ldloc.0   
      box        [mscorlib]System.Int32
      call       void [mscorlib]System.Console::WriteLine(string,
                                                          object)
      nop       
      .line 43,43 : 25,35
      ldc.i4.0  
      stloc.1   
      .line 0x00feefee
      br.s       IL_0005

    IL_0001:
      // {InstructionSequence 1 Original, Range=1d-45 }
      .line 45,45 : 25,26
      nop       
      .line 46,46 : 29,61
      ldarg.0   
      ldfld      class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> test.testit/'<>c__DisplayClass4'::q
      ldloc.0   
      ldc.i4     0x186a0
      mul       
      ldloc.1   
      add       
      callvirt   instance void class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::Enqueue(!0)
      nop       
      .line 47,47 : 29,33
      ldloc.1   
      ldc.i4.1  
      add       
      stloc.1   
      .line 48,48 : 29,40
      ldarg.0   
      ldfld      class test.testit test.testit/'<>c__DisplayClass4':: '<>4__this'
      dup       
      stloc.2   
      call       void [mscorlib]System.Threading.Monitor::Enter(object)
      nop       
      .try
      {
        // InstructionBlock 2 [], 0 local(s), 1 handler(s), 0 child(ren), 1 sequence(s), 45-57

      IL_0002:
        // {InstructionSequence 2 Original, Range=45-57 }
        .line 48,48 : 41,42
        nop       
        .line 48,48 : 43,55
        ldarg.0   
        dup       
        ldfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
        ldc.i4.1  
        add       
        stfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
        .line 48,48 : 56,57
        nop       
        .line 0x00feefee
        leave.s    IL_0004
      }
      finally
      {
        // InstructionBlock 4 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 57-5f

      IL_0003:
        // {InstructionSequence 3 Original, Range=57-5f }
        ldloc.2   
        call       void [mscorlib]System.Threading.Monitor::Exit(object)
        nop       
        endfinally
      }
      // InstructionBlock 5 [], 0 local(s), 0 handler(s), 0 child(ren), 3 sequence(s), 5f-d4

    IL_0004:
      // {InstructionSequence 4 Original, Range=5f-69 }
      .line 0x00feefee
      nop       
      .line 50,50 : 29,62
      ldc.i4.s   10
      call       void [mscorlib]System.Threading.Thread::Sleep(int32)
      nop       
      .line 51,51 : 25,26
      nop       

    IL_0005:
      // {InstructionSequence 5 Original, Range=69-75 }
      .line 44,44 : 25,53
      ldloc.1   
      ldc.i4     0x1f4
      clt       
      stloc.3   
      .line 0x00feefee
      ldloc.3   
      brtrue.s   IL_0001

    IL_0006:
      // {InstructionSequence 6 Original, Range=75-d4 }
      .line 53,53 : 25,137
      ldstr       "  prod {0} finished: {1}, queue count: {2}, nProduced: {3}"
      ldc.i4.4  
      newarr     [mscorlib]System.Object
      stloc.s    CS$0$0002
      ldloc.s    CS$0$0002
      ldc.i4.0  
      ldloc.0   
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      ldc.i4.1  
      ldloc.1   
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      ldc.i4.2  
      ldarg.0   
      ldfld      class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> test.testit/'<>c__DisplayClass4'::q
      callvirt   instance int32 class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::get_Count()
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      ldc.i4.3  
      ldarg.0   
      ldfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      call       void [mscorlib]System.Console::WriteLine(string,
                                                          object[] )
      nop       
      .line 54,54 : 25,45
      ldarg.0   
      ldfld      class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> test.testit/'<>c__DisplayClass4'::ares
      ldloc.0   
      callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent>::get_Item(int32)
      callvirt   instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
      pop       
      .line 0x00feefee
      ret       
    }
  }
  // End of method: test.testit::<>c__DisplayClass4/<TestQueueMultithreaded>b__0(object state) : void

  .method public hidebysig instance void
'<TestQueueMultithreaded>b__1'(object state) cil managed
  {
    .maxstack 4
    .language  '{3f5162f8-07c6-11d3-9053-00c04fa302a1}',  '{994b45c4-e6e9-11d2-903f-00c04fa302a1}',  '{5a869d0b-6611-11d3-bd2a-0000f80849bd}'

    {
      // InstructionBlock 0 [], 5 local(s), 0 handler(s), 5 child(ren), 0 sequence(s), 0-16f
      .locals init ( [0]  int32 thIndex,
                     [1]  int32 nConsumedByThread,
                     [3]  class test.testit CS$2$0000,
                     [4]  bool CS$4$0001,
                     [5]  object[] CS$0$0002)
      // InstructionBlock 16 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), 0-2f

    IL_0000:
      // {InstructionSequence 0 Original, Range=0-20 }
      .line 66,66 : 21,22  'C:\\My Software Projects\\XVSM\\SoftwareTransactionalMemory Prototype\\test.NTSM\\testit.cs'
      nop       
      .line 67,67 : 25,50
      ldarg.1   
      unbox.any  [mscorlib]System.Int32
      stloc.0   
      .line 68,68 : 25,75
      ldstr       "cons {0} started..."
      ldloc.0   
      box        [mscorlib]System.Int32
      call       void [mscorlib]System.Console::WriteLine(string,
                                                          object)
      nop       
      .line 70,70 : 25,51
      ldc.i4.0  
      stloc.1   
      .line 0x00feefee
      br         IL_0018

    IL_0001:
      // {InstructionSequence 1 Original, Range=20-2f }
      .line 72,72 : 25,26
      nop       
      .line 73,73 : 29,40
      ldarg.0   
      ldfld      class test.testit test.testit/'<>c__DisplayClass4':: '<>4__this'
      dup       
      stloc.3   
      call       void [mscorlib]System.Threading.Monitor::Enter(object)
      nop       
      .try
      {
        // InstructionBlock 17 [], 0 local(s), 1 handler(s), 0 child(ren), 6 sequence(s), 2f-5c

      IL_0002:
        // {InstructionSequence 2 Original, Range=2f-3d }
        .line 74,74 : 29,30
        nop       
        .line 75,75 : 33,109
        ldarg.0   
        ldfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
        ldc.i4     0x9c4
        bne.un.s   IL_0004

      IL_0003:
        // {InstructionSequence 3 Original, Range=3d-4d }
        ldarg.0   
        ldfld      int32 test.testit/'<>c__DisplayClass4'::nConsumed
        ldarg.0   
        ldfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
        clt       
        br.s       IL_0005

      IL_0004:
        // {InstructionSequence 4 Original, Range=4d-4e }
        ldc.i4.1  

      IL_0005:
        // {InstructionSequence 5 Original, Range=4e-54 }
        stloc.s    CS$4$0001
        .line 0x00feefee
        ldloc.s    CS$4$0001
        brtrue.s   IL_0007

      IL_0006:
        // {InstructionSequence 6 Original, Range=54-59 }
        .line 76,76 : 37,43
        leave      IL_0019

      IL_0007:
        // {InstructionSequence 7 Original, Range=59-5c }
        .line 77,77 : 29,30
        nop       
        .line 0x00feefee
        leave.s    IL_0009
      }
      finally
      {
        // InstructionBlock 19 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 5c-64

      IL_0008:
        // {InstructionSequence 8 Original, Range=5c-64 }
        ldloc.3   
        call       void [mscorlib]System.Threading.Monitor::Exit(object)
        nop       
        endfinally
      }
      {
        // InstructionBlock 21 [], 1 local(s), 0 handler(s), 4 child(ren), 0 sequence(s), 64-103
        .locals init ( [2]  class [NSTM]NSTM.INstmTransaction tx)
        // InstructionBlock 20 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 64-6e

      IL_0009:
        // {InstructionSequence 9 Original, Range=64-6e }
        .line 0x00feefee
        nop       
        .line 79,83 : 36,34
        ldc.i4.0  
        ldc.i4.1  
        ldc.i4.0  
        call       class [NSTM]NSTM.INstmTransaction [NSTM]NSTM.NstmMemory::BeginTransaction(valuetype [NSTM]NSTM.NstmTransactionScopeOption,
                                                                                             valuetype [NSTM]NSTM.NstmTransactionIsolationLevel,
                                                                                             valuetype [NSTM]NSTM.NstmTransactionCloneMode)
        stloc.2   
        .try
        {
          // InstructionBlock 2 [], 0 local(s), 1 handler(s), 7 child(ren), 0 sequence(s), 6e-f1
          // InstructionBlock 11 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), 6e-a1

        IL_000A:
          // {InstructionSequence 10 Original, Range=6e-86 }
          .line 84,84 : 29,30
          nop       
          .line 85,85 : 33,49
          ldarg.0   
          ldfld      class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> test.testit/'<>c__DisplayClass4'::q
          callvirt   instance int32 class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::get_Count()
          ldc.i4.0  
          cgt       
          ldc.i4.0  
          ceq       
          stloc.s    CS$4$0001
          .line 0x00feefee
          ldloc.s    CS$4$0001
          brtrue.s   IL_000F

        IL_000B:
          // {InstructionSequence 11 Original, Range=86-a1 }
          .line 86,86 : 33,34
          nop       
          .line 87,87 : 37,49
          ldarg.0   
          ldfld      class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> test.testit/'<>c__DisplayClass4'::q
          callvirt   instance !0 class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::Dequeue()
          pop       
          .line 89,89 : 37,48
          ldarg.0   
          ldfld      class test.testit test.testit/'<>c__DisplayClass4':: '<>4__this'
          dup       
          stloc.3   
          call       void [mscorlib]System.Threading.Monitor::Enter(object)
          nop       
          .try
          {
            // InstructionBlock 12 [], 0 local(s), 1 handler(s), 0 child(ren), 1 sequence(s), a1-b7

          IL_000C:
            // {InstructionSequence 12 Original, Range=a1-b7 }
            .line 90,90 : 37,38
            nop       
            .line 91,91 : 41,53
            ldarg.0   
            dup       
            ldfld      int32 test.testit/'<>c__DisplayClass4'::nConsumed
            ldc.i4.1  
            add       
            stfld      int32 test.testit/'<>c__DisplayClass4'::nConsumed
            .line 92,92 : 41,61
            ldloc.1   
            ldc.i4.1  
            add       
            stloc.1   
            .line 93,93 : 37,38
            nop       
            .line 0x00feefee
            leave.s    IL_000E
          }
          finally
          {
            // InstructionBlock 14 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), b7-bf

          IL_000D:
            // {InstructionSequence 13 Original, Range=b7-bf }
            ldloc.3   
            call       void [mscorlib]System.Threading.Monitor::Exit(object)
            nop       
            endfinally
          }
          // InstructionBlock 15 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), bf-d2

        IL_000E:
          // {InstructionSequence 14 Original, Range=bf-c3 }
          .line 0x00feefee
          nop       
          .line 94,94 : 33,34
          nop       
          .line 0x00feefee
          br.s       IL_0013

        IL_000F:
          // {InstructionSequence 15 Original, Range=c3-d2 }
          .line 96,96 : 33,34
          nop       
          .line 97,97 : 37,48
          ldarg.0   
          ldfld      class test.testit test.testit/'<>c__DisplayClass4':: '<>4__this'
          dup       
          stloc.3   
          call       void [mscorlib]System.Threading.Monitor::Enter(object)
          nop       
          .try
          {
            // InstructionBlock 7 [], 0 local(s), 1 handler(s), 0 child(ren), 1 sequence(s), d2-e4

          IL_0010:
            // {InstructionSequence 16 Original, Range=d2-e4 }
            .line 98,98 : 37,38
            nop       
            .line 99,99 : 41,59
            ldarg.0   
            dup       
            ldfld      int32 test.testit/'<>c__DisplayClass4'::nDequeueRetries
            ldc.i4.1  
            add       
            stfld      int32 test.testit/'<>c__DisplayClass4'::nDequeueRetries
            .line 100,100 : 37,38
            nop       
            .line 0x00feefee
            leave.s    IL_0012
          }
          finally
          {
            // InstructionBlock 9 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), e4-ec

          IL_0011:
            // {InstructionSequence 17 Original, Range=e4-ec }
            ldloc.3   
            call       void [mscorlib]System.Threading.Monitor::Exit(object)
            nop       
            endfinally
          }
          // InstructionBlock 10 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), ec-f1

        IL_0012:
          // {InstructionSequence 18 Original, Range=ec-ee }
          .line 0x00feefee
          nop       
          .line 101,101 : 33,34
          nop       

        IL_0013:
          // {InstructionSequence 19 Original, Range=ee-f1 }
          .line 102,102 : 29,30
          nop       
          .line 0x00feefee
          leave.s    IL_0017
        }
        finally
        {
          // InstructionBlock 4 [], 0 local(s), 0 handler(s), 0 child(ren), 3 sequence(s), f1-103

        IL_0014:
          // {InstructionSequence 20 Original, Range=f1-fb }
          ldloc.2   
          ldnull    
          ceq       
          stloc.s    CS$4$0001
          ldloc.s    CS$4$0001
          brtrue.s   IL_0016

        IL_0015:
          // {InstructionSequence 21 Original, Range=fb-102 }
          ldloc.2   
          callvirt   instance void [mscorlib]System.IDisposable::Dispose()
          nop       

        IL_0016:
          // {InstructionSequence 22 Original, Range=102-103 }
          endfinally
        }
        // InstructionBlock 22 [], 0 local(s), 0 handler(s), 0 child(ren), 0 sequence(s), 103-103
      }
      // InstructionBlock 23 [], 0 local(s), 0 handler(s), 0 child(ren), 3 sequence(s), 103-16f

    IL_0017:
      // {InstructionSequence 23 Original, Range=103-10c }
      .line 0x00feefee
      nop       
      .line 104,104 : 29,62
      ldc.i4.0  
      call       void [mscorlib]System.Threading.Thread::Sleep(int32)
      nop       
      .line 105,105 : 25,26
      nop       

    IL_0018:
      // {InstructionSequence 24 Original, Range=10c-114 }
      .line 71,71 : 25,37
      ldc.i4.1  
      stloc.s    CS$4$0001
      .line 0x00feefee
      br         IL_0001

    IL_0019:
      // {InstructionSequence 25 Original, Range=114-16f }
      .line 0x00feefee
      nop       
      .line 107,107 : 25,163
      ldstr       "  cons {0} finished, consumed: {1}, nProduced: {2}, nConsumed: {3}"
      ldc.i4.4  
      newarr     [mscorlib]System.Object
      stloc.s    CS$0$0002
      ldloc.s    CS$0$0002
      ldc.i4.0  
      ldloc.0   
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      ldc.i4.1  
      ldloc.1   
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      ldc.i4.2  
      ldarg.0   
      ldfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      ldc.i4.3  
      ldarg.0   
      ldfld      int32 test.testit/'<>c__DisplayClass4'::nConsumed
      box        [mscorlib]System.Int32
      stelem.ref
      ldloc.s    CS$0$0002
      call       void [mscorlib]System.Console::WriteLine(string,
                                                          object[] )
      nop       
      .line 109,109 : 25,45
      ldarg.0   
      ldfld      class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> test.testit/'<>c__DisplayClass4'::ares
      ldloc.0   
      callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent>::get_Item(int32)
      callvirt   instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
      pop       
      .line 0x00feefee
      ret       
    }
  }
  // End of method: test.testit::<>c__DisplayClass4/<TestQueueMultithreaded>b__1(object state) : void

}
// End of type: test.testit::<>c__DisplayClass4



  .method public hidebysig instance void
          TestQueueMultithreaded() cil managed
  {
    .custom instance void [nunit.framework]NUnit.Framework.TestAttribute::.ctor() = (  01 00 00 00)
    .maxstack 3
    .language  '{3f5162f8-07c6-11d3-9053-00c04fa302a1}',  '{994b45c4-e6e9-11d2-903f-00c04fa302a1}',  '{5a869d0b-6611-11d3-bd2a-0000f80849bd}'

    {
      // InstructionBlock 0 [], 4 local(s), 0 handler(s), 4 child(ren), 0 sequence(s), 0-102
      .locals init ( [1]  class [mscorlib]System.Threading.WaitCallback  '<>9__CachedAnonymousMethodDelegate2',
                     [2]  class [mscorlib]System.Threading.WaitCallback  '<>9__CachedAnonymousMethodDelegate3',
                     [3]  class test.testit/'<>c__DisplayClass4'  '<>8__locals5',
                     [4]  bool CS$4$0000)
      // InstructionBlock 1 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 0-3d

    IL_0000:
      // {InstructionSequence 0 Original, Range=0-3d }
      .line 0x00feefee
      ldnull    
      stloc.1   
      ldnull    
      stloc.2   
      newobj     instance void test.testit/'<>c__DisplayClass4'::.ctor()
      stloc.3   
      ldloc.3   
      ldarg.0   
      stfld      class test.testit test.testit/'<>c__DisplayClass4':: '<>4__this'
      .line 19,19 : 9,10  'C:\\My Software Projects\\XVSM\\SoftwareTransactionalMemory Prototype\\test.NTSM\\testit.cs'
      nop       
      .line 26,26 : 13,68
      ldloc.3   
      newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent>::.ctor()
      stfld      class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> test.testit/'<>c__DisplayClass4'::ares
      .line 28,28 : 13,53
      ldloc.3   
      newobj     instance void class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::.ctor()
      stfld      class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> test.testit/'<>c__DisplayClass4'::q
      .line 29,29 : 13,31
      ldloc.3   
      ldc.i4.0  
      stfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
      .line 30,30 : 13,31
      ldloc.3   
      ldc.i4.0  
      stfld      int32 test.testit/'<>c__DisplayClass4'::nConsumed
      .line 31,31 : 13,37
      ldloc.3   
      ldc.i4.0  
      stfld      int32 test.testit/'<>c__DisplayClass4'::nDequeueRetries
      {
        // InstructionBlock 2 [], 1 local(s), 0 handler(s), 0 child(ren), 5 sequence(s), 3d-82
        .locals init ( [0]  int32 i)

      IL_0001:
        // {InstructionSequence 1 Original, Range=3d-41 }
        .line 34,34 : 18,28
        ldc.i4.0  
        stloc.0   
        .line 0x00feefee
        br.s       IL_0005

      IL_0002:
        // {InstructionSequence 2 Original, Range=41-57 }
        .line 35,35 : 13,14
        nop       
        .line 36,36 : 17,53
        ldloc.3   
        ldfld      class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> test.testit/'<>c__DisplayClass4'::ares
        ldc.i4.0  
        newobj     instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
        callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent>::Add(!0)
        nop       
        .line 37,57 : 17,23
        ldloc.1   
        brtrue.s   IL_0004

      IL_0003:
        // {InstructionSequence 3 Original, Range=57-66 }
        ldloc.3   
        ldftn      instance void test.testit/'<>c__DisplayClass4':: '<TestQueueMultithreaded>b__0'(object)
        newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                native int)
        stloc.1   
        br.s       IL_0004

      IL_0004:
        // {InstructionSequence 4 Original, Range=66-78 }
        ldloc.1   
        ldloc.0   
        box        [mscorlib]System.Int32
        call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback,
                                                                                 object)
        pop       
        .line 58,58 : 13,14
        nop       
        .line 34,34 : 45,48
        ldloc.0   
        ldc.i4.1  
        add       
        stloc.0   

      IL_0005:
        // {InstructionSequence 5 Original, Range=78-82 }
        .line 34,34 : 29,43
        ldloc.0   
        ldc.i4.5  
        clt       
        stloc.s    CS$4$0000
        .line 0x00feefee
        ldloc.s    CS$4$0000
        brtrue.s   IL_0002
      }
      {
        // InstructionBlock 4 [], 1 local(s), 0 handler(s), 0 child(ren), 5 sequence(s), 82-c7
        .locals init ( [0]  int32 i)

      IL_0006:
        // {InstructionSequence 6 Original, Range=82-86 }
        .line 61,61 : 18,37
        ldc.i4.5  
        stloc.0   
        .line 0x00feefee
        br.s       IL_000A

      IL_0007:
        // {InstructionSequence 7 Original, Range=86-9c }
        .line 62,62 : 13,14
        nop       
        .line 63,63 : 17,53
        ldloc.3   
        ldfld      class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> test.testit/'<>c__DisplayClass4'::ares
        ldc.i4.0  
        newobj     instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
        callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent>::Add(!0)
        nop       
        .line 64,112 : 17,23
        ldloc.2   
        brtrue.s   IL_0009

      IL_0008:
        // {InstructionSequence 8 Original, Range=9c-ab }
        ldloc.3   
        ldftn      instance void test.testit/'<>c__DisplayClass4':: '<TestQueueMultithreaded>b__1'(object)
        newobj     instance void [mscorlib]System.Threading.WaitCallback::.ctor(object,
                                                                                native int)
        stloc.2   
        br.s       IL_0009

      IL_0009:
        // {InstructionSequence 9 Original, Range=ab-bd }
        ldloc.2   
        ldloc.0   
        box        [mscorlib]System.Int32
        call       bool [mscorlib]System.Threading.ThreadPool::QueueUserWorkItem(class [mscorlib]System.Threading.WaitCallback,
                                                                                 object)
        pop       
        .line 113,113 : 13,14
        nop       
        .line 61,61 : 67,70
        ldloc.0   
        ldc.i4.1  
        add       
        stloc.0   

      IL_000A:
        // {InstructionSequence 10 Original, Range=bd-c7 }
        .line 61,61 : 38,65
        ldloc.0   
        ldc.i4.8  
        clt       
        stloc.s    CS$4$0000
        .line 0x00feefee
        ldloc.s    CS$4$0000
        brtrue.s   IL_0007
      }
      // InstructionBlock 5 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), c7-102

    IL_000B:
      // {InstructionSequence 11 Original, Range=c7-102 }
      .line 115,115 : 13,48
      ldloc.3   
      ldfld      class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent> test.testit/'<>c__DisplayClass4'::ares
      callvirt   instance !0[] class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Threading.AutoResetEvent>::ToArray()
      call       bool [mscorlib]System.Threading.WaitHandle::WaitAll(class [mscorlib]System.Threading.WaitHandle[] )
      pop       
      .line 117,117 : 13,72
      ldstr       "Dequeue retries: {0}"
      ldloc.3   
      ldfld      int32 test.testit/'<>c__DisplayClass4'::nDequeueRetries
      box        [mscorlib]System.Int32
      call       void [mscorlib]System.Console::WriteLine(string,
                                                          object)
      nop       
      .line 119,119 : 13,51
      ldloc.3   
      ldfld      int32 test.testit/'<>c__DisplayClass4'::nProduced
      ldloc.3   
      ldfld      int32 test.testit/'<>c__DisplayClass4'::nConsumed
      call       void [nunit.framework]NUnit.Framework.Assert::AreEqual(int32,
                                                                        int32)
      nop       
      .line 120,120 : 9,10
      nop       
      ret       
    }
  }
  // End of method: test.testit/TestQueueMultithreaded() : void

  .method public hidebysig instance void
          TestQueuePerformance() cil managed
  {
    .custom instance void [nunit.framework]NUnit.Framework.TestAttribute::.ctor() = (  01 00 00 00)
    .maxstack 2
    .language  '{3f5162f8-07c6-11d3-9053-00c04fa302a1}',  '{994b45c4-e6e9-11d2-903f-00c04fa302a1}',  '{5a869d0b-6611-11d3-bd2a-0000f80849bd}'

    // InstructionBlock 0 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 0-4a

  IL_0000:
    // {InstructionSequence 0 Original, Range=0-4a }
    .line 125,125 : 9,10  'C:\\My Software Projects\\XVSM\\SoftwareTransactionalMemory Prototype\\test.NTSM\\testit.cs'
    nop       
    .line 127,127 : 13,42
    ldarg.0   
    ldc.i4     0x2ee
    call       instance void test.testit::testPerfOfBCLGenericQueue(int32)
    nop       
    .line 128,128 : 13,36
    ldarg.0   
    ldc.i4     0x2ee
    call       instance void test.testit::testPerfOfNstmQueue(int32)
    nop       
    .line 129,129 : 13,42
    ldarg.0   
    ldc.i4     0x2ee
    call       instance void test.testit::testPerfOfBCLGenericQueue(int32)
    nop       
    .line 130,130 : 13,36
    ldarg.0   
    ldc.i4     0x2ee
    call       instance void test.testit::testPerfOfNstmQueue(int32)
    nop       
    .line 131,131 : 13,42
    ldarg.0   
    ldc.i4     0x2ee
    call       instance void test.testit::testPerfOfBCLGenericQueue(int32)
    nop       
    .line 132,132 : 13,36
    ldarg.0   
    ldc.i4     0x2ee
    call       instance void test.testit::testPerfOfNstmQueue(int32)
    nop       
    .line 133,133 : 9,10
    ret       
  }
  // End of method: test.testit/TestQueuePerformance() : void

  .method private hidebysig instance void
          testPerfOfBCLGenericQueue(int32 N) cil managed
  {
    .maxstack 2
    .language  '{3f5162f8-07c6-11d3-9053-00c04fa302a1}',  '{994b45c4-e6e9-11d2-903f-00c04fa302a1}',  '{5a869d0b-6611-11d3-bd2a-0000f80849bd}'

    {
      // InstructionBlock 0 [], 3 local(s), 0 handler(s), 3 child(ren), 0 sequence(s), 0-36
      .locals init ( [0]  class [System]System.Collections.Generic.Queue`1<int32> q,
                     [2]  class [System]System.Collections.Generic.Queue`1<int32> CS$2$0000,
                     [3]  bool CS$4$0001)
      // InstructionBlock 7 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 0-7

    IL_0000:
      // {InstructionSequence 0 Original, Range=0-7 }
      .line 136,136 : 9,10  'C:\\My Software Projects\\XVSM\\SoftwareTransactionalMemory Prototype\\test.NTSM\\testit.cs'
      nop       
      .line 137,137 : 13,45
      newobj     instance void class [System]System.Collections.Generic.Queue`1<int32>::.ctor()
      stloc.0   
      {
        // InstructionBlock 6 [], 1 local(s), 0 handler(s), 4 child(ren), 0 sequence(s), 7-35
        .locals init ( [1]  int32 i)
        // InstructionBlock 8 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), 7-14

      IL_0001:
        // {InstructionSequence 1 Original, Range=7-b }
        .line 138,138 : 18,28
        ldc.i4.0  
        stloc.1   
        .line 0x00feefee
        br.s       IL_0006

      IL_0002:
        // {InstructionSequence 2 Original, Range=b-14 }
        .line 139,139 : 17,25
        ldloc.0   
        dup       
        stloc.2   
        call       void [mscorlib]System.Threading.Monitor::Enter(object)
        nop       
        .try
        {
          // InstructionBlock 2 [], 0 local(s), 1 handler(s), 0 child(ren), 1 sequence(s), 14-20

        IL_0003:
          // {InstructionSequence 3 Original, Range=14-20 }
          .line 140,140 : 17,18
          nop       
          .line 141,141 : 21,34
          ldloc.0   
          ldloc.1   
          callvirt   instance void class [System]System.Collections.Generic.Queue`1<int32>::Enqueue(!0)
          nop       
          .line 142,142 : 17,18
          nop       
          .line 0x00feefee
          leave.s    IL_0005
        }
        finally
        {
          // InstructionBlock 4 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 20-28

        IL_0004:
          // {InstructionSequence 4 Original, Range=20-28 }
          ldloc.2   
          call       void [mscorlib]System.Threading.Monitor::Exit(object)
          nop       
          endfinally
        }
        // InstructionBlock 9 [], 0 local(s), 0 handler(s), 0 child(ren), 2 sequence(s), 28-35

      IL_0005:
        // {InstructionSequence 5 Original, Range=28-2d }
        .line 0x00feefee
        nop       
        .line 138,138 : 36,39
        ldloc.1   
        ldc.i4.1  
        add       
        stloc.1   

      IL_0006:
        // {InstructionSequence 6 Original, Range=2d-35 }
        .line 138,138 : 29,34
        ldloc.1   
        ldarg.1   
        clt       
        stloc.3   
        .line 0x00feefee
        ldloc.3   
        brtrue.s   IL_0002
      }
      // InstructionBlock 10 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 35-36

    IL_0007:
      // {InstructionSequence 7 Original, Range=35-36 }
      .line 143,143 : 9,10
      ret       
    }
  }
  // End of method: test.testit/testPerfOfBCLGenericQueue(int32 N) : void

  .method private hidebysig instance void
          testPerfOfNstmQueue(int32 N) cil managed
  {
    .maxstack 2
    .language  '{3f5162f8-07c6-11d3-9053-00c04fa302a1}',  '{994b45c4-e6e9-11d2-903f-00c04fa302a1}',  '{5a869d0b-6611-11d3-bd2a-0000f80849bd}'

    {
      // InstructionBlock 0 [], 2 local(s), 0 handler(s), 3 child(ren), 0 sequence(s), 0-20
      .locals init ( [0]  class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32> q,
                     [2]  bool CS$4$0000)
      // InstructionBlock 1 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 0-7

    IL_0000:
      // {InstructionSequence 0 Original, Range=0-7 }
      .line 146,146 : 9,10  'C:\\My Software Projects\\XVSM\\SoftwareTransactionalMemory Prototype\\test.NTSM\\testit.cs'
      nop       
      .line 147,147 : 13,53
      newobj     instance void class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::.ctor()
      stloc.0   
      {
        // InstructionBlock 2 [], 1 local(s), 0 handler(s), 0 child(ren), 3 sequence(s), 7-1f
        .locals init ( [1]  int32 i)

      IL_0001:
        // {InstructionSequence 1 Original, Range=7-b }
        .line 148,148 : 18,28
        ldc.i4.0  
        stloc.1   
        .line 0x00feefee
        br.s       IL_0003

      IL_0002:
        // {InstructionSequence 2 Original, Range=b-17 }
        .line 149,149 : 17,30
        ldloc.0   
        ldloc.1   
        callvirt   instance void class [NSTM.Collections]NSTM.Collections.NstmQueue`1<int32>::Enqueue(!0)
        nop       
        .line 148,148 : 36,39
        ldloc.1   
        ldc.i4.1  
        add       
        stloc.1   

      IL_0003:
        // {InstructionSequence 3 Original, Range=17-1f }
        .line 148,148 : 29,34
        ldloc.1   
        ldarg.1   
        clt       
        stloc.2   
        .line 0x00feefee
        ldloc.2   
        brtrue.s   IL_0002
      }
      // InstructionBlock 3 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 1f-20

    IL_0004:
      // {InstructionSequence 4 Original, Range=1f-20 }
      .line 150,150 : 9,10
      ret       
    }
  }
  // End of method: test.testit/testPerfOfNstmQueue(int32 N) : void

  .method public hidebysig specialname rtspecialname
          instance void .ctor() cil managed
  {
    .maxstack 8
    // InstructionBlock 0 [], 0 local(s), 0 handler(s), 0 child(ren), 1 sequence(s), 0-7

  IL_0000:
    // {InstructionSequence 0 Original, Range=0-7 }
    ldarg.0   
    call       instance void [mscorlib]System.Object::.ctor()
    ret       
  }
  // End of method: test.testit/.ctor() : void

}
// End of type: test.testit


