using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Definitions;

using CPU;

namespace CPU.Instructions
{
    public class Instruction
    {


        /*
         *  00  01  02  03  04  05  06  07  08  
         *  
         */


        #region "Enumeration"

        public enum Enum_InstructionMetaData : int
        {
            SEG = 0,
            DEST = 1,
            NNN = 2,
            RM = 3,
            BASE = 4,
            INDEX = 5,
            SCALE = 6,
            MODRM = 7
        }
        #endregion

        

        #region "Static Attributes"
        
        protected static byte[] mResolve16BaseReg = new byte[8] 
        {       
           (byte) Enum_16BitReg.REG_BX ,
           (byte) Enum_16BitReg.REG_BX,
           (byte) Enum_16BitReg.REG_BP,
           (byte) Enum_16BitReg.REG_BP,
           (byte) Enum_16BitReg.REG_SI,
           (byte) Enum_16BitReg.REG_DI,
           (byte) Enum_16BitReg.REG_BP,
           (byte) Enum_16BitReg.REG_BX
        };

        protected static byte[] mResolve16IndexReg = new byte[8] 
        {
            (byte)Enum_16BitReg.REG_SI,
            (byte)Enum_16BitReg.REG_DI,
            (byte)Enum_16BitReg.REG_SI,
            (byte)Enum_16BitReg.REG_DI,
            (byte)Enum_GeneralReg.BX_NIL_REGISTER,
            (byte)Enum_GeneralReg.BX_NIL_REGISTER,
            (byte)Enum_GeneralReg.BX_NIL_REGISTER,
            (byte)Enum_GeneralReg.BX_NIL_REGISTER,
        };
        protected static byte[] msreg_mod00_rm16 = new byte[8] 
        {
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_SS,
            (byte) Enum_SegmentReg.REG_SS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_DS
        };

        protected static byte[] msreg_mod01or10_rm16 = new byte[8] 
        {
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_SS,
            (byte) Enum_SegmentReg.REG_SS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_DS,
            (byte) Enum_SegmentReg.REG_SS,
            (byte) Enum_SegmentReg.REG_DS
        };

        protected static byte[] msreg_mod01or10_rm32 = new byte[8]
        { 
            (byte) Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.BX_SEG_REG_NULL, // escape to SIB-byte
            (byte)Enum_SegmentReg.REG_SS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS
        };


        protected static byte[] msreg_mod0_base32 = new byte[8]
        { 
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_SS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS
        };


        protected static byte[] msreg_mod1or2_base32 = new byte[8]
        { 
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_SS,
            (byte)Enum_SegmentReg.REG_SS,
            (byte)Enum_SegmentReg.REG_DS,
            (byte)Enum_SegmentReg.REG_DS
        };

        #endregion 

        #region "Attributes"
        protected CPU mCPU;

        protected byte[] mValue;

        protected InstructionExecution.dlgt_OPCodeInstruction mExecute1;
        protected InstructionExecution.dlgt_OPCodeInstruction mExecute2;
        protected InstructionExecution.dlgt_OPCodeInstruction mResolveModrm;
        /// <summary>
        /// using 5-bit field for registers (16 regs in 64-bit, RIP, NIL)
        /// </summary>
        protected byte[] mMetaData;
        protected byte[] mMetaInfo;
        protected UInt64 mInstructionAddress;
        #endregion

        
        #region "Static Properties"



        public static byte[] Resolve16BaseReg
        {
            get
            {
                return mResolve16BaseReg;
            }
        }

        public static byte[] Resolve16IndexReg
        {
            get
            {
                return mResolve16IndexReg;
            }
        }

        public static byte[] sreg_mod00_rm16
        {
            get
            {
                return msreg_mod00_rm16;
            }
        }

        public static byte[] sreg_mod01or10_rm16
        {
            get
            {
                return msreg_mod01or10_rm16;
            }
        }

        public static byte[] sreg_mod01or10_rm32
        {
            get
            {
                return msreg_mod01or10_rm32;
            }
        }

        public static byte[] sreg_mod0_base32
        {
            get
            {
                return msreg_mod0_base32;
            }
        }


        public static byte[] sreg_mod1or2_base32
        {
            get
            {
                return msreg_mod1or2_base32;
            }
        }
        #endregion

        #region "Properties"

        #region "Relations"
        public CPU CPU
        {
            get
            {
                return mCPU;
            }
        }
        #endregion 

        #region "Instruction Delegates"

        public InstructionExecution.dlgt_OPCodeInstruction Execute1
        {
            get
            {
                return mExecute1;
            }
            set
            {
                mExecute1 = value;
            }
        }
        public InstructionExecution.dlgt_OPCodeInstruction Execute2
        {
            get
            {
                return mExecute2;
            }
            set
            {
                mExecute2 = value;
            }
        }
        public InstructionExecution.dlgt_OPCodeInstruction ResolveModrm
        {
            get
            {
                return mResolveModrm;
            }
            set
            {
                mResolveModrm = value;
            }
        }
        #endregion

        #region "ModRMForm, IxIxForm, IqForm"
        /*
        // union {
        // Form (longest case): [opcode+modrm+sib/displacement32/immediate32]
        //    struct {
        //      union {
        //        Bit32u Id;
        //        Bit16u Iw;
        //        Bit8u  Ib;
        //      };
        //      union {
        //        Bit16u displ16u; // for 16-bit modrm forms
        //        Bit32u displ32u; // for 32-bit modrm forms
        //      };
        //    } modRMForm;

        //    struct {
        //      union {
        //        Bit32u Id;
        //        Bit16u Iw;
        //        Bit8u  Ib;
        //      };
        //      union {
        //        Bit32u Id2; // Not used (for alignment)
        //        Bit16u Iw2;
        //        Bit8u  Ib2;
        //      };
        //    } IxIxForm;

        //#if BX_SUPPORT_X86_64
        //    struct {
        //      Bit64u   Iq;  // for MOV Rx,imm64
        //    } IqForm;
        //#endif
        //  };
        //
         */
        /// <summary>
        /// modRMForm_Id: byte [0..3]
        /// </summary>
        public UInt32 modRMForm_Id
        {
            get
            {
                return (UInt32)BitConverter.ToInt32(mValue, 0);
            }
            set
            {
                byte[] b = BitConverter.GetBytes(value);
                mValue[0] = b[0];
                mValue[1] = b[1];
                mValue[2] = b[2];
                mValue[3] = b[3];
            }
        }

        /// <summary>
        /// modRMForm_Iw: byte [0..1]
        /// </summary>
        public UInt16 modRMForm_Iw
        {
            get
            {
                return (UInt16)BitConverter.ToInt16(mValue, 0);
            }
            set
            {
                Definitions.DataTypes.GetInt16BytesToArray(mValue, value, 0);
            }
        }

        /// <summary>
        /// modRMForm_Ib: byte [0]
        /// </summary>
        public byte modRMForm_Ib
        {
            get
            {
                return mValue[0];
            }
            set
            {
                mValue[0] = value;
            }
        }

        /// <summary>
        /// = modRMForm_Id: byte [0..3] 
        /// </summary>
        public UInt32 IXIXForm_Id
        {
            get
            {
                return (UInt32)BitConverter.ToInt32(mValue, 0);
            }
            set
            {
                Definitions.DataTypes.GetInt32BytesToArray(mValue, value, 0);
            }
        }

        /// <summary>
        /// = modRMForm_Iw: byte [0..1]
        /// </summary>
        public UInt16 IXIXForm_Iw
        {
            get
            {
                return (UInt16)BitConverter.ToInt16(mValue, 0);
            }
            set
            {
                Definitions.DataTypes.GetInt16BytesToArray(mValue, value, 0);
            }
        }

        /// <summary>
        /// modRMForm_Ib: byte [0]
        /// </summary>
        public byte IXIXForm_Ib
        {
            get
            {
                return mValue[0];
            }
            set
            {
                mValue[0] = value;
            }
        }


        /// <summary>
        /// modRMForm_Displ16u: byte [4..5]
        /// </summary>
        public UInt16 modRMForm_Displ16u
        {
            get
            {
                return (UInt16)BitConverter.ToInt16(mValue, 4);
            }
            set
            {
                Definitions.DataTypes.GetInt16BytesToArray(mValue, value, 4);

            }
        }

        /// <summary>
        /// modRMForm_Displ32u: byte [4..7]
        /// </summary>
        public UInt32 modRMForm_Displ32u
        {
            get
            {
                return (UInt32)BitConverter.ToInt32(mValue, 4);
            }
            set
            {
                Definitions.DataTypes.GetInt32BytesToArray(mValue, value, 4);
            }
        }

        /// <summary>
        /// = modRMForm_Displ32u: byte [4..7]
        /// </summary>
        public UInt32 IXIXForm_Id2
        {
            get
            {
                return (UInt32)BitConverter.ToInt32(mValue, 4);
            }
            set
            {
                Definitions.DataTypes.GetInt32BytesToArray(mValue, value, 4);
            }
        }


        /// <summary>
        /// = modRMForm_Displ16u: byte [4..5]
        /// </summary>
        public UInt16 IXIXForm_Iw2
        {
            get
            {
                return (UInt16)BitConverter.ToInt16(mValue, 4);
            }
            set
            {
                Definitions.DataTypes.GetInt16BytesToArray(mValue, value, 4);
            }
        }

        /// <summary>
        /// byte [4]
        /// </summary>
        public byte IXIXForm_Ib2
        {
            get
            {
                return mValue[4];
            }
            set
            {
                mValue[4] = value;
            }
        }

        public UInt64 IqForm_Iq
        {
            get
            {
                return (UInt64)BitConverter.ToInt64(mValue, 0);
            }
            set
            {
                Definitions.DataTypes.GetInt64BytesToArray(mValue, value, 0);
            }
        }

        #endregion

        #region "MetaInfo"

        /// <summary>
        /// 7...1 (unused)
        /// 0...0 stop trace (used with trace cache)
        /// </summary>
        public byte MetaInfo4
        {
            get
            {
                return mMetaInfo[3];
            }
            set
            {
                mMetaInfo[3] = value;
            }
        }

        /// <summary>
        /// 7...0 b1 - opcode byte
        /// </summary>
        public byte MetaInfo3
        {
            get
            {
                return mMetaInfo[2];
            }
            set
            {
                mMetaInfo[2] = value;
            }
        }

        /// <summary>
        ///  7...4 (unused)
        ///  3...0 ilen (0..15)
        /// </summary>
        public byte MetaInfo2
        {
            get
            {
                return mMetaInfo[1];
            }
            set
            {
                mMetaInfo[1] = value;
            }
        }

        /// <summary>
        ///  7...7 extend8bit
        ///  6...6 as64
        ///  5...5 os64
        ///  4...4 as32
        ///  3...3 os32
        ///  2...2 mod==c0 (modrm)
        ///  1...0 repUsed (0=none, 2=0xF2, 3=0xF3)
        /// </summary>
        public byte MetaInfo1
        {
            get
            {
                return mMetaInfo[0];
            }
            set
            {
                mMetaInfo[0] = value;
            }
        }
        #endregion

        #region "MetaData"

        /// <summary>
        /// using 5-bit field for registers (16 regs in 64-bit, RIP, NIL)
        /// </summary>
        public byte[] MetaData
        {
            get
            {
                return mMetaData;
            }
        }
        #endregion

        public UInt64 InstructionAddress
        {
            get
            {
                return mInstructionAddress;
            }
            set
            {
                mInstructionAddress = value;
            }
        }
        #endregion


        #region "Constructor"

        public Instruction(CPU oCPU)
        {
            mCPU = oCPU;
            mValue = new byte[8];
            mMetaData = new byte[8];
            mMetaInfo = new byte[4];
        }

        #endregion


        #region "Methods"

        public UInt32 Id()
        {
            return modRMForm_Id;
        }

        /// <summary>
        ///  modRMForm_Ib that equals mValue[0]
        /// </summary>
        /// <returns></returns>
        public byte Ib()
        {
            return modRMForm_Ib;
        }

        

        
        public byte Ib2()
        {
            return IXIXForm_Ib2;
        }

        public UInt16 Iw()
        {
            return modRMForm_Iw;
        }

        public UInt16 Iw2()
        {
            return IXIXForm_Iw2 ;
        }

        public UInt64 Iq()
        {
            return IqForm_Iq;
        }

        public void Init(byte os32, byte as32, byte os64, byte as64)
        {
            MetaInfo1 = (byte)((os32 << 3) | (as32 << 4) | (os64 << 5) | (as64 << 6));
            MetaInfo4 = 0;
        }



        public byte Seg()
        {
            return mMetaData[(int)Enum_InstructionMetaData.SEG];
        }

        public void SetSeg(byte Value)
        {
            mMetaData[(int)Enum_InstructionMetaData.SEG] = Value;
        }


        /// <summary>
        /// return metaInfo.metaInfo1 & (1<<3);
        /// </summary>
        /// <returns></returns>
        public byte Os32L()
        {
            return (byte) (MetaInfo1 & (1<<3));
        }

        /// <summary>
        /// MetaInfo1 = (byte) ((MetaInfo1  & ~(1 gt gt 3)) | (bit gt gt 3));
        /// </summary>
        /// <param name="bit"></param>
        public void SetOS32B(byte bit)
        {
            MetaInfo1 = (byte)((MetaInfo1 & ~(1 << 3)) | (bit << 3));
        }

        /// <summary>
        /// MetaInfo1 = (byte)((MetaInfo1 & ~(1 gtgt 4)) | (bit gtgt 4));
        /// </summary>
        /// <param name="bit"></param>
        public void SetAS32B(byte bit)
        {
            MetaInfo1 = (byte)((MetaInfo1 & ~(1 << 4)) | (bit << 4));
        }

        /// <summary>
        /// return metaInfo.metaInfo1 & (1gtgt4);
        /// </summary>
        public byte AS32L()
        {
            return (byte)((MetaInfo1 & (1 << 4)));
        }

        public byte AS64L()
        {
            return (byte)(MetaInfo1 & (1<<6));
        }

        public void SetAS64L(byte value)
        {
            MetaInfo1 = (byte)((MetaInfo1 & (1 << 6)) | (value << 6));
        }

        /// <summary>
        /// return metaInfo.metaInfo1 & (1gtgt5);
        /// </summary>
        /// <param name="bit"></param>
        public byte OS64L()
        {
            return (byte)(MetaInfo1 & (1 << 5));
        }

     
        /// <summary>
        /// return MetaInfo3;
        /// </summary>
        /// <returns></returns>
        public byte B1()
        {
            return MetaInfo3;
        }

        /// <summary>
        /// MetaInfo3= (byte) (value & 0xff);
        /// </summary>
        /// <param name="value"></param>
        public void SetB1(byte value)
        {
            MetaInfo3 = (byte)(value & 0xff);
        }

        /// <summary>
        ///  MetaData[(byte)Enum_InstructionMetaData.MODRM] = ModRM;
        /// </summary>
        /// <param name="ModRM"></param>
        public void SetModRM(byte ModRM)
        {
            MetaData[(byte)Enum_InstructionMetaData.MODRM] = ModRM;
        }


        /// <summary>
        /// return MetaData[(byte)Enum_InstructionMetaData.MODRM];
        /// </summary>
        /// <returns></returns>
        public byte ModRM()
        {
            return MetaData[(byte)Enum_InstructionMetaData.MODRM];
        }


        /// <summary>
        /// MetaData[(byte)Enum_InstructionMetaData.NNN] = NNN;
        /// </summary>
        /// <param name="NNN"></param>
        public void SetNnn(byte NNN)
        {
            MetaData[(byte)Enum_InstructionMetaData.NNN] = NNN;
        }

        /// <summary>
        /// return MetaData[(byte)Enum_InstructionMetaData.NNN];
        /// </summary>
        /// <returns></returns>
        public byte Nnn()
        {
            return MetaData[(byte)Enum_InstructionMetaData.NNN];
        }

        /// <summary>
        /// MetaData[(byte)Enum_InstructionMetaData.RM] = RM;
        /// </summary>
        /// <param name="RM"></param>
        public void SetRM(byte RM)
        {
            MetaData[(byte)Enum_InstructionMetaData.RM] = RM;
        }


        /// <summary>
        /// return MetaData[(byte)Enum_InstructionMetaData.RM];
        /// </summary>
        /// <returns></returns>
        public byte RM()
        {
            return MetaData[(byte)Enum_InstructionMetaData.RM];
        }


        /// <summary>
        /// return MetaInfo1 |= (1 gtgt 2);
        /// </summary>
        /// <returns></returns>
        public byte AssertModC0()
        {
            return MetaInfo1 |= (1 << 2);
        }

        /// <summary>
        /// MetaData[(byte)Enum_InstructionMetaData.BASE] = Base;
        /// </summary>
        /// <param name="Base"></param>
        public void SetSibBase(byte Base)
        {
            MetaData[(byte)Enum_InstructionMetaData.BASE] = Base;
        }


        /// <summary>
        /// return MetaData[(byte)Enum_InstructionMetaData.BASE];
        /// </summary>
        /// <returns></returns>
        public byte SibBase()
        {
            return MetaData[(byte)Enum_InstructionMetaData.BASE];
        }


        public void SetSibIndex(byte Index)
        {
            MetaData[(byte)Enum_InstructionMetaData.INDEX] = Index;
        }

        public byte SibIndex()
        {
            return MetaData[(byte)Enum_InstructionMetaData.INDEX];
        }

        public void SetSibScale(byte Scale)
        {
            MetaData[(byte)Enum_InstructionMetaData.SCALE] = Scale;
        }

        public byte SibScale()
        {
            return MetaData[(byte)Enum_InstructionMetaData.SCALE];
        }

        public void SetILen(byte iLen)
        {
            MetaInfo2 = iLen;
        }

        public void SetStopTraceAttr()
        {
            MetaInfo4 |= 1;
        }


        public byte GetStopTraceAttr()
        {
            return (byte)(MetaInfo4 & 1);
        }

        public void SetOpcodeReg(byte opreg)
        {
            mMetaData[(byte)Enum_InstructionMetaData.RM] = opreg;
        }

        public byte OpcodeReg()
        {
            return mMetaData[(byte)Enum_InstructionMetaData.RM];
        }

        public byte Extend8BitL()
        {
            return (byte)(MetaInfo1  & (1 <<7));
        }

        public byte iLen()
        {
            return MetaInfo2;
        }

        public byte ModC0()
        {
            return (byte)(MetaInfo1 & (1 << 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return (byte) (MetaInfo1 & 3);</returns>
        public byte RepUsedL()
        {
            return (byte) (MetaInfo1 & 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return (byte) (MetaInfo1 & 3)</returns>
        public byte RepUsedValue()
        {
            return (byte) (MetaInfo1 & 3);
        }

        /// <summary>
        ///  MetaInfo1 = (byte)((MetaInfo1 & ~3) | value);
        /// </summary>
        /// <param name="value"></param>
        public void SetRepUsed(byte value)
        {
            MetaInfo1 = (byte) ((MetaInfo1 & (~3)) | (value));
        }


        #endregion


    }
}
