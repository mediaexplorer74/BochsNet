using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// The FLAGS register is the status register in Intel x86 microprocessors that contains the current state of the processor. 
    /// <para>This register is 16 bits wide. Its successors, the EFLAGS and RFLAGS registers are 32 bits and 64 bits wide, respectively. </para>
    /// <para>The wider registers retain compatibility with their smaller predecessors.</para>
    /// <see cref="http://en.wikipedia.org/wiki/FLAGS_register"/></para>
    /// <para>31|30|29|28| 27|26|25|24| 23|22|21|20| 19|18|17|16</para>
    /// <para> ==|==|=====| ==|==|==|==| ==|==|==|==| ==|==|==|==</para>
    /// <para>  0| 0| 0| 0|  0| 0| 0| 0|  0| 0|ID|VP| VF|AC|VM|RF</para>
    /// <para></para>
    /// <para> 15|14|13|12| 11|10| 9| 8|  7| 6| 5| 4|  3| 2| 1| 0</para>
    /// <para> ==|==|=====| ==|==|==|==| ==|==|==|==| ==|==|==|==</para>
    /// <para>  0|NT| IOPL| OF|DF|IF|TF| SF|ZF| 0|AF|  0|PF| 1|CF</para>   /// </summary>
    public class RFlagsRegister : Register64
    {

        #region "Enumeration"

        public enum Enum_Instruction : byte
        {

            BX_LF_INSTR_ADD8 = 1,
            BX_LF_INSTR_ADC8 = 2,

            BX_LF_INSTR_ADD16 = 3,
            BX_LF_INSTR_ADC16 = 4,

            BX_LF_INSTR_ADD32 = 5,
            BX_LF_INSTR_ADC32 = 6,

            BX_LF_INSTR_ADD64 = 7,
            BX_LF_INSTR_ADC64 = 8,

            // BX_LF_INSTR_ADD_ADC8(cf)  (1 + (cf))
            BX_LF_INSTR_ADD_ADC8 = 1,
            // BX_LF_INSTR_ADD_ADC16(cf) (3 + (cf))
            BX_LF_INSTR_ADD_ADC16 = 3,
            // BX_LF_INSTR_ADD_ADC32(cf) (5 + (cf))
            BX_LF_INSTR_ADD_ADC32 = 5,
            // BX_LF_INSTR_ADD_ADC64(cf) (7 + (cf))
            BX_LF_INSTR_ADD_ADC64 = 7,

            BX_LF_INSTR_SUB8 = 9,
            BX_LF_INSTR_SBB8 = 10,

            BX_LF_INSTR_SUB16 = 11,
            BX_LF_INSTR_SBB16 = 12,

            BX_LF_INSTR_SUB32 = 13,
            BX_LF_INSTR_SBB32 = 14,

            BX_LF_INSTR_SUB64 = 15,
            BX_LF_INSTR_SBB64 = 16,

            //BX_LF_INSTR_SUB_SBB8(cf)  (9  + (cf))
            BX_LF_INSTR_SUB_SBB8 = 9,
            //BX_LF_INSTR_SUB_SBB16(cf) (11 + (cf))
            BX_LF_INSTR_SUB_SBB16 = 11,
            //BX_LF_INSTR_SUB_SBB32(cf) (13 + (cf))
            BX_LF_INSTR_SUB_SBB32 = 13,
            //BX_LF_INSTR_SUB_SBB64(cf) (15 + (cf))
            BX_LF_INSTR_SUB_SBB64 = 15,

            BX_LF_INSTR_INC8 = 17,
            BX_LF_INSTR_INC16 = 18,
            BX_LF_INSTR_INC32 = 19,
            BX_LF_INSTR_INC64 = 20,

            BX_LF_INSTR_DEC8 = 21,
            BX_LF_INSTR_DEC16 = 22,
            BX_LF_INSTR_DEC32 = 23,
            BX_LF_INSTR_DEC64 = 24,

            BX_LF_INSTR_NEG8 = 25,
            BX_LF_INSTR_NEG16 = 26,
            BX_LF_INSTR_NEG32 = 27,
            BX_LF_INSTR_NEG64 = 28,

            BX_LF_INSTR_LOGIC8 = 29,
            BX_LF_INSTR_LOGIC16 = 30,
            BX_LF_INSTR_LOGIC32 = 31,
            BX_LF_INSTR_LOGIC64 = 32,

            BX_LF_SIGN_BIT_64 = 63,
            BX_LF_SIGN_BIT_32 = 31

        }
        #endregion

        #region "Constants"

        public const byte const_BX_LF_SIGN_BIT = 63;
        /*
         * #if BX_SUPPORT_X86_64
         * #define BX_LF_SIGN_BIT  63
         * #else
         * #define BX_LF_SIGN_BIT  31
         * #endif
         */
        #endregion

        #region "Attributes"

        /// <summary>
        /// see  Zero Flag issue [LazyCalcZF].
        /// </summary>
        protected bool mNoOperationYet=true;
        protected string mName;

        #region "Last Cached Data"
        /*
         * pls see http://bochs.sourceforge.net/VirtNoJit.pdf 
         */
        /// <summary>
        /// Last operand value
        /// </summary>
        protected UInt64 mOperand1;
        /// <summary>
        /// Last Operand value
        /// </summary>
        protected UInt64 mOperand2;
        /// <summary>
        /// operation result
        /// </summary>
        protected UInt64 mResult;

        protected Enum_Instruction mInstruction;
        protected byte mInstruction_CF;

        #endregion

       

        // This array defines a look-up table for the even parity-ness
        // of an 8bit quantity, for optimal assignment of the parity bit
        // in the EFLAGS register
        protected byte[] mBxParityLookup = new byte[]
        {
             1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0,
  1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1
        };

        #endregion



        #region "Properties"

        public Enum_Instruction Instruction
        {
            get
            {
                return mInstruction;
            }
            set
            {
                mInstruction = value;
            }
        }
        public byte Instruction_CF
        {
            get
            {
                return mInstruction_CF;
            }
            set
            {
                mInstruction_CF = value;
            }

        }

        public UInt64 Result
        {
            get
            {
                return mResult;
            }
            set
            {
                mNoOperationYet = false; // see  Zero Flag issue [LazyCalcZF].
                mResult = value;
            }
        }

        public UInt64 Operand1
        {
            get
            {
                return mOperand1;
            }
            set
            {
                mNoOperationYet = false; // see  Zero Flag issue [LazyCalcZF].
                mOperand1 = value;
            }
        }


        public UInt64 Operand2
        {
            get
            {
                return mOperand2;
            }
            set
            {
                mOperand2 = value;
            }
        }

        public override ulong Value64
        {
            get
            {
                return (UInt64)BitConverter.ToInt64(mValue, 0);
            }
            set
            {
                mValue[0] = (byte)(value & 0x00ff);
                mValue[1] = (byte)((value >> 8) & 0xff);
                mValue[2] = (byte)((value >> 16) & 0xff);
                mValue[3] = (byte)((value >> 24) & 0xff);
                mValue[4] = (byte)((value >> 32) & 0xff);
                mValue[5] = (byte)((value >> 40) & 0xff);
                mValue[6] = (byte)((value >> 48) & 0xff);
                mValue[7] = (byte)((value >> 56) & 0xff);
            }
        }

        /// <summary>
        /// FFlags
        /// Carry Flag
        /// <para> bits 0 </para>
        /// </summary>
        public Enum_Signal CF
        {
            get
            {
                LazyCalc_CF();
                return (Enum_Signal)((mValue[0]) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[0] = (byte)(mValue[0] | 0x01);
                }
                else
                {
                    mValue[0] = (byte)(mValue[0] & 0xfe);
                }

            }
        }

        /// <summary>
        /// FFlags
        /// Parity Flag
        /// <para> bits 2 </para>
        /// </summary>
        public Enum_Signal PF
        {
            get
            {
                LazyCalcPF();
                return (Enum_Signal)((mValue[0] >> 2) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[0] = (byte)(mValue[0] | 0x04);
                }
                else
                {
                    mValue[0] = (byte)(mValue[0] & 0xfb);
                }

            }
        }



        /// <summary>
        /// FFlags
        /// Adjust Flag
        /// <para> bits 4 </para>
        /// </summary>
        public Enum_Signal AF
        {
            get
            {
                LazyCalc_AF();
                return (Enum_Signal)((mValue[0] >> 4) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[0] = (byte)(mValue[0] | 0x10);
                }
                else
                {
                    mValue[0] = (byte)(mValue[0] & 0xef);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// Zero Flag
        /// <para> bits 6 </para>
        /// </summary>
        public Enum_Signal ZF
        {
            get
            {
                LazyCalcZF();
                return (Enum_Signal)((mValue[0] >> 6) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[0] = (byte)(mValue[0] | 0x40);
                }
                else
                {
                    mValue[0] = (byte)(mValue[0] & 0xbf);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// Sign Flag
        /// <para> bits 7 </para>
        /// </summary>
        public Enum_Signal SF
        {
            get
            {
                LazyCalcSF();
                return (Enum_Signal)((mValue[0] >> 7) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[0] = (byte)(mValue[0] | 0x80);
                }
                else
                {
                    mValue[0] = (byte)(mValue[0] & 0x7f);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// Trap flag (single step)
        /// <para> bits 8 </para>
        /// </summary>
        public Enum_Signal TF
        {
            get
            {
                return (Enum_Signal)((mValue[1]) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x01);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xfe);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// Interrupt enable flag
        /// <para> bits 9 </para>
        /// </summary>
        public Enum_Signal IF
        {
            get
            {
                return (Enum_Signal)((mValue[1] >> 1) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x02);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xfd);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// Direction flag
        /// <para> bits 10 </para>
        /// </summary>
        public Enum_Signal DF
        {
            get
            {
                return (Enum_Signal)((mValue[1] >> 2) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x04);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xfb);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// Overflow flag
        /// <para> bits 11 </para>
        /// </summary>
        public Enum_Signal OF
        {
            get
            {
                LazyCalc_OF();
                return (Enum_Signal)((mValue[1] >> 3) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x08);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xf7);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// I/O privilege level
        /// This is Bit 12
        /// <para> bits 12, 13 </para>
        /// </summary>
        public Enum_Signal IOPL0
        {
            get
            {
                return (Enum_Signal)((mValue[1] >> 4) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x10);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xef);
                }

            }
        }


        /// <summary>
        /// FFlags
        /// I/O privilege level
        /// This is Bit 13
        /// <para> bits 12, 13 </para>
        /// </summary>
        public Enum_Signal IOPL1
        {
            get
            {
                return (Enum_Signal)((mValue[1] >> 5) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x20);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xdf);
                }

            }
        }

        /// <summary>
        /// (mValue[1] >> 4) & 0x03
        /// </summary>
        public byte IOPL
        {
            get
            {
                //move bits & select them
                return (byte)( (mValue[1] >> 4) & 0x03);
            }
        }

        /// <summary>
        /// FFlags
        /// Nested task flag (286+)
        /// This is Bit 14
        /// </summary>
        public Enum_Signal NT
        {
            get
            {
                return (Enum_Signal)((mValue[1] >> 6) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[1] = (byte)(mValue[1] | 0x40);
                }
                else
                {
                    mValue[1] = (byte)(mValue[1] & 0xbf);
                }

            }
        }


        /// <summary>
        /// EFFlags
        /// Resume flag (386+ only)
        /// This is Bit 16
        /// </summary>
        public Enum_Signal RF
        {
            get
            {
                return (Enum_Signal)((mValue[2]) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[2] = (byte)(mValue[2] | 0x01);
                }
                else
                {
                    mValue[2] = (byte)(mValue[2] & 0xfe);
                }

            }
        }


        /// <summary>
        /// EFlags
        /// Virtual 8086 mode flag (386+ onl
        /// This is Bit 17
        /// </summary>
        public Enum_Signal VM
        {
            get
            {
                return (Enum_Signal)((mValue[2] >> 1) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[2] = (byte)(mValue[2] | 0x02);
                }
                else
                {
                    mValue[2] = (byte)(mValue[2] & 0xfd);
                }

            }
        }


        /// <summary>
        /// EFlags
        /// Alignment check (486SX+ only)
        /// This is Bit 18
        /// </summary>
        public Enum_Signal AC
        {
            get
            {
                return (Enum_Signal)((mValue[2] >> 2) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[2] = (byte)(mValue[2] | 0x04);
                }
                else
                {
                    mValue[2] = (byte)(mValue[2] & 0xfb);
                }

            }
        }

        /// <summary>
        /// EFlags
        /// Virtual interrupt flag (Pentium+)
        /// This is Bit 19
        /// </summary>
        public Enum_Signal VIF
        {
            get
            {
                return (Enum_Signal)((mValue[2] >> 3) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[2] = (byte)(mValue[2] | 0x08);
                }
                else
                {
                    mValue[2] = (byte)(mValue[2] & 0xf7);
                }

            }
        }



        /// <summary>
        /// EFlags
        /// Virtual interrupt pending (Pentium+)
        /// This is Bit 20
        /// </summary>
        public Enum_Signal VIP
        {
            get
            {
                return (Enum_Signal)((mValue[2] >> 4) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[2] = (byte)(mValue[2] | 0x10);
                }
                else
                {
                    mValue[2] = (byte)(mValue[2] & 0xef);
                }

            }
        }


        /// <summary>
        /// EFlags
        /// Identification (Pentium+)
        /// This is Bit 21
        /// </summary>
        public Enum_Signal ID
        {
            get
            {
                return (Enum_Signal)((mValue[2] >> 5) & 0x01);
            }

            set
            {
                if (value == Enum_Signal.High)
                {
                    mValue[2] = (byte)(mValue[2] | 0x20);
                }
                else
                {
                    mValue[2] = (byte)(mValue[2] & 0xdf);
                }

            }
        }


        #endregion


        #region "Constructor"

        public RFlagsRegister()
            : base(8)
        {
            mName = "RFlags";
            mName64 = "RFlags";
            mName32 = "EFlags";
            mName16 = "Flags";
            mName8 = "";
        }


        #endregion


        #region "Methods"

        public override void Reset()
        {
            // Bit1 is always set
            Value64 = 0x0000000000000002;
        }
        
        #region "SET_FLAGS_OSZAPC"

        #region "SetFlags_OSZAPC_Logic"

        /// <summary>
        /// Save result for lazy calculation of FFlag
        /// with Inst: BX_LF_INSTR_LOGIC8
        /// </summary>
        /// <param name="Result8"></param>
        public void SetFlags_OSZAPC_Logic8(byte Result8)
        {
            mResult = (UInt64)Result8;
            Instruction = Enum_Instruction.BX_LF_INSTR_LOGIC8;
        }

        /// <summary>
        /// Save result for lazy calculation of FFlag
        /// with Inst: BX_LF_INSTR_LOGIC16
        /// </summary>
        /// <param name="Result16"></param>
        public void SetFlags_OSZAPC_Logic16(UInt16 Result16)
        {
            Result = (UInt64)Result16;
            Instruction = Enum_Instruction.BX_LF_INSTR_LOGIC16;
        }

        /// <summary>
        /// Save result for lazy calculation of FFlag
        /// with Inst: BX_LF_INSTR_LOGIC32
        /// </summary>
        /// <param name="Result32"></param>
        public void SetFlags_OSZAPC_Logic32(UInt32 Result32)
        {
            Result = (UInt64)Result32;
            Instruction = Enum_Instruction.BX_LF_INSTR_LOGIC32;
        }

        /// <summary>
        /// Save result for lazy calculation of FFlag
        /// with Inst: BX_LF_INSTR_LOGIC64
        /// </summary>
        /// <param name="Result64"></param>
        public void SetFlags_OSZAPC_Logic64(UInt64 Result64)
        {
            Result = (UInt64)Result64;
            Instruction = Enum_Instruction.BX_LF_INSTR_LOGIC64;
        }

        #endregion

        #region "SET_FLAGS_OSZAPC_SUB"
        public void SetFlags_OSZAPC_Sub8(byte Operand1,byte Operand2, byte Result8)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result8;
            Instruction = Enum_Instruction.BX_LF_INSTR_SUB8;
        }

        public void SetFlags_OSZAPC_Sub16(UInt16 Operand1, UInt16 Operand2, UInt16 Result16)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result16;
            Instruction = Enum_Instruction.BX_LF_INSTR_SUB16;
        }

        public void SetFlags_OSZAPC_Sub32(UInt32 Operand1, UInt32 Operand2, UInt32 Result32)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result32;
            Instruction = Enum_Instruction.BX_LF_INSTR_SUB32;
        }

        public void SetFlags_OSZAPC_Sub64(UInt64 Operand1, UInt64 Operand2, UInt64 Result64)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result64;
            Instruction = Enum_Instruction.BX_LF_INSTR_SUB64;
        }
        #endregion

        #region "SET_FLAGS_OSZAPC_ADD"
        public void SetFlags_OSZAPC_Add8(byte Operand1, byte Operand2, byte Result8)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result8;
            Instruction = Enum_Instruction.BX_LF_INSTR_ADD8 ;
        }

        public void SetFlags_OSZAPC_Add16(UInt16 Operand1, UInt16 Operand2, UInt16 Result16)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result16;
            Instruction = Enum_Instruction.BX_LF_INSTR_ADD16;
        }

        public void SetFlags_OSZAPC_Add32(UInt32 Operand1, UInt32 Operand2, UInt32 Result32)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result32;
            Instruction = Enum_Instruction.BX_LF_INSTR_ADD32;
        }

        public void SetFlags_OSZAPC_Add64(UInt64 Operand1, UInt64 Operand2, UInt64 Result64)
        {
            mOperand1 = (UInt64)Operand1;
            mOperand2 = (UInt64)Operand2;
            mResult = (UInt64)Result64;
            Instruction = Enum_Instruction.BX_LF_INSTR_ADD64;
        }
        #endregion

        #region "SET_FLAGS_OSZAPC_INC"
        public void SetFlags_OSZAPC_Inc8(byte Result8)
        {
            mResult = (UInt64)Result8;
            Instruction = Enum_Instruction.BX_LF_INSTR_INC8 ;
        }

        public void SetFlags_OSZAPC_Inc16(UInt16 Result16)
        {
            mResult = (UInt64)Result16;
            Instruction = Enum_Instruction.BX_LF_INSTR_INC16;
        }

        public void SetFlags_OSZAPC_Inc32(UInt32 Result32)
        {
            mResult = (UInt64)Result32;
            Instruction = Enum_Instruction.BX_LF_INSTR_INC32;
        }

        public void SetFlags_OSZAPC_Inc64(UInt64 Result64)
        {
            mResult = (UInt64)Result64;
            Instruction = Enum_Instruction.BX_LF_INSTR_INC64;
        }
        #endregion

        #region "SET_FLAGS_OSZAPC_DEC"
        public void SetFlags_OSZAPC_Dec8(byte Result8)
        {
            Result = (UInt64)Result8;
            Instruction = Enum_Instruction.BX_LF_INSTR_DEC8;
        }

        public void SetFlags_OSZAPC_Dec16(UInt16 Result16)
        {
            Result = (UInt64)Result16;
            Instruction = Enum_Instruction.BX_LF_INSTR_DEC16;
        }

        public void SetFlags_OSZAPC_Dec32(UInt32 Result32)
        {
            Result = (UInt64)Result32;
            Instruction = Enum_Instruction.BX_LF_INSTR_DEC32 ;
        }

        public void SetFlags_OSZAPC_Dec64(UInt64 Result64)
        {
            Result = (UInt64)Result64;
            Instruction = Enum_Instruction.BX_LF_INSTR_DEC64;
        }
        #endregion

        #region "SET_FLAGS_OSZAPC_RESULT"
        public void SetFlags_OSZAPC_Result(UInt64 Result64, Enum_Instruction Ins)
        {
            Result = (UInt64)Result64;
            Instruction = Ins;
        }
        #endregion

        #endregion 

        #region "Lazy Initialize"

        /// <summary>
        /// Calculate OF flag based on save Instruction and value.
        /// </summary>
        /// <returns></returns>
        protected void LazyCalc_OF()
        {
            bool of;

            switch (Instruction)
            {
                case Enum_Instruction.BX_LF_INSTR_ADD8:
                case Enum_Instruction.BX_LF_INSTR_ADC8:
                case Enum_Instruction.BX_LF_INSTR_ADD16:
                case Enum_Instruction.BX_LF_INSTR_ADC16:
                case Enum_Instruction.BX_LF_INSTR_ADD32:
                case Enum_Instruction.BX_LF_INSTR_ADC32:
                    of = GetAddOverflow((UInt32)Operand1, (UInt32)Operand2, (UInt32)Result, 0x80000000);
                    break;

                case Enum_Instruction.BX_LF_INSTR_ADD64:
                case Enum_Instruction.BX_LF_INSTR_ADC64:
                    of = GetAddOverflow(Operand1, Operand2, Result, 0x8000000000000000);
                    break;

                case Enum_Instruction.BX_LF_INSTR_SUB8:
                case Enum_Instruction.BX_LF_INSTR_SBB8:
                case Enum_Instruction.BX_LF_INSTR_SUB16:
                case Enum_Instruction.BX_LF_INSTR_SBB16:
                case Enum_Instruction.BX_LF_INSTR_SUB32:
                case Enum_Instruction.BX_LF_INSTR_SBB32:
                    of = GetSubOverFlow((UInt32)Operand1, (UInt32)Operand2, (UInt32)Result, 0x80000000);
                    break;

                case Enum_Instruction.BX_LF_INSTR_SUB64:
                case Enum_Instruction.BX_LF_INSTR_SBB64:
                    of = GetSubOverFlow(Operand1, Operand2, Result, 0x8000000000000000);
                    break;

                case Enum_Instruction.BX_LF_INSTR_LOGIC8:
                case Enum_Instruction.BX_LF_INSTR_LOGIC16:
                case Enum_Instruction.BX_LF_INSTR_LOGIC32:

                case Enum_Instruction.BX_LF_INSTR_LOGIC64:
                    of = false;
                    break;
                case Enum_Instruction.BX_LF_INSTR_NEG8:
                case Enum_Instruction.BX_LF_INSTR_INC8:
                    of = ((byte)Result == 0x80);
                    break;
                case Enum_Instruction.BX_LF_INSTR_NEG16:
                case Enum_Instruction.BX_LF_INSTR_INC16:
                    of = ((UInt16)Result == 0x8000);
                    break;
                case Enum_Instruction.BX_LF_INSTR_NEG32:
                case Enum_Instruction.BX_LF_INSTR_INC32:
                    of = ((UInt32)Result == 0x80000000);
                    break;

                case Enum_Instruction.BX_LF_INSTR_NEG64:
                case Enum_Instruction.BX_LF_INSTR_INC64:
                    of = (Result == 0x8000000000000000);
                    break;

                case Enum_Instruction.BX_LF_INSTR_DEC8:
                    of = ((byte)Result == 0x7F);
                    break;
                case Enum_Instruction.BX_LF_INSTR_DEC16:
                    of = ((UInt16)Result == 0x7FFF);
                    break;
                case Enum_Instruction.BX_LF_INSTR_DEC32:
                    of = ((UInt32)Result == 0x7FFFFFFF);
                    break;

                case Enum_Instruction.BX_LF_INSTR_DEC64:
                    of = (Result == 0x7FFFFFFFFFFFFFFF);
                    break;

                default:
                    of = false; // Keep compiler happy.
                    //throw new Exception("get_OF: OSZAPC: unknown instr");
                    break;
            }

            if (of == true)
            {
                OF = Enum_Signal.High;
            }
            else
            {
                OF = Enum_Signal.Low;
            }
            return ;
        }

        /// <summary>
        /// Calculate AF flag based on save Instruction and value.
        /// </summary>
        /// <returns></returns>
        protected void LazyCalc_AF()
        {
            bool af;

            switch (mInstruction)
            {
                case Enum_Instruction.BX_LF_INSTR_ADD8:
                case Enum_Instruction.BX_LF_INSTR_ADC8:
                case Enum_Instruction.BX_LF_INSTR_SUB8:
                case Enum_Instruction.BX_LF_INSTR_SBB8:
                case Enum_Instruction.BX_LF_INSTR_ADD16:
                case Enum_Instruction.BX_LF_INSTR_ADC16:
                case Enum_Instruction.BX_LF_INSTR_SUB16:
                case Enum_Instruction.BX_LF_INSTR_SBB16:
                case Enum_Instruction.BX_LF_INSTR_ADD32:
                case Enum_Instruction.BX_LF_INSTR_ADC32:
                case Enum_Instruction.BX_LF_INSTR_SUB32:
                case Enum_Instruction.BX_LF_INSTR_SBB32:

                case Enum_Instruction.BX_LF_INSTR_ADD64:
                case Enum_Instruction.BX_LF_INSTR_ADC64:
                case Enum_Instruction.BX_LF_INSTR_SUB64:
                case Enum_Instruction.BX_LF_INSTR_SBB64:

                    af = (bool)(((((byte)Operand1 ^ (byte)Operand2) ^ (byte)Result) & 0x10) != 0);
                    break;
                case Enum_Instruction.BX_LF_INSTR_NEG8:
                case Enum_Instruction.BX_LF_INSTR_NEG16:
                case Enum_Instruction.BX_LF_INSTR_NEG32:

                case Enum_Instruction.BX_LF_INSTR_NEG64:

                    af = ((byte)Result & 0xf) != 0;
                    break;
                case Enum_Instruction.BX_LF_INSTR_INC8:
                case Enum_Instruction.BX_LF_INSTR_INC16:
                case Enum_Instruction.BX_LF_INSTR_INC32:

                case Enum_Instruction.BX_LF_INSTR_INC64:

                    af = ((byte)Result & 0xf) == 0;
                    break;
                case Enum_Instruction.BX_LF_INSTR_DEC8:
                case Enum_Instruction.BX_LF_INSTR_DEC16:
                case Enum_Instruction.BX_LF_INSTR_DEC32:

                case Enum_Instruction.BX_LF_INSTR_DEC64:

                    af = ((byte)Result & 0xf) == 0xf;
                    break;
                case Enum_Instruction.BX_LF_INSTR_LOGIC8:
                case Enum_Instruction.BX_LF_INSTR_LOGIC16:
                case Enum_Instruction.BX_LF_INSTR_LOGIC32:

                case Enum_Instruction.BX_LF_INSTR_LOGIC64:

                    af = false;
                    break;
                default:
                    af = false; // Keep compiler quiet.
                    //throw new Exception("get_AF: OSZAPC: unknown instr %u");
                    break;
            }

            if (af == true)
            {
                AF = Enum_Signal.High;
            }
            else
            {
                AF = Enum_Signal.Low;
            }
            return ;
        }

        /// <summary>
        /// Calculate CF flag based on save Instruction and value.
        /// </summary>
        /// <returns></returns>
        protected void  LazyCalc_CF()
        {
            bool cf;
            switch (mInstruction)
            {
                case Enum_Instruction.BX_LF_INSTR_ADD8:
                case Enum_Instruction.BX_LF_INSTR_ADD16:
                case Enum_Instruction.BX_LF_INSTR_ADD32:
                    cf = ((UInt32)Result < (UInt32)Operand1);
                    break;
                case Enum_Instruction.BX_LF_INSTR_ADD64:
                    cf = (Result < Operand1);
                    break;
                // used only if CF = 1 when executing ADC instruction
                case Enum_Instruction.BX_LF_INSTR_ADC8:
                case Enum_Instruction.BX_LF_INSTR_ADC16:
                case Enum_Instruction.BX_LF_INSTR_ADC32:
                    cf = ((UInt32)Result <= (UInt32)Operand1);
                    break;
                // used only if CF = 1 when executing ADC instruction
                case Enum_Instruction.BX_LF_INSTR_ADC64:
                    cf = (Result <= Operand1);
                    break;
                case Enum_Instruction.BX_LF_INSTR_SUB8:
                case Enum_Instruction.BX_LF_INSTR_SUB16:
                case Enum_Instruction.BX_LF_INSTR_SUB32:
                    cf = ((UInt32)Operand1 < (UInt32)Operand2);
                    break;

                case Enum_Instruction.BX_LF_INSTR_SUB64:
                    cf = (Operand1 < Operand2);
                    break;

                // used only if CF = 1 when executing SBB instruction
                case Enum_Instruction.BX_LF_INSTR_SBB8:
                    cf = ((byte)Operand1 < (byte)Result) || ((byte)Operand2 == 0xff);
                    break;
                // used only if CF = 1 when executing SBB instruction
                case Enum_Instruction.BX_LF_INSTR_SBB16:
                    cf = ((UInt16)Operand1 < (UInt16)Result) || ((UInt16)Operand2 == 0xffff);
                    break;
                // used only if CF = 1 when executing SBB instruction
                case Enum_Instruction.BX_LF_INSTR_SBB32:
                    cf = ((UInt32)Operand1 < (UInt32)Result) || ((UInt32)Operand2 == 0xffffffff);
                    break;

                // used only if CF = 1 when executing SBB instruction
                case Enum_Instruction.BX_LF_INSTR_SBB64:
                    cf = (Operand1 < Result) || (Operand2 == 0xffffffffffffffff);
                    break;

                case Enum_Instruction.BX_LF_INSTR_NEG8:
                case Enum_Instruction.BX_LF_INSTR_NEG16:
                case Enum_Instruction.BX_LF_INSTR_NEG32:
                    cf = ((UInt32)Result != 0);
                    break;

                case Enum_Instruction.BX_LF_INSTR_NEG64:
                    cf = (Result != 0);
                    break;

                case Enum_Instruction.BX_LF_INSTR_LOGIC8:
                case Enum_Instruction.BX_LF_INSTR_LOGIC16:
                case Enum_Instruction.BX_LF_INSTR_LOGIC32:

                case Enum_Instruction.BX_LF_INSTR_LOGIC64:

                    cf = false;
                    break;
                default:
                    cf = false; // Keep compiler quiet.
                    //throw new Exception("get_CF: OSZAPC: unknown instr %u");
                    break;
            }

            if (cf == true)
            {
                CF = Enum_Signal.High;
            }
            else
            {
                CF = Enum_Signal.Low;
            }
            return ;
        }

        /// <summary>
        ///  return (BX_CPU_THIS_PTR oszapc.result == 0);
        /// </summary>
        /// <returns></returns>
        protected void  LazyCalcZF()
        {
            /*
             * The issue here is that the CPU initalize this flag with Zero.
             * So even in case of no operation when trying to read this flag the result
             * is errorously one. So we need to ignore mResult = 0 in case of no operation yet.
             * see  [mNoOperationYet]
             */

            if (mNoOperationYet == true)
            {
                ZF = Enum_Signal.Low;
                return;
            }

            if (this.mResult == 0)
            {
                ZF = Enum_Signal.High;
            }
            else
            {
                 ZF=Enum_Signal.Low ;
            }
            return ;
        }

        /// <summary>
        ///  return (BX_CPU_THIS_PTR oszapc.result >> BX_LF_SIGN_BIT);
        /// </summary>
        /// <returns></returns>
        protected void LazyCalcSF()
        {
            if (mNoOperationYet == true)
            {
                return;
            }
            if (this.mResult == const_BX_LF_SIGN_BIT)
            {
                SF = Enum_Signal.High;
            }
            else
            {
                SF = Enum_Signal.Low;
            }
            return;
        }


        /// <summary>
        ///    return bx_parity_lookup[(Bit8u) BX_CPU_THIS_PTR oszapc.result];
        /// </summary>
        /// <returns></returns>
        protected void LazyCalcPF()
        {
            if (mNoOperationYet == true)
            {
                return;
            }
            if (mBxParityLookup[(byte)(this.mResult)]==1)
            {
                PF = Enum_Signal.High;
            }
            else
            {
                PF = Enum_Signal.Low;
            }
            return;
        }

        protected bool GetAddOverflow(UInt64 Operand1, UInt64 Operand2, UInt64 Result,UInt64 Mask)
        {
            return (((~((Operand1) ^ (Operand2)) & ((Operand2) ^ (Result))) & (Mask)) != 0);
        }

        protected bool GetSubOverFlow(UInt64 Operand1, UInt64 Operand2, UInt64 Result, UInt64 Mask)
        {
            return (((((Operand1) ^ (Operand2)) & ((Operand1) ^ (Result))) & (Mask)) != 0);
        }
        #endregion
        #endregion
    }
}
