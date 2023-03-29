using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Instructions
{
    public class Resolver
    {

        #region "Attributes"

        protected CPU mCPU;

        #endregion

         #region "Constructor"



        public Resolver(CPU oCPU)
        {
            mCPU = oCPU;
        }

        #endregion

        #region "Methods"


        public  UInt16 BxResolve16BaseIndex(Instruction i)
        {
            UInt16 ret = (UInt16)(i.CPU.Read16BitRegX(i.SibBase()).Value16 + i.CPU.Read16BitRegX(i.SibIndex()).Value16 + i.modRMForm_Displ16u);
            return ret;

        }


        public  UInt32  BxResolve32Base(Instruction i)
        {
            return i.CPU.Read32BitRegX(i.SibBase()) + i.modRMForm_Displ32u; 
        }

        public void BxResolve32BaseIndex(Instruction i)
        {
            
        }


        #region "SUPPORT_X86_64"

        public void BxResolve64Base(Instruction i)
        {

        }

        public void BxResolve64BaseIndex(Instruction i)
        {

        }

        #endregion 
        
        #endregion
    }
}
