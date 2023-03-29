using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Core.CPU;
using Core.PCBoard;
using Definitions.Enumerations;


namespace Core.Memory
{
    public class MemoryBase : Component
    {

        #region "Constants"

        public const UInt32 const_BIOSROMSZ = (UInt32)(1 << 21); //   2M BIOS ROM @0xffe00000, must be a power of 2
        public const UInt32 const_EXROMSIZE = (UInt32)(0x20000); // ROMs 0xc0000-0xdffff (area 0xe0000-0xfffff=bios mapped)
        public const UInt32 const_BIOS_MASK = (UInt32)const_BIOSROMSZ - 1;
        public const UInt32 const_EXROM_MASK = (UInt32)const_EXROMSIZE - 1;
        public const UInt32 const_MEM_BLOCK_LEN = (1024 * 1024);//BX_MEM_BLOCK_LEN;
        public const UInt32 const_MEM_VECTOR_ALIGN = 4096;
        #endregion

        #region "Attributes"


        protected byte[] mMemory = null;
        protected UInt64 mMemorySize;
        protected byte mVector;
        protected byte[] mActualVector;
        protected UInt64?[] mBlocks;
        protected UInt64 mAllocatedBlocks;
        protected UInt32 mUsedBlocks;
        protected UInt32 mROMPointer;  // rom 
        protected UInt32 mBogusPointer;
        protected List<MemoryResourceEntry> mlstMemoryDeviceEntry = new List<MemoryResourceEntry>();
        protected PCBoard.PCBoard mPCBoard = null;
        protected bool mPCIEnabled;
        #region "SMRAM"
        protected bool mSMRAM_Available;
        protected bool mSMRAM_Enable;
        protected bool mSMRAM_Restricted;
        #endregion

        #endregion


        #region "Properties"


        public PCBoard.PCBoard PCBoard
        {
            get
            {
                return mPCBoard;
            }

            set
            {
                mPCBoard = value;
            }
        }

        public virtual UInt64 AllocatedBlocks
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public virtual UInt64 UsedBlocks
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Current memory size in bytes.
        /// </summary>
        public virtual UInt64 MemorySize
        {
            get
            {
                throw new NotImplementedException();
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

        public MemoryBase(PCBoard.PCBoard oPCBoard)
        {
            mPCBoard = oPCBoard;
        }

        #endregion


        #region"Methods"

        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }


        public UInt32 BIOS_Map_Last128K(UInt32 Address)
        {
            return ((Address | 0xfff00000) & const_BIOS_MASK);
        }

        public virtual void AddMemoryResource(MemoryResourceEntry oMemoryResourceEntry)
        {
            mlstMemoryDeviceEntry.Add(oMemoryResourceEntry);
        }


        public void DisableSMRAM()
        {
            SMRAM_Available = false;
            SMRAM_Enable = false;
            SMRAM_Restricted = false;
        }

        public void EnableSMRAM(bool Enable, bool Restricted)
        {
            SMRAM_Available = true;
            SMRAM_Enable = Enable;
            SMRAM_Restricted = Restricted;
        }


        /// <summary>
        /// <para>Memory map inside the 1st megabyte:</para>
        /// 
        /// <para>Return a host address corresponding to the guest physical memory</para>
        /// <para>address (with A20 already applied), given that the calling</para>
        /// <para>code will perform an 'op' operation.  This address will be</para>
        /// <para>used for direct access to guest memory.</para>
        /// <para>Values of 'op' are { BX_READ, BX_WRITE, BX_EXECUTE, BX_RW }.</para>
        ///
        /// <para>The other assumption is that the calling code _only_ accesses memory</para>
        /// <para>directly within the page that encompasses the address requested.</para>
        /// <para></para>
        /// <para>0x00000 - 0x7ffff    DOS area (512K)</para>
        /// <para>0x80000 - 0x9ffff    Optional fixed memory hole (128K)</para>
        /// <para>0xa0000 - 0xbffff    Standard PCI/ISA Video Mem / SMMRAM (128K)</para>
        /// <para>0xc0000 - 0xdffff    Expansion Card BIOS and Buffer Area (128K)</para>
        /// <para>0xe0000 - 0xeffff    Lower BIOS Area (64K)</para>
        /// <para>0xf0000 - 0xfffff    Upper BIOS Area (64K)</para>
        /// </summary>
        /// <param name="oCPU"></param>
        /// <param name="PhysicalAddress"></param>
        /// <param name="RW"></param>
        /// <returns></returns>
        public UInt64? GetHostMemoryAddress(UInt64 PhysicalAddress, Enum_MemoryAccessType RW)
        {
            UInt32 uA20Addr = (UInt32)mPCBoard.A20Addr(PhysicalAddress);

            bool bIsBios = (bool)(uA20Addr >= (UInt32)(~MemoryBase.const_BIOS_MASK));

            #region "SUPPORT_APIC"
            #endregion

            #region "SMM Read"
            // allow direct access to SMRAM memory space for code and veto data
            if (RW == Enum_MemoryAccessType.Execute)
            {
                // reading from SMRAM memory space
                if ((uA20Addr >= 0x000a0000 && uA20Addr < 0x000c0000)
                    && (mSMRAM_Available))
                {
                    if (SMRAM_Enable ||
                        ((this.PCBoard.CPU.CPUMode == Enum_CPUModes.SystemManagementMode) && (mSMRAM_Restricted == false)))
                    {
                        // get_vector(a20addr)
                    }
                }
            }
            #endregion
            #region "Memory Handler"

            #endregion

            if ((RW & Enum_MemoryAccessType.Write) == 0)
            {
                if ((uA20Addr >= 0x000a0000) && (uA20Addr < 0x000c0000))
                    return null; // invalid [Mem mapped IO (VGA)

                #region "PCI Support"
                if ((this.mPCIEnabled == true) && (uA20Addr >= 0x000c0000 && uA20Addr < 0x00100000))
                {
                    /*
                      switch (DEV_pci_rd_memtype ((Bit32u) a20addr)) {
                        case 0x0:   // Read from ROM
                        if ((a20addr & 0xfffe0000) == 0x000e0000) {
                        // last 128K of BIOS ROM mapped to 0xE0000-0xFFFFF
                        return (Bit8u *) &BX_MEM_THIS rom[BIOS_MAP_LAST128K(a20addr)];
                        }
                        else {
                        return (Bit8u *) &BX_MEM_THIS rom[(a20addr & EXROM_MASK) + BIOSROMSZ];
                        }
                        break;
                     */
                    // Read from ROM
                    if ((uA20Addr & 0xfffe0000) == 0x000e0000)
                    {
                        // last 128K of BIOS ROM mapped to 0xE0000-0xFFFFF
                        return this.BIOS_Map_Last128K(uA20Addr);
                    }
                    else
                    {
                        return (uA20Addr & const_EXROM_MASK) + const_BIOSROMSZ;
                    }
                }
                #endregion

                if ((uA20Addr < mMemorySize) && (bIsBios == false))
                {
                    if (uA20Addr < 0x000c0000 || uA20Addr >= 0x00100000)
                    {
                        return this.GetVector(uA20Addr );
                    }
                    else
                    {
                        // must be in C0000 - FFFFF range
                        if ((uA20Addr & 0xfffe0000) == 0x000e0000)
                        {
                            return (UInt32)((UInt32)uA20Addr & (UInt32)MemoryBase.const_BIOS_MASK);  // (Bit8u *) &BX_MEM_THIS rom[a20addr & BIOS_MASK];
                        }
                        else
                        {
                            return (uA20Addr & const_EXROM_MASK) + const_BIOSROMSZ; //((Bit8u *) &BX_MEM_THIS rom[(a20addr & EXROM_MASK) + BIOSROMSZ]);
                        }
                    }
                }

                // if (a20addr >= (bx_phy_address)~BIOS_MASK)
                if (bIsBios)
                {
                    return (UInt32)((UInt32)uA20Addr & (UInt32)MemoryBase.const_BIOS_MASK); //(Bit8u *) &BX_MEM_THIS rom[a20addr & BIOS_MASK];
                }
                else
                {
                    // Error, requested addr is out of bounds.
                    return mBogusPointer + (uA20Addr & 0xfff); // (Bit8u *) &BX_MEM_THIS bogus[a20addr & 0xfff];
                }
            }
            else
            { // op == {BX_WRITE, BX_RW}
                  if (uA20Addr >= this.MemorySize || bIsBios)
                        return 0x00; // Error, requested addr is out of bounds.
                  else if (uA20Addr >= 0x000a0000 && uA20Addr < 0x000c0000)
                  {
                      return null; // Vetoed!  Mem mapped IO (VGA)
                  }
                  else if ((this.mPCIEnabled == true) && (uA20Addr >= 0x000c0000 & uA20Addr < 0x00100000))
                  {
                      // Veto direct writes to this area. Otherwise, there is a chance
                      // for Guest2HostTLB and memory consistency problems, for example
                      // when some 16K block marked as write-only using PAM registers.
                      return null;
                  }
                  else if (uA20Addr < 0x000c0000 || uA20Addr >=0x00100000)
                  {
                      return (UInt64) GetVector(uA20Addr);
                  }
                      else
                      {
                          return null;
                      }
            }
            return null;
        }

        public virtual UInt64 GetVector(UInt64 PhysicalAddress)
        {
            throw new NotImplementedException(); ;
        }

        public virtual void WriteByte(UInt64 Address, byte Value)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteWord(UInt64 Address, byte[] Word)
        {
            throw new NotImplementedException();
        }

        public virtual void Write(UInt64 Address, UInt64 Length, byte[] Data)
        {
            throw new NotImplementedException();
        }


        public virtual byte ReadByte(UInt64 Address)
        {
            throw new NotImplementedException();
        }

        public virtual UInt16 ReadWord(UInt64 Address)
        {
            throw new NotImplementedException();
        }

        public virtual UInt32 ReadDWord(UInt64 Address)
        {
            throw new NotImplementedException();
        }

        public virtual UInt64 ReadLong(UInt64 Address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a memory entry to memory system.
        /// </summary>
        /// <param name="oMemoryDeviceEntry"></param>
        public virtual void RegisterDevice(MemoryResourceEntry oMemoryDeviceEntry)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// select which memory entry to write into.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public virtual MemoryResourceEntry GetMemoryEntry(UInt64 Address)
        {
            throw new NotImplementedException();
        }


        public virtual void WritePhysicalPage(UInt64 Address, byte[] Data, UInt64 Length)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] Read(UInt64 Address, UInt64 Length)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] ReadPhysicalPage(UInt64 Address, UInt64 Length)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
