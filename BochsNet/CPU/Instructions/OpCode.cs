using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Instructions
{
    public class OpCode
    {

        #region "Attribute"
        /// <summary>
        /// List of OpCode stored by PrimaryOpCode Number as in the URL below.
        /// <see cref="http://ref.x86asm.net/coder32-abc.html"/>
        /// </summary>
        InstructionExecution.dlgt_OPCodeInstruction mExecute;
        InstructionExecution.dlgt_OPCodeInstruction mExecute2;
        DecodingAttribute mAttribute;
        OpCode[] mAnotherArray;
        #endregion 


        #region "Properties"

        public InstructionExecution.dlgt_OPCodeInstruction Execute
        {
            get
            {
                return mExecute;
            }
        }

        public InstructionExecution.dlgt_OPCodeInstruction Execute2
        {
            get
            {
                return mExecute2;
            }
        }

        public OpCode[] AnotherArray
        {
            get
            {
                return mAnotherArray;
            }
        }

        public DecodingAttribute Attribute
        {
            get
            {
                return mAttribute;
            }
        }
        #endregion 


        #region "Constructors"

        public OpCode(DecodingAttribute nAttribute, InstructionExecution.dlgt_OPCodeInstruction Execute)
        {
            mAttribute = nAttribute;
            mExecute = Execute;
        }

        public OpCode(DecodingAttribute nAttribute, InstructionExecution.dlgt_OPCodeInstruction Execute, InstructionExecution.dlgt_OPCodeInstruction Execute2)
        {
            mAttribute = nAttribute;
            mExecute = Execute;
            mExecute2 = Execute2;
        }

        public OpCode(DecodingAttribute nAttribute, InstructionExecution.dlgt_OPCodeInstruction Execute, OpCode[] AnotherArray)
        {
            mAttribute = nAttribute;
            mExecute = Execute;
            mAnotherArray =AnotherArray;
        
        }

        public OpCode(DecodingAttribute nAttribute, InstructionExecution.dlgt_OPCodeInstruction Execute, InstructionExecution.dlgt_OPCodeInstruction Execute2,OpCode[] AnotherArray)
        {
            mAttribute = nAttribute;
            mExecute = Execute;
            mExecute2 = Execute2;
            mAnotherArray = AnotherArray;

        }

        #endregion 
    }
}
