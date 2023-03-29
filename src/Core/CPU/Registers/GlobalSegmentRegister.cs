using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    public class GlobalSegmentRegister: Register64
    {

        #region "Attributes"

        UInt64 mBase;
        UInt16 mLimit; 
        #endregion 



        #region "Properties"


        public UInt64 Base
        {
            get
            {
                return mBase;
            }
            set
            {
                mBase = value;
            }
        }


        public UInt16 Limit
        {
            get
            {
                return mLimit;
            }
            set
            {
                mLimit = value;
            }
        }


        #endregion 


        #region "Constructors"
        
        public GlobalSegmentRegister()
        {
        }

        
        public GlobalSegmentRegister(string Name64)
            : base(Name64)
        {
            mName64 = Name64;
        }
        #endregion 


        #region "Methods"

        #endregion



    }
}
