using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Core.Memory;
using Core.CPU;
using Core.PCBoard;
using Definitions.Enumerations;

namespace Memory
{

    /// <summary>
    /// This represents whole memory module including cache
    /// </summary>
    public class Memory : MemoryBase
    {

        /*
         * X86 processor's paging mechanism (page table structure)
          //1, using 4K pages, divided three ways:

          //1,32-bit mode (non-PAE), use the two page form. Address structure: 10-bit page directory table index (PD) 10 位 page table index (PT) 12 位 pages offset.

          //2,32-bit mode (enable PAE), using the three page form. Address structure: two page directory pointer table index (PDP) 9 位 page table of contents index (PD) 9 位 page table index (PT) 12 位 pages offset.

          //3,64-bit mode (X86_64), using the four page form. Address structure: 16-bit sign extension nine fourth-level page of a bitmap index (PML4) 9 位 page directory pointer table index (PDP) 9 位 page Catalogue (PD) 9 位 page table index (PT) 12 位 pages offset.

          //Second, use 4M/2M pages, in fact, will address the structure of the lowest levels combined, therefore, for non-PAE 32-bit mode, offset is 22 pages, corresponding to 4M page size for the remaining two cases, shift to 21 pages, the size of the corresponding page 2M.

          // Note: PAE by increasing the address bus to expand the support of the maximum physical memory, but still use 32-bit linear address, different operating systems use different ways to access the physical address of 36.

         */

        /* 
        // 
        // Memory map inside the 1st megabyte:
        //
        // 0x00000 - 0x7ffff    DOS area (512K)
        // 0x80000 - 0x9ffff    Optional fixed memory hole (128K)
        // 0xa0000 - 0xbffff    Standard PCI/ISA Video Mem / SMMRAM (128K)
        // 0xc0000 - 0xdffff    Expansion Card BIOS and Buffer Area (128K)
        // 0xe0000 - 0xeffff    Lower BIOS Area (64K)
        // 0xf0000 - 0xfffff    Upper BIOS Area (64K)
        //
         */

        #region "enumeration"

        public enum Enum_RomType : short
        {
            SystemBios = 0,
            VgaBios = 1,
            OptionalRomBios = 2
        }

        #endregion






        #region "Properties"

        /// <summary>
        /// Current memory size in bytes.
        /// </summary>
        public override UInt64 MemorySize
        {
            get
            {
                return mMemorySize;
            }
        }


        public override UInt64 AllocatedBlocks
        {
            get
            {
                return mAllocatedBlocks;
            }
        }

        public override UInt64 UsedBlocks
        {
            get
            {
                return mUsedBlocks;
            }
        }

       #region "SMRAM"

        public bool SMRAM_Available
        {
            get
            {
                return mSMRAM_Available;
            }
            set
            {
                mSMRAM_Available = value;
            }
        }
        public bool SMRAM_Enable
        {
            get
            {
                return mSMRAM_Enable;
            }
            set
            {
                mSMRAM_Enable = value;
            }
        }
        public bool SMRAM_Restricted
        {
            get
            {
                return mSMRAM_Restricted;
            }
            set
            {
                mSMRAM_Restricted = value;
            }
        }

        #endregion
        #endregion


        #region "Constructors"

        /// <summary>
        /// Creates memory with initial size.
        /// </summary>
        /// <param name="MemorySize">Memory size in bytes</param>
        public Memory(UInt64 MemorySize, PCBoard oPCBoard)
            : base(oPCBoard)
        {
            mMemorySize = MemorySize;
        }

        #endregion


        #region "Methods"

        public override void Initialize()
        {
            mMemory = new byte[mMemorySize];
            mAllocatedBlocks = mMemorySize;
            mPCIEnabled = true;
            UInt32 num_blocks = (UInt32)(mMemorySize/ const_MEM_BLOCK_LEN);

            mVector = AllocateVectorAligned((UInt32)(mMemorySize + const_BIOSROMSZ + const_EXROMSIZE + 4096), const_MEM_VECTOR_ALIGN);

            for (UInt64 i = 0; i < const_BIOSROMSZ + const_EXROMSIZE + 4096; ++i)
            {
                mMemory[i] = 0xff;
            }

            mBlocks = new UInt64?[num_blocks];
            mROMPointer = 0;
            mBogusPointer = const_BIOSROMSZ + const_EXROMSIZE;
            for (UInt64 i = 0; i < num_blocks; ++i)
            {
                mBlocks[i] = null;
            }
            mUsedBlocks = 0;

            // Load BIOS
            LoadROM(@".\Bios\BIOS-bochs-latest", (UInt64)0x00000, Enum_RomType.SystemBios, true);

            // Load Video RAM
            LoadROM(@".\Bios\VGABIOS-lgpl-latest", 0xC0000, Enum_RomType.VgaBios, false);
        }


        public void Enable_SMRAM(bool Enable, bool Restricted)
        {
            SMRAM_Available = true;
            SMRAM_Enable = Enable;
            SMRAM_Restricted = Restricted;
        }

        public void Disable_SMRAM()
        {
            SMRAM_Available = false;
            SMRAM_Enable = false;
            SMRAM_Restricted = false;
        }

        /// <summary>
        /// Check if SMRAM is aavailable for CPU data accesses
        /// </summary>
        /// <returns></returns>
        public bool Is_SMRAM_Accessible()
        {
            return SMRAM_Available && (SMRAM_Enable || (!SMRAM_Restricted));
        }


        public byte AllocateVectorAligned(UInt32 Bytes, UInt32 Alignment)
        {
            UInt64 test_mask = Alignment - 1;
            mActualVector = new byte[(UInt32)(Bytes + test_mask)];
  
            // round address forward to nearest multiple of alignment.  Alignment
            // MUST BE a power of two for this to work.
            
             /*
              * Bit64u masked = ((Bit64u)(BX_MEM_THIS actual_vector + test_mask)) & ~test_mask;
              * Bit8u *vector = (Bit8u *) masked;
              * */

            UInt64 masked = test_mask & ~test_mask; // actual_vector is an address that is [0] as in C# we used it as a separate array of bytes.
            return (byte)masked;
        }

        public void AllocateBlock(UInt32 Block)
        {
            UInt32 max_blocks = (UInt32) (AllocatedBlocks / const_MEM_BLOCK_LEN);
            if (mUsedBlocks >= max_blocks)
            {
                throw new Exception("FATAL ERROR: all available memory is already allocated !"); // BX_PANIC(("FATAL ERROR: all available memory is already allocated !"));
            }
            else
            {
                mBlocks[Block] =mVector + (mUsedBlocks * const_MEM_BLOCK_LEN);
                mUsedBlocks++;
            }
        }

        public override UInt64 GetVector(UInt64 PhysicalAddress)
        {
            UInt32 block = (UInt32)(PhysicalAddress / MemoryBase.const_MEM_BLOCK_LEN);
            if (mBlocks[block] == null)
            {
                AllocateBlock(block);
            }

            return (UInt64)(mBlocks[block] + (UInt64)(PhysicalAddress & (const_MEM_BLOCK_LEN - 1)));

        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address">Start memory address</param>
        /// <param name="Data">Data to write</param>
        /// <param name="Length">Number of bytes to write</param>
        public override void WritePhysicalPage(UInt64 Address, byte[] Data, UInt64 Length)
        {

            byte data_ptr;
            UInt64 a20addr = this.PCBoard.A20Addr(Address);

            //struct memory_handler_struct *memory_handler = NULL;


            // Note: accesses should always be contained within a single page now
            if ((Address >> 12) != ((Address + Length - 1) >> 12))
            {
                throw new Exception(("writePhysicalPage: cross page access at address"));
                //BX_PANIC(("writePhysicalPage: cross page access at address 0x" FMT_PHY_ADDRX ", len=%d", addr, len));
            }


#if BX_SUPPORT_MONITOR_MWAIT
                BX_MEM_THIS check_monitor(a20addr, len);
#endif



            bool is_bios = (a20addr >= (UInt64)~const_BIOS_MASK);

#if BX_PHY_ADDRESS_LONG
            if (a20addr > BX_CONST64(0xffffffff)) is_bios = 0;
#endif

             /*
                memory_handler = BX_MEM_THIS memory_handlers[a20addr >> 20];
                while (memory_handler) {
                    if (memory_handler->begin <= a20addr &&
                        memory_handler->end >= a20addr &&
                        memory_handler->write_handler(a20addr, len, data, memory_handler->param))
                    {
                        return;
                    }
                    memory_handler = memory_handler->next;
                } 
             */



            if (this.PCBoard.CPU != null)
            {
#if BX_SUPPORT_IODEBUG
     bx_devices.pluginIODebug->mem_write(cpu, a20addr, len, data);
#endif

                // all memory access fits in single 4K page
         
    // all of data is within limits of physical memory
                if ((a20addr >= 0x000a0000 && a20addr < 0x000c0000) && this.SMRAM_Available)
                {
                    // SMRAM memory space
                    if (this.SMRAM_Enable  && (((CPU.CPU)this.PCBoard.CPU).SMRAM_Map.SMM_Mode && !this.SMRAM_Restricted))
                    {
                        goto mem_write;
                        
                    }
                }

                // My COde 
                UInt64 len = 0;
                UInt64 StartIndex = 0;
                UInt64 EntryAddress = a20addr;
                while (StartIndex < Length)
                {
                    MemoryResourceEntry oMemEntry = GetMemoryEntry(EntryAddress);
                    if (oMemEntry == null)
                    {
                        break;
                    }
                        len = Length - (EntryAddress - oMemEntry.EndAddress);
                        oMemEntry.DeviceWrite(EntryAddress, len, Data, StartIndex);
                        EntryAddress += len;
                        StartIndex += len;
                }

            mem_write:
                     if (a20addr < this.MemorySize && ! is_bios) {
                        // all of data is within limits of physical memory
                        if (a20addr < 0x000a0000 || a20addr >= 0x00100000)
                        {
       if (Length == 8) {
           ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr,8);
            //WriteHostQWordToLittleEndian(BX_MEM_THIS get_vector(a20addr), *(Bit64u*)data);
            Write (a20addr,8,Data);
         return;
       }
       if (Length == 4) {
         ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 4);
         //WriteHostDWordToLittleEndian(BX_MEM_THIS get_vector(a20addr), *(Bit32u*)data);
           Write (a20addr,4,Data);
         return;
       }
       if (Length == 2) {
         ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 2);
         //WriteHostWordToLittleEndian(BX_MEM_THIS get_vector(a20addr), *(Bit16u*)data);
           Write (a20addr,2,Data);
         return;
       }
       if (Length == 1) {
         ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 1);
         ;
           WriteByte (GetVector(a20addr),Data[0]);
         return;
       }
       // len == other, just fall thru to special cases handling
     }
                              ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr);

    /*
#ifdef BX_LITTLE_ENDIAN
     data_ptr = (Bit8u *) data;
#else // BX_BIG_ENDIAN
     data_ptr = (Bit8u *) data + (len - 1);
#endif
                             */
            
           
   /*        

 mem_write:

   // all memory access fits in single 4K page
  

    
 #ifdef BX_LITTLE_ENDIAN
     data_ptr = (Bit8u *) data;
 #else // BX_BIG_ENDIAN
     data_ptr = (Bit8u *) data + (len - 1);
 #endif

     if (a20addr < 0x000a0000 || a20addr >= 0x00100000)
     {
       // addr *not* in range 000A0000 .. 000FFFFF
       while(1) {
         *(BX_MEM_THIS get_vector(a20addr)) = *data_ptr;
         if (len == 1) return;
         len--;
         a20addr++;
 #ifdef BX_LITTLE_ENDIAN
         data_ptr++;
 #else // BX_BIG_ENDIAN
         data_ptr--;
 #endif
       }
     }

     // addr must be in range 000A0000 .. 000FFFFF

     for(unsigned i=0; i<len; i++) {

       // SMMRAM
       if (a20addr < 0x000c0000) {
         // devices are not allowed to access SMMRAM under VGA memory
         if (cpu) {
           *(BX_MEM_THIS get_vector(a20addr)) = *data_ptr;
         }
         goto inc_one;
       }

       // adapter ROM     C0000 .. DFFFF
       // ROM BIOS memory E0000 .. FFFFF
 #if BX_SUPPORT_PCI == 0
       // ignore write to ROM
 #else
       // Write Based on 440fx Programming
       if (BX_MEM_THIS pci_enabled && ((a20addr & 0xfffc0000) == 0x000c0000))
       {
         switch (DEV_pci_wr_memtype((Bit32u) a20addr)) {
           case 0x1:   // Writes to ShadowRAM
             BX_DEBUG(("Writing to ShadowRAM: address 0x" FMT_PHY_ADDRX ", data %02x", a20addr, *data_ptr));
             *(BX_MEM_THIS get_vector(a20addr)) = *data_ptr;
             break;

           case 0x0:   // Writes to ROM, Inhibit
             BX_DEBUG(("Write to ROM ignored: address 0x" FMT_PHY_ADDRX ", data %02x", a20addr, *data_ptr));
             break;

           default:
             BX_PANIC(("writePhysicalPage: default case"));
         }
       }
 #endif

 inc_one:
       a20addr++;
 #ifdef BX_LITTLE_ENDIAN
       data_ptr++;
 #else // BX_BIG_ENDIAN
       data_ptr--;
 #endif

     }
   }
   else {
     // access outside limits of physical memory, ignore
     BX_DEBUG(("Write outside the limits of physical memory (0x"FMT_PHY_ADDRX") (ignore)", a20addr));
   }
             * */
        }

            
        }
    }


        public override byte[] ReadPhysicalPage(UInt64 Address, UInt64 Length)
        {
            byte data_ptr;
            UInt64 a20addr = this.PCBoard.A20Addr(Address);

            //struct memory_handler_struct *memory_handler = NULL;


            // Note: accesses should always be contained within a single page now
            if ((Address >> 12) != ((Address + Length - 1) >> 12))
            {
                throw new Exception(("writePhysicalPage: cross page access at address"));
                //BX_PANIC(("writePhysicalPage: cross page access at address 0x" FMT_PHY_ADDRX ", len=%d", addr, len));
            }

            bool is_bios = (a20addr >= (UInt64)~const_BIOS_MASK);

#if BX_PHY_ADDRESS_LONG
            if (a20addr > BX_CONST64(0xffffffff)) is_bios = 0;
#endif


            if (this.PCBoard.CPU != null)
            {
#if BX_SUPPORT_IODEBUG
     bx_devices.pluginIODebug->mem_read(cpu, a20addr, len, data);
#endif
                if ((a20addr >= 0x000a0000 && a20addr < 0x000c0000) && this.SMRAM_Available)
                {
                    // SMRAM memory space
                    if (this.SMRAM_Enable && (((CPU.CPU)this.PCBoard.CPU).SMRAM_Map.SMM_Mode && !this.SMRAM_Restricted))
                    {
                        goto mem_read;

                    }
                }

                // My COde 
                UInt64 len = 0;
                UInt64 StartIndex = 0;
                UInt64 EntryAddress = a20addr;
                while (StartIndex < Length)
                {
                    MemoryResourceEntry oMemEntry = GetMemoryEntry(EntryAddress);
                    if (oMemEntry == null)
                    {
                        break;
                    }
                    len = Length - (EntryAddress - oMemEntry.EndAddress);
                    //Data.Add = oMemEntry.DeviceRead(EntryAddress, len, StartIndex);
                    EntryAddress += len;
                    StartIndex += len;
                }

            mem_read:
                if (a20addr < this.MemorySize && !is_bios)
                {
                    // all of data is within limits of physical memory
                    if (a20addr < 0x000a0000 || a20addr >= 0x00100000)
                    {
                        if (Length == 8)
                        {
                            ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 8);
                            //WriteHostQWordToLittleEndian(BX_MEM_THIS get_vector(a20addr), *(Bit64u*)data);
                            
                            return Read (a20addr, 8);
                        }
                        if (Length == 4)
                        {
                            ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 4);
                            //WriteHostDWordToLittleEndian(BX_MEM_THIS get_vector(a20addr), *(Bit32u*)data);

                            return Read(a20addr, 4);
                        }
                        if (Length == 2)
                        {
                            ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 2);
                            //WriteHostWordToLittleEndian(BX_MEM_THIS get_vector(a20addr), *(Bit16u*)data);

                            return Read(a20addr, 2);
                        }
                        if (Length == 1)
                        {
                            ((CPU.CPU)(this.PCBoard.CPU)).PageWriteStampTable.decWriteStamp(a20addr, 1);
                            byte[] Data = new byte[1];
                            Data[0] = ReadByte(a20addr);
                            return Data;
                        }
                        // len == other, just fall thru to special cases handling
                    }
                    /*
                     * 
#ifdef BX_LITTLE_ENDIAN
  data_ptr = (Bit8u *) data;
#else // BX_BIG_ENDIAN
  data_ptr = (Bit8u *) data + (len - 1);
#endif

  if (a20addr < 0x000a0000 || a20addr >= 0x00100000)
  {
    // addr *not* in range 000A0000 .. 000FFFFF
    while(1) {
      *data_ptr = *(BX_MEM_THIS get_vector(a20addr));
      if (len == 1) return;
      len--;
      a20addr++;
#ifdef BX_LITTLE_ENDIAN
      data_ptr++;
#else // BX_BIG_ENDIAN
      data_ptr--;
#endif
    }
  }

  // addr must be in range 000A0000 .. 000FFFFF

  for (unsigned i=0; i<len; i++) {

    // SMMRAM
    if (a20addr < 0x000c0000) {
      // devices are not allowed to access SMMRAM under VGA memory
      if (cpu) *data_ptr = *(BX_MEM_THIS get_vector(a20addr));
      goto inc_one;
    }

#if BX_SUPPORT_PCI
    if (BX_MEM_THIS pci_enabled && ((a20addr & 0xfffc0000) == 0x000c0000))
    {
      switch (DEV_pci_rd_memtype((Bit32u) a20addr)) {
        case 0x0:  // Read from ROM
          if ((a20addr & 0xfffe0000) == 0x000e0000) {
            // last 128K of BIOS ROM mapped to 0xE0000-0xFFFFF
            *data_ptr = BX_MEM_THIS rom[BIOS_MAP_LAST128K(a20addr)];
          }
          else {
            *data_ptr = BX_MEM_THIS rom[(a20addr & EXROM_MASK) + BIOSROMSZ];
          }
          break;
        case 0x1:  // Read from ShadowRAM
          *data_ptr = *(BX_MEM_THIS get_vector(a20addr));
          break;
        default:
          BX_PANIC(("readPhysicalPage: default case"));
      }
    }
    else
#endif  // #if BX_SUPPORT_PCI
    {
      if ((a20addr & 0xfffc0000) != 0x000c0000) {
        *data_ptr = *(BX_MEM_THIS get_vector(a20addr));
      }
      else if ((a20addr & 0xfffe0000) == 0x000e0000) {
        // last 128K of BIOS ROM mapped to 0xE0000-0xFFFFF
        *data_ptr = BX_MEM_THIS rom[BIOS_MAP_LAST128K(a20addr)];
      }
      else {
        *data_ptr = BX_MEM_THIS rom[(a20addr & EXROM_MASK) + BIOSROMSZ];
      }
    }

inc_one:
    a20addr++;
#ifdef BX_LITTLE_ENDIAN
    data_ptr++;
#else // BX_BIG_ENDIAN
    data_ptr--;
#endif
                    
  }
}
else  // access outside limits of physical memory
{
#if BX_PHY_ADDRESS_LONG
  if (a20addr > BX_CONST64(0xffffffff)) {
    memset(data, 0xFF, len);
    return;
  }
#endif

#ifdef BX_LITTLE_ENDIAN
  data_ptr = (Bit8u *) data;
#else // BX_BIG_ENDIAN
  data_ptr = (Bit8u *) data + (len - 1);
#endif

  if (a20addr >= (bx_phy_address)~BIOS_MASK) {
    for (unsigned i = 0; i < len; i++) {
      *data_ptr = BX_MEM_THIS rom[a20addr & BIOS_MASK];
       a20addr++;
#ifdef BX_LITTLE_ENDIAN
       data_ptr++;
#else // BX_BIG_ENDIAN
       data_ptr--;
#endif
    }
  }
  else {
    memset(data, 0xFF, len);
  }
                     * */

                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address">Start memory address</param>
        /// <param name="Length">Number of bytes to read</param>
        /// <returns></returns>
        public override byte[] Read(UInt64 Address, UInt64 Length)
        {
            byte[] Result = new byte[Length];

            for (UInt64 i = 0; i < Length; ++i)
            {
                Result[i] = mMemory[Address + i];
            }

            return Result;
        }

        /// <summary>
        /// Writes one byte to memory
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Value"></param>
        public override void WriteByte(UInt64 Address, byte Value)
        {
            mMemory[Address] = (byte)Value;
        }


        /// <summary>
        /// Writes two bytes to memory
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Value"></param>
        public override void WriteWord(UInt64 Address, byte[] Word)
        {
            mMemory[Address] = Word[0];
            mMemory[Address+1] = Word[1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Value"></param>
        public override void Write(UInt64 Address, UInt64 Length, byte[] Data)
        {
            for (UInt64 i = 0; i < Length; ++i)
            {
                mMemory[Address + i] =Data[i];
            }
        }

        /// <summary>
        /// Read one byte from memory
        /// </summary>
        /// <param name="Address">Start memory address</param>
        /// <returns></returns>
        public override byte ReadByte(UInt64 Address)
        {
            return (byte)mMemory[Address];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address">Start memory address</param>
        /// <returns></returns>
        public override UInt16 ReadWord(UInt64 Address)
        {
            return (UInt16)BitConverter.ToInt16(Read(Address, 2), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address">Start memory address</param>
        /// <returns></returns>
        public override UInt32 ReadDWord(UInt64 Address)
        {
            return (UInt32)BitConverter.ToInt32(Read(Address, 4), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address">Start memory address</param>
        /// <returns></returns>
        public override UInt64 ReadLong(UInt64 Address)
        {
            return (UInt64)BitConverter.ToInt32(Read(Address, 8), 0);
        }



        /// <summary>
        /// Load BIOS binary.
        /// </summary>
        /// <param name="FileName">file path of the bios</param>
        /// <param name="Address">start address: 
        /// <remarks>BIOS E0000 .. FFFFF</remarks></param>
        public void LoadBinary(System.IO.FileStream oFileStream, UInt64 Offset)
        {
            byte[] mReadFile = new byte[oFileStream.Length];
            oFileStream.Read(mReadFile, (int)0, (int)(oFileStream.Length - 1));
            Write(Offset, (ulong)oFileStream.Length, mReadFile);
        }



        public void LoadROM(string FileName, UInt64 Address, Enum_RomType RomType, bool IsBochRom)
        {
            UInt64 uSize;
            UInt64 uMaxSize;
            UInt64 uOffset;
            UInt64 uStartIndex;
            UInt64 uEndIndex;


            System.IO.FileStream oFS = System.IO.File.OpenRead(FileName);
            uSize = (UInt64)oFS.Length;
            if (RomType != Enum_RomType.SystemBios)
            {
                uMaxSize = 0x20000;
            }
            else
            {
                uMaxSize = const_BIOSROMSZ;
            }
            if (uSize > uMaxSize)
            {
                throw new Exception("ROM: ROM image too large");
            }
            if (RomType == Enum_RomType.SystemBios)
            {
                if (Address > 0)
                {
                    if ((Address + uSize) != 0x100000 && (Address + uSize) > 0)
                    {
                        throw new Exception("ROM: System BIOS must end at 0xfffff");
                    }
                }
                else
                {
                    // ToDO: check this convension
                    Address = (UInt64)(-(Int64)uSize); // = -size
                }
                uOffset = (UInt64)(Address & const_BIOS_MASK);
                if ((UInt64)(Address & 0xf0000) < (UInt64)0xf0000)
                {
                    // BX_MEM_THIS rom_present[64] = 1;
                }
            }
            else
            {
                if ((uSize % 512) != 0)
                {
                    throw new Exception("ROM: ROM image size must be multiple of 512");
                }
                if ((Address % 2048) != 0)
                {
                    throw new Exception("ROM: ROM image must start at a 2k boundary");
                }

                if ((Address < 0xc0000) ||
                (((Address + uSize - 1) > 0xdffff) && (Address < 0xe0000)))
                {
                    throw new Exception("ROM: ROM address space out of range");
                }

                if (Address < 0xe0000)
                {
                    uOffset = (Address & const_EXROM_MASK) + const_BIOSROMSZ;
                    uStartIndex = ((Address - 0xc0000) >> 11);
                    uEndIndex = uStartIndex + (uSize >> 11) + (UInt64)(((uSize % 2048) > 0) ? 1 : 0);
                }
                else
                {
                    uOffset = Address & const_BIOS_MASK;
                    uStartIndex = 64;
                    uEndIndex = 64;
                }
            }
            LoadBinary(oFS, uOffset);

            #region "Check SUM"
            // TODO
            #endregion

        }


        /// <summary>
        /// Adds a memory mapped device to memory manager.
        /// <para>This function throws exception if the given input intersects with a currently registered address.</para>
        /// </summary>
        /// <param name="oMemoryDeviceEntry"></param>
        public override void RegisterDevice(MemoryResourceEntry oMemoryDeviceEntry)
        {
            foreach (var entry in this.mlstMemoryDeviceEntry)
            {
                // Check Intersection
                if (
                    ((entry.StartAddress <= oMemoryDeviceEntry.StartAddress)
                 && (entry.EndAddress >= oMemoryDeviceEntry.StartAddress))
                    ||
                ((entry.StartAddress <= oMemoryDeviceEntry.EndAddress)
                 && (entry.EndAddress >= oMemoryDeviceEntry.EndAddress))
                    )
                {
                    throw new Exception("Bad Machine Configuration -- Addresses Intersect");
                }
            }

            mlstMemoryDeviceEntry.Add(oMemoryDeviceEntry);
        }

        public override MemoryResourceEntry GetMemoryEntry(UInt64 Address)
        {
            foreach (var entry in this.mlstMemoryDeviceEntry)
            {
                if (
                    ((entry.StartAddress <= Address)
                 && (entry.EndAddress >= Address)))
                {
                    return entry;
                }
            }

            return null;
        }


        #endregion

    }
}
