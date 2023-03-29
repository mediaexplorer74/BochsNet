using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CPU.Instructions;

namespace CPU
{
    public class ICache
    {



        #region "Constants"

        public const int const_ICacheEntries = (64 * 1024);  // Must be a power of 2.
        public const int const_ICacheMemPool = (384 * 1024);
        public const int const_PHY_MEM_PAGES = (1024 * 1024);
        public const int const_MAX_TRACE_LENGTH = 32;
        public const UInt64 const_ICACHE_INVALID_PHY_ADDRESS = (UInt64)(UInt64.MaxValue); // BX_ICACHE_INVALID_PHY_ADDRESS (bx_phy_address(-1))
        public const UInt64 const_ICACHE_PAGE_SPLIT_ENTRIES = 8;            // BX_ICACHE_PAGE_SPLIT_ENTRIES
        #endregion

        #region "Attributes"

        protected CPU mCPU;
        protected ICacheEntry[] mEntry;
        protected Instruction[] mPool;
        protected UInt32 mPIndex;
        protected UInt32 mPIndex_Start;
        protected UInt32 mPIndex_End;
        
        protected PageSplitEntryIndex[] mPageSplitEntryIndex = new PageSplitEntryIndex[const_ICACHE_PAGE_SPLIT_ENTRIES];
        protected UInt64 mNextPageSplitIndex = 0;
        #endregion



        #region "Properties"

        public ICacheEntry[] Entry
        {
            get
            {
                return mEntry;
            }
        }

        public Instruction[] Pool
        {
            get
            {
                return mPool;
            }
        }

        public PageSplitEntryIndex[] PageSplitEntryIndex
        {
            get
            {
                return mPageSplitEntryIndex;
            }
        }

        public UInt32 PIndex
        {
            get
            {
                return mPIndex;
            }
            set
            {
                mPIndex = value;
            }
        }
        public UInt32 PIndex_Start
        {
            get
            {
                return mPIndex_Start;
            }
            set
            {
                mPIndex_Start = value;
            }
        }
        public UInt32 PIndex_End
        {
            get
            {
                return mPIndex_End;
            }
            set
            {
                mPIndex_End = value;
            }
        }

     
        #endregion

        #region "Constructors"

        public ICache(CPU oCPU)
        {
            mCPU = oCPU;
            mEntry = new ICacheEntry[const_ICacheEntries];
            mPool = new Instruction[const_ICacheMemPool];
            FlushICacheEntries();



        }

        public ICacheEntry GetEntry(UInt64 PhysicalAddress, UInt64 FetchModeMask)
        {

            UInt64 uHash = Hash(PhysicalAddress, FetchModeMask);
            return mEntry[uHash];
        }


        /// <summary>
        /// Return the index of an Entry in ICacheEntry
        /// This function is helper by Bochs.Net to allow e++
        /// </summary>
        /// <param name="oEntry"></param>
        /// <returns></returns>
        public int GetIndex(ICacheEntry oEntry)
        {
            int len = this.mEntry.Length;
            for (int i = 0; i < len; ++i)
            {
                if (mEntry[i].PhysicalAddress == oEntry.PhysicalAddress)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion


        #region "Methods - ICache"

        protected UInt64 Hash(UInt64 PhysicalAddress, UInt64 FetchModeMask)
        {
            // Bochs: Hash Function:return ((pAddr) & (BxICacheEntries-1)) ^ fetchModeMask;
            return ((PhysicalAddress) & (const_ICacheEntries - 1)) ^ FetchModeMask;
        }


        public void AllocTrace(ICacheEntry oEntry)
        {
            if (mPIndex + const_MAX_TRACE_LENGTH > const_ICacheMemPool)
            {
                FlushICacheEntries();
            }
            oEntry.Instruction = mPool[mPIndex];
            oEntry.InstructionLength = 0;
        }



        public void FlushICacheEntries()
        {
            for (int i = 0; i < const_ICacheEntries; ++i)
            {
                mEntry[i] = new ICacheEntry();
                mEntry[i].WriteStamp = PageWriteStampTable.const_ICacheWriteStampInvalid;
            }

            if (mCPU.SUPPORT_TRACE_CACHE)
            {
                for (int i = 0; i < const_ICacheMemPool; ++i)
                {
                    mPool[i] = new Instruction(mCPU);
                }
                mNextPageSplitIndex = 0;
                mPIndex = 0;
            }
        }


        /// <summary>
        /// Fetches instruction to instruction cache
        /// </summary>
        /// <param name="oEntry"></param>
        /// <param name="EIPBiased"></param>
        /// <param name="PhysicalAddress"></param>
        public void ServeICacheMiss(ICacheEntry oEntry, UInt32 EIPBiased, UInt64 PhysicalAddress)
        {
            AllocTrace(oEntry);

            // Cache miss. We weren't so lucky, but let's be optimistic - try to build 
            // trace from incoming instruction bytes stream !
            oEntry.PhysicalAddress = PhysicalAddress;
            oEntry.TraceMask = 0;


            mCPU.PageWriteStampTable.MarkICache(PhysicalAddress);
            oEntry.WriteStamp = (uint)mCPU.CurrPageWriteStampPtr;
            UInt64 nRemainingInPage = mCPU.EIPPageWindowSize - EIPBiased;
            UInt64 FetchPtr = mCPU.EIPFetchPtr + EIPBiased;
            UInt32 PageOffset = this.mCPU.PageOffset(PhysicalAddress);
            UInt32 TraceMask = 0;


            Instruction oInstruction = null;
            mPIndex_Start = mPIndex;

            for (uint n = 0; n < const_MAX_TRACE_LENGTH; n++)
            {
                oInstruction = mCPU.InstructionExecution.FetchDecode32(FetchPtr, nRemainingInPage);
                if (oInstruction == null)
                {// Fetching instruction on segment/page boundary
                    if (n > 0)
                    {
                        // The trace is already valid, it has several instructions inside,
                        // in this case just drop the boundary instruction and stop
                        // tracing.
                        break;
                    }
                    // First instruction is boundary fetch, leave the trace cache entry 
                    // invalid and do not cache the instruction.
                    oEntry.WriteStamp = PageWriteStampTable.const_ICacheWriteStampInvalid;
                    oEntry.InstructionLength = 1;
                    mCPU.BoundryFetch(FetchPtr, nRemainingInPage, oInstruction);


                    // Add the instruction to trace cache
                    oEntry.PhysicalAddress = ~oEntry.PhysicalAddress;
                    oEntry.TraceMask = 0x80000000; /* last line in page */
                    this.mCPU.PageWriteStampTable.MarkICacheMask(oEntry.PhysicalAddress, oEntry.TraceMask);
                    // this.mCPU.PageWriteStampTable.MarkICacheMask(BX_CPU_THIS_PTR pAddrPage, 0x1);
                    //BX_CPU_THIS_PTR iCache.commit_page_split_trace(BX_CPU_THIS_PTR pAddrPage, entry);

                    return;
                }

                // add instruction to the trace
                byte iLen = oInstruction.iLen();
                oEntry.InstructionLength++;

                TraceMask |=(UInt32) (1 << ((int)PageOffset >> 7));
                TraceMask |= (UInt32)(1 << ((int)((int)PageOffset + (int)iLen - 1) >> 7));

                // continue to the next instruction
                nRemainingInPage -= iLen;


                Pool[PIndex] = oInstruction;
                /**
                * dont fetch any more because 
                *  1- instruction has "Stope Trace Attribute"  e.g.: ret instruction.
                *  2- because no remianing entries on page.
                **/
                if (oInstruction.GetStopTraceAttr() != 0x0 || nRemainingInPage == 0) break;


                PhysicalAddress += iLen;
                FetchPtr += iLen;

                
                PIndex++;  //in Bochs i = i + 1; I try to advance to next i.

                // try to find a trace starting from current pAddr and merge
                if (nRemainingInPage > 15)
                {
                    Instruction oInstRet = MergeTraces(oEntry, PhysicalAddress);
                    if (oInstRet != null)
                    {
                        // [PIndex-1]: -1 to replace the current Instruction.
                        Pool[PIndex - 1] = oInstruction;
                        break;
                    }
                }
            }

            oEntry.TraceMask |= TraceMask;
            this.mCPU.PageWriteStampTable.MarkICacheMask(PhysicalAddress, oEntry.TraceMask);

            //mPIndex_EOI = mPIndex;
            CommitTrace(oEntry.InstructionLength);
        }


        /// <summary>
        /// BX_CPU_THIS_PTR iCache.commit_trace(entry->ilen);
        /// </summary>
        /// <param name="InstructionLength"></param>
        public void CommitTrace(uint InstructionLength)
        {
            mPIndex = mPIndex_Start+ InstructionLength;
        }

        /// <summary>
        /// bx_bool BX_CPU_C::mergeTraces(bxICacheEntry_c *entry, bxInstruction_c *i, bx_phy_address pAddr)
        /// </summary>
        /// <param name="oEntry"></param>
        /// <param name="PhysicalAddress"></param>
        /// <returns>if NULL then equivelant to false in Bochs</returns>
        protected Instruction MergeTraces(ICacheEntry oEntry, UInt64 PhysicalAddress)
        {
            Instruction oInstruction = null;
            ICacheEntry oE = GetEntry(PhysicalAddress, mCPU.FetchModeMask());
            if ((oE.PhysicalAddress == PhysicalAddress) && (oE.WriteStamp == oEntry.WriteStamp))
            {

                // determine max amount of instruction to take from another entry
                uint max_length = oE.InstructionLength;
                if (max_length + oEntry.InstructionLength > const_MAX_TRACE_LENGTH)
                    max_length = const_MAX_TRACE_LENGTH - oEntry.InstructionLength;
                if (max_length == 0) return null;

                // memcpy(oInstruction, e->i, sizeof(bxInstruction_c)*max_length);
                oInstruction = oEntry.Instruction;

                oEntry.InstructionLength += max_length;
                //BX_ASSERT(entry->ilen <= const_MAX_TRACE_LENGTH);

                return oInstruction;
            }

            return null;
        }

        public void HandleSMC(UInt64 PhysicalAddres, UInt32 Mask)
        {
            // TODO: invalidate only entries in same page as pAddr

            PhysicalAddres = this.mCPU.LPFOf(PhysicalAddres);

            if (this.mCPU.SUPPORT_TRACE_CACHE)
            {
                if ((Mask & 0x1) != 0)
                {
                    // the store touched 1st cache line in the page, check for
                    // page split traces to invalidate.
                    for (UInt32 i = 0; i < ICache.const_ICACHE_PAGE_SPLIT_ENTRIES; i++)
                    {
                        if (PhysicalAddres == PageSplitEntryIndex[i].PhysicalAddress)
                        {
                            PageSplitEntryIndex[i].PhysicalAddress = ICache.const_ICACHE_INVALID_PHY_ADDRESS;
                        }
                    }
                }
            }

            ICacheEntry oEntry = GetEntry(PhysicalAddres, 0);

            for (byte n = 0; n < 32; n++)
            {
                UInt32 line_mask = (UInt32)(1 << n);
                if (line_mask > Mask) break;
                int eIndex = GetIndex(oEntry);
                if (eIndex == -1) throw new InvalidOperationException("eIndex == -1");
                for (byte index = 0; index < 128; index++, eIndex++)
                {
                    if (PhysicalAddres == this.mCPU.LPFOf(oEntry.PhysicalAddress) && (oEntry.TraceMask & Mask) != 0)
                    {
                        mEntry[eIndex].PhysicalAddress = const_ICACHE_INVALID_PHY_ADDRESS;
                    }
                }
            }

        }



        #endregion



    }
}
