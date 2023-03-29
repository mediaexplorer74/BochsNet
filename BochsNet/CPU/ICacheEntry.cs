using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Instructions;

namespace CPU
{
    public class ICacheEntry
    {

        #region "Attributes"

        UInt64 mPhysicalAddress;
        UInt32 mWriteStamp;

        UInt32 mInstructionLength;
        UInt32 mTraceMask;
        Instruction mInstruction;

        #endregion


        #region "Properties"

        public UInt64 PhysicalAddress
        {
            get
            {
                return mPhysicalAddress;
            }
            set
            {
                mPhysicalAddress = value;
            }
        }

        public UInt32 TraceMask
        {
            get
            {
                return mTraceMask;
            }
            set
            {
                mTraceMask = value;
            }
        }

        public UInt32 WriteStamp
        {
            get
            {
                return mWriteStamp;
            }
            set
            {
                mWriteStamp = value;
            }
        }


        /// <summary>
        /// Bochs: ilen
        /// </summary>
        public UInt32 InstructionLength
        {
            get
            {
                return mInstructionLength;
            }
            set
            {
                mInstructionLength = value;
            }
        }

        public Instruction Instruction
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

        #endregion 

        #region "Constructors"

        #endregion 

        #region "Methods"

      
        #endregion 
    }
}
