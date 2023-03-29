using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Registers;

namespace CPU
{
    public class Stack
    {



        #region "Attributes"


        protected CPU mCPU;

        #endregion


        #region "Properties"

        #endregion 

        #region "Constructors"
        public Stack(CPU oCPU)
        {
            mCPU = oCPU;
        }

        #endregion 




        #region "Methods"

        public void Push16(UInt16 value16)
        {
            if (mCPU.SUPPORT_X86_64 == true)
            {
                throw new NotImplementedException();
            }
            else
            {
                this.mCPU.RSP.Value64 -= 2;
                this.mCPU.WriteVirtualWord((byte)Enum_SegmentReg.REG_SS, this.mCPU.RSP.Value16, value16);
            }
        }


         public void Push32(UInt32 value32)
        {
            if (mCPU.SUPPORT_X86_64 == true)
            {
                throw new NotImplementedException();
            }
            else
            {
                this.mCPU.RSP.Value64 -=4;
                this.mCPU.WriteVirtualDWord32((byte)Enum_SegmentReg.REG_SS, this.mCPU.RSP.Value16, value32);
            }
        }

         public UInt16 Pop16()
         {
             UInt16 Value16;

             if (mCPU.SUPPORT_X86_64 == true)
             {
                 throw new NotImplementedException();
             }
             else
             {
                 Value16 = this.mCPU.ReadVirtualWord32((byte)Enum_SegmentReg.REG_SS, this.mCPU.RSP.Value16);
                 this.mCPU.RSP.Value64 += 2;
             }



             return Value16;
         }


         public UInt32 Pop32()
         {
             UInt32 Value32;

             if (mCPU.SUPPORT_X86_64 == true)
             {
                 throw new NotImplementedException();
             }
             else
             {
                 Value32 = this.mCPU.ReadVirtualDWord32((byte)Enum_SegmentReg.REG_SS, this.mCPU.RSP.Value16);
                 this.mCPU.RSP.Value64 += 2;
             }

             return Value32;
         }
        #endregion 

    }
}
