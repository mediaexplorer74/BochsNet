using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Pagging
{
    public class TLBEntry
    {
      
        #region "Attributes"
        UInt64 mLPF;
        UInt64 mLPFMask;
        UInt32 mPPF;
        UInt32 mAccessBits;
        UInt32 mHostPageAddress;

        #endregion 


        #region "Properties"
        /// <summary>
        /// Linear Page Frame
        /// </summary>
        public UInt64 LPF
        {
            get
            {
                return mLPF;
            }

            set
            {
                mLPF = value;
            }
        }

        /// <summary>
        /// Linear address mask of the page size
        /// </summary>
        public UInt64 LPFMask
        {
            get
            {
                return mLPFMask;
            }

            set
            {
                mLPFMask = value;
            }
        }

        /// <summary>
        ///  Physical page frame
        /// </summary>
        public UInt32 PPF
        {
            get
            {
                return mPPF;
            }

            set
            {
                mPPF = value;
            }
        }
        public UInt32   AccessBits
        {
            get
            {
                return mAccessBits;
            }

            set
            {
                mAccessBits = value;
            }
        }
        public UInt32 HostPageAddress
        {
            get
            {
                return mHostPageAddress;
            }

            set
            {
                mHostPageAddress = value;
            }
        }
        #endregion 


        #region "Constructor"

        public TLBEntry()
        {
            mLPF = TLB.const_INVALID_TLB_ENTRY ;
        }


        public TLBEntry(UInt64 LPF, UInt64 LPFMask, UInt32 PPF, UInt32 AccessBits, UInt32 HostPageAddress)
        {
            mLPF = LPF;
            mLPFMask = LPFMask;
            PPF = mPPF;
            mAccessBits = AccessBits;
            mHostPageAddress = HostPageAddress;
        }
        #endregion

    }
}
