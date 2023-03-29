using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Pagging;
using CPU.Registers;
using CPU.Instructions;
using Core.Memory;
using Core.CPU;
using Definitions.Enumerations;
using Definitions.Delegates;
using System.Diagnostics;

namespace CPU
{


    public class CPU : CPUBase
    {

        #region "Enumerations"


        #endregion

        #region "Constants"

        #region "Others"

        public const UInt64 const_LPF_MASK = 0xfffffffffffff000;  // linear page frame
        public const UInt64 const_PPF_MASK = 0xfffffffffffff000;  // physical page frame
        public const UInt64 const_Alignment_Check_Mask = 0x0;
        #endregion

        #region "Handles"

        protected const byte const_DEBUG_SINGLESTEP_RESET_EVENT = 0;
        protected const byte const_SHUTDOWN_RESET_EVENT = 1;

        #endregion

        #region  "Async"
        public const UInt16 const_BX_DEBUG_DR_ACCESS_BIT = (1 << 13);
        public const UInt16 const_BX_DEBUG_SINGLE_STEP_BIT = (1 << 14);
        public const UInt16 const_BX_DEBUG_TRAP_TASK_SWITCH_BIT = (1 << 15);
        public const UInt64 const_BX_ASYNC_EVENT_ZERO = 0;
        public const UInt64 const_BX_ASYNC_EVENT_ONE = 1;
        public const UInt64 const_BX_ASYNC_EVENT_STOP_TRACE = 0x80000000;
        #endregion

        #region "InHibit"
        public const byte const_BX_INHIBIT_INTERRUPTS = 0x01;
        public const byte const_BX_INHIBIT_DEBUG = 0x02;
        public const byte const_BX_INHIBIT_INTERRUPTS_SHADOW = 0x04;
        public const byte const_BX_INHIBIT_DEBUG_SHADOW = 0x08;

        public const byte const_BX_INHIBIT_INTERRUPTS_BY_MOVSS = (const_BX_INHIBIT_INTERRUPTS | const_BX_INHIBIT_DEBUG);
        public const byte const_BX_INHIBIT_INTERRUPTS_BY_MOVSS_SHADOW = (const_BX_INHIBIT_INTERRUPTS_SHADOW | const_BX_INHIBIT_DEBUG_SHADOW);
        #endregion

        #endregion

        #region "Attributes"


        #region "Config Support Attributes"
        protected bool mSUPPORT_X86_64;
        protected bool mSUPPORT_ALIGNMENT_CHECK = true;
        protected bool mSUPPORT_FPU = true;
        protected bool mSUPPORT_3DNOW = false;
        protected bool mSUPPORT_MISALIGNED_SSE = false;
        protected bool mSUPPORT_MONITOR_MWAIT = false;
        protected byte mSUPPORT_VMX = 0;
        protected bool mSUPPORT_TRACE_CACHE = true;
        protected bool mSUPPORT_APIC = true;
        #endregion

        protected byte mInhibitMask;
        protected UInt64 mAsyncEvent;
        protected UInt16 mDebugTrap;  // See DR6
        protected UInt64 mPrev_RIP;
        protected Register64 mPrev_RSP;
        protected UInt32 mAddrPage; // Helper represents TlbEntry.PPF

        protected CPUGeneralRegister mCPUGeneralRegisters;
        protected CPUSpecialRegister mCPUSpecialRegisters;
        protected Enum_CPUModes mCPUMode;
        protected Enum_CPUActivityState mCPUActivityState;
        protected TLB mTLB;
        protected SMRAM mSMRAM_Map = new SMRAM();
        protected Stack mStack;

        #region "address_xlation"
        protected UInt64 maddress_xlation_rm_addr;          // The address offset after resolution
        protected UInt64 maddress_xlation_paddress1;        // physical address after translation of 1st len1 bytes of data
        protected UInt64 maddress_xlation_paddress2;        // physical address after translation of 2nd len2 bytes of data
        protected UInt32 maddress_xlation_len1;             // Number of bytes in page 1
        protected UInt32 maddress_xlation_len2;             // Number of bytes in page 2
        protected UInt64 maddress_xlation_pages;            // Number of pages access spans (1 or 2).  Also used
        // for the case when a native host pointer is
        // available for the R-M-W instructions.  The host
        // pointer is stuffed here.  Since this field has
        // to be checked anyways (and thus cached), if it
        // is greated than 2 (the maximum possible for
        // normal cases) it is a native pointer and is used
        // for a direct write access.


        protected bool mSpeculativeRSP;
        #endregion

        #region "Control Signals"

        protected Enum_Signal mINTA;
        protected Enum_Signal mINTR;
        protected Enum_Signal mSMI;   // http://en.wikipedia.org/wiki/System_Management_Mode




        #endregion


        #region "Pagging & Addresses"
        protected UInt64 mFetchAddress;
        protected UInt64 mEIPFetchPtr;
        protected UInt32 mEIPPageWindowSize;
        //protected UInt64 mEIPBiased;
        protected UInt64 mEIPPageBias;
        protected UInt64 mCurrPageWriteStampPtr;
        protected ICache mICache;
        protected InstructionExecution mInstructionExecution;
        protected PageWriteStampTable mPageWriteStampTable;
        protected MemoryPagging mPagging;
        protected Resolver mResolver;
        #endregion

        #region "CPU Registers"


        #region "Control Registers"
        protected CR0_Register mCR0 = new CR0_Register();
        protected CR2_Register mCR2 = new CR2_Register();
        protected CR3_Register mCR3 = new CR3_Register();
        protected CR4_Register mCR4 = new CR4_Register();
        protected EFER_Register mEFER = new EFER_Register();
        #endregion

        #region "Debug Registers"

        /// <summary>
        /// <para>DR0 - DR3 Each of these registers contains the linear address associated with one of four breakpoint conditions.</para>
        /// <para>Each breakpoint condition is further defined by bits in DR7.</para>
        /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
        /// </summary>
        protected DR_Register mDR0 = new DR_Register("DR0");

        /// <summary>
        /// <para>DR0 - DR3 Each of these registers contains the linear address associated with one of four breakpoint conditions.</para>
        /// <para>Each breakpoint condition is further defined by bits in DR7.</para>
        /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
        /// </summary>
        protected DR_Register mDR1 = new DR_Register("DR1");

        /// <summary>
        /// <para>DR0 - DR3 Each of these registers contains the linear address associated with one of four breakpoint conditions.</para>
        /// <para>Each breakpoint condition is further defined by bits in DR7.</para>
        /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
        /// </summary>
        protected DR_Register mDR2 = new DR_Register("DR2");

        /// <summary>
        /// <para>DR0 - DR3 Each of these registers contains the linear address associated with one of four breakpoint conditions.</para>
        /// <para>Each breakpoint condition is further defined by bits in DR7.</para>
        /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
        /// </summary>
        protected DR_Register mDR3 = new DR_Register("DR3");

        /// <summary>
        /// DR6 - Debug status
        /// <para>The debug status register permits the debugger to determine which debug conditions have occurred. </para>
        /// <para>When the processor detects an enabled debug exception, it sets the low-order bits of this register (0,1,2,3) before entering the debug exception handler.</para>
        /// <para>Note that the bits of DR6 are never cleared by the processor. To avoid any confusion in identifying the next debug exception, the debug handler should move zeros to DR6 immediately before returning.</para>
        /// </summary>
        protected DR7_Register mDR6 = new DR7_Register("DR6");

        /// <summary>
        /// DR7 - Debug control
        /// <para>The low-order eight bits of DR7 (0,2,4,6 and 1,3,5,7) selectively enable the four address breakpoint conditions.</para>
        /// <para>There are two levels of enabling: the local (0,2,4,6) and global (1,3,5,7) levels. </para>
        /// <para>The local enable bits are automatically reset by the processor at every task switch to avoid unwanted breakpoint conditions in the new task.</para>
        /// <para>The global enable bits are not reset by a task switch; therefore, they can be used for conditions that are global to all tasks.</para>
        /// <para>Bits 16-17 (DR0), 20-21 (DR1), 24-25 (DR2), 28-29 (DR3), define when breakpoints trigger.</para>
        /// <para> Each breakpoint has a two-bit entry that specifies whether they break on execution (00b), data write (01b), data read or write (11b). </para>
        /// <para>10b is defined to mean break on IO read or write but no hardware supports it. Bits 18-19 (DR0), 22-23 (DR1), 26-27 (DR2), 30-31 (DR3), define how large area of memory is watched by breakpoints. Again each breakpoint has a two-bit entry that specifies whether they watch one (00b), two (01b), eight (10b) or four (11b) bytes.</para>
        /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
        /// </summary>
        protected DR7_Register mDR7 = new DR7_Register("DR7");
        #endregion

        #region "General Registers"

        protected Register64 mRAX;
        protected Register64 mRCX;
        protected Register64 mRDX;
        protected Register64 mRBX;

        protected Register64 mRSP;
        protected Register64 mRBP;
        protected Register64 mRSI;
        protected Register64 mRDI;

        protected Register64 mR8;
        protected Register64 mR9;
        protected Register64 mR10;
        protected Register64 mR11;
        protected Register64 mR12;
        protected Register64 mR13;
        protected Register64 mR14;
        protected Register64 mR15;



        #endregion

        #region "Segment Registers"

        protected SegmentRegister mES;
        protected SegmentRegister mCS;
        protected SegmentRegister mSS;
        protected SegmentRegister mDS;
        protected SegmentRegister mFS;
        protected SegmentRegister mGS;

        #endregion

        #region "Global Segment Registers"

        /// <summary>
        /// GDTR (Global Descriptor Table Register)
        /// </summary>
        protected GlobalSegmentRegister mGDTR;

        /// <summary>
        /// IDTR (Interrupt Descriptor Table Register)
        /// </summary>
        protected GlobalSegmentRegister mIDTR;


        /// <summary>
        /// LDTR (Local Descriptor Table Register)
        /// </summary>
        protected SegmentRegister mLDTR;
        #endregion

        #region "Special Registers"

        protected Register64 mRIP = new Register64("RIP", "EIP", "IP", "", "");
        protected RFlagsRegister mRFlags = new RFlagsRegister();
        #endregion


        /// <summary>
        /// The TR register is a 16-bit register which holds a segment selector for the TSS. 
        /// <para>It may be loaded through the LTR instruction. LTR is a privileged instruction and acts in a manner similar to other segment register loads. </para>
        /// <para>The task register has two parts: a portion visible and accessible by the programmer and an invisible one that is automatically loaded from the TSS descriptor</para>
        /// </summary>
        protected SegmentRegister mTR = new SegmentRegister("TR");
        #endregion

        #endregion

        #region "Properties"

        #region "CPU Registers"



        #region "Control Registers"
        public CR0_Register CR0
        {
            get
            {
                return mCR0;
            }
        }
        public CR2_Register CR2
        {
            get
            {
                return mCR2;
            }
        }
        public CR3_Register CR3
        {
            get
            {
                return mCR3;
            }
        }
        public CR4_Register CR4
        {
            get
            {
                return mCR4;
            }
        }
        public EFER_Register EFER
        {
            get
            {
                return mEFER;
            }
        }
        #endregion

        #region "General Registers"

        public Register64 RAX
        {
            get
            {
                return mRAX;
            }
            set
            {
                mRAX = value;
            }
        }


        public Register64 RBX
        {
            get
            {
                return mRBX;
            }
        }

        public Register64 RCX
        {
            get
            {
                return mRCX;
            }
        }

        public Register64 RDX
        {
            get
            {
                return mRDX;
            }
        }


        public Register64 RSP
        {
            get
            {
                return mRSP;
            }
        }

        public Register64 RBP
        {
            get
            {
                return mRBP;
            }
        }

        public Register64 RSI
        {
            get
            {
                return mRSI;
            }
        }

        public Register64 RDI
        {
            get
            {
                return mRDI;
            }
        }

        public Register64 R8
        {
            get
            {
                return mR8;
            }
        }

        public Register64 R9
        {
            get
            {
                return mR9;
            }
        }

        public Register64 R10
        {
            get
            {
                return mR10;
            }
        }

        public Register64 R11
        {
            get
            {
                return mR11;
            }
        }

        public Register64 R12
        {
            get
            {
                return mR12;
            }
        }

        public Register64 R13
        {
            get
            {
                return mR13;
            }
        }

        public Register64 R14
        {
            get
            {
                return mR14;
            }
        }

        public Register64 R15
        {
            get
            {
                return mR15;
            }
        }

        #endregion

        #region "Segment Registers"

        public SegmentRegister ES
        {
            get
            {
                return mES;
            }
        }

        public SegmentRegister CS
        {
            get
            {
                return mCS;
            }
        }

        public SegmentRegister SS
        {
            get
            {
                return mSS;
            }
        }

        public SegmentRegister DS
        {
            get
            {
                return mDS;
            }
        }

        public SegmentRegister FS
        {
            get
            {
                return mFS;
            }
        }

        public SegmentRegister GS
        {
            get
            {
                return mGS;
            }
        }


        #endregion

        #region "Global Segment Registers"

        public GlobalSegmentRegister GDTR
        {
            get
            {
                return mGDTR;
            }
            set
            {
                mGDTR = value;
            }
        }

        public GlobalSegmentRegister IDTR
        {
            get
            {
                return mIDTR;
            }
            set
            {
                mIDTR = value;
            }
        }

        #endregion

        #region "Special Registers"

        public Register64 RIP
        {
            get
            {
                return mRIP;
            }
            set
            {
                mRIP = value;
            }
        }

        public RFlagsRegister RFlags
        {
            get
            {
                return mRFlags;
            }
            set
            {
                mRFlags = value;
            }
        }
        #endregion


        #endregion


        #region "address_xlation"
        /// <summary>
        /// The address offset after resolution
        /// </summary>
        public UInt64 address_xlation_rm_addr
        {
            get
            {
                return maddress_xlation_rm_addr;
            }
            set
            {
                maddress_xlation_rm_addr = value;
            }
        }

        /// <summary>
        /// physical address after translation of 1st len1 bytes of data
        /// The address offset after resolution
        /// </summary>
        public UInt64 address_xlation_paddress1
        {
            get
            {
                return maddress_xlation_paddress1;
            }
            set
            {
                maddress_xlation_paddress1 = value;
            }
        }

        /// <summary>
        /// physical address after translation of 2nd len2 bytes of data
        /// The address offset after resolution
        /// </summary>
        public UInt64 address_xlation_paddress2
        {
            get
            {
                return maddress_xlation_paddress2;
            }
            set
            {
                maddress_xlation_paddress2 = value;
            }
        }

        /// <summary>
        /// Number of bytes in page 1
        /// The address offset after resolution
        /// </summary>
        public UInt32 address_xlation_len1
        {
            get
            {
                return maddress_xlation_len1;
            }
            set
            {
                maddress_xlation_len1 = value;
            }
        }

        /// <summary>
        /// Number of bytes in page 2
        /// The address offset after resolution
        /// </summary>
        public UInt32 address_xlation_len2
        {
            get
            {
                return maddress_xlation_len2;
            }
            set
            {
                maddress_xlation_len2 = value;
            }
        }


        /// <summary>
        /// Number of pages access spans (1 or 2).  
        /// Also used the address offset after resolution
        /// </summary>
        public UInt64 address_xlation_pages
        {
            get
            {
                return maddress_xlation_pages;
            }
            set
            {
                maddress_xlation_pages = value;
            }
        }

        #endregion

        public SMRAM SMRAM_Map
        {
            get
            {
                return mSMRAM_Map;
            }
        }

        public Stack Stack
        {
            get
            {
                return mStack;
            }
        }



        public TLB TLB
        {
            get
            {
                return mTLB;
            }
        }

        public UInt16 DebugTrap
        {
            get
            {
                return mDebugTrap;
            }
            set
            {
                mDebugTrap = value;
            }
        }
        public override Enum_CPUModes CPUMode
        {
            get
            {
                return mCPUMode;
            }
            set
            {
                mCPUMode = value;
                EvaluateCPUMode();
            }
        }
        public Enum_CPUActivityState CPUActivityState
        {
            get
            {
                return mCPUActivityState;
            }

            set
            {
                mCPUActivityState = value;
            }
        }

        public bool SUPPORT_X86_64
        {
            get
            {
                return mSUPPORT_X86_64;
            }
            set
            {
                mSUPPORT_X86_64 = value;
            }

        }

        public bool SUPPORT_TRACE_CACHE
        {
            get
            {
                return mSUPPORT_TRACE_CACHE;
            }
            set
            {
                mSUPPORT_TRACE_CACHE = value;
            }

        }

        public bool SUPPORT_APIC
        {
            get
            {
                return mSUPPORT_APIC;
            }
            set
            {
                mSUPPORT_APIC = value;
            }
        }

        public byte SUPPORT_VMX
        {
            get
            {
                return mSUPPORT_VMX;
            }
            set
            {
                mSUPPORT_VMX = value;
            }
        }

        #region "Control Signals"
        public Enum_Signal INTR
        {
            set
            {
                mINTR = value;
            }
            get
            {
                return mINTR;/*|| mApic.INTR*/
            }
        }
        #endregion

        #region "Pagging & Addresses"

        public UInt64 FetchAddress
        {
            get
            {
                return mFetchAddress;
            }
            set
            {
                mFetchAddress = value;
            }
        }

        /// <summary>
        /// calculated by getHostMemAddr () where the instruction
        /// Page in the host (real machine) on the base address
        /// </summary>
        public UInt64 EIPFetchPtr
        {
            get
            {
                return mEIPFetchPtr;
            }
            set
            {
                mEIPFetchPtr = value;
            }
        }
        public UInt32 EIPPageWindowSize
        {
            get
            {
                return mEIPPageWindowSize;
            }
            set
            {
                mEIPPageWindowSize = value;
            }
        }
        ////public UInt64 EIPBiased
        ////{
        ////    get
        ////    {
        ////        return mEIPBiased;
        ////    }
        ////    set
        ////    {
        ////        mEIPBiased = value;
        ////    }
        ////}
        public UInt64 EIPPageBias
        {
            get
            {
                // PageOffset - EIP 
                // return Pagging.Pagging.PageOffset((uint) mFetchAddress) - RIP.Value32;
                return mEIPPageBias;
            }
            set
            {
                mEIPPageBias = value;
            }
        }

        public Resolver Resolver
        {
            get
            {
                return mResolver;
            }
        }
        #endregion

        public UInt64 CurrPageWriteStampPtr
        {
            get
            {
                return mCurrPageWriteStampPtr;
            }
            set
            {
                mCurrPageWriteStampPtr = value;
            }
        }


        public InstructionExecution InstructionExecution
        {
            get
            {
                return mInstructionExecution;
            }
        }

        public ICache ICache
        {
            get
            {
                return mICache;
            }
        }

        public PageWriteStampTable PageWriteStampTable
        {
            get
            {
                return mPageWriteStampTable;
            }
        }


        /// <summary>
        /// This is a helper flag for decoding and excution
        /// it helps to tell CPU to flush Instruction Cache 
        /// </summary>
        public UInt64 AsyncEvent
        {
            get
            {
                return mAsyncEvent;
            }
            set
            {
                mAsyncEvent = value;
            }
        }

        /// <summary>
        /// This flag is used to control interrupts in CPU
        /// </summary>
        public byte InhibitMask
        {
            get
            {
                return mInhibitMask;
            }
            set
            {
                mInhibitMask = value;
            }
        }

        #region "Helper ShortCuts"

        public ushort CPL
        {
            get
            {
                return CS.Selector.Selector_RPL;
            }
        }


        #endregion

        #endregion

        #region "Constructor"

        public CPU(Core.PCBoard.PCBoard PCBoard)
        {
            mAsyncEvent = const_BX_ASYNC_EVENT_ZERO;
            mPCBoard = PCBoard;
            mInstructionExecution = new InstructionExecution(this);
            mICache = new ICache(this);
            mPageWriteStampTable = new PageWriteStampTable(this);

            #region "General Registers"

            mRAX = new Register64("RAX", "EAX", "AX", "AH", "AL");
            mRCX = new Register64("RCX", "ECX", "CX", "CH", "CL");
            mRDX = new Register64("RDX", "EDX", "DX", "DH", "DL");
            mRBX = new Register64("RBX", "EBX", "BX", "BH", "BL");

            mPrev_RSP = new Register64("RSP", "ESP", "SP", "", "");
            mRSP = new Register64("RSP", "ESP", "SP", "", "");
            mRBP = new Register64("RBP", "EBP", "BP", "", "");
            mRSI = new Register64("RSI", "ESI", "SI", "", "");
            mRDI = new Register64("RDI", "EDI", "DI", "", "");

            mR8 = new Register64("R8");
            mR9 = new Register64("R9");
            mR10 = new Register64("R10");
            mR11 = new Register64("R11");
            mR12 = new Register64("R12");
            mR13 = new Register64("R13");
            mR14 = new Register64("R14");
            mR15 = new Register64("R15");

            #endregion

            mTLB = new TLB(this);
            mPagging = new MemoryPagging(this);
            mResolver = new Resolver(this);
            #region "Segment Registers"

            mES = new SegmentRegister("ES");
            mCS = new SegmentRegister("CS");
            mSS = new SegmentRegister("SS");
            mDS = new SegmentRegister("DS");
            mFS = new SegmentRegister("FS");
            mGS = new SegmentRegister("GS");

            #endregion


            #region "Global Segment Register"
            mGDTR = new GlobalSegmentRegister("GDTR");
            mIDTR = new GlobalSegmentRegister("IDTR");
            mLDTR = new SegmentRegister("LDTR");

            #endregion


            mCPUGeneralRegisters = new CPUGeneralRegister(this);
            mCPUSpecialRegisters = new CPUSpecialRegister(this);
            mCPUGeneralRegisters.Initialize();
            mCPUSpecialRegisters.Initialize();
            mPCBoard.Scheduler.RegisterSchedule(new Core.Simulator.ScheduleEntry(this.CPULoop, "CPU LOOP", 0, true, 10, true));


        }

        #endregion

        #region "Methods"




        /// <summary>
        /// Full details at <see cref="http://www.sandpile.org/ia32/cpuid.htm"/>
        /// Get CPU version information.
        /// </summary>
        /// <returns></returns>
        public UInt32 Get_CPU_Version_Information()
        {
            UInt32 family = 0, model = 0, stepping = 0;
            UInt32 extended_model = 0;
            UInt32 extended_family = 0;

            extended_family = 0;
            extended_model = 0;
            family = 0x0f;
            model = 2;
            stepping = 0;


            return (extended_family << 20) |
                   (extended_model << 16) |
                   (family << 8) |
                   (model << 4) | stepping;
        }


        #region "Reset Methods"

        public override void ResetCPU()
        {
            this.CPUActivityState = Enum_CPUActivityState.BX_Activity_State_ACTIVE;
            this.InhibitMask = 0;
            this.DebugTrap = 0;
            this.mStack = new Stack(this);

            ResetIO();
            ResetSMRAM();
            ResetPagging();
            ResetCPURegisters();
            mTLB.InitTLB();

            EvaluateCPUMode();

            //ResetCPU_I387();
            //ResetFloatingPointUnit();
            //ResetXMM();



        }

        protected void ResetIO()
        {
            mINTA = Enum_Signal.Low;
        }

        protected void ResetPagging()
        {
            mTLB.InitTLB();
            EIPPageBias = 0;
            EIPPageWindowSize = 0;
            FetchAddress = 0x000;
            EIPFetchPtr = 0;
        }

        protected void ResetSMRAM()
        {
            mSMRAM_Map.InitSMMRAM();
        }
        protected void ResetCPURegisters()
        {

            #region "Control Registers"
            if (mCPULevel != 3)
            {
                this.mCR0.Value32 = 0x60000010;
            }
            else
            {
                this.mCR0.Value32 = 0x7ffffff0;
            }

            this.mCR2.Reset();
            this.mCR3.Reset();
            this.mCR4.Reset();
            this.mEFER.Reset();
            #endregion

            #region "DebugRegisters"
            if (mCPULevel >= 3)
            {

                this.mDR0.Reset();
                this.mDR1.Reset();
                this.mDR2.Reset();
                this.mDR3.Reset();

            }

            /*
             * BX_CPU_LEVEL == 3 : DR6 = 0xFFFF1FF0;
             * BX_CPU_LEVEL == 4 : DR6 = 0xFFFF1FF0;
             * BX_CPU_LEVEL == 5 : DR6 = 0xFFFF0FF0;
             */
            switch (mCPULevel)
            {
                case 3:
                    mDR6.Value32 = 0xFFFF1FF0;
                    break;
                case 4:
                    mDR6.Value32 = 0xFFFF1FF0;
                    break;
                case 5:
                    mDR6.Value32 = 0xFFFF0FF0;
                    break;
                case 6:
                    mDR6.Value32 = 0xFFFF0FF0;
                    break;
            }

            mDR7.Value32 = 0x00000400;

            #endregion

            #region "General Registers"

            this.RAX.Value32 = 0; // processor passed test :-)
            this.RBX.Reset();
            this.RCX.Reset();
            this.RDX.Value32 = Get_CPU_Version_Information();

            if (mCPULevel < 2)
            {
                this.mRIP.Value32 = 0x00000000;
            }
            else
            {
                this.mRIP.Value64 = 0x000000000000fff0;
            }

            this.mRFlags.Reset();

            #endregion

            #region "Segment Register"

            #region "CS Register"

            /* CS (Code Segment) and descriptor cache */
            /* Note: on a real cpu, CS initially points to upper memory.  After
             * the 1st jump, the descriptor base is zero'd out.  
             * Since I'm just going to jump to my BIOS, I don't need to do this.
             * For future reference:
             *   processor  cs.selector   cs.base    cs.limit    EIP
             *        8086    FFFF          FFFF0        FFFF   0000
             *        286     F000         FF0000        FFFF   FFF0
             *        386+    F000       FFFF0000        FFFF   FFF0
             */

            this.mCS.Reset();
            this.mCS.Selector.Selector_Value = 0xf000;
            this.mCS.Cache_u_Segment_Base = 0xffff0000;
            this.mCS.Cache_u_Segment_LimitScaled = 0xFFFF;
            #endregion

            this.mDS.Reset();
            this.mES.Reset();
            this.mSS.Reset();
            this.mFS.Reset();
            this.mGS.Reset();


            #endregion


            #region "Descriptor Table Registers"


            #region "GDTR - (Global Descriptor Table Register) "
            this.mGDTR.Base = 0x00000000;
            this.mGDTR.Limit = 0xFFFF; // always byte granular
            #endregion

            #region "IDTR - (Interrupt Descriptor Table Register)"
            this.mIDTR.Base = 0x00000000;
            this.mIDTR.Limit = 0xFFFF; // always byte granular
            #endregion

            #region "LDTR"
            mLDTR.Selector.Selector_Value = 0x0000;
            mLDTR.Selector.Selector_Index = 0x0000;
            mLDTR.Cache_Valid = 1;
            mLDTR.Cache_Present = true;
            mLDTR.Cache_DPL = 0;
            mLDTR.Cache_Segment = SegmentRegister.Enum_SegmentType.SystemGate;
            mLDTR.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.SysSegment_LDT;
            mLDTR.Cache_u_Segment_Base = 0x00000000;
            mLDTR.Cache_u_Segment_LimitScaled = 0xffff;
            mLDTR.Cache_u_Segment_G = SegmentRegister.Enum_Granularity.gradularity_byte;
            mLDTR.Cache_u_Segment_AVL = 0;
            #endregion

            #endregion

            #region "Task Register"

            mTR.Selector.Selector_Value = 0x0000;
            mTR.Selector.Selector_Index = 0x0000;    // Undefined
            mTR.Cache_Valid = 1;            // valid
            mTR.Cache_DPL = 0;              // field ot used
            mTR.Cache_Segment = 0;          // System Segment
            mTR.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.SysSegmentBusy_386_TSS;
            mTR.Cache_u_Segment_Base = 0x00000000;
            mTR.Cache_u_Segment_LimitScaled = 0xffff;
            mTR.Cache_u_Segment_AVL = 0;
            mTR.Cache_u_Segment_G = SegmentRegister.Enum_Granularity.gradularity_byte;

            #endregion


        }
        #endregion

        #region "General Register Access"
        /// <summary>
        /// Gets CPU registers by Index.
        /// This is mainly used by CPU instructions executors.
        /// <para>This is equivelant to:</para>
        /// <para>BX_READ_16BIT_REG and any BX_READ_xxBIT_REG</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <returns>An empty register with value zero is returned if register is not found.
        /// <para>This is to simulate Bochs Behaviour</para></returns>
        public Register GetCPUGeneralRegisters(byte Index)
        {
            Register oRegister;
            if (mCPUGeneralRegisters.TryGetValue(Index, out oRegister) == false) return new Register64("NoName");
            return oRegister;
        }

        /// <summary>
        /// Gets Special CPU registers by Index.
        /// This is mainly used by CPU instructions executors.
        /// <para>This is equivelant to:</para>
        /// <para>BX_READ_16BIT_REG and any BX_READ_xxBIT_REG</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public SegmentRegister GetCPUSpeciallRegisters(byte Index)
        {
            Register oRegister;
            mCPUSpecialRegisters.TryGetValue(Index, out oRegister);
            return (SegmentRegister)oRegister;
        }

        /// <summary>
        /// Reads value in 8 bit Register "General Registers" using Register Index.
        /// <para>e.g.: al,dl..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Extended"></param>
        /// <returns></returns>
        public byte Read8BitRegX(byte Index, byte Extended)
        {
            if (((Index & 4) == 0) || (Extended != 0))
            {
                return ((Register8)(GetCPUGeneralRegisters(Index))).Value8;
            }
            else
            {
                return ((Register16)(GetCPUGeneralRegisters((byte)(Index - 4)))).Value8H;
            }
        }


        /// <summary>
        /// Reads Name in 8 bit Register "General Registers" using Register Index.
        /// <para>e.g.: al,dl..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Extended"></param>
        /// <returns></returns>
        public string Get8BitRegX(byte Index, byte Extended)
        {
            if (((Index & 4) == 0) || (Extended != 0))
            {
                return ((Register8)(GetCPUGeneralRegisters(Index))).Name8;
            }
            else
            {
                return ((Register16)(GetCPUGeneralRegisters((byte)(Index - 4)))).Name8H;
            }
        }

        /// <summary>
        /// Read value in 8 bit Register "General Registers" using Register Index.
        /// <para>e.g.: ah,dh..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Register8 Read8BitRegH(byte Index)
        {
            return ((Register16)(GetCPUGeneralRegisters((byte)Index)));
        }


        /// <summary>
        /// Read value in 32 bit Register "General Registers" using Register Index.
        /// <para>e.g.: eax,edx..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Register16 Read16BitRegX(byte Index)
        {
            return ((Register16)(GetCPUGeneralRegisters((byte)Index)));
        }

        /// <summary>
        /// Read value in 32 bit Register "General Registers" using Register Index.
        /// <para>e.g.: eax,edx..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public UInt32 Read32BitRegX(byte Index)
        {
            return ((Register32)(GetCPUGeneralRegisters((byte)Index))).Value32;
        }


        /// <summary>
        /// Returns register name:
        /// </summary>
        /// <param name="Index"></param>
        public string Get32BitRegX(byte Index)
        {
            return ((Register32)(GetCPUGeneralRegisters((byte)Index))).Name32;
        }

        /// <summary>
        /// Write value in 8 bit Register "General Registers" using Register Index.
        /// <para>e.g.: al,dl..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Extended"></param>
        /// <param name="Value"></param>
        public void Write8BitRegX(byte Index, byte Extended, byte Value)
        {
            if (((Index & 4) == 0) || (Extended != 0))
            {
                ((Register8)(GetCPUGeneralRegisters(Index))).Value8 = Value;
            }
            else
            {
                ((Register16)(GetCPUGeneralRegisters((byte)(Index - 4)))).Value8H = Value;
            }
        }

        /// <summary>
        /// Write value in 8 bit Register "General Registers" using Register Index.
        /// <para>e.g.: ah,dh..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Value"></param>
        public void Write8BitRegH(byte Index, byte Value)
        {
            ((Register16)(GetCPUGeneralRegisters((byte)Index))).Value8H = Value;
        }


        /// <summary>
        /// Returns register name:
        /// </summary>
        /// <param name="Index"></param>
        public string Get8BitRegH(byte Index)
        {
            return ((Register16)(GetCPUGeneralRegisters((byte)Index))).Name8H;
        }

        /// <summary>
        /// Write value in 16 bit Register "General Registers" using Register Index.
        /// <para>e.g.: ax,dx..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Value"></param>
        public void Write16BitRegX(byte Index, UInt16 Value)
        {
            ((Register16)(GetCPUGeneralRegisters((byte)Index))).Value16 = Value;
        }

        /// <summary>
        /// Write value in 16 bit Register "Special Registers" using Register Index.
        /// <para>e.g.: es,ds..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Value"></param>
        public void Write16BitSegRegX(byte Index, UInt16 Value)
        {
            ((Register16)(GetCPUSpeciallRegisters((byte)Index))).Value16 = Value;
        }

        /// <summary>
        /// Returns register name:
        /// </summary>
        /// <param name="Index"></param>
        public string Get16BitRegX(byte Index)
        {
            return ((Register16)(GetCPUGeneralRegisters((byte)Index))).Name16;
        }

        /// <summary>
        /// Write value in 32 bit Register "General Registers" using Register Index.
        /// <para>e.g.: eax,edx..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Value"></param>
        public void Write32BitRegX(byte Index, UInt32 Value)
        {
            ((Register32)(GetCPUGeneralRegisters((byte)Index))).Value32 = Value;
        }


        /// <summary>
        /// Write value in 64 bit Register "General Registers" using Register Index.
        /// <para>e.g.: rax,rdx..etc.</para>
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Value"></param>
        public void Write64BitRegX(byte Index, UInt64 Value)
        {
            ((Register64)(GetCPUGeneralRegisters((byte)Index))).Value64 = Value;
        }


        /// <summary>
        /// equivelent to: #define BX_WRITE_64BIT_REG(index, val) in Bochs
        /// </summary>
        /// <param name="Index"></param>
        public void Clear64BitHigh(byte Index)
        {
            ((Register64)(GetCPUGeneralRegisters((byte)Index))).Value32H = 0;
        }

        #endregion



        protected void EvaluateCPUMode()
        {
            if (this.mEFER.LMA == Enum_Signal.High)
            {
                if (this.mCR0.PE == Enum_Signal.Low)
                {
                    throw new Exception("change_cpu_mode: EFER.LMA is set when CR0.PE=0 !");
                }
                if (this.mCS.Cache_u_Segment_LongMode == SegmentRegister.Enum_LongMode.bit64)
                {
                    mCPUMode = Enum_CPUModes.LongMode;
                }
                else
                {
                    mCPUMode = Enum_CPUModes.LongCompatMode;
                    RIP.Reset();
                    RSP.Reset();
                }
            }
            else
            {
                if (mCR0.PE == Enum_Signal.High)
                {
                    if (mRFlags.VM == Enum_Signal.High)
                    {
                        mCPUMode = Enum_CPUModes.Virtual8086Mode;
                    }
                    else
                    {
                        mCPUMode = Enum_CPUModes.ProtectedMode;
                    }
                }
                else
                {
                    mCPUMode = Enum_CPUModes.RealMode;

                    // CS segment in real mode always allows full access
                    CS.Cache_Present = true;
                    CS.Cache_Segment = SegmentRegister.Enum_SegmentType.DataOrCode;  /* data/code segment */
                    CS.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.DataReadWriteAccessed; // BX_DATA_READ_WRITE_ACCESSED;


                }
            }

            OnCPUModeChange(mCPUMode);
        }

        public override void CPULoop()
        {

            FetchNext();
            Execute();

            /*
            // check on events which occurred for previous instructions (traps)
            // and ones which are asynchronous to the CPU (hardware interrupts)
             */
            HandleAsyncEvent();
        }

        #region "Memory Translation"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Seg"></param>
        /// <param name="Offset"></param>
        /// <returns>Seg.Cache.Base + Offset</returns>
        public UInt32 GetLAddress32(SegmentRegister Seg, UInt32 Offset)
        {
            return ((UInt32)Seg.Cache_u_Segment_Base) + Offset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Seg"></param>
        /// <param name="Offset"></param>
        /// <returns>Seg.Cache.Base + Offset</returns>
        public UInt64 GetLAddress64(SegmentRegister Seg, UInt64 Offset)
        {
            if (Seg.Name == "FS" || Seg.Name == "GS")
            {
                // Segment logic is disabled for FS & GS
                return Offset;
            }

            return Seg.Cache_u_Segment_Base + Offset;


        }

        #region "WriteVirtual32"

        public void WriteVirtualByte32(byte SegmentRegIndex, UInt32 Offset, byte Data)
        {
            SegmentRegister Seg = (SegmentRegister)this.GetCPUSpeciallRegisters(SegmentRegIndex);

            if ((Seg.Cache_Valid & (uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK) != 0)
            {
                if (!(Offset <= Seg.Cache_u_Segment_LimitScaled))
                {
                    /*
                      BX_ERROR(("write_virtual_byte_32(): segment limit violation"));
                      exception(int_number(s), 0, 0);
                     */
                    throw new InvalidOperationException("write_virtual_byte_32(): segment limit violation");
                }
            }
            else
            {
                if (WriteVirtualChecks(Seg, Offset, 1) == false)
                    throw new Exception("WriteVirtualChecks (Seg,Offset,1)==false)");
            }

            UInt32 lAddr;
            lAddr = GetLAddress32(Seg, Offset);
            UInt16 tlbIndex = this.mTLB.IndexOf(lAddr, 0);
            UInt64 LPF = this.LPFOf(lAddr);
            TLBEntry oTLBEntry = this.mTLB[tlbIndex];
            if (oTLBEntry.LPF == LPF)
            {
                // See if the TLB entry privilege level allows us write access
                // from this CPL.
                if ((oTLBEntry.AccessBits & (0x02 | User_PL())) == 0)
                {
                    uint HostPageAddr = oTLBEntry.HostPageAddress;
                    UInt64 oPageOffset = this.PageOffset(lAddr);
                    /*
                     *  BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr, tlbEntry->ppf | pageOffset, 1, BX_WRITE);
                          BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
                              tlbEntry->ppf | pageOffset, 1, CPL, BX_WRITE, (Bit8u*) &data);
                     * 
                     * Bit8u *hostAddr = (Bit8u*) (hostPageAddr | pageOffset);
                       pageWriteStampTable.decWriteStamp(pAddr, 1);
                       *hostAddr = data;
                     */
                    uint hostAddr = (uint)(HostPageAddr | oPageOffset);
                    this.PageWriteStampTable.decWriteStamp(oTLBEntry.PPF, 1);
                    this.MainMemory.WriteByte(hostAddr, Data);
                    return;

                }
            }
            mPagging.AccessWriteLinear(lAddr, 1, CPL, BitConverter.GetBytes(Data));

            return;
        }


        public void WriteVirtualWord(byte SegmentRegIndex, UInt64 Offset, UInt16 Data)
        {
            if (this.CPUMode != Enum_CPUModes.LongMode)
            {
                WriteVirtualWord32(SegmentRegIndex, (UInt32)Offset, Data);
            }
            else
            {
                WriteVirtualWord64(SegmentRegIndex, Offset, Data);

            }
        }


        public void WriteVirtualWord32(byte SegmentRegIndex, UInt32 Offset, UInt16 Data)
        {
            /*
             *  The code has been refactored to remove the goto statement from bochs.
             * */
            UInt32 lAddr;
            SegmentRegister SegReg = GetCPUSpeciallRegisters(SegmentRegIndex);

            if (this.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new Exception("Invalid operation");
            }

            if ((SegReg.Cache_Valid & ((uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK)) == 0)
            {
                if (!(Offset < SegReg.Cache_u_Segment_LimitScaled))
                {
                    throw new Exception("write_virtual_word_32(): segment limit violation");
                }
            }
            else
            {
                if (!this.WriteVirtualChecks(SegReg, Offset, 2))
                    throw new Exception("WriteVirtualChecks failed");
            }



            // accessOK: label in originl Bochs [code mark]
            lAddr = GetLAddress32(SegReg, Offset);
            ushort tlbIndex = this.mTLB.IndexOf(lAddr, 1);

            UInt64 lpf;
            if ((mSUPPORT_ALIGNMENT_CHECK == true) && (this.CPULevel >= 4))
            {
                lpf = AlignedAccessLPFOf(lAddr, (1 & const_Alignment_Check_Mask));
            }
            else
            {

                lpf = LPFOf(lAddr);
            }
            TLBEntry tlbEntry = this.mTLB[tlbIndex];
            if (tlbEntry.LPF == lpf)
            {
                // See if the TLB entry privilege level allows us write access from this CPL.
                if ((tlbEntry.AccessBits & (0x2 | this.User_PL())) == 0)
                {
                    uint hostPageAddr = tlbEntry.HostPageAddress;
                    uint pageOffset = this.PageOffset(lAddr);
                    UInt64 pAddr = tlbEntry.PPF | pageOffset;
                    //BX_INSTR_LIN_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, Enum_MemoryAccessType.Write);
                    //BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, CPL, Enum_MemoryAccessType.Write, (Bit8u*) &Data);
                    UInt16 hostAddr = (UInt16)(hostPageAddr | pageOffset);
                    this.PageWriteStampTable.decWriteStamp(pAddr, 2);
                    this.MainMemory.WriteWord(hostAddr, BitConverter.GetBytes(Data));
                    /////// WriteHostWordToLittleEndian(hostAddr, Data);
                    return;
                }
            }

            /***                    
                 #if BX_CPU_LEVEL >= 4 && BX_SUPPORT_ALIGNMENT_CHECK
                  if (BX_CPU_THIS_PTR alignment_check()) {
                    if (laddr & 1) {
                      BX_ERROR(("write_virtual_word_32(): #AC misaligned access"));
                      exception(BX_AC_EXCEPTION, 0);
                    }
                  }
            #endif
            ****/
            ///// access_write_linear(laddr, 2, CPL, (void *) &data);
            mPagging.AccessWriteLinear(lAddr, 2, CPL, BitConverter.GetBytes(Data));

            return;

        }

        public void WriteVirtualDWord32(byte SegmentRegIndex, UInt32 Offset, UInt32 Data)
        {
            /*
           *  The code has been refactored to remove the goto statement from bochs.
           * */
            UInt32 lAddr;
            SegmentRegister SegReg = GetCPUSpeciallRegisters(SegmentRegIndex);

            if (this.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new Exception("Invalid operation");
            }

            if ((SegReg.Cache_Valid & ((uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK)) == 0)
            {
                if (!(Offset < SegReg.Cache_u_Segment_LimitScaled - 2))
                {
                    throw new Exception("write_virtual_word_32(): segment limit violation");
                }
            }
            else
            {
                if (!this.WriteVirtualChecks(SegReg, Offset, 4))
                    throw new Exception("WriteVirtualChecks failed");
            }



            // accessOK: label in originl Bochs [code mark]
            lAddr = GetLAddress32(SegReg, Offset);
            ushort tlbIndex = this.mTLB.IndexOf(lAddr, 3);

            UInt64 lpf;
            if ((mSUPPORT_ALIGNMENT_CHECK == true) && (this.CPULevel >= 4))
            {
                lpf = AlignedAccessLPFOf(lAddr, (3 & const_Alignment_Check_Mask));
            }
            else
            {

                lpf = LPFOf(lAddr);
            }
            TLBEntry tlbEntry = this.mTLB[tlbIndex];
            if (tlbEntry.LPF == lpf)
            {
                // See if the TLB entry privilege level allows us write access from this CPL.
                if ((tlbEntry.AccessBits & (0x2 | this.User_PL())) == 0)
                {
                    uint hostPageAddr = tlbEntry.HostPageAddress;
                    uint pageOffset = this.PageOffset(lAddr);
                    UInt64 pAddr = tlbEntry.PPF | pageOffset;
                    //BX_INSTR_LIN_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, Enum_MemoryAccessType.Write);
                    //BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, CPL, Enum_MemoryAccessType.Write, (Bit8u*) &Data);
                    UInt16 hostAddr = (UInt16)(hostPageAddr | pageOffset);
                    this.PageWriteStampTable.decWriteStamp(pAddr, 4);
                    this.MainMemory.WriteWord(hostAddr, BitConverter.GetBytes(Data));
                    /////// WriteHostWordToLittleEndian(hostAddr, Data);
                    return;
                }
            }

            /***                    
               #if BX_CPU_LEVEL >= 4 && BX_SUPPORT_ALIGNMENT_CHECK
      if (BX_CPU_THIS_PTR alignment_check()) {
        if (laddr & 7) {
          BX_ERROR(("write_virtual_qword_32(): #AC misaligned access"));
          exception(BX_AC_EXCEPTION, 0);
        }
      }
#endif
            #endif
            ****/
            ///// access_write_linear(laddr, 4, CPL, (void *) &data);
            mPagging.AccessWriteLinear(lAddr, 4, CPL, BitConverter.GetBytes(Data));


        }

        public void WriteVirtualQWord32(byte SegmentRegIndex, UInt32 Offset, UInt64 Data)
        {
            /*
        *  The code has been refactored to remove the goto statement from bochs.
        * */
            UInt32 lAddr;
            SegmentRegister SegReg = GetCPUSpeciallRegisters(SegmentRegIndex);

            if (this.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new Exception("Invalid operation");
            }

            if ((SegReg.Cache_Valid & ((uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK)) == 0)
            {
                if (!(Offset < SegReg.Cache_u_Segment_LimitScaled - 7))
                {
                    throw new Exception("write_virtual_word_32(): segment limit violation");
                }
            }
            else
            {
                if (!this.WriteVirtualChecks(SegReg, Offset, 8))
                    throw new Exception("WriteVirtualChecks failed");
            }



            // accessOK: label in originl Bochs [code mark]
            lAddr = GetLAddress32(SegReg, Offset);
            ushort tlbIndex = this.mTLB.IndexOf(lAddr, 7);

            UInt64 lpf;
            if ((mSUPPORT_ALIGNMENT_CHECK == true) && (this.CPULevel >= 4))
            {
                lpf = AlignedAccessLPFOf(lAddr, (7 & const_Alignment_Check_Mask));
            }
            else
            {

                lpf = LPFOf(lAddr);
            }
            TLBEntry tlbEntry = this.mTLB[tlbIndex];
            if (tlbEntry.LPF == lpf)
            {
                // See if the TLB entry privilege level allows us write access from this CPL.
                if ((tlbEntry.AccessBits & (0x2 | this.User_PL())) == 0)
                {
                    uint hostPageAddr = tlbEntry.HostPageAddress;
                    uint pageOffset = this.PageOffset(lAddr);
                    UInt64 pAddr = tlbEntry.PPF | pageOffset;
                    //BX_INSTR_LIN_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, Enum_MemoryAccessType.Write);
                    //BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, CPL, Enum_MemoryAccessType.Write, (Bit8u*) &Data);
                    UInt16 hostAddr = (UInt16)(hostPageAddr | pageOffset);
                    this.PageWriteStampTable.decWriteStamp(pAddr, 8);
                    this.MainMemory.WriteWord(hostAddr, BitConverter.GetBytes(Data));
                    /////// WriteHostWordToLittleEndian(hostAddr, Data);
                    return;
                }
            }

            /***                    
               #if BX_CPU_LEVEL >= 4 && BX_SUPPORT_ALIGNMENT_CHECK
                  if (BX_CPU_THIS_PTR alignment_check()) {
                    if (laddr & 7) {
                      BX_ERROR(("write_virtual_qword_32(): #AC misaligned access"));
                      exception(BX_AC_EXCEPTION, 0);
                    }
                  }
            #endif
            ****/
            ///// access_write_linear(laddr, 8, CPL, (void *) &data);
            mPagging.AccessWriteLinear(lAddr, 8, CPL, BitConverter.GetBytes(Data));

            return;

        }


        public void WriteVirtualDQWord32(byte SegmentRegIndex, UInt32 Offset, byte Data)
        {
            // ToDo: not implemented
            throw new NotImplementedException("");
        }


        #endregion

        public void WriteVirtualWord64(byte SegmentRegIndex, UInt64 Offset, UInt16 Data)
        {
            // ToDo: not implemented
            throw new NotImplementedException("");
        }

        protected bool WriteVirtualChecks(SegmentRegister Segment, uint Offset, uint Length)
        {
            UInt32 UpperLimit = 0;
            if (Segment.Cache_Valid == 0)
            {
                //BX_DEBUG(("write_virtual_checks(): segment descriptor not valid"));
                return false;
            }


            if (Segment.Cache_Present == false)  /* not present */
            {
                //BX_ERROR(("write_virtual_checks(): segment not present"));
                return false;
            }

            switch ((int)Segment.Cache_Type)
            {
                case 0:
                case 1:   // read only
                case 4:
                case 5:   // read only, expand down
                case 8:
                case 9:   // execute only
                case 10:
                case 11: // execute/read
                case 12:
                case 13: // execute only, conforming
                case 14:
                case 15: // execute/read-only, conforming
                    // BX_ERROR(("write_virtual_checks(): no write access to seg"));
                    return false;

                case 2:
                case 3: /* read/write */
                    if (Offset > (Segment.Cache_u_Segment_LimitScaled - Length + 1)
                        || (Length - 1 > Segment.Cache_u_Segment_LimitScaled))
                    {
                        //BX_ERROR(("write_virtual_checks(): write beyond limit, r/w"));
                        return false;
                    }
                    if (Segment.Cache_u_Segment_LimitScaled >= 15)
                    {
                        // Mark cache as being OK type for succeeding read/writes. The limit
                        // checks still needs to be done though, but is more simple. We
                        // could probably also optimize that out with a flag for the case
                        // when limit is the maximum 32bit value. Limit should accomodate
                        // at least a dword, since we subtract from it in the simple
                        // limit check in other functions, and we don't want the value to roll.
                        // Only normal segments (not expand down) are handled this way.
                        Segment.Cache_Valid |= (uint)(SegmentRegister.Enum_AccessStatus.SegAccessROK | SegmentRegister.Enum_AccessStatus.SegAccessWOK);
                    }
                    break;

                case 6:
                case 7: /* read/write, expand down */
                    if (Segment.Cache_u_Segment_D_B != 0)
                        UpperLimit = 0xffffffff;
                    else
                        UpperLimit = 0x0000ffff;
                    if ((Offset <= Segment.Cache_u_Segment_LimitScaled) ||
                         (Offset > UpperLimit) || ((UpperLimit - Offset) < (Length - 1)))
                    {
                        // BX_ERROR(("write_virtual_checks(): write beyond limit, r/w ED"));
                        return false;
                    }
                    break;

                default:
                    throw new Exception("write_virtual_checks(): unknown descriptor type=%d");
                //BX_PANIC(("write_virtual_checks(): unknown descriptor type=%d", seg->cache.type));
            }

            return true;
        }

        #region "ReadVirtual32"

        public byte ReadVirtualByte32(byte SegmentRegIndex, UInt32 Offset)
        {
            UInt32 lAddr;
            SegmentRegister Seg = (SegmentRegister)this.GetCPUSpeciallRegisters(SegmentRegIndex);

            if ((Seg.Cache_Valid & (uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK) != 0)
            {
                if (!(Offset <= Seg.Cache_u_Segment_LimitScaled))
                {
                    /*
                      BX_ERROR(("write_virtual_byte_32(): segment limit violation"));
                      exception(int_number(s), 0, 0);
                     */
                    throw new InvalidOperationException("write_virtual_byte_32(): segment limit violation");
                }
            }
            else
            {
                if (ReadVirtualChecks(Seg, Offset, 1) == false)
                    throw new Exception("WriteVirtualChecks (Seg,Offset,1)==false)");
            }


            lAddr = this.GetLAddress32(Seg, Offset);
            UInt16 tlbIndex = this.mTLB.IndexOf(lAddr, 0);
            UInt64 LPF = this.LPFOf(lAddr);
            TLBEntry oTLBEntry = this.mTLB[tlbIndex];
            if (oTLBEntry.LPF == LPF)
            {
                // See if the TLB entry privilege level allows us write access
                // from this CPL.
                if ((oTLBEntry.AccessBits &  User_PL()) == 0)
                {
                    uint HostPageAddr = oTLBEntry.HostPageAddress;
                    UInt64 oPageOffset = this.PageOffset(lAddr);
                    /*
                     *  BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr, tlbEntry->ppf | pageOffset, 1, BX_WRITE);
                          BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
                              tlbEntry->ppf | pageOffset, 1, CPL, BX_WRITE, (Bit8u*) &data);
                     * 
                     * Bit8u *hostAddr = (Bit8u*) (hostPageAddr | pageOffset);
                       pageWriteStampTable.decWriteStamp(pAddr, 1);
                       *hostAddr = data;
                     */
                    uint hostAddr = (uint)(HostPageAddr | oPageOffset);
                    this.PageWriteStampTable.decWriteStamp(oTLBEntry.PPF, 1);
                    return this.MainMemory.ReadByte(hostAddr);


                }
            }
            byte[] Data = mPagging.AccessReadLinear(lAddr, 1, CPL);

            return Data[0];
        }

        public UInt16 ReadVirtualWord32(byte SegmentRegIndex, UInt32 Offset)
        {
            /*
         *  The code has been refactored to remove the goto statement from bochs.
         * */
            UInt32 lAddr;
            SegmentRegister SegReg = GetCPUSpeciallRegisters(SegmentRegIndex);

            if (this.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new Exception("Invalid operation");
            }

            if ((SegReg.Cache_Valid & ((uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK)) == 0)
            {
                if (!(Offset < SegReg.Cache_u_Segment_LimitScaled))
                {
                    throw new Exception("write_virtual_word_32(): segment limit violation");
                }
            }
            else
            {
                if (!this.ReadVirtualChecks(SegReg, Offset, 2))
                    throw new Exception("WriteVirtualChecks failed");
            }



            // accessOK: label in originl Bochs [code mark]
            lAddr = GetLAddress32(SegReg, Offset);
            ushort tlbIndex = this.mTLB.IndexOf(lAddr, 1);

            UInt64 lpf;
            if ((mSUPPORT_ALIGNMENT_CHECK == true) && (this.CPULevel >= 4))
            {
                lpf = AlignedAccessLPFOf(lAddr, (1 & const_Alignment_Check_Mask));
            }
            else
            {

                lpf = LPFOf(lAddr);
            }
            TLBEntry tlbEntry = this.mTLB[tlbIndex];
            if (tlbEntry.LPF == lpf)
            {
                // See if the TLB entry privilege level allows us write access from this CPL.
                if ((tlbEntry.AccessBits & this.User_PL()) == 0)
                {
                    uint hostPageAddr = tlbEntry.HostPageAddress;
                    uint pageOffset = this.PageOffset(lAddr);
                    UInt64 pAddr = tlbEntry.PPF | pageOffset;
                    //BX_INSTR_LIN_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, Enum_MemoryAccessType.Write);
                    //BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, CPL, Enum_MemoryAccessType.Write, (Bit8u*) &Data);
                    UInt16 hostAddr = (UInt16)(hostPageAddr | pageOffset);
                    this.PageWriteStampTable.decWriteStamp(pAddr, 2);
                    UInt16 Data = this.MainMemory.ReadWord(hostAddr);
                    /////// WriteHostWordToLittleEndian(hostAddr, Data);
                    return Data;
                }
            }

            /***                    
         
                #if BX_CPU_LEVEL >= 4 && BX_SUPPORT_ALIGNMENT_CHECK
                      if (BX_CPU_THIS_PTR alignment_check()) {
                        if (laddr & 1) {
                          BX_ERROR(("read_virtual_word_32(): #AC misaligned access"));
                          exception(BX_AC_EXCEPTION, 0);
                        }
                      }
                #endif
            ****/
            byte[] DataBytes = mPagging.AccessReadLinear(lAddr, 2, CPL);
            return (UInt16)BitConverter.ToInt16(DataBytes, 0);
        }

        public UInt32 ReadVirtualDWord32(byte SegmentRegIndex, UInt32 Offset)
        {
            /*
       *  The code has been refactored to remove the goto statement from bochs.
       * */
            UInt32 lAddr;
            SegmentRegister SegReg = GetCPUSpeciallRegisters(SegmentRegIndex);

            if (this.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new Exception("Invalid operation");
            }

            if ((SegReg.Cache_Valid & ((uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK)) == 0)
            {
                if (!(Offset < SegReg.Cache_u_Segment_LimitScaled - 2))
                {
                    throw new Exception("write_virtual_word_32(): segment limit violation");
                }
            }
            else
            {
                if (!this.ReadVirtualChecks(SegReg, Offset, 4))
                    throw new Exception("WriteVirtualChecks failed");
            }



            // accessOK: label in originl Bochs [code mark]
            lAddr = GetLAddress32(SegReg, Offset);
            ushort tlbIndex = this.mTLB.IndexOf(lAddr, 3);

            UInt64 lpf;
            if ((mSUPPORT_ALIGNMENT_CHECK == true) && (this.CPULevel >= 4))
            {
                lpf = AlignedAccessLPFOf(lAddr, (3 & const_Alignment_Check_Mask));
            }
            else
            {

                lpf = LPFOf(lAddr);
            }
            TLBEntry tlbEntry = this.mTLB[tlbIndex];
            if (tlbEntry.LPF == lpf)
            {
                // See if the TLB entry privilege level allows us write access from this CPL.
                if ((tlbEntry.AccessBits & this.User_PL()) == 0)
                {
                    uint hostPageAddr = tlbEntry.HostPageAddress;
                    uint pageOffset = this.PageOffset(lAddr);
                    UInt64 pAddr = tlbEntry.PPF | pageOffset;
                    //BX_INSTR_LIN_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, Enum_MemoryAccessType.Write);
                    //BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, CPL, Enum_MemoryAccessType.Write, (Bit8u*) &Data);
                    UInt16 hostAddr = (UInt16)(hostPageAddr | pageOffset);
                    this.PageWriteStampTable.decWriteStamp(pAddr, 4);
                    UInt32 Data = this.MainMemory.ReadDWord(hostAddr);
                    /////// WriteHostWordToLittleEndian(hostAddr, Data);
                    return Data;
                }
            }

            /***                    
                 
               #if BX_CPU_LEVEL >= 4 && BX_SUPPORT_ALIGNMENT_CHECK
                      if (BX_CPU_THIS_PTR alignment_check()) {
                        if (laddr & 3) {
                          BX_ERROR(("read_virtual_dword_32(): #AC misaligned access"));
                          exception(BX_AC_EXCEPTION, 0);
                        }
                      }
               #endif
            ****/
            ///// access_read_linear(laddr, 4, CPL, BX_READ, (void *) &data);

            byte[] DataBytes = mPagging.AccessReadLinear(lAddr, 4, CPL);
            return (UInt32)BitConverter.ToInt32(DataBytes, 0);

        }


        public UInt64 ReadVirtualQWord32(byte SegmentRegIndex, UInt32 Offset)
        {
            /*
       *  The code has been refactored to remove the goto statement from bochs.
       * */
            UInt32 lAddr;
            SegmentRegister SegReg = GetCPUSpeciallRegisters(SegmentRegIndex);

            if (this.CPUMode == Enum_CPUModes.LongMode)
            {
                throw new Exception("Invalid operation");
            }

            if ((SegReg.Cache_Valid & ((uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK)) == 0)
            {
                if (!(Offset < SegReg.Cache_u_Segment_LimitScaled - 7))
                {
                    throw new Exception("write_virtual_word_32(): segment limit violation");
                }
            }
            else
            {
                if (!this.WriteVirtualChecks(SegReg, Offset, 8))
                    throw new Exception("WriteVirtualChecks failed");
            }



            // accessOK: label in originl Bochs [code mark]
            lAddr = GetLAddress32(SegReg, Offset);
            ushort tlbIndex = this.mTLB.IndexOf(lAddr, 7);

            UInt64 lpf;
            if ((mSUPPORT_ALIGNMENT_CHECK == true) && (this.CPULevel >= 4))
            {
                lpf = AlignedAccessLPFOf(lAddr, (7 & const_Alignment_Check_Mask));
            }
            else
            {

                lpf = LPFOf(lAddr);
            }
            TLBEntry tlbEntry = this.mTLB[tlbIndex];
            if (tlbEntry.LPF == lpf)
            {
                // See if the TLB entry privilege level allows us write access from this CPL.
                if ((tlbEntry.AccessBits & this.User_PL()) == 0)
                {
                    uint hostPageAddr = tlbEntry.HostPageAddress;
                    uint pageOffset = this.PageOffset(lAddr);
                    UInt64 pAddr = tlbEntry.PPF | pageOffset;
                    //BX_INSTR_LIN_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, Enum_MemoryAccessType.Write);
                    //BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, lAddr, pAddr, 2, CPL, Enum_MemoryAccessType.Write, (Bit8u*) &Data);
                    UInt16 hostAddr = (UInt16)(hostPageAddr | pageOffset);
                    this.PageWriteStampTable.decWriteStamp(pAddr, 8);
                    UInt64 Data = this.MainMemory.ReadLong(hostAddr);
                    /////// WriteHostWordToLittleEndian(hostAddr, Data);
                    return Data;
                }
            }

            /***                    
               #if BX_CPU_LEVEL >= 4 && BX_SUPPORT_ALIGNMENT_CHECK
                    if (BX_CPU_THIS_PTR alignment_check()) {
                    if (laddr & 7) {
                      BX_ERROR(("read_virtual_qword_32(): #AC misaligned access"));
                      exception(BX_AC_EXCEPTION, 0);
                    }
                  }
            #endif
            ****/
            ///// access_write_linear(laddr, 8, CPL, (void *) &data);
            byte[] DataBytes = mPagging.AccessReadLinear(lAddr, 8, CPL);

            return (UInt64) BitConverter.ToInt64(DataBytes,0);
        }


        protected bool ReadVirtualChecks(SegmentRegister Segment, uint Offset, uint Length)
        {
            UInt32 UpperLimit = 0;
            if (Segment.Cache_Valid == 0)
            {
                //BX_DEBUG(("write_virtual_checks(): segment descriptor not valid"));
                return false;
            }


            if (Segment.Cache_Present == false)  /* not present */
            {
                //BX_ERROR(("write_virtual_checks(): segment not present"));
                return false;
            }

            switch ((int)Segment.Cache_Type)
            {
                case 0:
                case 1:  /* read only */
                case 2:
                case 3:  /* read/write */
                case 10:
                case 11: /* execute/read */
                case 14:
                case 15: /* execute/read-only, conforming */
                    if (Offset > (Segment.Cache_u_Segment_LimitScaled - Length + 1)
                       || (Length - 1 > Segment.Cache_u_Segment_LimitScaled))
                    {
                        //BX_ERROR(("write_virtual_checks(): write beyond limit, r/w"));
                        return false;
                    }
                    if (Segment.Cache_u_Segment_LimitScaled >= 15)
                    {
                        // Mark cache as being OK type for succeeding reads. See notes for
                        // write checks; similar code.
                        Segment.Cache_Valid |= (uint)(SegmentRegister.Enum_AccessStatus.SegAccessROK);
                    }
                    break;

                case 4:
                case 5: /* read only, expand down */
                case 6:
                case 7: /* read/write, expand down */
                    if (Segment.Cache_u_Segment_D_B != 0)
                        UpperLimit = 0xffffffff;
                    else
                        UpperLimit = 0x0000ffff;
                    if ((Offset <= Segment.Cache_u_Segment_LimitScaled) ||
                         (Offset > UpperLimit) || ((UpperLimit - Offset) < (Length - 1)))
                    {
                        // BX_ERROR(("write_virtual_checks(): write beyond limit, r/w ED"));
                        return false;
                    }
                    break;

                case 8:
                case 9: /* execute only */
                case 12:
                case 13: /* execute only, conforming */
                    {
                        /* can't read or write an execute-only segment */
                        //BX_ERROR(("read_virtual_checks(): execute only"));
                        return false;
                    }

                default:
                    throw new Exception("write_virtual_checks(): unknown descriptor type=%d");
                //BX_PANIC(("write_virtual_checks(): unknown descriptor type=%d", seg->cache.type));
            }

            return true;
        }


        #endregion 


        #region "Write Special"
       
        public void Write_RMW_VirtualByte(byte Data)
        {

            // ToDo: need Test
            if (address_xlation_pages > 2)
            {
                UInt64 Address = this.address_xlation_paddress1;
                this.MainMemory.WriteByte(Address,Data);
            }
            else
            {
                byte[] Val = new byte[1];
                Val[0] = Data;
                mPagging.AccessWritePhysical(address_xlation_paddress1,1,Val);
            }

            return;
        }
          
        #endregion

        #endregion 

        #region "Read Special Read-Modify-Write operations"

        //////////////////////////////////////////////////////////////
        // special Read-Modify-Write operations                     //
        // address translation info is kept across read/write calls //
        //////////////////////////////////////////////////////////////


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SegmentRegIndex"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public byte Read_RMW_VirtualByte32(byte SegmentRegIndex, UInt32 Offset)
        {

            // ToDo: Need Test.
            UInt32 lAddr;
            SegmentRegister Seg = (SegmentRegister)this.GetCPUSpeciallRegisters(SegmentRegIndex);

            if ((Seg.Cache_Valid & (uint)SegmentRegister.Enum_AccessStatus.SegAccessWOK) != 0)
            {
                if (!(Offset <= Seg.Cache_u_Segment_LimitScaled))
                {
                    /*
                      BX_ERROR(("write_virtual_byte_32(): segment limit violation"));
                      exception(int_number(s), 0, 0);
                     */
                    throw new InvalidOperationException("write_virtual_byte_32(): segment limit violation");
                }
            }
            else
            {
                if (ReadVirtualChecks(Seg, Offset, 1) == false)
                    throw new Exception("WriteVirtualChecks (Seg,Offset,1)==false)");
            }


            lAddr = this.GetLAddress32(Seg, Offset);
            UInt16 tlbIndex = this.mTLB.IndexOf(lAddr, 0);
            UInt64 LPF = this.LPFOf(lAddr);
            TLBEntry oTLBEntry = this.mTLB[tlbIndex];
            if (oTLBEntry.LPF == LPF)
            {
                // See if the TLB entry privilege level allows us write access
                // from this CPL.
                if ((oTLBEntry.AccessBits & (0x02 | User_PL())) == 0)
                {
                    uint HostPageAddr = oTLBEntry.HostPageAddress;
                    UInt64 oPageOffset = this.PageOffset(lAddr);
                    /*
                     *  BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr, tlbEntry->ppf | pageOffset, 1, BX_WRITE);
                          BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
                              tlbEntry->ppf | pageOffset, 1, CPL, BX_WRITE, (Bit8u*) &data);
                     * 
                     * Bit8u *hostAddr = (Bit8u*) (hostPageAddr | pageOffset);
                       pageWriteStampTable.decWriteStamp(pAddr, 1);
                       *hostAddr = data;
                     */
                    uint hostAddr = (uint)(HostPageAddr | oPageOffset);
                    this.PageWriteStampTable.decWriteStamp(oTLBEntry.PPF, 1);
                    return this.MainMemory.ReadByte(hostAddr);


                }
            }
            byte[] Data = mPagging.AccessReadLinear(lAddr, 1, CPL);

            return Data[0];
        }


        public byte Read_RMW_VirtualByte64(byte SegmentRegIndex, UInt64 Offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// BOCHS inplementation
        ///     #define read_RMW_virtual_byte(seg, offset)        \
        ///     (BX_CPU_THIS_PTR cpu_mode == BX_MODE_LONG_64) ? \
        ///     read_RMW_virtual_byte_64(seg, (Bit64u) offset) : \
        ///     read_RMW_virtual_byte_32(seg, (Bit32u) offset)
        /// </summary>
        /// <param name="SegmentRegIndex"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public byte Read_RMW_VirtualByte(byte SegmentRegIndex, UInt64 Offset)
        {
            if (this.CPUMode== Enum_CPUModes.LongMode)
            {
                return Read_RMW_VirtualByte64(SegmentRegIndex, Offset);
            }
            else
            {
                return Read_RMW_VirtualByte32(SegmentRegIndex, (UInt32) Offset);
            }
        }
          
        #endregion


        /// <summary>
        /// LPF:Linear Page Frame 
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public UInt64 LPFOf(UInt64 Address)
        {
            return (Address & const_LPF_MASK);
        }


        /// <summary>
        /// PPF: Physical Page Frame
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public UInt64 PPFOf(UInt64 Address)
        {
            return (Address & const_PPF_MASK);
        }


        /// <summary>
        /// Does what #define AlignedAccessLPFOf(laddr, alignment_mask) \ ((laddr) & (LPF_MASK | (alignment_mask))) does in Bochs
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="AlignmentMask"></param>
        /// <returns></returns>
        public UInt64 AlignedAccessLPFOf(UInt64 Address, UInt64 AlignmentMask)
        {
            return ((Address) & (const_LPF_MASK | (AlignmentMask)));
        }

        /// <summary>
        /// Does what #define PAGE_OFFSET(laddr) ((Bit32u)(laddr) & 0xfff) does in Bochs
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public UInt32 PageOffset(UInt64 Address)
        {
            return ((UInt32)(Address & 0xfff));
        }




        #endregion

        #region "IO Logic"

        /// <summary>
        /// <para>If CPL (less than or equal) IOPL, then all IO portesses are accessible.</para>
        /// <para>Otherwise, must check the IO permission map on >286.</para>
        /// <para>On the 286, there is no IO permissions map </para>
        /// <para>BOCHS function:</para>
        /// bx_bool BX_CPP_AttrRegparmN(3) BX_CPU_C::allow_io(bxInstruction_c *i, Bit16u port, unsigned len)
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public bool AllowIO(UInt64 Port, UInt16 Length)
        {

            //if (BX_CPU_THIS_PTR cr0.get_PE() && (BX_CPU_THIS_PTR get_VM() || (CPL>BX_CPU_THIS_PTR get_IOPL())))
            if (mCR0.PE == Enum_Signal.High
                && (this.mRFlags.VM == Enum_Signal.High || ((CPL == 0) && (this.mRFlags.IOPL0 == Enum_Signal.Low))))
            {
                if (mTR.Cache_Valid == 0 ||
                    (mTR.Cache_Type != SegmentRegister.ENUM_SystemGateDescription.SysSegmentAvail_386_TSS &&
                     mTR.Cache_Type != SegmentRegister.ENUM_SystemGateDescription.SysSegmentBusy_386_TSS))
                {
                    //  BX_ERROR(("allow_io(): TR doesn't point to a valid 32bit TSS, TR.TYPE=%u", BX_CPU_THIS_PTR tr.cache.type));
                    return false;
                }

                if (mTR.Cache_u_Segment_LimitScaled < 103)
                {
                    // BX_ERROR(("allow_io(): TR.limit < 103"));
                    return false;
                }

                /* not translated YET
                   Bit32u io_base = system_read_word(BX_CPU_THIS_PTR tr.cache.u.segment.base + 102);

                    if ((io_base + port/8) >= BX_CPU_THIS_PTR tr.cache.u.segment.limit_scaled) {
                    BX_DEBUG(("allow_io(): IO port %x (len %d) outside TSS IO permission map (base=%x, limit=%x) #GP(0)",
                    port, len, io_base, BX_CPU_THIS_PTR tr.cache.u.segment.limit_scaled));
                    return(0);
                    }

                    Bit16u permission16 = system_read_word(BX_CPU_THIS_PTR tr.cache.u.segment.base + io_base + port/8);

                    unsigned bit_index = port & 0x7;
                    unsigned mask = (1 << len) - 1;
                    if ((permission16 >> bit_index) & mask)
                    return(0);
                */
            }
            return true;

        }

        #endregion


        #region "Instruction Execution"

        /// <summary>
        /// High-Level function that fetch the next instruction to be executed.
        /// <para>In real CPU there is parallel execution in side the CPU. EVen out-of-order execution
        /// and instruction defussion.</para>
        /// <para> We will not simulate these details as from the PC viewpoint the execution is sequential 
        /// and in order.</para>
        /// </summary>
        protected void FetchNext()
        {
            UInt64 uEIPBiased = this.RIP.Value64 + EIPPageBias;

            if (uEIPBiased >= mEIPPageWindowSize)
            {
                PreFetch();
                uEIPBiased = this.RIP.Value64 + EIPPageBias;
            }

            UInt64 pAddr = this.mAddrPage + uEIPBiased;

            //bochs: bxICacheEntry_c *entry = BX_CPU_THIS_PTR iCache.get_entry(pAddr, BX_CPU_THIS_PTR fetchModeMask);
            ICacheEntry oEntry = mICache.GetEntry(pAddr, FetchModeMask());
            Instruction oInstruction = null; //= oEntry.Instruction;


            if ((oEntry.PhysicalAddress != pAddr)
            || (oEntry.WriteStamp != CurrPageWriteStampPtr))
            {
                // iCache miss. No validated instruction with matching fetch parameters
                // is in the iCache.

                this.mICache.ServeICacheMiss(oEntry, (UInt32)uEIPBiased, pAddr);
                oInstruction = oEntry.Instruction;
            }



        }

        /// <summary>
        /// The so-called instruction prefetch, instruction is the calculation of the physical address and other relevant information, 
        /// to prepare for subsequent instruction decoding.
        ///  <para>boundaries of consideration:</para>
        ///  <para></para>
        ///  <para>* physical memory boundary: 1024k (1Megabyte) (increments of...)</para>
        ///  <para>* A20 boundary:             1024k (1Megabyte)</para>
        ///  <para>* page boundary:            4k</para>
        ///  <para>* ROM boundary:             2k (dont care since we are only reading)</para>
        ///  <para>* segment boundary:         any</para>
        ///
        /// </summary>
        protected void PreFetch()
        {
            UInt64 laddr = 0x0;
            UInt32 nPageOffset;

            // if 64BitMode
            if (mCPUMode == Enum_CPUModes.LongMode)
            {
                throw new NotImplementedException("Long Mode PreFetch");
            }
            else
            { // Address Translation and boundry checks
                laddr = GetLAddress32(this.CS, this.RIP.Value32);
                nPageOffset = Pagging.MemoryPagging.PageOffset((UInt32)laddr);

                // Calculate RIP at the beginning of the page.
                mEIPPageBias = (UInt64)((UInt64)nPageOffset - RIP.Value32);

                UInt32 nLimit = this.CS.Cache_u_Segment_LimitScaled;
                if (this.RIP.Value32 > nLimit)
                {
                    // Error
                    throw new InvalidOperationException("prefetch: EIP [%08x] > CS.limit [%08x]");
                    // BX_ERROR(("prefetch: EIP [%08x] > CS.limit [%08x]", EIP, limit));
                    // exception(BX_GP_EXCEPTION, 0, 0);
                }

                this.mEIPPageWindowSize = 4096;
                if (nLimit + this.mEIPPageBias < 4096)
                {
                    this.mEIPPageWindowSize = (UInt32)(nLimit + this.mEIPPageBias + 1);
                }
            }
            UInt64 lpf = LPFOf(laddr);
            UInt32 FetchPtr = 0x00;

            // get stored entry in TLB based on the required address  [Hash Functions]
            TLBEntry oTLBEntry = mTLB[(ushort)mTLB.IndexOf(lpf, 0)];
            if (
                (oTLBEntry.LPF == lpf)  // Is the stored entry is the one we are looking for?
                &&
                (
                    (((uint)oTLBEntry.AccessBits & (uint)(0x4 | User_PL())) == 0)
                )
                )
            {
                this.mAddrPage = oTLBEntry.PPF;
                FetchPtr = (byte)oTLBEntry.HostPageAddress;
                EIPFetchPtr = FetchPtr;
            }
            else
            {
                UInt32 PhysicalAddr;
                PhysicalAddr = (UInt32)this.mPagging.TranslateLinear(laddr, CS.Selector.Selector_RPL, Enum_MemoryAccessType.Execute);
                mAddrPage = (uint)LPFOf(PhysicalAddr);
                EIPFetchPtr = (UInt64)this.MainMemory.GetHostMemoryAddress(mAddrPage, Enum_MemoryAccessType.Execute);
            }

            /* Code restructureed in the previous iff statement
            if (FetchPtr != 0)  //i.e. Data is not found in Cache
            {
                  EIPFetchPtr = FetchPtr;
            }
            else
            {
                EIPFetchPtr = GetHostMemoryAddress(mAddrPage, Enum_MemoryAccessType.Execute);
            }
            */
            CurrPageWriteStampPtr = mPageWriteStampTable.GetPageWriteStamp(mAddrPage);
        }

        /// <summary>
        /// Used to execute Repeat Instuctions.
        /// Executes instructions in the this.mICache.Pool.
        /// Each Instruction is executed by calling instruction.Execute()
        /// </summary>
        protected void Execute()
        {
            for (uint i = this.mICache.PIndex_Start; i < this.mICache.PIndex; ++i)
            {
                // decoding instruction compeleted -> continue with execution
                RIP.Value64 += this.mICache.Pool[i].iLen();
                if (mSingleDebugStep == CPUDebugMode.SingleStep)
                {
                    int Index = System.Threading.WaitHandle.WaitAny(mManualResetEvent);
                    if (Index == const_SHUTDOWN_RESET_EVENT) return; //Shutdown Mode
                    mSingleDebugManualResetEvent.Reset(); // wait again next time.
                }

                // Execute Instruction

                try
                {
                    this.mICache.Pool[i].Execute1(this.mICache.Pool[i]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[!] " + ex.Message);
                }

                mPrev_RIP = RIP.Value64;

                if (mAsyncEvent != 0)
                {
                    // clear stop trace magic indication that probably was set by repeat or branch32/64
                    mAsyncEvent &= ~const_BX_ASYNC_EVENT_STOP_TRACE;
                    break; // we need to fetch again
                }
            }
        }

        public void Repeat(Instruction i, InstructionExecution.dlgt_OPCodeInstruction OpCodeInstruction)
        {
            // non repeated instruction
            if (i.RepUsedL() == 0)
            {
                OpCodeInstruction(i);
            }

            if (i.AS64L() != 0)
            {
                while (RCX.Value64 != 0)
                {
                    OpCodeInstruction(i);
                    RCX.Value64--;

                    if (this.mAsyncEvent != 0)
                        break;
                }
            }
            else if (i.AS32L() != 0)
            {
                while (RCX.Value32 != 0)
                {
                    OpCodeInstruction(i);
                    RCX.Value64--;

                    if (this.mAsyncEvent != 0)
                        break;
                }
            }
            else
            {
                while (RCX.Value16 != 0)
                {
                    OpCodeInstruction(i);
                    RCX.Value64--;

                    if (this.mAsyncEvent != 0)
                        break;
                }
            }
            if (this.RCX.Value64 == 0) return;
            RIP.Value64 = mPrev_RIP;
            return;
        }
        #endregion

        /// <summary>
        /// #define RSP_SPECULATIVE {              \
        /// BX_CPU_THIS_PTR speculative_rsp = 1; \
        /// BX_CPU_THIS_PTR prev_rsp = RSP;      \
        /// </summary>
        public void RSP_Speculative()
        {
            this.mSpeculativeRSP = true;
            this.mPrev_RSP.Value64 = this.RSP.Value64;
        }


        /// <summary>
        ///  BX_CPU_THIS_PTR speculative_rsp = 0;
        /// </summary>
        public void RSP_Commit()
        {
            this.mSpeculativeRSP = false;
        }

        public void HandleAlignmentCheck()
        {


        }

        public void BoundryFetch(UInt64 FetchPtr, UInt64 uRemainingInPage, Instruction oInstruction)
        {
            byte[] FetchBuffer = new byte[32];
            UInt64 j;
            if (uRemainingInPage >= 15)
            {
                throw new Exception("boundaryFetch #GP(0): too many instruction prefixes");
            }

            // Read all leftover bytes in current page up to boundary.
            for (j = 0; j < uRemainingInPage; j++)
            {
                FetchBuffer[j] = MainMemory.ReadByte(FetchPtr);
                FetchPtr++;
            }

            // The 2nd chunk of the instruction is on the next page.
            // Set RIP to the 0th byte of the 2nd page, and force a
            // prefetch so direct access of that physical page is possible, and
            // all the associated info is updated.
            RIP.Value64 += (UInt64)uRemainingInPage;
            PreFetch();

            uint fetchBufferLimit = 15;
            if (this.EIPPageWindowSize < 15)
            {
                //BX_DEBUG(("boundaryFetch: small window size after prefetch=%d bytes, remainingInPage=%d bytes", BX_CPU_THIS_PTR eipPageWindowSize, remainingInPage));
                fetchBufferLimit = this.EIPPageWindowSize;
            }

            // We can fetch straight from the 0th byte, which is eipFetchPtr;
            FetchPtr = this.EIPFetchPtr;

            // read leftover bytes in next page
            for (int k = 0; k < fetchBufferLimit; k++, j++)
            {
                FetchBuffer[j] = MainMemory.ReadByte(FetchPtr);
                FetchPtr++;
            }

            oInstruction = InstructionExecution.FetchDecode32(FetchPtr, uRemainingInPage);
            if (oInstruction == null)
            {
                // BX_INFO(("boundaryFetch #GP(0): failed to complete instruction decoding"));
                throw new Exception("boundaryFetch #GP(0): failed to complete instruction decoding");
            }
            // Restore EIP since we fudged it to start at the 2nd page boundary.
            RIP.Value64 = mPrev_RIP;

        }

        //BX_CPU_C::load_seg_reg(bx_segment_reg_t *seg, Bit16u new_value)
        public void LoadSegReg(SegmentRegister SegReg, UInt16 NewValue)
        {
            if (CPUMode == Enum_CPUModes.ProtectedMode)
            {
                Selector SS_Selector = new Selector();
                SegmentRegister Descriptor = new SegmentRegister();
                UInt32 dword1, dword2;
                SS_Selector.ParseSelector(NewValue);
                if ((NewValue & 0xfffc) == 0)
                {
                    // Null Selector
                    if (SS_Selector.Selector_RPL == CS.Selector.Selector_RPL)
                    {
                        LoadNullSelector(SegReg, NewValue);
                        return;
                    }
                    throw new Exception("load_seg_reg(SS): loading null selector");
                }


            }
            /* real or v8086 mode */

            /* www.x86.org:
              According  to  Intel, each time any segment register is loaded in real
              mode,  the  base  address is calculated as 16 times the segment value,
              while  the  access  rights  and size limit attributes are given fixed,
              "real-mode  compatible" values. This is not true. In fact, only the CS
              descriptor  caches  for  the  286,  386, and 486 get loaded with fixed
              values  each  time  the segment register is loaded. Loading CS, or any
              other segment register in real mode, on later Intel processors doesn't
              change  the  access rights or the segment size limit attributes stored
              in  the  descriptor  cache  registers.  For these segments, the access
              rights and segment size limit attributes from any previous setting are
              honored. */
            SegReg.Selector.Selector_Value = NewValue;
            SegReg.Selector.Selector_RPL = (byte)((CPUMode == Enum_CPUModes.RealMode) ? 0 : 3);
            SegReg.Cache_Valid = 1;
            SegReg.Cache_u_Segment_Base = (ulong)(NewValue << 4);
            SegReg.Cache_Segment = SegmentRegister.Enum_SegmentType.DataOrCode; /* regular segment */
            SegReg.Cache_Present = true; /* present */

            /* Do not modify segment limit and AR bytes when in real mode */
            /* Support for big real mode */

            if (CPUMode != Enum_CPUModes.RealMode)
            {
                SegReg.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.DataReadWriteAccessed;
                SegReg.Cache_DPL = 3; /* we are in v8086 mode */
                SegReg.Cache_u_Segment_LimitScaled = 0xffff;
                SegReg.Cache_u_Segment_G = 0; /* byte granular */
                SegReg.Cache_u_Segment_D_B = SegmentRegister.Enum_SegmentDB.bit16; /* default 16bit size */
                SegReg.Cache_u_Segment_LongMode = 0; /* default 16bit size */
                SegReg.Cache_u_Segment_AVL = 0;

            }

            if (SegReg.Name == "CS")
            {
                //invalidate_prefetch_q();

                // updateFetchModeMask(); the following 2 lines are equivelent.
                FetchModeMask();
                User_PL();

                if (mCPULevel >= 4 /*&& BX_SUPPORT_ALIGNMENT_CHECK*/)
                    HandleAlignmentCheck(); // CPL was modified

            }
        }


        /// <summary>
        /// Put ZEROS in a given Segment Register.
        /// </summary>
        /// <param name="SegReg"></param>
        /// <param name="NewValue"></param>
        public void LoadNullSelector(SegmentRegister SegReg, UInt16 NewValue)
        {
            SegReg.Selector.Selector_Index = 0;
            SegReg.Selector.Selector_TI = 0;
            SegReg.Selector.Selector_RPL = (ushort)(NewValue & 0x03);
            SegReg.Selector.Selector_Value = NewValue;

            SegReg.Cache_Valid = 0; /* invalidate null selector */
            SegReg.Cache_Present = false;
            SegReg.Cache_DPL = 0;
            SegReg.Cache_Segment = SegmentRegister.Enum_SegmentType.DataOrCode;
            SegReg.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.InavlidDescriptor;

            SegReg.Cache_u_Segment_Base = 0;
            SegReg.Cache_u_Segment_LimitScaled = 0;
            SegReg.Cache_u_Segment_G = 0;
            SegReg.Cache_u_Segment_D_B = 0;
            SegReg.Cache_u_Segment_AVL = 0;
            SegReg.Cache_u_Segment_LongMode = 0;

        }


        protected void HandleAsyncEvent()
        {
            //
            // This area is where we process special conditions and events.
            //
            if (CPUActivityState != Enum_CPUActivityState.BX_Activity_State_ACTIVE)
            {
                throw new NotImplementedException();
                /*
                 // For one processor, pass the time as quickly as possible until
    // an interrupt wakes up the CPU.
    while (1)
    {
      if ((BX_CPU_INTR && (BX_CPU_THIS_PTR get_IF() || 
          (BX_CPU_THIS_PTR activity_state == BX_ACTIVITY_STATE_MWAIT_IF))) ||
           BX_CPU_THIS_PTR pending_NMI || BX_CPU_THIS_PTR pending_SMI || BX_CPU_THIS_PTR pending_INIT)
      {
        // interrupt ends the HALT condition
#if BX_SUPPORT_MONITOR_MWAIT
        if (BX_CPU_THIS_PTR activity_state >= BX_ACTIVITY_STATE_MWAIT)
          BX_MEM(0)->clear_monitor(BX_CPU_THIS_PTR bx_cpuid);
#endif
        BX_CPU_THIS_PTR activity_state = 0;
        BX_CPU_THIS_PTR inhibit_mask = 0; // clear inhibits for after resume
        break;
      }
      if (BX_CPU_THIS_PTR activity_state == BX_ACTIVITY_STATE_ACTIVE) {
        BX_INFO(("handleAsyncEvent: reset detected in HLT state"));
        break;
      }
                 */
            }
            else
            {

                /*
                 * if (bx_pc_system.kill_bochs_request) {
    // setting kill_bochs_request causes the cpu loop to return ASAP.
    return 1; // Return to caller of cpu_loop.
                 * */
            }

            // VMLAUNCH/VMRESUME cannot be executed with interrupts inhibited.
            // Save inhibit interrupts state into shadow bits after clearing
            this.InhibitMask = (byte)((InhibitMask << 2) & 0xf);


            // Priority 1: Hardware Reset and Machine Checks
            //   RESET
            //   Machine Check
            // (bochs doesn't support these)

            // Priority 2: Trap on Task Switch
            //   T flag in TSS is set
            if ((this.DebugTrap & const_BX_DEBUG_TRAP_TASK_SWITCH_BIT) != 0)
            {
                this.Exception(0, 0);   // no error, not interrupt
            }

            // Priority 3: External Hardware Interventions
            //   FLUSH
            //   STOPCLK
            //   SMI
            //   INIT
            /// ToDo: Implement
            /* NOT IMPLEMENTED */

            // Priority 4: Traps on Previous Instruction
            //   Breakpoints
            //   Debug Trap Exceptions (TF flag set or data/IO breakpoint)
            if ((this.DebugTrap != 0) && (this.InhibitMask & const_BX_INHIBIT_DEBUG_SHADOW) != 0)
            {
                // A trap may be inhibited on this boundary due to an instruction
                // which loaded SS.  If so we clear the inhibit_mask below
                // and don't execute this code until the next boundary.
                Exception(0, 0); // no error, not interrupt
            }

            // Priority 5: External Interrupts
            //   NMI Interrupts
            //   Maskable Hardware Interrupts
            if ((this.InhibitMask & const_BX_INHIBIT_INTERRUPTS_SHADOW) != 0)
            {
                // Processing external interrupts is inhibited on this
                // boundary because of certain instructions like STI.
                // inhibit_mask is cleared below, in which case we will have
                // an opportunity to check interrupts on the next instruction
                // boundary.
            }
            /*
   #if BX_SUPPORT_VMX
  else if (! BX_CPU_THIS_PTR disable_NMI && BX_CPU_THIS_PTR in_vmx_guest && 
       VMEXIT(VMX_VM_EXEC_CTRL2_NMI_WINDOW_VMEXIT))
  {
    // NMI-window exiting
    BX_ERROR(("VMEXIT: NMI window exiting"));
    VMexit(0, VMX_VMEXIT_NMI_WINDOW, 0);
  }
             */

            AsyncEvent = 0; // Continue executing CPU LOOP.
        }




        #region "Events"

        public override void OnA20Change()
        {
            // If there has been a transition, we need to notify the CPUs so
            // they can potentially invalidate certain cache info based on
            // A20-line-applied physical addresses.
            mTLB.InitTLB();  // Flush TLB
        }

        protected void OnCPUModeChange(Enum_CPUModes CPUMode)
        {
            switch (CPUMode)
            {
                case Enum_CPUModes.RealMode:
                    this.mCS.Cache_Present = true;
                    this.mCS.Cache_Segment = SegmentRegister.Enum_SegmentType.DataOrCode;
                    this.mCS.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.DataReadWriteAccessed;

                    break;
                case Enum_CPUModes.LongMode:
                    break;
                case Enum_CPUModes.LongCompatMode:
                    mRIP.Value32H = 0x0;
                    mRSP.Value32H = 0x0;
                    break;
                case Enum_CPUModes.ProtectedMode:
                    break;
                case Enum_CPUModes.SystemManagementMode:
                    break;
                case Enum_CPUModes.UnRealMode:
                    break;
                case Enum_CPUModes.Virtual8086Mode:
                    break;
            }

            // Update Fetch Mode Mask
            mEIPPageWindowSize = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Vector">0..255: vector in IDT</param>
        /// <param name="ErrorCode">if exception generates and error, push this error code</param>
        public void Exception(byte Vector, UInt16 ErrorCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This is updated in "updateFetchModeMask" in original Bochs
        /// </summary>
        /// <returns></returns>
        protected uint User_PL()
        {
            return (uint)((CS.Selector.Selector_RPL == 3) ? 0x01 : 0x00);
        }

        /// <summary>
        /// This is updated in "updateFetchModeMask" in original Bochs
        /// </summary>
        /// <returns></returns>
        public UInt64 FetchModeMask()
        {
            UInt64 Mask;
            if (SUPPORT_X86_64)
            {
                if (mCPUMode == Enum_CPUModes.LongMode)
                {
                    Mask = (UInt64)0x02 | (UInt64)CS.Cache_u_Segment_D_B;
                }
                else
                {
                    Mask = (UInt64)CS.Cache_u_Segment_D_B;
                }
            }
            else
            {
                Mask = (UInt64)CS.Cache_u_Segment_D_B;
            }

            return Mask;
        }
        #endregion

       

    }
}
