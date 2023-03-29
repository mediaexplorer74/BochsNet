using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.PCBoard;
using Core.Memory;
using Core.IO;
using Core.CPU;
using Definitions.Enumerations;

namespace PCMachine
{
    /// <summary>
    /// Represents a PC Machine
    /// </summary>
    public class Machine : PCBoard
    {

        #region "Constants"

        public const int const_BASE_MEMORY_IN_K = 640;

        #endregion

        #region "Attributes"

        protected Devices.DMA.DMA mDMA;
        protected Devices.VGA.VGACard mVGACard;
        protected Devices.CMOS.CMOS mCMOS;
        protected Devices.PIC.PIC mPIC;
        protected UInt64 mMemorySize;


        #endregion

        #region "Properties"

        public Enum_MachineStatus MachineStatus
        {
            get
            {
                return mMachineStatus;
            }

            set
            {
                mMachineStatus = value;
            }
        }



        #endregion


        #region "Constructors"

        /// <summary>
        /// Machine constructor.
        /// </summary>
        public Machine()
        {
            mMachineStatus = Enum_MachineStatus.Stopped;
            mMemorySize = 0x02000000; // 33,554,432
        }


        /// <summary>
        /// Open a machine from a file.
        /// </summary>
        /// <param name="MachineFileName">File Path</param>
        public Machine(string MachineFileName)
            : this(new EmulatorFiles.MachineFile(MachineFileName))
        {

        }


        /// <summary>
        /// Open a machine from a file object.
        /// </summary>
        /// <param name="oMachineFile"></param>
        public Machine(EmulatorFiles.MachineFile oMachineFile)
            : this()
        {
            if (oMachineFile == null) throw new NotSupportedException("MachineFile is null");
        }

        #endregion



        #region "Methods"


        #region "Start Stop"

        /// <summary>
        /// Start a machine.
        /// </summary>
        public override void Start()
        {
            if (MachineStatus != Enum_MachineStatus.Stopped)
            {
                throw new InvalidOperationException("Machine status should be stopped.");
            }
            //Initialize();
            mScheduler.Enabled = true;
            mScheduler.Run();
        }

        /// <summary>
        /// Stop a machine
        /// </summary>
        public override void Stop()
        {
            if ((MachineStatus == Enum_MachineStatus.Stopped) || (MachineStatus == Enum_MachineStatus.Stopping))
            {
                return; // machine is already stopped.
            }

        }


        public override void Pause()
        {

        }


        public override void SaveAndClose()
        {

        }


        //public override void Reset(Enum_ResetType Type)
        //{

        //}

        #endregion



        public override void Initialize()
        {
            mIOManager.Initialize();
            mMemory = new Memory.Memory(this.mMemorySize,(PCBoard)this);

            mCMOS = new Devices.CMOS.CMOS(this);
            mDMA = new Devices.DMA.DMA(this);
            mPIC = new Devices.PIC.PIC(this);
            mVGACard = new Devices.VGA.VGACard(this);

            mMemory.Initialize();
            mPIC.Initialize();
            mCMOS.Initialize();
            mDMA.Initialize();
            

            // Init CPU
            mCPU = new CPU.CPU(this);
            // ToDo: this should be variable.
            mCPU.CPULevel = 6;         
            mCPU.MainMemory = mMemory;  // plug memory.

            // Set System Hardware
            mIOManager.AddIOResource(new IODeviceEntry(this, 0x0092, "Port 92h System Control", this.DeviceWriteByte, this.DeviceReadByte));

            // Set CMOS based on Memory
            UInt32 memory_in_k = (UInt32)(this.mMemory.MemorySize / 1024);
            UInt32 extended_memory_in_k = memory_in_k > 1024 ? (memory_in_k - 1024) : 0;
            if (extended_memory_in_k > 0xfc00) extended_memory_in_k = 0xfc00;
            
            // CMOS initialization
            mCMOS.StateInformation.Reg[0x15] = (byte)(const_BASE_MEMORY_IN_K & 0xff);
            mCMOS.StateInformation.Reg[0x16] = (byte)((const_BASE_MEMORY_IN_K >> 8) & 0xff);
            mCMOS.StateInformation.Reg[0x17] = (byte)(extended_memory_in_k & 0xff);
            mCMOS.StateInformation.Reg[0x18] = (byte)(((extended_memory_in_k >> 8) & 0xff));
            mCMOS.StateInformation.Reg[0x30] = (byte)(extended_memory_in_k & 0xff);
            mCMOS.StateInformation.Reg[0x31] = (byte)((extended_memory_in_k >> 8) & 0xff);

            UInt32 extended_memory_in_64k = memory_in_k > 16384 ? (memory_in_k - 16384) / 64 : 0;
            if (extended_memory_in_64k > 0xffff) extended_memory_in_64k = 0xffff;

            mCMOS.StateInformation.Reg[0x34] = (byte)(extended_memory_in_64k & 0xff);
            mCMOS.StateInformation.Reg[0x35] = (byte)((extended_memory_in_64k >> 8) & 0xff);

            //now perform checksum of CMOS memory
            mCMOS.CheckSUM();

            ResetDevice(Enum_ResetType.HardwareReset);
        }

        protected void ResetDevice(Enum_ResetType ResetType)
        {
            SetEnableA20(Enum_Signal.High);
            mCPU.ResetCPU();
            mMemory.DisableSMRAM();
            mCMOS.Reset(ResetType);
            mDMA.Reset(ResetType);
            //pluginFloppyDevice->reset(type);
            //mVGADevice.Reset();
            mPIC.Reset(ResetType);
            //pluginPitDevice->reset(type);
            //mPITDevice.Reset();
            //ResetOptionalPlugins
        }

        #region "RW Functions"
        public override void DeviceWriteByte(UInt64 Address, byte Value)
        {

        }

        public override byte DeviceReadByte(UInt64 Address)
        {
            return 0;
        }
        #endregion


        #region "DMA"
        public virtual void RegisterDMA8Channel(uint Channel, Definitions.Delegates.delegate_DMAWrite DMAWrite, Definitions.Delegates.delegate_DMARead DMARead)
        {
            mDMA.RegisterDMA8Channel(Channel, DMAWrite, DMARead);
        }
        public virtual void RegisterDMA16Channel(uint Channel, Definitions.Delegates.delegate_DMAWrite DMAWrite, Definitions.Delegates.delegate_DMARead DMARead)
        {
            mDMA.RegisterDMA16Channel(Channel, DMAWrite, DMARead);
        }

        #endregion


        #region "IRQ"

        public override void IRQRegister(IRQDeviceEntry oIRQDeviceEntry)
        {
            mPIC.IRQRegister(oIRQDeviceEntry);
        }
        public override void IRQRaise(uint IRQ)
        {
            mPIC.IRQRaise(IRQ);
        }

        public override void IRQLow(uint IRQ)
        {
            mPIC.IRQLow(IRQ);
        }

        #endregion
        #endregion

    }
}
