using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU
{
    /// <summary>
    /// pageSplitEntryIndex
    /// </summary>
    public class PageSplitEntryIndex
    {


        #region "Attributes"

        protected UInt64 mPhysicalAddress; // ppf Physical address of 2nd page of the trace 

        protected ICacheEntry mICacheEntry;

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

        public ICacheEntry ICacheEntry
        {
            get
            {
                return mICacheEntry;
            }

            set
            {
                mICacheEntry = value;
            }
        }

        #endregion 


        #region "Constructors"

        public PageSplitEntryIndex(UInt64 PhysicalAddress, ICacheEntry ICacheEntry)
        {
            this.mPhysicalAddress = PhysicalAddress;
            this.mICacheEntry = ICacheEntry;
        }

        #endregion 

    }
}
