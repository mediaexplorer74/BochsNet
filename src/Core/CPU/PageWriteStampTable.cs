using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU
{
    public class PageWriteStampTable
    {

        #region "Constants"

        //ToDo: please use const_MEM_BLOCK_LEN in both
        public const int const_PHY_MEM_PAGES = (1024 * 1024); // must equal to const_MEM_BLOCK_LEN 
        public const int const_MAX_TRACE_LENGTH = 32;

        public const int const_ICacheEntries = (64 * 1024);  // Must be a power of 2.  [BxICacheEntries ]
        public const int const_ICacheMemPool = (384 * 1024);   // [BxICacheMemPool] 

        public const UInt32 const_ICacheWriteStampInvalid = 0xffffffff;
        public const UInt32 const_ICacheWriteStampStart = 0x7fffffff;
        public const UInt32 const_ICacheWriteStampFetchModeMask = ~const_ICacheWriteStampStart;

        #endregion

        #region "Attributes"

        protected CPU mCPU;
        protected UInt32[] mPageWriteStampEntry;

        #endregion

        #region "Properties"

        public UInt32[] PageWriteStampEntry
        {
            get
            {
                return mPageWriteStampEntry;
            }
        }

        #endregion


        #region "Constructors"

        public PageWriteStampTable(CPU oCPU)
        {
            mCPU = oCPU;
            mPageWriteStampEntry = new UInt32[const_PHY_MEM_PAGES];
            ResetWriteStamps();
        }
        #endregion

        #region "Methods"

        protected UInt32 Hash(UInt64 PhysicalAddress, UInt32 FetchModeMask)
        {
            //return ((pAddr + (pAddr << 2) + (pAddr>>6)) & (BxICacheEntries-1)) ^ fetchModeMask;
            return (UInt32) (((PhysicalAddress) & (const_ICacheEntries - 1)) ^ FetchModeMask);
        }


        protected UInt32 Hash(UInt64 PhysicalAddress)
        {
            // Bochs: Hash Function:return ((pAddr) & (BxICacheEntries-1)) ^ fetchModeMask;
            return (UInt32) PhysicalAddress >> 12;
        }

        protected void ResetWriteStamps()
        {
            for (int i = 0; i < const_PHY_MEM_PAGES; ++i)
            {
                mPageWriteStampEntry[i] = 0;
            }
        }

        public UInt32 GetPageWriteStamp(UInt64 PhysicalAddress)
        {
            return mPageWriteStampEntry[Hash(PhysicalAddress)];
        }

        public void SetPageWriteStamp(UInt64 PhysicalAddress, UInt32 PageWriteStamp)
        {
            mPageWriteStampEntry[Hash(PhysicalAddress)] = PageWriteStamp;
        }

        public void MarkICache(UInt64 PhysicalAddress)
        {
            mPageWriteStampEntry[Hash(PhysicalAddress)] |= const_ICacheWriteStampFetchModeMask;
        }

        public void MarkICacheMask(UInt64 PhysicalAddress, UInt32 Mask)
        {
             mPageWriteStampEntry[Hash(PhysicalAddress)] = Mask;
        }

        // whole page is being altered
        public void decWriteStamp(UInt64 PhysicalAddress)
        {
            UInt32 index = Hash(PhysicalAddress);

            if (mPageWriteStampEntry[index]!=0)
            {
                HandleSMC(PhysicalAddress, 0xffffffff); // one of the CPUs might be running trace from this page
                mPageWriteStampEntry[index] = 0;
            }
        }

        // whole page is being altered
        public void decWriteStamp(UInt64 PhysicalAddress, UInt32 Length)
        {
            UInt32 index = Hash(PhysicalAddress);
            if (mPageWriteStampEntry[index]!=0)
            {
                UInt32 mask = (UInt32) (1 << (int) ( (this.mCPU.PageOffset((UInt64)PhysicalAddress)) >> 7));
                mask |=(UInt32) ( 1 << (int)(this.mCPU.PageOffset((UInt64)(PhysicalAddress + Length - 1)) >> 7));

                if ((mPageWriteStampEntry[index] & mask)!=0)
                {
                    // one of the CPUs might be running trace from this page
                    HandleSMC(PhysicalAddress, mask);
                    mPageWriteStampEntry[index] &= ~mask;
                }
            }
        }

        public void HandleSMC(UInt64 PhysicalAddres, UInt32 Mask)
        {
            /* for Multi CPU support.
            for (UInt32 i = 0; i < BX_SMP_PROCESSORS; i++)
            {
                // ToDo:in case of multiple CPU then loop on them
                
                if (this.mCPU.SUPPORT_TRACE_CACHE)
                {
                    this.mCPU.AsyncEvent |= CPU.const_BX_ASYNC_EVENT_STOP_TRACE;
                }

                //this.mCPU[i].HandleSMC(PhysicalAddres, Mask);
            }*/

            this.mCPU.ICache.HandleSMC(PhysicalAddres, Mask);
        }

        #endregion

    }
}
