using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class Register
    {

        #region "Enumeration"

        public enum Enum_RegisterType
        {
            Bit8s = 1,
            Bit8  = 2,
            Bit16s = 3, 
            Bit16 = 4,
            Bit32s = 5,
            Bit32 = 6,
            Bit64s = 7,
            Bit64 = 8
        };
        #endregion

        #region "Attributes"

        
        
        protected Enum_RegisterType  mType;

        protected byte[] mValue;
        
        #endregion

        #region "Properties"

        public virtual string Name
        {
            get
            {
                throw new NotImplementedException ("Property is not implemented");
            }
        }

        public virtual string Type
        {
            get
            {
                throw new System.NotImplementedException("Property is not implemented");
            }
        }
             

        #endregion


        #region "Methods"

        public virtual void Reset()
        {
            mValue.SetValue ((byte)0,0);
        }

        #endregion 

    }
}
