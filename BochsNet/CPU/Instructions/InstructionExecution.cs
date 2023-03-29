using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CPU.Instructions
{
    /// <summary>
    /// 
    /// </summary>
    public class InstructionExecution
    {

        #region "Enumeration"

        protected enum Enum_SSE : byte
        {
            SSE_PREFIX_NONE = 0,
            SSE_PREFIX_66 = 1,
            SSE_PREFIX_F2 = 2,
            SSE_PREFIX_F3 = 3
        }



        #endregion

        #region "Constants"

        #endregion

        #region "Attributes"
        protected CPU mCPU;

        protected Arith8 mArith8;
        protected Arith16 mArith16;
        protected Arith32 mArith32;
        protected Arith64 mArith64;
        protected BCD mBCD;
        protected Ctrl_Xfer16 mCtrl_Xref16;
        protected Ctrl_Xfer32 mCtrl_Xref32;
        protected Ctrl_Xfer64 mCtrl_Xref64;
        protected Data_Xfer8 mData_Xref8;
        protected Data_Xfer16 mData_Xref16;
        protected Data_Xfer32 mData_Xref32;
        protected Error mError;
        protected Flag_Ctrl mFlag_Ctrl;
        protected FPU_Emu mFPU_Emu;
        protected IO mIO;
        protected Logical8 mLogical8;
        protected Logical16 mLogical16;
        protected Logical32 mLogical32;
        protected Logical64 mLogical64;
        protected MMX mMMX;
        protected Mult16 mMult16;
        protected Proc_Ctrl mProc_Ctrl;
        protected Protected_Ctrl mProtected_Ctrl;
        protected Shift8 mShift8;
        protected Shift16 mShift16;
        protected Shift32 mShift32;
        protected Shift64 mShift64;
        protected SoftInt mSoftInt;
        protected SSE mSSE;
        protected Stack16 mStack16;
        protected Stack32 mStack32;
        protected Stack64 mStack64;
        protected StringInst mStringInst;

        public delegate void dlgt_OPCodeInstruction(Instruction oInstruction);

        private const byte X = 1;
        protected byte[] mBxOpcodeHasModrm32 = new byte[]
        {
        /*       0 1 2 3 4 5 6 7 8 9 a b c d e f          */
  /*       -------------------------------          */
  /* 00 */ 1,1,1,1,0,0,0,0,1,1,1,1,0,0,0,X,
  /* 10 */ 1,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,
  /* 20 */ 1,1,1,1,0,0,X,0,1,1,1,1,0,0,X,0,
  /* 30 */ 1,1,1,1,0,0,X,0,1,1,1,1,0,0,X,0,
  /* 40 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* 50 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* 60 */ 0,0,1,1,X,X,X,X,0,1,0,1,0,0,0,0,
  /* 70 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* 80 */ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
  /* 90 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* A0 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* B0 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* C0 */ 1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,
  /* D0 */ 1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,
  /* E0 */ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
  /* F0 */ X,0,X,X,0,0,1,1,0,0,0,0,0,0,1,1,
  /*       0 1 2 3 4 5 6 7 8 9 a b c d e f           */
  /*       -------------------------------           */
           1,1,1,1,X,0,0,0,0,0,X,0,X,1,0,1, /* 0F 00 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F 10 */
           1,1,1,1,1,X,1,X,1,1,1,1,1,1,1,1, /* 0F 20 */
           0,0,0,0,0,0,X,X,1,X,1,X,X,X,X,X, /* 0F 30 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F 40 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F 50 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F 60 */
           1,1,1,1,1,1,1,0,1,1,X,X,1,1,1,1, /* 0F 70 */
           0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, /* 0F 80 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F 90 */
           0,0,0,1,1,1,0,0,0,0,0,1,1,1,1,1, /* 0F A0 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F B0 */
           1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0, /* 0F C0 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F D0 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, /* 0F E0 */
           1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,X  /* 0F F0 */
  /*       -------------------------------           */
  /*       0 1 2 3 4 5 6 7 8 9 a b c d e f           */
};

        /// <summary>
        /// List of OpCode stored by PrimaryOpCode Number as in the URL below.
        /// <see cref="http://ref.x86asm.net/coder32-abc.html"/>
        /// </summary>
        protected OpCode[] mOPCodeInstruction32R = new OpCode[512];
        protected OpCode[] mOPCodeInstruction32M = new OpCode[512];
        protected OpCode[] mOPCodeInfoG1EbIbM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1EbIbR = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1EwM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1EwR = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1EdM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1EdR = new OpCode[8];
        protected OpCode[] mOPCodeInfo64G1EqM = new OpCode[8];
        protected OpCode[] mOPCodeInfo64G1EqR = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1AEwR = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1AEwM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1AEdR = new OpCode[8];
        protected OpCode[] mOPCodeInfoG1AEdM = new OpCode[8];
        protected OpCode[] mOPCodeInfo64G1AEqR = new OpCode[8];
        protected OpCode[] mOPCodeInfo64G1AEqM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG2Eb = new OpCode[8];
        protected OpCode[] mOPCodeInfoG2Ew = new OpCode[8];
        protected OpCode[] mOPCodeInfoG2EdM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG2EdR = new OpCode[8];
        protected OpCode[] mOPCodeInfo64G2EqM = new OpCode[8];
        protected OpCode[] mOPCodeInfo64G2EqR = new OpCode[8];
        protected OpCode[] mOPCodeInfoG3EbM = new OpCode[8];
        protected OpCode[] mOPCodeInfoG3EbR = new OpCode[8];

        protected OpCode[] mOPCodeGroupSSE_PAUSE = new OpCode[8];
        protected OpCode[] mOPCodeGroupSSE_0fe8 = new OpCode[3];
        protected OpCode[] mOPCodeGroupSSE_0fe9 = new OpCode[3];
        #endregion

        #region "Properties"

        public byte[] BxOpcodeHasModrm32
        {
            get
            {
                return mBxOpcodeHasModrm32;
            }
        }
        public OpCode[] OPCodeInstruction32R
        {
            get
            {
                return mOPCodeInstruction32R;
            }
        }


        public OpCode[] OPCodeInstruction32M
        {
            get
            {
                return mOPCodeInstruction32M;
            }
        }


        /// <summary>
        /// Group 1
        /// </summary>
        public OpCode[] OPCodeInfoG1EbIbM
        {
            get
            {
                return mOPCodeInfoG1EbIbM;
            }
        }



        #endregion

        #region "Constructor"

        public InstructionExecution(CPU oCPU)
        {
            mCPU = oCPU;
            Initialize();
        }

        #endregion
        
        #region "Methods"

        public void Initialize()
        {

            #region "Insructions"

            mBCD = new BCD();
            mCtrl_Xref16 = new Ctrl_Xfer16();
            mCtrl_Xref32 = new Ctrl_Xfer32();
            mCtrl_Xref64 = new Ctrl_Xfer64();
            mData_Xref16 = new Data_Xfer16();
            mError = new Error();
            mFlag_Ctrl = new Flag_Ctrl();
            mFPU_Emu = new FPU_Emu();

            mIO = new IO();
            mMMX = new MMX();
            mMult16 = new Mult16();
            mProtected_Ctrl = new Protected_Ctrl();
            mProc_Ctrl = new Proc_Ctrl();
            

            mShift8 = new Shift8();
            mShift16 = new Shift16();
            mShift32 = new Shift32();
            mShift64 = new Shift64();
            mSoftInt = new SoftInt();
            mSSE = new SSE();
            mStack16 = new Stack16();
            mStack32 = new Stack32();
            mStack64 = new Stack64();
            mStringInst = new StringInst();

            mLogical8 = new Logical8();
            mLogical16 = new Logical16();
            mLogical32 = new Logical32();
            mLogical64 = new Logical64();
            mArith8 = new Arith8();
            mArith16 = new Arith16();
            mArith32 = new Arith32();
            mArith64 = new Arith64();
            mData_Xref8 = new Data_Xfer8();
            mData_Xref32 = new Data_Xfer32();
            #endregion

            #region "mOPCodeInfoG1EbIbM"
            mOPCodeInfoG1EbIbM[0] = new OpCode(DecodingAttribute.BxLockable, mArith8.ADD_EbIbM);
            mOPCodeInfoG1EbIbM[1] = new OpCode(DecodingAttribute.BxLockable, mLogical8.OR_EbIbM);
            mOPCodeInfoG1EbIbM[2] = new OpCode(DecodingAttribute.BxLockable, mArith8.ADC_EbIbM);
            mOPCodeInfoG1EbIbM[3] = new OpCode(DecodingAttribute.BxLockable, mArith8.SBB_EbIbM);
            mOPCodeInfoG1EbIbM[4] = new OpCode(DecodingAttribute.BxLockable, mLogical8.AND_EbIbM);
            mOPCodeInfoG1EbIbM[5] = new OpCode(DecodingAttribute.BxLockable, mArith8.SUB_EbIbM);
            mOPCodeInfoG1EbIbM[6] = new OpCode(DecodingAttribute.BxLockable, mLogical8.XOR_EbIbM);
            mOPCodeInfoG1EbIbM[7] = new OpCode(DecodingAttribute.BXZero, mArith8.CMP_EbIbM);
            #endregion



            #region "mOPCodeInfoG1EbIbR "
            mOPCodeInfoG1EbIbR[0] = new OpCode(DecodingAttribute.BXZero, mArith8.ADD_EbIbR);
            mOPCodeInfoG1EbIbR[1] = new OpCode(DecodingAttribute.BXZero, mLogical8.OR_EbIbR);
            mOPCodeInfoG1EbIbR[2] = new OpCode(DecodingAttribute.BXZero, mArith8.ADC_EbIbR);
            mOPCodeInfoG1EbIbR[3] = new OpCode(DecodingAttribute.BXZero, mArith8.SBB_EbIbR);
            mOPCodeInfoG1EbIbR[4] = new OpCode(DecodingAttribute.BXZero, mLogical8.AND_EbIbR);
            mOPCodeInfoG1EbIbR[5] = new OpCode(DecodingAttribute.BXZero, mArith8.SUB_EbIbR);
            mOPCodeInfoG1EbIbR[6] = new OpCode(DecodingAttribute.BXZero, mLogical8.XOR_EbIbR);
            mOPCodeInfoG1EbIbR[7] = new OpCode(DecodingAttribute.BXZero, mArith8.CMP_EbIbR);
            #endregion


            #region "mOPCodeInfoG1EwM "
            mOPCodeInfoG1EwM[0] = new OpCode(DecodingAttribute.BxLockable, mArith16.ADD_EwIwM);
            mOPCodeInfoG1EwM[1] = new OpCode(DecodingAttribute.BxLockable, mLogical16.OR_EwIwM);
            mOPCodeInfoG1EwM[2] = new OpCode(DecodingAttribute.BxLockable, mArith16.ADC_EwIwM);
            mOPCodeInfoG1EwM[3] = new OpCode(DecodingAttribute.BxLockable, mArith16.SBB_EwIwM);
            mOPCodeInfoG1EwM[4] = new OpCode(DecodingAttribute.BxLockable, mLogical16.AND_EwIwM);
            mOPCodeInfoG1EwM[5] = new OpCode(DecodingAttribute.BxLockable, mArith16.SUB_EwIwM);
            mOPCodeInfoG1EwM[6] = new OpCode(DecodingAttribute.BxLockable, mLogical16.XOR_EwIwM);
            mOPCodeInfoG1EwM[7] = new OpCode(DecodingAttribute.BXZero, mArith16.CMP_EwIwM);
            #endregion

            #region "mOPCodeInfoG1EwR "
            mOPCodeInfoG1EwR[0] = new OpCode(DecodingAttribute.BXZero, mArith16.ADD_EwIwR);
            mOPCodeInfoG1EwR[1] = new OpCode(DecodingAttribute.BXZero, mLogical16.OR_EwIwR);
            mOPCodeInfoG1EwR[2] = new OpCode(DecodingAttribute.BXZero, mArith16.ADC_EwIwR);
            mOPCodeInfoG1EwR[3] = new OpCode(DecodingAttribute.BXZero, mArith16.SBB_EwIwR);
            mOPCodeInfoG1EwR[4] = new OpCode(DecodingAttribute.BXZero, mLogical16.AND_EwIwR);
            mOPCodeInfoG1EwR[5] = new OpCode(DecodingAttribute.BXZero, mArith16.SUB_EwIwR);
            mOPCodeInfoG1EwR[6] = new OpCode(DecodingAttribute.BXZero, mLogical16.XOR_EwIwR);
            mOPCodeInfoG1EwR[7] = new OpCode(DecodingAttribute.BXZero, mArith16.CMP_EwIwR);
            #endregion

            #region "mOPCodeInfoG1EdM "
            mOPCodeInfoG1EdM[0] = new OpCode(DecodingAttribute.BXZero, mArith32.ADD_EdIdM);
            mOPCodeInfoG1EdM[1] = new OpCode(DecodingAttribute.BXZero, mLogical32.OR_EdIdM);
            mOPCodeInfoG1EdM[2] = new OpCode(DecodingAttribute.BXZero, mArith32.ADC_EdIdM);
            mOPCodeInfoG1EdM[3] = new OpCode(DecodingAttribute.BXZero, mArith32.SBB_EdIdM);
            mOPCodeInfoG1EdM[4] = new OpCode(DecodingAttribute.BXZero, mLogical32.AND_EdIdM);
            mOPCodeInfoG1EdM[5] = new OpCode(DecodingAttribute.BXZero, mArith32.SUB_EdIdM);
            mOPCodeInfoG1EdM[6] = new OpCode(DecodingAttribute.BXZero, mLogical32.XOR_EdIdM);
            mOPCodeInfoG1EdM[7] = new OpCode(DecodingAttribute.BXZero, mArith32.CMP_EdIdM);
            #endregion



            #region "mOPCodeInfoG1EdR "
            mOPCodeInfoG1EdM[0] = new OpCode(DecodingAttribute.BXZero, mArith32.ADD_EdIdM);
            mOPCodeInfoG1EdM[1] = new OpCode(DecodingAttribute.BXZero, mLogical32.OR_EdIdM);
            mOPCodeInfoG1EdM[2] = new OpCode(DecodingAttribute.BXZero, mArith32.ADC_EdIdM);
            mOPCodeInfoG1EdM[3] = new OpCode(DecodingAttribute.BXZero, mArith32.SBB_EdIdM);
            mOPCodeInfoG1EdM[4] = new OpCode(DecodingAttribute.BXZero, mLogical32.AND_EdIdM);
            mOPCodeInfoG1EdM[5] = new OpCode(DecodingAttribute.BXZero, mArith32.SUB_EdIdM);
            mOPCodeInfoG1EdM[6] = new OpCode(DecodingAttribute.BXZero, mLogical32.XOR_EdIdM);
            mOPCodeInfoG1EdM[7] = new OpCode(DecodingAttribute.BXZero, mArith32.CMP_EdIdM);
            #endregion


            #region "mOPCodeInfoG1EdR"
            mOPCodeInfoG1EdR[0] = new OpCode(DecodingAttribute.BXZero, mArith32.ADD_EdIdR);
            mOPCodeInfoG1EdR[1] = new OpCode(DecodingAttribute.BXZero, mLogical32.OR_EdIdR);
            mOPCodeInfoG1EdR[2] = new OpCode(DecodingAttribute.BXZero, mArith32.ADC_EdIdR);
            mOPCodeInfoG1EdR[3] = new OpCode(DecodingAttribute.BXZero, mArith32.SBB_EdIdR);
            mOPCodeInfoG1EdR[4] = new OpCode(DecodingAttribute.BXZero, mLogical32.AND_EdIdR);
            mOPCodeInfoG1EdR[5] = new OpCode(DecodingAttribute.BXZero, mArith32.SUB_EdIdR);
            mOPCodeInfoG1EdR[6] = new OpCode(DecodingAttribute.BXZero, mLogical32.XOR_EdIdR);
            mOPCodeInfoG1EdR[7] = new OpCode(DecodingAttribute.BXZero, mArith32.CMP_EdIdR);
            #endregion


            #region "mOPCodeInfo64G1EqM"
            mOPCodeInfo64G1EqM[0] = new OpCode(DecodingAttribute.BxLockable, mArith64.ADD_EqIdM);
            mOPCodeInfo64G1EqM[1] = new OpCode(DecodingAttribute.BxLockable, mLogical64.OR_EqIdM);
            mOPCodeInfo64G1EqM[2] = new OpCode(DecodingAttribute.BxLockable, mArith64.ADC_EqIdM);
            mOPCodeInfo64G1EqM[3] = new OpCode(DecodingAttribute.BxLockable, mArith64.SBB_EqIdM);
            mOPCodeInfo64G1EqM[4] = new OpCode(DecodingAttribute.BxLockable, mLogical64.AND_EqIdM);
            mOPCodeInfo64G1EqM[5] = new OpCode(DecodingAttribute.BxLockable, mArith64.SUB_EqIdM);
            mOPCodeInfo64G1EqM[6] = new OpCode(DecodingAttribute.BxLockable, mLogical64.XOR_EqIdM);
            mOPCodeInfo64G1EqM[7] = new OpCode(DecodingAttribute.BXZero, mArith64.CMP_EqIdM);
            #endregion


            #region "mOPCodeInfo64G1EqR"
            mOPCodeInfo64G1EqR[0] = new OpCode(DecodingAttribute.BxLockable, mArith64.ADD_EqIdR);
            mOPCodeInfo64G1EqR[1] = new OpCode(DecodingAttribute.BxLockable, mLogical64.OR_EqIdR);
            mOPCodeInfo64G1EqR[2] = new OpCode(DecodingAttribute.BxLockable, mArith64.ADC_EqIdR);
            mOPCodeInfo64G1EqR[3] = new OpCode(DecodingAttribute.BxLockable, mArith64.SBB_EqIdR);
            mOPCodeInfo64G1EqR[4] = new OpCode(DecodingAttribute.BxLockable, mLogical64.AND_EqIdR);
            mOPCodeInfo64G1EqR[5] = new OpCode(DecodingAttribute.BxLockable, mArith64.SUB_EqIdR);
            mOPCodeInfo64G1EqR[6] = new OpCode(DecodingAttribute.BxLockable, mLogical64.XOR_EqIdR);
            mOPCodeInfo64G1EqR[7] = new OpCode(DecodingAttribute.BXZero, mArith64.CMP_EqIdR);
            #endregion




            #region "mOPCodeInfoG1AEwR"
            mOPCodeInfoG1AEwR[0] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInfoG1AEwR[1] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEwR[2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEwR[3] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEwR[4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEwR[5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEwR[6] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEwR[7] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion



            #region "mOPCodeInfoG1AEdR"
            mOPCodeInfoG1AEdR[0] = new OpCode(DecodingAttribute.BXZero, mStack32.POP_ERX);
            mOPCodeInfoG1AEdR[1] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdR[2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdR[3] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdR[4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdR[5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdR[6] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdR[7] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion


            #region "mOPCodeInfoG1AEdM"
            mOPCodeInfoG1AEdM[0] = new OpCode(DecodingAttribute.BXZero, mStack32.POP_EdM);
            mOPCodeInfoG1AEdM[1] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdM[2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdM[3] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdM[4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdM[5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdM[6] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfoG1AEdM[7] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion


            #region "mOPCodeInfo64G1AEqR"
            mOPCodeInfo64G1AEqR[0] = new OpCode(DecodingAttribute.BXZero, mStack64.POP_RRX);
            mOPCodeInfo64G1AEqR[1] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqR[2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqR[3] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqR[4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqR[5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqR[6] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqR[7] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion


            #region "mOPCodeInfo64G1AEqM"
            mOPCodeInfo64G1AEqM[0] = new OpCode(DecodingAttribute.BXZero, mStack64.POP_EqM);
            mOPCodeInfo64G1AEqM[1] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqM[2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqM[3] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqM[4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqM[5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqM[6] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInfo64G1AEqM[7] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion


            #region "mOPCodeInfoG2Eb"
            mOPCodeInfoG2Eb[0] = new OpCode(DecodingAttribute.BXZero, mShift8.ROL_Eb);
            mOPCodeInfoG2Eb[1] = new OpCode(DecodingAttribute.BXZero, mShift8.ROR_Eb);
            mOPCodeInfoG2Eb[2] = new OpCode(DecodingAttribute.BXZero, mShift8.RCL_Eb);
            mOPCodeInfoG2Eb[3] = new OpCode(DecodingAttribute.BXZero, mShift8.RCR_Eb);
            mOPCodeInfoG2Eb[4] = new OpCode(DecodingAttribute.BXZero, mShift8.SHL_Eb);
            mOPCodeInfoG2Eb[5] = new OpCode(DecodingAttribute.BXZero, mShift8.SHR_Eb);
            mOPCodeInfoG2Eb[6] = new OpCode(DecodingAttribute.BXZero, mShift8.SHL_Eb);
            mOPCodeInfoG2Eb[7] = new OpCode(DecodingAttribute.BXZero, mShift8.SAR_Eb);
            #endregion

            #region "mOPCodeInfoG2Ew"
            mOPCodeInfoG2Ew[0] = new OpCode(DecodingAttribute.BXZero, mShift16.ROL_Ew);
            mOPCodeInfoG2Ew[1] = new OpCode(DecodingAttribute.BXZero, mShift16.ROR_Ew);
            mOPCodeInfoG2Ew[2] = new OpCode(DecodingAttribute.BXZero, mShift16.RCL_Ew);
            mOPCodeInfoG2Ew[3] = new OpCode(DecodingAttribute.BXZero, mShift16.RCR_Ew);
            mOPCodeInfoG2Ew[4] = new OpCode(DecodingAttribute.BXZero, mShift16.SHL_Ew);
            mOPCodeInfoG2Ew[5] = new OpCode(DecodingAttribute.BXZero, mShift16.SHR_Ew);
            mOPCodeInfoG2Ew[6] = new OpCode(DecodingAttribute.BXZero, mShift16.SHL_Ew);
            mOPCodeInfoG2Ew[7] = new OpCode(DecodingAttribute.BXZero, mShift16.SAR_Ew);
            #endregion


            #region "mOPCodeInfoG2EdM"
            mOPCodeInfoG2EdM[0] = new OpCode(DecodingAttribute.BXZero, mShift32.ROL_EdM);
            mOPCodeInfoG2EdM[1] = new OpCode(DecodingAttribute.BXZero, mShift32.ROR_EdM);
            mOPCodeInfoG2EdM[2] = new OpCode(DecodingAttribute.BXZero, mShift32.RCL_EdM);
            mOPCodeInfoG2EdM[3] = new OpCode(DecodingAttribute.BXZero, mShift32.RCR_EdM);
            mOPCodeInfoG2EdM[4] = new OpCode(DecodingAttribute.BXZero, mShift32.SHL_EdM);
            mOPCodeInfoG2EdM[5] = new OpCode(DecodingAttribute.BXZero, mShift32.SHR_EdM);
            mOPCodeInfoG2EdM[6] = new OpCode(DecodingAttribute.BXZero, mShift32.SHL_EdM);
            mOPCodeInfoG2EdM[7] = new OpCode(DecodingAttribute.BXZero, mShift32.SAR_EdM);
            #endregion



            #region "mOPCodeInfoG2EdR"
            mOPCodeInfoG2EdR[0] = new OpCode(DecodingAttribute.BXZero, mShift32.ROL_EdR);
            mOPCodeInfoG2EdR[1] = new OpCode(DecodingAttribute.BXZero, mShift32.ROR_EdR);
            mOPCodeInfoG2EdR[2] = new OpCode(DecodingAttribute.BXZero, mShift32.RCL_EdR);
            mOPCodeInfoG2EdR[3] = new OpCode(DecodingAttribute.BXZero, mShift32.RCR_EdR);
            mOPCodeInfoG2EdR[4] = new OpCode(DecodingAttribute.BXZero, mShift32.SHL_EdR);
            mOPCodeInfoG2EdR[5] = new OpCode(DecodingAttribute.BXZero, mShift32.SHR_EdR);
            mOPCodeInfoG2EdR[6] = new OpCode(DecodingAttribute.BXZero, mShift32.SHL_EdR);
            mOPCodeInfoG2EdR[7] = new OpCode(DecodingAttribute.BXZero, mShift32.SAR_EdR);
            #endregion


            #region "mOPCodeInfo64G2EqM"
            mOPCodeInfo64G2EqM[0] = new OpCode(DecodingAttribute.BXZero, mShift64.ROL_EqM);
            mOPCodeInfo64G2EqM[1] = new OpCode(DecodingAttribute.BXZero, mShift64.ROR_EqM);
            mOPCodeInfo64G2EqM[2] = new OpCode(DecodingAttribute.BXZero, mShift64.RCL_EqM);
            mOPCodeInfo64G2EqM[3] = new OpCode(DecodingAttribute.BXZero, mShift64.RCR_EqM);
            mOPCodeInfo64G2EqM[4] = new OpCode(DecodingAttribute.BXZero, mShift64.SHL_EqM);
            mOPCodeInfo64G2EqM[5] = new OpCode(DecodingAttribute.BXZero, mShift64.SHR_EqM);
            mOPCodeInfo64G2EqM[6] = new OpCode(DecodingAttribute.BXZero, mShift64.SHL_EqM);
            mOPCodeInfo64G2EqM[7] = new OpCode(DecodingAttribute.BXZero, mShift64.SAR_EqM);
            #endregion


            #region "mOPCodeInfo64G2EqR"
            mOPCodeInfo64G2EqR[0] = new OpCode(DecodingAttribute.BXZero, mShift64.ROL_EqR);
            mOPCodeInfo64G2EqR[1] = new OpCode(DecodingAttribute.BXZero, mShift64.ROR_EqR);
            mOPCodeInfo64G2EqR[2] = new OpCode(DecodingAttribute.BXZero, mShift64.RCL_EqR);
            mOPCodeInfo64G2EqR[3] = new OpCode(DecodingAttribute.BXZero, mShift64.RCR_EqR);
            mOPCodeInfo64G2EqR[4] = new OpCode(DecodingAttribute.BXZero, mShift64.SHL_EqR);
            mOPCodeInfo64G2EqR[5] = new OpCode(DecodingAttribute.BXZero, mShift64.SHR_EqR);
            mOPCodeInfo64G2EqR[6] = new OpCode(DecodingAttribute.BXZero, mShift64.SHL_EqR);
            mOPCodeInfo64G2EqR[7] = new OpCode(DecodingAttribute.BXZero, mShift64.SAR_EqR);
            #endregion

            #region "Group 3"

            #endregion

            #region "BxOpcodeGroupSSE_0fe8"
            mOPCodeGroupSSE_0fe8[0] = new OpCode(DecodingAttribute.BXZero, mSSE.PSUBSB_VdqWdq);
            mOPCodeGroupSSE_0fe8[1] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeGroupSSE_0fe8[2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion

            #region "mOPCodeGroupSSE_0fe9"
            mOPCodeGroupSSE_0fe9[0] = new OpCode(DecodingAttribute.BXZero, mSSE.PSUBSW_VdqWdq);
            mOPCodeGroupSSE_0fe9[0] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeGroupSSE_0fe9[0] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            #endregion


            #region "mOPCodeInstruction32R"
            mOPCodeInstruction32R[0] = new OpCode(DecodingAttribute.BxArithDstRM, mArith8.ADD_GbEbR);
            mOPCodeInstruction32R[1] = new OpCode(DecodingAttribute.BxArithDstRM, mArith16.ADD_GwEwR);
            mOPCodeInstruction32R[2] = new OpCode(DecodingAttribute.BXZero, mArith8.ADD_GbEbR);
            mOPCodeInstruction32R[3] = new OpCode(DecodingAttribute.BXZero, mArith16.ADD_GwEwR);
            mOPCodeInstruction32R[4] = new OpCode(DecodingAttribute.BxImmediate_Ib, mArith8.ADD_ALIb);
            mOPCodeInstruction32R[5] = new OpCode(DecodingAttribute.BxImmediate_Iw, mArith16.ADC_AXIw);

            mOPCodeInstruction32R[6] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH16_ES);
            mOPCodeInstruction32R[7] = new OpCode(DecodingAttribute.BXZero, mStack16.POP16_ES);

            mOPCodeInstruction32R[8] = new OpCode(DecodingAttribute.BxArithDstRM, mLogical8.OR_GbEbR);
            mOPCodeInstruction32R[9] = new OpCode(DecodingAttribute.BxArithDstRM, mLogical16.OR_GwEwR);
            mOPCodeInstruction32R[0x0a] = new OpCode(DecodingAttribute.BXZero, mLogical8.OR_GbEbR);
            mOPCodeInstruction32R[0x0b] = new OpCode(DecodingAttribute.BXZero, mLogical16.OR_GwEwR);

            mOPCodeInstruction32R[0x0c] = new OpCode(DecodingAttribute.BxImmediate_Ib, mLogical8.OR_ALIb);
            mOPCodeInstruction32R[0x0d] = new OpCode(DecodingAttribute.BxImmediate_Iw, mLogical16.OR_AXIw);
            mOPCodeInstruction32R[0x0e] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH16_CS);
            mOPCodeInstruction32R[0x0f] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);

            mOPCodeInstruction32R[0x10] = new OpCode(DecodingAttribute.BxArithDstRM, mArith8.ADC_GbEbR);
            mOPCodeInstruction32R[0x11] = new OpCode(DecodingAttribute.BxArithDstRM, mArith16.ADC_GwEwR);
            mOPCodeInstruction32R[0x12] = new OpCode(DecodingAttribute.BXZero, mArith8.ADC_GbEbR);
            mOPCodeInstruction32R[0x13] = new OpCode(DecodingAttribute.BXZero, mArith16.ADC_GwEwR);
            mOPCodeInstruction32R[0x14] = new OpCode(DecodingAttribute.BxImmediate_Ib, mArith8.ADC_ALIb);
            mOPCodeInstruction32R[0x15] = new OpCode(DecodingAttribute.BxImmediate_Iw, mArith16.ADC_AXIw);

            mOPCodeInstruction32R[0x16] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH16_SS);
            mOPCodeInstruction32R[0x17] = new OpCode(DecodingAttribute.BXZero, mStack16.POP16_SS);

            mOPCodeInstruction32R[0x18] = new OpCode(DecodingAttribute.BxArithDstRM, mArith8.SBB_GbEbR);
            mOPCodeInstruction32R[0x19] = new OpCode(DecodingAttribute.BxArithDstRM, mArith16.SBB_GwEwR);
            mOPCodeInstruction32R[0x1a] = new OpCode(DecodingAttribute.BXZero, mArith8.SBB_GbEbR);
            mOPCodeInstruction32R[0x1b] = new OpCode(DecodingAttribute.BXZero, mArith16.SBB_GwEwR);
            mOPCodeInstruction32R[0x1c] = new OpCode(DecodingAttribute.BxImmediate_Ib, mArith8.SBB_ALIb);
            mOPCodeInstruction32R[0x1d] = new OpCode(DecodingAttribute.BxImmediate_Iw, mArith16.SBB_AXIw);

            mOPCodeInstruction32R[0x1e] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH16_DS);
            mOPCodeInstruction32R[0x1f] = new OpCode(DecodingAttribute.BXZero, mStack16.POP16_DS);

            mOPCodeInstruction32R[0x20] = new OpCode(DecodingAttribute.BxArithDstRM, mLogical8.AND_GbEbR);
            mOPCodeInstruction32R[0x21] = new OpCode(DecodingAttribute.BxArithDstRM, mLogical16.AND_EwGwM);
            mOPCodeInstruction32R[0x22] = new OpCode(DecodingAttribute.BXZero, mLogical8.AND_GbEbR);
            mOPCodeInstruction32R[0x23] = new OpCode(DecodingAttribute.BXZero, mLogical16.AND_EwGwM);
            mOPCodeInstruction32R[0x24] = new OpCode(DecodingAttribute.BxImmediate_Ib, mLogical8.AND_ALIb);
            mOPCodeInstruction32R[0x25] = new OpCode(DecodingAttribute.BxImmediate_Iw, mLogical16.AND_AXIw);
            mOPCodeInstruction32R[0x26] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // ES:
            mOPCodeInstruction32R[0x27] = new OpCode(DecodingAttribute.BXZero, mBCD.DAA);

            mOPCodeInstruction32R[0x28] = new OpCode(DecodingAttribute.BxArithDstRM, mArith8.SUB_GbEbR);
            mOPCodeInstruction32R[0x29] = new OpCode(DecodingAttribute.BxArithDstRM, mArith16.SUB_GwEwR);
            mOPCodeInstruction32R[0x2a] = new OpCode(DecodingAttribute.BXZero, mArith8.SUB_GbEbR);
            mOPCodeInstruction32R[0x2b] = new OpCode(DecodingAttribute.BXZero, mArith16.SUB_GwEwR);
            mOPCodeInstruction32R[0x2c] = new OpCode(DecodingAttribute.BxImmediate_Ib, mArith8.SUB_ALIb);
            mOPCodeInstruction32R[0x2d] = new OpCode(DecodingAttribute.BxImmediate_Iw, mArith16.SUB_AXIw);
            mOPCodeInstruction32R[0x2e] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // CS:
            mOPCodeInstruction32R[0x2f] = new OpCode(DecodingAttribute.BXZero, mBCD.DAS);

            mOPCodeInstruction32R[0x30] = new OpCode(DecodingAttribute.BxArithDstRM, mLogical8.XOR_GbEbR);
            mOPCodeInstruction32R[0x31] = new OpCode(DecodingAttribute.BxArithDstRM, mLogical16.XOR_GwEwR);
            mOPCodeInstruction32R[0x32] = new OpCode(DecodingAttribute.BXZero, mLogical8.XOR_GbEbR);
            mOPCodeInstruction32R[0x33] = new OpCode(DecodingAttribute.BXZero, mLogical16.XOR_GwEwR);
            mOPCodeInstruction32R[0x34] = new OpCode(DecodingAttribute.BxImmediate_Ib, mLogical8.XOR_ALIb);
            mOPCodeInstruction32R[0x35] = new OpCode(DecodingAttribute.BxImmediate_Iw, mLogical16.XOR_AXIw);
            mOPCodeInstruction32R[0x36] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // SS:
            mOPCodeInstruction32R[0x37] = new OpCode(DecodingAttribute.BXZero, mBCD.AAA);

            mOPCodeInstruction32R[0x38] = new OpCode(DecodingAttribute.BxArithDstRM, mArith8.CMP_GbEbR);
            mOPCodeInstruction32R[0x39] = new OpCode(DecodingAttribute.BxArithDstRM, mArith16.CMP_GwEwR);
            mOPCodeInstruction32R[0x3a] = new OpCode(DecodingAttribute.BXZero, mArith8.CMP_GbEbR);
            mOPCodeInstruction32R[0x3b] = new OpCode(DecodingAttribute.BXZero, mArith16.CMP_GwEwR);
            mOPCodeInstruction32R[0x3c] = new OpCode(DecodingAttribute.BxImmediate_Ib, mArith8.CMP_ALIb);
            mOPCodeInstruction32R[0x3d] = new OpCode(DecodingAttribute.BxImmediate_Iw, mArith16.CMP_AXIw);
            mOPCodeInstruction32R[0x3e] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // DS:
            mOPCodeInstruction32R[0x3f] = new OpCode(DecodingAttribute.BXZero, mBCD.AAS);

            mOPCodeInstruction32R[0x40] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x41] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x42] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x43] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x44] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x45] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x46] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x47] = new OpCode(DecodingAttribute.BXZero, mArith16.INC_RX);
            mOPCodeInstruction32R[0x48] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x49] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x4a] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x4b] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x4c] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x4d] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x4e] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);
            mOPCodeInstruction32R[0x4f] = new OpCode(DecodingAttribute.BXZero, mArith16.DEC_RX);

            mOPCodeInstruction32R[0x50] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x51] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x52] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x53] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x54] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x55] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x56] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x57] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSH_RX);
            mOPCodeInstruction32R[0x58] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x59] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x5a] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x5b] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x5c] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x5d] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x5e] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);
            mOPCodeInstruction32R[0x5f] = new OpCode(DecodingAttribute.BXZero, mStack16.POP_RX);

            mOPCodeInstruction32R[0x60] = new OpCode(DecodingAttribute.BXZero, mStack16.PUSHAD16);
            mOPCodeInstruction32R[0x61] = new OpCode(DecodingAttribute.BXZero, mStack16.POPAD16);
            mOPCodeInstruction32R[0x62] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);

            mOPCodeInstruction32R[0x63] = new OpCode(DecodingAttribute.BXZero, mProtected_Ctrl.ARPL_EwGw);
            mOPCodeInstruction32R[0x64] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // FS:
            mOPCodeInstruction32R[0x65] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // GS:
            mOPCodeInstruction32R[0x66] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // OS:
            mOPCodeInstruction32R[0x67] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);  // AS:

            mOPCodeInstruction32R[0x68] = new OpCode(DecodingAttribute.BxImmediate_Iw, mStack16.PUSH_Iw);
            mOPCodeInstruction32R[0x69] = new OpCode(DecodingAttribute.BxImmediate_Iw, mMult16.IMUL_GwEwIwR);
            mOPCodeInstruction32R[0x6a] = new OpCode(DecodingAttribute.BxImmediate_Ib, mStack16.PUSH_Iw);
            mOPCodeInstruction32R[0x6b] = new OpCode(DecodingAttribute.BxImmediate_Ib, mMult16.IMUL_GwEwIwR);
            mOPCodeInstruction32R[0x6c] = new OpCode(DecodingAttribute.BXZero, mIO.REP_INSB_YbDX);
            mOPCodeInstruction32R[0x6d] = new OpCode(DecodingAttribute.BXZero, mIO.REP_INSW_YwDX);
            mOPCodeInstruction32R[0x6e] = new OpCode(DecodingAttribute.BXZero, mIO.REP_OUTSB_DXXb);
            mOPCodeInstruction32R[0x6f] = new OpCode(DecodingAttribute.BXZero, mIO.REP_OUTSW_DXXw);

            mOPCodeInstruction32R[0x70] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JO_Jw);
            mOPCodeInstruction32R[0x71] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNO_Jw);
            mOPCodeInstruction32R[0x72] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JB_Jw);
            mOPCodeInstruction32R[0x73] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNB_Jw);
            mOPCodeInstruction32R[0x74] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JZ_Jw);
            mOPCodeInstruction32R[0x75] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNZ_Jw);
            mOPCodeInstruction32R[0x76] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JBE_Jw);
            mOPCodeInstruction32R[0x77] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNBE_Jw);
            mOPCodeInstruction32R[0x78] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JS_Jw);
            mOPCodeInstruction32R[0x79] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNS_Jw);
            mOPCodeInstruction32R[0x7a] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JP_Jw);
            mOPCodeInstruction32R[0x7b] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNP_Jw);
            mOPCodeInstruction32R[0x7c] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JL_Jw);
            mOPCodeInstruction32R[0x7d] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNL_Jw);
            mOPCodeInstruction32R[0x7e] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JLE_Jw);
            mOPCodeInstruction32R[0x7f] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceJCC, mCtrl_Xref16.JNLE_Jw);

            mOPCodeInstruction32R[0x80] = new OpCode(DecodingAttribute.BxGroup1 | DecodingAttribute.BxImmediate_Ib, mError.Unknown, mOPCodeInfoG1EbIbR);
            mOPCodeInstruction32R[0x81] = new OpCode(DecodingAttribute.BxGroup1 | DecodingAttribute.BxImmediate_Iw, mError.Unknown, mOPCodeInfoG1EwR);
            mOPCodeInstruction32R[0x82] = new OpCode(DecodingAttribute.BxGroup1 | DecodingAttribute.BxImmediate_Ib, mError.Unknown, mOPCodeInfoG1EbIbR);
            mOPCodeInstruction32R[0x83] = new OpCode(DecodingAttribute.BxGroup1 | DecodingAttribute.BxImmediate_Ib_SE, mError.Unknown, mOPCodeInfoG1EwR);
            mOPCodeInstruction32R[0x84] = new OpCode(DecodingAttribute.BXZero, mLogical8.TEST_EbGbR);
            mOPCodeInstruction32R[0x85] = new OpCode(DecodingAttribute.BXZero, mLogical16.TEST_EwGwR);
            mOPCodeInstruction32R[0x86] = new OpCode(DecodingAttribute.BXZero, mLogical8.TEST_EbGbR);
            mOPCodeInstruction32R[0x87] = new OpCode(DecodingAttribute.BXZero, mLogical16.TEST_EwGwR);
            mOPCodeInstruction32R[0x88] = new OpCode(DecodingAttribute.BxArithDstRM, mData_Xref8.MOV_GbEbR);
            mOPCodeInstruction32R[0x89] = new OpCode(DecodingAttribute.BxArithDstRM, mData_Xref16.MOV_GwEwR);
            mOPCodeInstruction32R[0x8a] = new OpCode(DecodingAttribute.BXZero, mData_Xref8.MOV_GbEbR);
            mOPCodeInstruction32R[0x8b] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.MOV_GwEwR);
            mOPCodeInstruction32R[0x8c] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.MOV_EwSwR);
            mOPCodeInstruction32R[0x8d] = new OpCode(DecodingAttribute.BXZero, mError.Unknown); // LEA
            mOPCodeInstruction32R[0x8e] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.MOV_SwEw);
            mOPCodeInstruction32R[0x8f] = new OpCode(DecodingAttribute.BxGroup1A, mError.Unknown, mOPCodeInfoG1AEwR);
            mOPCodeInstruction32R[0x90] = new OpCode(DecodingAttribute.BxPrefixSSE, mProc_Ctrl.NOP, mOPCodeGroupSSE_PAUSE);

            mOPCodeInstruction32R[0x91] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x92] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x93] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x94] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x95] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x96] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x97] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32R[0x98] = new OpCode(DecodingAttribute.BXZero, mArith16.CBW);
            mOPCodeInstruction32R[0x99] = new OpCode(DecodingAttribute.BXZero, mArith16.CWD);
            mOPCodeInstruction32R[0x9a] = new OpCode(DecodingAttribute.BxImmediate_IwIw | DecodingAttribute.BxTraceEnd, mCtrl_Xref16.CALL16_Ap);
            mOPCodeInstruction32R[0x9b] = new OpCode(DecodingAttribute.BXZero, mFPU_Emu.FWAIT);
            mOPCodeInstruction32R[0x9c] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.PUSHF_Fw);
            mOPCodeInstruction32R[0x9d] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.POPF_Fw);
            mOPCodeInstruction32R[0x9e] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.SAHF);
            mOPCodeInstruction32R[0x9f] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.LAHF);

            mOPCodeInstruction32R[0xa0] = new OpCode(DecodingAttribute.BxImmediate_O, mData_Xref8.MOV_ALOd);
            mOPCodeInstruction32R[0xa1] = new OpCode(DecodingAttribute.BxImmediate_O, mData_Xref16.MOV_AXOd);
            mOPCodeInstruction32R[0xa2] = new OpCode(DecodingAttribute.BxImmediate_O, mData_Xref8.MOV_OdAL);
            mOPCodeInstruction32R[0xa3] = new OpCode(DecodingAttribute.BxImmediate_O, mData_Xref16.MOV_OdAX);
            mOPCodeInstruction32R[0xa4] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_MOVSD_XdYd);
            mOPCodeInstruction32R[0xa5] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_MOVSW_XwYw);
            mOPCodeInstruction32R[0xa6] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_CMPSB_XbYb);
            mOPCodeInstruction32R[0xa7] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_CMPSW_XwYw);
            mOPCodeInstruction32R[0xa8] = new OpCode(DecodingAttribute.BxImmediate_Ib, mLogical8.TEST_ALIb);
            mOPCodeInstruction32R[0xa9] = new OpCode(DecodingAttribute.BxImmediate_Iw, mLogical16.AND_AXIw);
            mOPCodeInstruction32R[0xaa] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_STOSB_YbAL);
            mOPCodeInstruction32R[0xab] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_STOSW_YwAX);
            mOPCodeInstruction32R[0xac] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_LODSB_ALXb);
            mOPCodeInstruction32R[0xad] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_LODSW_AXXw);
            mOPCodeInstruction32R[0xae] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_SCASB_ALXb);
            mOPCodeInstruction32R[0xaf] = new OpCode(DecodingAttribute.BXZero, mStringInst.REP_SCASW_AXXw);

            mOPCodeInstruction32R[0xb0] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RLIb);
            mOPCodeInstruction32R[0xb1] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RLIb);
            mOPCodeInstruction32R[0xb2] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RLIb);
            mOPCodeInstruction32R[0xb3] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RLIb);
            mOPCodeInstruction32R[0xb4] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RHIb);
            mOPCodeInstruction32R[0xb5] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RHIb);
            mOPCodeInstruction32R[0xb6] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RHIb);
            mOPCodeInstruction32R[0xb7] = new OpCode(DecodingAttribute.BxImmediate_Ib, mData_Xref8.MOV_RHIb);
            mOPCodeInstruction32R[0xb8] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xb9] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xba] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xbb] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xbc] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xbd] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xbe] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);
            mOPCodeInstruction32R[0xbf] = new OpCode(DecodingAttribute.BxImmediate_Iw, mData_Xref16.MOV_RXIw);

            mOPCodeInstruction32R[0xc0] = new OpCode(DecodingAttribute.BxGroup2 | DecodingAttribute.BxImmediate_Ib, mError.Unknown, mOPCodeInfoG2Eb);
            mOPCodeInstruction32R[0xc1] = new OpCode(DecodingAttribute.BxGroup2 | DecodingAttribute.BxImmediate_Ib, mError.Unknown, mOPCodeInfoG2Ew);
            mOPCodeInstruction32R[0xc2] = new OpCode(DecodingAttribute.BxTraceEnd, mCtrl_Xref16.RETnear16_Iw);
            mOPCodeInstruction32R[0xc3] = new OpCode(DecodingAttribute.BxTraceEnd, mCtrl_Xref16.RETnear16);
            mOPCodeInstruction32R[0xc4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInstruction32R[0xc5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);


            mOPCodeInstruction32R[0xe4] = new OpCode(DecodingAttribute.BxImmediate_Ib, mIO.IN_ALIb);
            mOPCodeInstruction32R[0xe5] = new OpCode(DecodingAttribute.BxImmediate_Ib, mIO.IN_AXIb);
            mOPCodeInstruction32R[0xe6] = new OpCode(DecodingAttribute.BxImmediate_Ib, mIO.OUT_IbAL);
            mOPCodeInstruction32R[0xe7] = new OpCode(DecodingAttribute.BxImmediate_Ib, mIO.OUT_IbAX);

            mOPCodeInstruction32R[0xe8] = new OpCode(DecodingAttribute.BxImmediate_BrOff16 | DecodingAttribute.BxTraceEnd, mCtrl_Xref16.CALL_Jw);
            mOPCodeInstruction32R[0xe9] = new OpCode(DecodingAttribute.BxImmediate_BrOff16 | DecodingAttribute.BxTraceEnd, mCtrl_Xref16.JMP_Jw);

            mOPCodeInstruction32R[0xea] = new OpCode(DecodingAttribute.BxImmediate_IwIw | DecodingAttribute.BxTraceEnd, mCtrl_Xref32.JMP_Ap);
            mOPCodeInstruction32R[0xeb] = new OpCode(DecodingAttribute.BxImmediate_BrOff8 | DecodingAttribute.BxTraceEnd, mCtrl_Xref16.JMP_Jw);
            mOPCodeInstruction32R[0xec] = new OpCode(DecodingAttribute.BXZero, mIO.IN_ALDX);
            mOPCodeInstruction32R[0xed] = new OpCode(DecodingAttribute.BXZero, mIO.IN_AXDX);
            mOPCodeInstruction32R[0xee] = new OpCode(DecodingAttribute.BXZero, mIO.OUT_DXAL);
            mOPCodeInstruction32R[0xef] = new OpCode(DecodingAttribute.BXZero, mIO.OUT_DXAX);

            mOPCodeInstruction32R[0xf0] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);// LOCK
            mOPCodeInstruction32R[0xf1] = new OpCode(DecodingAttribute.BxTraceEnd, mSoftInt.INT1);
            mOPCodeInstruction32R[0xf2] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);// REPNE/REPNZ
            mOPCodeInstruction32R[0xf3] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);// REP, REPE/REPZ
            mOPCodeInstruction32R[0xf4] = new OpCode(DecodingAttribute.BxTraceEnd, mProc_Ctrl.HLT);

            mOPCodeInstruction32R[0xf8] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.CLC);
            mOPCodeInstruction32R[0xf9] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.STC);
            mOPCodeInstruction32R[0xfa] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.CLI);
            mOPCodeInstruction32R[0xfb] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.STI);
            mOPCodeInstruction32R[0xfc] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.CLD);
            mOPCodeInstruction32R[0xfd] = new OpCode(DecodingAttribute.BXZero, mFlag_Ctrl.STD);

            #endregion

            
            #region "mOPCodeInstruction32M"
            mOPCodeInstruction32M[0x88] = new OpCode(DecodingAttribute.BXZero, mData_Xref8.MOV_EbGbM);
            mOPCodeInstruction32M[0x89] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.MOV_EwGwM);
            mOPCodeInstruction32M[0x8a] = new OpCode(DecodingAttribute.BXZero, mData_Xref8.MOV_GbEbM);
            mOPCodeInstruction32M[0x8b] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.MOV_GwEwM);
            mOPCodeInstruction32M[0x8c] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.MOV_EwSwM);
            mOPCodeInstruction32M[0x8D] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.LEA_GwM);

            mOPCodeInstruction32M[0x91] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32M[0x92] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32M[0x93] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32M[0x94] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32M[0x95] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32M[0x96] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);
            mOPCodeInstruction32M[0x97] = new OpCode(DecodingAttribute.BXZero, mData_Xref16.XCHG_RXAX);


            mOPCodeInstruction32M[0xc2] = new OpCode(DecodingAttribute.BxTraceEnd, mCtrl_Xref32.RETnear32_Iw);
            mOPCodeInstruction32M[0xc3] = new OpCode(DecodingAttribute.BxTraceEnd, mCtrl_Xref32.RETnear32);
            mOPCodeInstruction32M[0xc4] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);
            mOPCodeInstruction32M[0xc5] = new OpCode(DecodingAttribute.BXZero, mError.Unknown);

            #endregion
        }


        public Instruction FetchDecode32(UInt64 FetchPtr, UInt64 RemainingInPage)
        {
            Instruction oInstruction = mCPU.ICache.Pool[mCPU.ICache.PIndex]; // new Instruction();
            oInstruction.InstructionAddress = FetchPtr;
            OpCode OpcodeInfoPtr=null;
            bool bUseOPCodeInstruction32R = true;
            byte SSE_prefix = (byte)Enum_SSE.SSE_PREFIX_NONE;

            // remain must be at least 1
            UInt64 uRemain = (RemainingInPage < 15) ? RemainingInPage : 15;
            byte b1, b2;
            uint ia_opcode = 0;
            uint uImm_Mode, uOffset, ilen = 0;
            Enum_Resolve Resolve = Enum_Resolve.BX_RESOLVE_NONE;
            byte uRM = 0, uMod = 0, uNNN = 0;
            bool bLock = false;
            byte OS32 = (byte)(mCPU.CS.Cache_u_Segment_D_B);
            byte Is32 = OS32;
            byte Seg = (byte)Enum_SegmentReg.REG_DS;
            byte Seg_Override = (byte)Enum_SegmentReg.BX_SEG_REG_NULL;
            oInstruction.Init(OS32, OS32, 0, 0);

            uOffset = (uint)OS32 << 9; // * 512
          
        fetch_b1: // ugly code
            b1 = mCPU.MainMemory.ReadByte(FetchPtr);
            ilen++;
            FetchPtr++;
            switch (b1)
            {
                case 0x0f: // 2-byte escape
                    if (ilen < uRemain)
                    {
                        ilen++;
                        b1 = (byte)(0x100 | mCPU.MainMemory.ReadByte(FetchPtr));
                        FetchPtr++;
                    }
                    break;

                case 0x66: //OpSize
                    OS32 = (byte)(0xff - Is32); // convert
                    uOffset = (uint)OS32 << 9; // * 512
                    if (SSE_prefix == (byte)Enum_SSE.SSE_PREFIX_NONE)
                        SSE_prefix = (byte)Enum_SSE.SSE_PREFIX_66;
                    oInstruction.SetOS32B(OS32);
                    if (ilen < uRemain)
                        goto fetch_b1;
                    return oInstruction;

                case 0x67: // AddrSize
                    oInstruction.SetAS32B((byte)(0xff - Is32));
                    if (ilen < uRemain)
                        goto fetch_b1;
                    return oInstruction;

                case 0xf2: // REPNE/REPNZ
                case 0xf3: // REP/REPE/REPZ
                    SSE_prefix = (byte)(b1 & 0x03);
                    oInstruction.SetRepUsed(SSE_prefix);
                    if (ilen < uRemain)
                        goto fetch_b1;
                    return oInstruction;
                case 0x26: // ES:
                case 0x2e: // CS:
                case 0x36: // SS:
                case 0x3e: // DS:
                    Seg_Override = (byte)((b1 >> 3) & 3);
                    if (ilen < uRemain)
                        goto fetch_b1;
                    return oInstruction;
                case 0x64: // FS:
                case 0x65: // GS:
                    Seg_Override = (byte)(b1 & 0x0f);
                    if (ilen < uRemain)
                        goto fetch_b1;
                    return oInstruction;
                case 0xf0: // LOCK:
                    bLock = true;
                    if (ilen < uRemain)
                        goto fetch_b1;
                    return oInstruction;
                default:
                    break;
            }

            DecodingAttribute attr = default;
            try
            {
                attr = mCPU.InstructionExecution.OPCodeInstruction32R[b1 + uOffset].Attribute;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] " + ex.Message);
            }

            if (mCPU.InstructionExecution.BxOpcodeHasModrm32[b1] == 1)
            {
                // opcode requires modrm byte
                if (ilen > uRemain) return oInstruction;

                ++ilen;
                b2 = (byte)(mCPU.MainMemory.ReadByte(FetchPtr));
                FetchPtr++;

                // Parse mod-nnn-rm and related bytes
                uMod = (byte)(b2 & 0xc0); // leave unshifted
                uNNN = (byte)((b2 >> 3) & 0x7);
                uRM = (byte)(b2 & 0x7);


                // MOVs with CRx and DRx always use register ops and ignore the mod field.
                if ((b1 & ~3) == 0x120)
                    uMod = 0xc0;

                oInstruction.SetModRM(b2);
                oInstruction.SetNnn(uNNN);

                if (uMod == 0xc0)
                {
                    oInstruction.AssertModC0();
                    oInstruction.SetRM(uRM);
                    goto modrm_done;
                }

                oInstruction.SetRM((byte)Enum_GeneralReg.BX_TMP_REGISTER);
                oInstruction.SetSibBase(uRM);      // initialize with rm to use BxResolve32Base
                oInstruction.SetSibIndex((byte)(Enum_GeneralReg.BX_NIL_REGISTER));
                // initialize displ32 with zero to include cases with no diplacement
                oInstruction.modRMForm_Displ32u = 0;

                if (oInstruction.AS32L() != 0)
                {
                    Resolve = Enum_Resolve.BX_RESOLVE32_BASE;
                    //oInstruction.ResolveModrm = mCPU.Resolver.BxResolve32Base(oInstruction);
                    if (uRM != 4)
                    {
                        if (uMod == 0x00)
                        {
                            if (uRM == 5)
                            {
                                oInstruction.SetSibBase((byte)Enum_GeneralReg.BX_NIL_REGISTER);
                                if ((ilen + 3) < uRemain)
                                {
                                    oInstruction.modRMForm_Displ32u = mCPU.MainMemory.ReadDWord(FetchPtr);
                                    FetchPtr += 4;
                                    ilen += 4;
                                }
                                else
                                {
                                    return oInstruction;
                                }
                            }
                            // mod==00b, rm!=4, rm!=5
                            goto modrm_done;
                        }
                        //Seg_Override= InstructionExecution.
                        ///////////Continue from Here 
                        Seg = Instruction.sreg_mod01or10_rm32[uRM];
                        if (uMod == 0x40)
                        {
                            if (ilen < uRemain)
                            {
                                oInstruction.modRMForm_Displ32u = mCPU.MainMemory.ReadByte(FetchPtr);
                                FetchPtr++;
                                ilen++;
                                goto modrm_done;
                            }
                            else
                            {
                                return oInstruction;
                            }
                        }
                        if ((ilen + 3) < uRemain)
                        {
                            oInstruction.modRMForm_Displ32u = mCPU.MainMemory.ReadDWord(FetchPtr);
                            FetchPtr += 4;
                            ilen += 4;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        goto modrm_done;
                    }

                    else // mod!=11b, rm==4, s-i-b byte follows
                    {
                        byte bSIB, bBase, bIndex, bScale;
                        if (ilen < uRemain)
                        {
                            bSIB = mCPU.MainMemory.ReadByte(FetchPtr);
                            FetchPtr++;
                            ilen++;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        bBase = (byte)(bSIB & 0x7); bSIB >>= 3;
                        bIndex = (byte)(bSIB & 0x7); bSIB >>= 3;
                        bScale = bSIB;
                        oInstruction.SetSibScale(bScale);
                        oInstruction.SetSibBase(bScale);
                        if (bIndex != 4)
                        {
                            Resolve = Enum_Resolve.BX_RESOLVE32_BASE_INDEX;
                            oInstruction.ResolveModrm = mCPU.Resolver.BxResolve32BaseIndex;
                            oInstruction.SetSibIndex(bIndex);
                        }
                        if (uMod == 0x00)
                        { // mod==00b, rm==4
                            Seg = Instruction.sreg_mod0_base32[bBase];
                            if (bBase == 0x05)
                            {
                                oInstruction.SetSibBase((byte)Enum_GeneralReg.BX_NIL_REGISTER);
                                if ((ilen + 3) < uRemain)
                                {
                                    oInstruction.modRMForm_Displ32u = mCPU.MainMemory.ReadDWord(FetchPtr);
                                    FetchPtr += 4;
                                    ilen += 4;
                                }
                                else
                                {
                                    return oInstruction;
                                }
                            }
                            goto modrm_done;
                        }

                        Seg = Instruction.sreg_mod1or2_base32[bBase];
                        if (uMod == 0x40)
                        { // mod==01b, rm==4
                            if (ilen < uRemain)
                            {
                                // 8 sign extended to 32
                                oInstruction.modRMForm_Displ32u = mCPU.MainMemory.ReadByte(FetchPtr);
                                FetchPtr++;
                                ilen++;
                                goto modrm_done;
                            }
                            else
                            {
                                return oInstruction;
                            }
                            goto modrm_done;
                        }
                        // (mod == 0x80),   mod==10b, rm==4
                        if ((ilen + 3) < uRemain)
                        {
                            oInstruction.modRMForm_Displ32u = mCPU.MainMemory.ReadDWord(FetchPtr);
                            FetchPtr += 4;
                            ilen += 4;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        goto modrm_done;
                    }
                }
                else
                {
                    // 16-bit addressing modes, mod==11b handled above
                    Resolve = Enum_Resolve.BX_RESOLVE16;
                    //oInstruction.ResolveModrm = mCPU.Resolver.BxResolve16BaseIndex;
                    oInstruction.SetSibBase(Instruction.Resolve16BaseReg[uRM]);
                    oInstruction.SetSibIndex(Instruction.Resolve16IndexReg[uRM]);
                    if (uMod == 0x00)
                    { // mod == 00b
                        Seg = Instruction.sreg_mod00_rm16[uRM];
                        if (uRM == 0x06)
                        {
                            oInstruction.SetSibBase((byte)Enum_GeneralReg.BX_NIL_REGISTER);
                            if ((ilen + 1) < uRemain)
                            {
                                oInstruction.modRMForm_Displ16u = mCPU.MainMemory.ReadWord(FetchPtr);
                                FetchPtr += 2;
                                ilen += 2;
                                goto modrm_done;
                            }
                            else return oInstruction;
                        }
                        goto modrm_done;
                    }

                    Seg = Instruction.sreg_mod01or10_rm16[uRM];
                    if (uMod == 0x40)
                    { // mod == 01b
                        if (ilen < uRemain)
                        {
                            // 8 sign extended to 16
                            oInstruction.modRMForm_Displ16u = mCPU.MainMemory.ReadByte(FetchPtr);
                            FetchPtr++;
                            ilen++;
                            goto modrm_done;
                        }
                        else return oInstruction;
                    }
                    // (mod == 0x80)      mod == 10b
                    if ((ilen + 1) < uRemain)
                    {
                        oInstruction.modRMForm_Displ16u = mCPU.MainMemory.ReadWord(FetchPtr);
                        FetchPtr += 2;
                        ilen += 2;
                    }
                    else return oInstruction;
                }

            modrm_done:
                // Resolve ExecutePtr and additional opcode Attr

                if (uMod == 0xc0)
                {
                    OpcodeInfoPtr = mOPCodeInstruction32R[b1 + uOffset];
                    //check(ing) "if (OpcodeInfoPtr != null)" added by me
                    if (OpcodeInfoPtr != null)
                    {
                        attr = mOPCodeInstruction32R[b1 + uOffset].Attribute;
                    }
                }
                else
                {
                    bUseOPCodeInstruction32R = false;
                    OpcodeInfoPtr = mOPCodeInstruction32M[b1 + uOffset];

                    //check(ing) "if (OpcodeInfoPtr != null)" added by me
                    if (OpcodeInfoPtr != null)
                    {
                        attr = mOPCodeInstruction32M[b1 + uOffset].Attribute;
                    }
                    else
                    {
                        Debug.WriteLine("[i] OpcodeInfoPtr = null !");
                    }
                }

                while ((attr & DecodingAttribute.BxGroupX) != 0)
                {
                    uint group = (UInt32)(attr & DecodingAttribute.BxGroupX);
                    attr &= ~DecodingAttribute.BxGroupX;
                    switch (group)
                    {
                        case (uint)DecodingAttribute.BxGroupN:
                            OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[uNNN];
                            break;
                        case (uint)DecodingAttribute.BxRMGroup:
                            OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[uRM];
                            break;



                        case (uint)DecodingAttribute.BxOSizeGrp:
                            OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[OS32];
                            break;
                        case (uint)DecodingAttribute.BxPrefixSSE:
                            /* For SSE opcodes look into another table
                                       with the opcode prefixes (NONE, 0x66, 0xF2, 0xF3) */
                            if (SSE_prefix == (byte)Enum_SSE.SSE_PREFIX_NONE)
                            {
                                OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[SSE_prefix - 1];
                                break;
                            }
                            continue;
                        case (uint)DecodingAttribute.BxPrefixSSE66:
                            /* For SSE opcodes with prefix 66 only */
                            if (SSE_prefix != (byte)Enum_SSE.SSE_PREFIX_66)
                            {
                                OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[0]; // BX_IA_ERROR
                            }
                            continue;
                        case (uint)DecodingAttribute.BxFPEscape:
                            OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[b2 & 0x3f];
                            break;
                        default:
                            throw new Exception("fetchdecode: Unknown opcode group");
                    }

                    /* get additional attributes from group table */
                    attr |= OpcodeInfoPtr.Attribute;
                }

                //ia_opcode = OpcodeInfoPtr->IA;
                
            }
            else
            {
                // Opcode does not require a MODRM byte.
                // Note that a 2-byte opcode (0F XX) will jump to before
                // the if() above after fetching the 2nd byte, so this path is
                // taken in all cases if a modrm byte is NOT required.

                OpcodeInfoPtr = mOPCodeInstruction32R[b1 + uOffset];

                uint group = (uint)(attr & DecodingAttribute.BxGroupX);
                ///BX_ASSERT(group == DecodingAttribute.BxPrefixSSE || group == 0);
                if ((group == (uint)DecodingAttribute.BxPrefixSSE) && (SSE_prefix != (byte)Enum_SSE.SSE_PREFIX_NONE))
                    OpcodeInfoPtr = OpcodeInfoPtr.AnotherArray[SSE_prefix - 1];

                /////ia_opcode = OpcodeInfoPtr->IA;

                oInstruction.SetOpcodeReg((byte)(b1 & 7));
            }

            if (bLock)
            { // lock prefix invalid opcode
                // lock prefix not allowed or destination operand is not memory
                // mod == 0xc0 can't be BxLockable in fetchdecode tables
                if (/*(mod == 0xc0) ||*/ (attr & DecodingAttribute.BxLockable) != 0x00)
                {
                    ///BX_INFO(("LOCK prefix unallowed (op1=0x%x, attr=0x%x, mod=0x%x, nnn=%u)", b1, attr, uMod, uNNN ));
                    // replace execution function with undefined-opcode

                    ////////ia_opcode = mError.Unknown;
                }
            }

            oInstruction.modRMForm_Id = 0;
            uImm_Mode = (uint)(attr & DecodingAttribute.BxImmediate);
            if (uImm_Mode != 0)
            {
                switch (uImm_Mode)
                {
                    case (uint)DecodingAttribute.BxImmediate_I1:
                        oInstruction.modRMForm_Ib = 1;
                        break;
                    case (uint)DecodingAttribute.BxImmediate_Ib:
                        if (ilen < uRemain)
                        {
                            oInstruction.modRMForm_Id = mCPU.MainMemory.ReadByte(FetchPtr); ;
                            ilen++;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_Ib_SE: // Sign extend to OS size
                        if (ilen < uRemain)
                        {
                            byte temp8s = mCPU.MainMemory.ReadByte(FetchPtr);
                            if (oInstruction.Os32L() != 0)
                                oInstruction.modRMForm_Id = (uint)temp8s;
                            else
                                oInstruction.modRMForm_Iw = temp8s;
                            ilen++;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_Iw:
                        if ((ilen + 1) < uRemain)
                        {
                            oInstruction.modRMForm_Iw = mCPU.MainMemory.ReadWord(FetchPtr);
                            ilen += 2;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_Id:
                        if ((ilen + 3) < uRemain)
                        {
                            oInstruction.modRMForm_Id = mCPU.MainMemory.ReadDWord(FetchPtr);
                            ilen += 4;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_BrOff8:
                        if (ilen < uRemain)
                        {
                            oInstruction.modRMForm_Id = mCPU.MainMemory.ReadByte(FetchPtr);
                            ilen++;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_IwIb:
                        if ((ilen + 2) < uRemain)
                        {
                            oInstruction.modRMForm_Iw = mCPU.MainMemory.ReadWord(FetchPtr);
                            FetchPtr += 2;
                            oInstruction.IXIXForm_Ib2 = mCPU.MainMemory.ReadByte(FetchPtr);
                            ilen += 3;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_IwIw: // CALL_Ap
                        if ((ilen + 3) < uRemain)
                        {
                            oInstruction.IXIXForm_Iw = mCPU.MainMemory.ReadWord(FetchPtr);
                            FetchPtr += 2;
                            oInstruction.IXIXForm_Iw2 = mCPU.MainMemory.ReadWord(FetchPtr);
                            ilen += 4;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_IdIw: // CALL_Ap
                        if ((ilen + 5) < uRemain)
                        {
                            oInstruction.IXIXForm_Id = mCPU.MainMemory.ReadDWord(FetchPtr);
                            FetchPtr += 4;
                            oInstruction.IXIXForm_Iw2 = mCPU.MainMemory.ReadWord(FetchPtr);
                            ilen += 6;
                        }
                        else
                        {
                            return oInstruction;
                        }
                        break;
                    case (uint)DecodingAttribute.BxImmediate_O:
                        // For is which embed the address in the opcode.
                        if (oInstruction.AS32L() != 0)
                        {
                            // fetch 32bit address into Id
                            if ((ilen + 3) < uRemain)
                            {
                                oInstruction.modRMForm_Id = mCPU.MainMemory.ReadDWord(FetchPtr);
                                ilen += 4;
                            }
                            else return oInstruction;
                        }
                        else
                        {
                            // fetch 16bit address into Id
                            if ((ilen + 1) < uRemain)
                            {
                                oInstruction.modRMForm_Id = mCPU.MainMemory.ReadWord(FetchPtr);
                                ilen += 2;
                            }
                            else return oInstruction;
                        }
                        break;
                    default:
                        throw new Exception("fetchdecode: imm_mode = %u");
                }
            }

            if (Seg_Override != (byte)Enum_SegmentReg.BX_SEG_REG_NULL)
                Seg = Seg_Override;
            oInstruction.SetSeg(Seg);
            if (((attr & DecodingAttribute.BxTraceEnd) != 0) /*|| ia_opcode == mError.Unknown*/)
                oInstruction.SetStopTraceAttr();

            if ((uint)(attr & DecodingAttribute.BxArithDstRM) != 0)
            {
                oInstruction.SetRM(uNNN);
                oInstruction.SetNnn(uRM);
            }

            if ((OpcodeInfoPtr  != null) && (OpcodeInfoPtr.Execute != null))
            {
                oInstruction.Execute1 =OpcodeInfoPtr.Execute ;
                oInstruction.Execute2 = OpcodeInfoPtr.Execute2;
            }
            else
            {
                if (bUseOPCodeInstruction32R== true)
                {
                    //oInstruction.Execute1 = mOPCodeInstruction32R[b1 + uOffset].Execute;
                    //oInstruction.Execute2 = mOPCodeInstruction32R[b1 + uOffset].Execute2;
                    try
                    {
                        oInstruction.Execute1 = mOPCodeInstruction32R[b1 + uOffset].Execute;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[ex] " + ex.Message);
                    }

                    try
                    {
                        oInstruction.Execute2 = mOPCodeInstruction32R[b1 + uOffset].Execute2;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[ex] " + ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        oInstruction.Execute1 = mOPCodeInstruction32M[b1 + uOffset].Execute;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[ex] " + ex.Message);
                    }

                    try
                    {
                        oInstruction.Execute2 = mOPCodeInstruction32M[b1 + uOffset].Execute2;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[ex] " + ex.Message);
                    }

                }
            }
            oInstruction.SetB1(b1);
            oInstruction.SetILen((byte)ilen);

            return oInstruction;
        }
        #endregion
    }
}