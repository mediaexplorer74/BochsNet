using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Pagging
{
    /// <summary>
    /// A TLB is a cache of memory translations (i.e. page table entries). When the processor needs to translate a given virtual address into a physical address, the TLB is checked first. 
    /// <para>On x86 systems, TLB misses are handled transparently by hardware. Only if the page table entry (or any higher level directory entry) is not present will the operating system be notified.</para>
    /// <see cref="http://wiki.osdev.org/TLB"/>
    /// </summary>
    public class TLB : Dictionary<UInt16, TLBEntry>
    {

        /*
         * Like a cache, the TLB is mostly transparent. There are some cases which the OS must be aware of:
         * writing to page tables The TLB is not aware of changes you make to the page tables (at any level). 
         * If you make a change, you must flush the TLBs. 
         * On x86, this can be done by writing to the page table base register (CR3). That is:
         * mov EAX, CR3
         * mov CR3, EAX
         * Note: setting the global (G) bit in a page table entry will prevent that entry from being flushed. This is useful for pinning interrupt handlers in place.
         * multi-processor consistency
         * The above is more complicated in the multi-processor case. If another processor could also be affected by a page table write (because of shared memory, or multiple threads from the same process), you must also flush the TLBs on those processors. This will require some form of inter-processor communication.
         */

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

        #region "Constants"

        // Each entry in the TLB cache has 3 entries:
        //
        //   lpf:         Linear Page Frame (page aligned linear address of page)
        //     bits 32..12  Linear page frame
        //     bit  11      0: TLB HostPtr access allowed, 1: not allowed
        //     bit  10...0  Invalidate index
        //
        //   ppf:         Physical Page Frame (page aligned phy address of page)
        //
        //   hostPageAddr:
        //                Host Page Frame address used for direct access to
        //                the mem.vector[] space allocated for the guest physical
        //                memory.  If this is zero, it means that a pointer
        //                to the host space could not be generated, likely because
        //                that page of memory is not standard memory (it might
        //                be memory mapped IO, ROM, etc).
        //
        //   accessBits:
        //
        //     bit  31:     Page is a global page.
        //
        //       The following bits are used for a very efficient permissions
        //       check.  The goal is to be able, using only the current privilege
        //       level and access type, to determine if the page tables allow the
        //       access to occur or at least should rewalk the page tables.  On
        //       the first read access, permissions are set to only read, so a
        //       rewalk is necessary when a subsequent write fails the tests.
        //       This allows for the dirty bit to be set properly, but for the
        //       test to be efficient.  Note that the CR0.WP flag is not present.
        //       The values in the following flags is based on the current CR0.WP
        //       value, necessitating a TLB flush when CR0.WP changes.
        //
        //       The test is:
        //         OK = (accessBits & ((E<<2) | (W<<1) | U)) <> 0
        //
        //       where E:1=Execute, 0=Data;
        //             W:1=Write, 0=Read;
        //             U:1=CPL3, 0=CPL0-2
        //
        //       Thus for reads, it is:
        //         OK =       (          U )
        //       for writes:
        //         OK = 0x2 | (          U )
        //       and for code fetches:
        //         OK = 0x4 | (          U )
        //
        //       Note, that the TLB should have TLB_HostPtr bit set when direct
        //       access through host pointer is NOT allowed for the page. A memory
        //       operation asking for a direct access through host pointer will
        //       set TLB_HostPtr bit in its lpf field and thus get TLB miss result 
        //       when the direct access is not allowed.
        //

        public const UInt16 const_TLB_SIZE = 1024;
        public const UInt64 const_TLB_MASK = ((const_TLB_SIZE - 1) << 12);
        public const UInt64 const_INVALID_TLB_ENTRY = 0xffffffffffffffff;
        public const UInt16 const_TLB_HostPtr = 0x800; /* set this bit when direct access is NOT allowed */
        public const UInt16 const_TLB_SysOnly = 0x1;
        public const UInt16 const_TLB_ReadOnly = 0x2;
        public const UInt16 const_TLB_NoExecute = 0x4;
        public const UInt32 const_TLB_GlobalPage = 0x80000000;
        #endregion


        #region "Attributes"

        protected CPU mCPU;

        /// <summary>
        /// BX_TLB_SIZE: Number of entries in TLB
        /// BX_TLB_INDEX_OF(lpf): This macro is passed the linear page frame
        ///   (top 20 bits of the linear address.  It must map these bits to
        ///   one of the TLB cache slots, given the size of BX_TLB_SIZE.
        ///   There will be a many-to-one mapping to each TLB cache slot.
        ///   When there are collisions, the old entry is overwritten with
        ///   one for the newest access.
        /// </summary>
        protected UInt16 mTLB_Size = const_TLB_SIZE;
        #endregion

        #region "Properties"



        public CPU CPU
        {
            get
            {
                return mCPU;
            }
        }

        #endregion


        #region "Constructor"

        protected TLB()
        {

        }

        public TLB(CPU oCPU)
        {
            mCPU = oCPU;
        }
        #endregion

        #region "Methods"

        public void InitTLB()
        {
            for (ushort n = 0; n < mTLB_Size; n++)
            {
                this[n] = new TLBEntry();
                this[n].LPF = const_INVALID_TLB_ENTRY;

            }
        }


        public UInt16 IndexOf(UInt64 LPF, UInt64 Length)
        {
            return (UInt16) (((LPF + Length) & const_TLB_MASK) >> 12);
        }


        /// <summary>
        /// bit [11] of the TLB lpf used for TLB_HostPtr valid indication
        /// #define TLB_LPFOf(laddr) AlignedAccessLPFOf(laddr, 0x7ff)
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public UInt64 TLB_LPFOf (UInt64 Address)
        {
            return this.mCPU.AlignedAccessLPFOf(Address, 0x7ff);
        }


        #endregion

       
    }
}

