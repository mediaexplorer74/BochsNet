using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Pagging
{
    public class MemoryPagging
    {

        // X86 Registers Which Affect Paging:
        // ==================================
        //
        // CR0:
        //   bit 31: PG, Paging (386+)
        //   bit 16: WP, Write Protect (486+)
        //     0: allow   supervisor level writes into user level RO pages
        //     1: inhibit supervisor level writes into user level RO pages
        //
        // CR3:
        //   bit 31..12: PDBR, Page Directory Base Register (386+)
        //   bit      4: PCD, Page level Cache Disable (486+)
        //     Controls caching of current page directory.  Affects only the processor's
        //     internal caches (L1 and L2).
        //     This flag ignored if paging disabled (PG=0) or cache disabled (CD=1).
        //     Values:
        //       0: Page Directory can be cached
        //       1: Page Directory not cached
        //   bit      3: PWT, Page level Writes Transparent (486+)
        //     Controls write-through or write-back caching policy of current page
        //     directory.  Affects only the processor's internal caches (L1 and L2).
        //     This flag ignored if paging disabled (PG=0) or cache disabled (CD=1).
        //     Values:
        //       0: write-back caching enabled
        //       1: write-through caching enabled
        //
        // CR4:
        //   bit 4: PSE, Page Size Extension (Pentium+)
        //     0: 4KByte pages (typical)
        //     1: 4MByte or 2MByte pages
        //   bit 5: PAE, Physical Address Extension (Pentium Pro+)
        //     0: 32bit physical addresses
        //     1: 36bit physical addresses
        //   bit 7: PGE, Page Global Enable (Pentium Pro+)
        //     The global page feature allows frequently used or shared pages
        //     to be marked as global (PDE or PTE bit 8).  Global pages are
        //     not flushed from TLB on a task switch or write to CR3.
        //     Values:
        //       0: disables global page feature
        //       1: enables global page feature
        //
        //    page size extention and physical address size extention matrix (legacy mode)
        //   ==============================================================================
        //   CR0.PG  CR4.PAE  CR4.PSE  PDPE.PS  PDE.PS | page size   physical address size
        //   ==============================================================================
        //      0       X        X       R         X   |   --          paging disabled
        //      1       0        0       R         X   |   4K              32bits
        //      1       0        1       R         0   |   4K              32bits
        //      1       0        1       R         1   |   4M              32bits
        //      1       1        X       R         0   |   4K              36bits
        //      1       1        X       R         1   |   2M              36bits

        //     page size extention and physical address size extention matrix (long mode)
        //   ==============================================================================
        //   CR0.PG  CR4.PAE  CR4.PSE  PDPE.PS  PDE.PS | page size   physical address size
        //   ==============================================================================
        //      1       1        X       0         0   |   4K              52bits
        //      1       1        X       0         1   |   2M              52bits
        //      1       1        X       1         -   |   1G              52bits

        // Page Directory/Table Entry Fields Defined:
        // ==========================================
        // NX: No Execute
        //   This bit controls the ability to execute code from all physical
        //   pages mapped by the table entry.
        //     0: Code can be executed from the mapped physical pages
        //     1: Code cannot be executed
        //   The NX bit can only be set when the no-execute page-protection
        //   feature is enabled by setting EFER.NXE=1, If EFER.NXE=0, the
        //   NX bit is treated as reserved. In this case, #PF occurs if the
        //   NX bit is not cleared to zero.
        //
        // G: Global flag
        //   Indiciates a global page when set.  When a page is marked
        //   global and the PGE flag in CR4 is set, the page table or
        //   directory entry for the page is not invalidated in the TLB
        //   when CR3 is loaded or a task switch occurs.  Only software
        //   clears and sets this flag.  For page directory entries that
        //   point to page tables, this flag is ignored and the global
        //   characteristics of a page are set in the page table entries.
        //
        // PS: Page Size flag
        //   Only used in page directory entries.  When PS=0, the page
        //   size is 4KBytes and the page directory entry points to a
        //   page table.  When PS=1, the page size is 4MBytes for
        //   normal 32-bit addressing and 2MBytes if extended physical
        //   addressing.
        //
        // PAT: Page-Attribute Table
        //   This bit is only present in the lowest level of the page
        //   translation hierarchy. The PAT bit is the high-order bit
        //   of a 3-bit index into the PAT register. The other two
        //   bits involved in forming the index are the PCD and PWT
        //   bits.
        //
        // D: Dirty bit:
        //   Processor sets the Dirty bit in the 2nd-level page table before a
        //   write operation to an address mapped by that page table entry.
        //   Dirty bit in directory entries is undefined.
        //
        // A: Accessed bit:
        //   Processor sets the Accessed bits in both levels of page tables before
        //   a read/write operation to a page.
        //
        // PCD: Page level Cache Disable
        //   Controls caching of individual pages or page tables.
        //   This allows a per-page based mechanism to disable caching, for
        //   those pages which contained memory mapped IO, or otherwise
        //   should not be cached.  Processor ignores this flag if paging
        //   is not used (CR0.PG=0) or the cache disable bit is set (CR0.CD=1).
        //   Values:
        //     0: page or page table can be cached
        //     1: page or page table is not cached (prevented)
        //
        // PWT: Page level Write Through
        //   Controls the write-through or write-back caching policy of individual
        //   pages or page tables.  Processor ignores this flag if paging
        //   is not used (CR0.PG=0) or the cache disable bit is set (CR0.CD=1).
        //   Values:
        //     0: write-back caching
        //     1: write-through caching
        //
        // U/S: User/Supervisor level
        //   0: Supervisor level - for the OS, drivers, etc.
        //   1: User level - application code and data
        //
        // R/W: Read/Write access
        //   0: read-only access
        //   1: read/write access
        //
        // P: Present
        //   0: Not present
        //   1: Present
        // ==========================================


        #region "Constance"


        //#define PAGE_DIRECTORY_NX_BIT (BX_CONST64(0x8000000000000000))
        public UInt64 const_PAGE_DIRECTORY_NX_BIT = 0x8000000000000000;

        //#define BX_CR3_PAGING_MASK    (BX_CONST64(0x000ffffffffff000))
        public UInt64 const_CR3_PAGING_MASK = 0x000ffffffffff000;

        //#define BX_CR3_LEGACY_PAE_PAGING_MASK (0xffffffe0)
        public UInt32 const_CR3_LEGACY_PAE_PAGING_MASK = 0xffffffe0;


        #endregion

        #region "Attributes"

        CPU mCPU;
        #endregion

        #region "Constructors"

        public MemoryPagging(CPU oCPU)
        {
            mCPU = oCPU;
        }

        #endregion

        #region "Methods"
        /// <summary>
        ///  access_write_linear in Bochs
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Length"></param>
        /// <param name="Current_PL"></param>
        /// <param name="Data"></param>
        public void AccessWriteLinear(UInt64 Address, uint Length, uint Current_PL, byte[] Data)
        {
            UInt64 nPageOffset = mCPU.PageOffset(Address);


            if ((nPageOffset + Length) <= 4096)
            {
                // Access within single page.
                mCPU.address_xlation_paddress1 = this.TranslateLinear(Address, Current_PL, Enum_MemoryAccessType.Write);
                mCPU.address_xlation_pages = 1;
                // do not replace to the TLB if there is a breakpoint defined
                // in the same page
                AccessWritePhysical(mCPU.address_xlation_paddress1, Length, Data);
            }
            else
            {
                throw new NotImplementedException("");
                /*
                 *  // access across 2 pages
BX_CPU_THIS_PTR address_xlation.paddress1 =
  dtranslate_linear(laddr, curr_pl, BX_WRITE);
BX_CPU_THIS_PTR address_xlation.len1 = 4096 - pageOffset;
BX_CPU_THIS_PTR address_xlation.len2 = len -
  BX_CPU_THIS_PTR address_xlation.len1;
BX_CPU_THIS_PTR address_xlation.pages     = 2;
bx_address laddr2 = laddr + BX_CPU_THIS_PTR address_xlation.len1;
BX_CPU_THIS_PTR address_xlation.paddress2 =
  dtranslate_linear(laddr2, curr_pl, BX_WRITE);

#ifdef BX_LITTLE_ENDIAN
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, curr_pl, 
  BX_WRITE, (Bit8u*) data);
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, data);
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, curr_pl, 
  BX_WRITE, ((Bit8u*)data) + BX_CPU_THIS_PTR address_xlation.len1);
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2,
  ((Bit8u*)data) + BX_CPU_THIS_PTR address_xlation.len1);
#else // BX_BIG_ENDIAN
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, curr_pl, 
  BX_WRITE, ((Bit8u*)data) + (len - BX_CPU_THIS_PTR address_xlation.len1));
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1,
  ((Bit8u*)data) + (len - BX_CPU_THIS_PTR address_xlation.len1));
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, curr_pl, 
  BX_WRITE, (Bit8u*) data);
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, data);
#endif
                 * */

            }


        }


        public void AccessWritePhysical(UInt64 Address, uint Length, byte[] Data)
        {
            if (this.mCPU.SUPPORT_VMX >= 2)
            {
                throw new NotImplementedException("");

                /*
                 * if (is_virtual_apic_page(paddr)) {
            VMX_Virtual_Apic_Write(paddr, len, data);
            return;
          }
                 * */
            }

            if (this.mCPU.SUPPORT_APIC)
            {
                /*
                 *  if (BX_CPU_THIS_PTR lapic.is_selected(paddr)) {
                    BX_CPU_THIS_PTR lapic.write(paddr, data, len);
                    return;
                    }
                 * */
            }

            this.mCPU.MainMemory.WritePhysicalPage(Address, Data, Length);

            return;
        }


        public byte[] AccessReadLinear(UInt64 Address, uint Length, uint Current_PL)
        {
            UInt64 nPageOffset = mCPU.PageOffset(Address);


            if ((nPageOffset + Length) <= 4096)
            {
                // Access within single page.
                mCPU.address_xlation_paddress1 = this.TranslateLinear(Address, Current_PL, Enum_MemoryAccessType.Write);
                mCPU.address_xlation_pages = 1;
                // do not replace to the TLB if there is a breakpoint defined
                // in the same page
                return AccessReadPhysical(mCPU.address_xlation_paddress1, Length);
            }
            else
            {
                throw new NotImplementedException("");
                /*
                 *  // access across 2 pages
BX_CPU_THIS_PTR address_xlation.paddress1 =
  dtranslate_linear(laddr, curr_pl, BX_WRITE);
BX_CPU_THIS_PTR address_xlation.len1 = 4096 - pageOffset;
BX_CPU_THIS_PTR address_xlation.len2 = len -
  BX_CPU_THIS_PTR address_xlation.len1;
BX_CPU_THIS_PTR address_xlation.pages     = 2;
bx_address laddr2 = laddr + BX_CPU_THIS_PTR address_xlation.len1;
BX_CPU_THIS_PTR address_xlation.paddress2 =
  dtranslate_linear(laddr2, curr_pl, BX_WRITE);

#ifdef BX_LITTLE_ENDIAN
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, curr_pl, 
  BX_WRITE, (Bit8u*) data);
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, data);
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, curr_pl, 
  BX_WRITE, ((Bit8u*)data) + BX_CPU_THIS_PTR address_xlation.len1);
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2,
  ((Bit8u*)data) + BX_CPU_THIS_PTR address_xlation.len1);
#else // BX_BIG_ENDIAN
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr,
  BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1, curr_pl, 
  BX_WRITE, ((Bit8u*)data) + (len - BX_CPU_THIS_PTR address_xlation.len1));
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress1,
  BX_CPU_THIS_PTR address_xlation.len1,
  ((Bit8u*)data) + (len - BX_CPU_THIS_PTR address_xlation.len1));
BX_INSTR_LIN_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, BX_WRITE);
BX_DBG_LIN_MEMORY_ACCESS(BX_CPU_ID, laddr2, BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, curr_pl, 
  BX_WRITE, (Bit8u*) data);
access_write_physical(BX_CPU_THIS_PTR address_xlation.paddress2,
  BX_CPU_THIS_PTR address_xlation.len2, data);
#endif
                 * */

            }


        }

        public byte[] AccessReadPhysical(UInt64 Address, uint Length)
        {
            if (this.mCPU.SUPPORT_VMX >= 2)
            {
                throw new NotImplementedException("");

                /*
              if (is_virtual_apic_page(paddr)) {
    VMX_Virtual_Apic_Read(paddr, len, data);
    return;
  }
                 * */
            }

            if (this.mCPU.SUPPORT_APIC)
            {
                /*
                if (is_virtual_apic_page(paddr)) {
                    VMX_Virtual_Apic_Read(paddr, len, data);
                    return;
                }
                 * */
            }

            return this.mCPU.MainMemory.ReadPhysicalPage(Address, Length);
        }
        #endregion

        /// <summary>
        /// Translate a linear address to a physical addre
        /// <para>
        ///           Format of a PDE that Maps a 4-MByte Page
        /// -----------------------------------------------------------
        /// 00    | Present (P)
        /// 01    | R/W
        /// 02    | U/S
        /// 03    | Page-Level Write-Through (PWT)
        /// 04    | Page-Level Cache-Disable (PCD)
        /// 05    | Accessed (A)
        /// 06    | Dirty (D)
        /// 07    | Page size, must be 1 to indicate 4-Mbyte page
        /// 08    | Global (G) (if CR4.PGE=1, ignored otherwise)
        /// 11-09 | (ignored)
        /// 12    | PAT (if PAT is supported, reserved otherwise)
        /// PA-13 | Bits PA-32 of physical address of the 4-MByte page
        /// 21-PA | Reserved (must be zero)
        /// 31-22 | Bits 31-22 of physical address of the 4-MByte page
        /// -----------------------------------------------------------
        ///</para>
        /// </summary>
        /// <param name="LongAddress"></param>
        /// <param name="CPL"></param>
        /// <param name="RW"></param>
        /// <returns></returns>
        public UInt64 TranslateLinear(UInt64 LongAddress, uint CPL, Enum_MemoryAccessType RW)
        {

            UInt32 CombinedAcess = 0x06;
            UInt64 lpf_mask = 0xfff; //4kPages
            byte priv_index;


            // IMPORTANT
            // note - we assume physical memory < 4gig so for brevity & speed, we'll use
            // 32 bit entries although cr3 is expanded to 64 bits.
            UInt64 paddress, ppf = 0, poffset = this.mCPU.PageOffset(LongAddress);
            byte isWrite = (byte)((byte)RW & 1); // write or r-m-w
            byte pl = (CPL == 3) ? (byte)1 : (byte)0;

            UInt64 lpf = this.mCPU.LPFOf(LongAddress);
            ushort TLB_index = this.mCPU.TLB.IndexOf(lpf, 0);
            TLBEntry tlbEntry = this.mCPU.TLB[TLB_index];

            // already looked up TLB for code access
            if (this.mCPU.TLB.TLB_LPFOf(tlbEntry.LPF) == lpf)
            {
                paddress = tlbEntry.PPF | poffset;

                byte isExecute = (RW == Enum_MemoryAccessType.Execute) ? (byte)1 : (byte)0;
                if ((tlbEntry.AccessBits & (uint)((byte)(isExecute << 2) | (byte)(isWrite << 1) | (byte)pl)) != 0)
                    return paddress;

                // The current access does not have permission according to the info
                // in our TLB cache entry.  Re-walk the page tables, in case there is
                // updated information in the memory image, and let the long path code
                // generate an exception if one is warranted.
            }

            if (this.mCPU.CR0.PG == Enum_Signal.High)
            {
                if (this.mCPU.CPULevel >= 6)
                {
                    if (this.mCPU.CR4.PAE == Enum_Signal.High)
                    {
                        ppf = TranslateLinear_PAE(LongAddress, lpf_mask, CombinedAcess, CPL, RW);

                    }
                }

                if ((this.mCPU.CPULevel < 6) || (this.mCPU.CR4.PAE == Enum_Signal.Low))
                {
                    // CR4.PAE==0 (and EFER.LMA==0)
                    UInt32 pde, cr3_masked = (UInt32)this.mCPU.CR3.Value32 & (UInt32)const_CR3_PAGING_MASK;
                    byte[] pte;
                    UInt64 pde_addr = (UInt64)(cr3_masked | ((LongAddress & 0xffc00000) >> 20));

                    if (this.mCPU.SUPPORT_VMX >= 2)
                    {
                        throw new NotImplementedException();
                    }
                    pte = AccessReadPhysical(pde_addr, 4);
                    if ((pte[0] & 0x1) == 0)
                    {
                        PageFault(Enum_PageFaults.ERROR_NOT_PRESENT, LongAddress, pl, RW);
                    }






                }
            }
            else
            {
                // no paging
                ppf = (UInt64)lpf;
            }


            /*#if BX_SUPPORT_VMX >= 2
              if (BX_CPU_THIS_PTR in_vmx_guest) {
                if (SECONDARY_VMEXEC_CONTROL(VMX_VM_EXEC_CTRL3_EPT_ENABLE)) {
                  ppf = translate_guest_physical(ppf, laddr, 1, 0, rw);
                }
              }
            #endif
             * */

            // Calculate physical memory address and fill in TLB cache entry
            paddress = ppf | poffset;

            // direct memory access is NOT allowed by default
            tlbEntry.LPF = lpf | TLB.const_TLB_HostPtr;
            tlbEntry.LPFMask = lpf_mask;
            tlbEntry.PPF = (UInt32)ppf;
            tlbEntry.AccessBits = 0;


            if ((CombinedAcess & 4) == 0)
            { // System
                tlbEntry.AccessBits |= TLB.const_TLB_SysOnly;
                if (isWrite == 0)
                    tlbEntry.AccessBits |= TLB.const_TLB_ReadOnly;
            }
            else
            {
                // Current operation is a read or a page is read only
                // Not efficient handling of system write to user read only page:
                // hopefully it is very rare case, optimize later
                if ((isWrite == 0) || (CombinedAcess & 2) == 0)
                {
                    tlbEntry.AccessBits |= TLB.const_TLB_ReadOnly;
                }
            }




            if ((this.mCPU.CPULevel >= 6) && ((CombinedAcess & 0x100) != 0)) // Global bit
            {
                tlbEntry.AccessBits |= TLB.const_TLB_GlobalPage;
            }

            // EFER.NXE change won't flush TLB
            if ((this.mCPU.SUPPORT_X86_64) && ((this.mCPU.CR4.PAE != 0) && (RW != Enum_MemoryAccessType.Execute)))
            {
                tlbEntry.AccessBits |= TLB.const_TLB_NoExecute;
            }


            // Attempt to get a host pointer to this physical page. Put that
            // pointer in the TLB cache. Note if the request is vetoed, NULL
            // will be returned, and it's OK to OR zero in anyways.
            UInt64? HostPageAddress = this.mCPU.MainMemory.GetHostMemoryAddress((UInt64)ppf, RW);
            if (HostPageAddress == null) 
            {
                tlbEntry.HostPageAddress =0;
            }
            else
            {
                tlbEntry.HostPageAddress = (UInt32)HostPageAddress;
                // All access allowed also via direct pointer
                /*
                 * #if BX_X86_DEBUGGER
                    if (! hwbreakpoint_check(laddr))
                #endif
                 * */
                tlbEntry.LPF = lpf; // allow direct access with HostPtr
            }
            
            
            return paddress;


        }


        public UInt64 TranslateLinear_PAE(UInt64 LongAddress, UInt64 LPF_Mask, UInt32 CombinedAccess, uint CPL, Enum_MemoryAccessType RW)
        {
            throw new NotImplementedException();
        }


        public void PageFault(Enum_PageFaults Fault, UInt64 LongAddress, byte USer, Enum_MemoryAccessType RW)
        {
        }

        #region "Shared Methods"


        public static UInt32 PageOffset(UInt32 FetchAddress)
        {
            return (UInt32)((FetchAddress) & 0xfff);
        }




        #endregion
    }
}
