using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.IO;
using Core.CPU;
using Core.Memory;
using Core.Simulator;
using Definitions.Enumerations;

namespace Core.PCBoard
{
    public class PCBoard: DeviceBase
    {
        #region "Enumeration"

        public enum Enum_MachineStatus
        {
            Stopped,
            Stopping,
            Start,
            Starting,
            Paused,
            IsPausing
        };

        #endregion 

        #region "Constants"

        #endregion 

        #region "Attributes"
        #region "Config Support Attributes"

        protected bool mSUPPORT_PHY_ADDRESS_LONG = true ; // BX_PHY_ADDRESS_LONG
        #endregion 


        protected Scheduler mScheduler=new Scheduler();
        protected Enum_MachineStatus mMachineStatus;

        protected CPUBase  mCPU;
        protected IOManager mIOManager; 
        protected MemoryBase mMemory;


        #region "A20"
        /// <summary>
        /// The A20 Address Line is the physical representation of the 21st bit (number 20, counting from 0) of any memory access. 
        /// <para>When the IBM-AT (Intel 286) was introduced, it was able to access up to sixteen megabytes of memory (instead of the 1 MByte of the 8086). </para>
        /// <para>But to remain compatible with the 8086, a quirk in the 8086 architecture (memory wraparound) had to be duplicated in the AT. To achieve this, the A20 line on the address bus was disabled by default.</para>
        /// </summary>
        protected Enum_Signal mA20;


        protected bool mSupportA20;

        /// <summary>
        /// <para>Address line 20 control:</para>
        /// <para>  1 = enabled: extended memory is accessible</para>
        /// <para>  0 = disabled: A20 address line is forced low to simulate</para>
        /// <para>      an 8088 address map</para>
        /// </summary>
        protected bool mEnable_A20;

        /// <summary>
        /// <para>Start out masking physical memory addresses to:</para>
        /// <para>  8086:      20 bits</para>
        /// <para>  286:      24 bits</para>
        /// <para>  386:      32 bits</para>
        /// <para>When A20 line is disabled, mask physical memory addresses to:</para>
        /// <para>  286:      20 bits</para>
        /// <para>  386:      20 bits</para>
        /// </summary>
        protected UInt32 mA20_Mask;

        #endregion


        #endregion

        #region "Properties"

        #region "Config Support Attributes"

        public bool SUPPORT_PHY_ADDRESS_LONG
        {
            get
            {
                return mSUPPORT_PHY_ADDRESS_LONG;
            }
            set
            {
                mSUPPORT_PHY_ADDRESS_LONG = value;
            }
        }


        #endregion 



        #region "A20"

        /// <summary>
        /// The A20 Address Line is the physical representation of the 21st bit (number 20, counting from 0) of any memory access. 
        /// <para>When the IBM-AT (Intel 286) was introduced, it was able to access up to sixteen megabytes of memory (instead of the 1 MByte of the 8086). </para>
        /// <para>But to remain compatible with the 8086, a quirk in the 8086 architecture (memory wraparound) had to be duplicated in the AT. To achieve this, the A20 line on the address bus was disabled by default.</para>
        /// </summary>
        public Enum_Signal A20
        {
            get
            {
                return mA20;
            }
            set
            {
                mA20 = value;
            }
        }

        /// <summary>
        /// <para>Address line 20 control:</para>
        /// <para>  1 = enabled: extended memory is accessible</para>
        /// <para>  0 = disabled: A20 address line is forced low to simulate</para>
        /// <para>      an 8088 address map</para>
        /// </summary>
        public bool Enable_A20
        {
            get
            {
                return mEnable_A20;
            }
            set
            {
                mEnable_A20 = value;
            }
        }

        /// <summary>
        /// <para>Start out masking physical memory addresses to:</para>
        /// <para>  8086:      20 bits</para>
        /// <para>  286:      24 bits</para>
        /// <para>  386:      32 bits</para>
        /// <para>When A20 line is disabled, mask physical memory addresses to:</para>
        /// <para>  286:      20 bits</para>
        /// <para>  386:      20 bits</para>
        /// </summary>
        public UInt32 A20_Mask
        {
            get
            {
                return mA20_Mask;
            }
            set
            {
                mA20_Mask = value;
            }
        }

        public bool SupportA20
        {
            get
            {
                return mSupportA20;
            }
            set
            {
                mSupportA20 = value;
            }

        }
        #endregion

        public virtual CPUBase CPU
        {
            get
            {
                return mCPU;
            }
        }

        public MemoryBase Memory
        {
            get
            {
                return mMemory;
            }

        }

        public IOManager IOManager
        {
            get
            {
                return mIOManager;
            }
        }

        public Scheduler Scheduler
        {
            get
            {
                return mScheduler;
            }
        }

        #endregion

        #region "Constructor"
        public PCBoard()
        {
            mIOManager = new IOManager();
            


            #region "Register Events"
            Core.Monitor.EventRegisterar.AddEvent("PCBorad.Start");
            Core.Monitor.EventRegisterar.AddEvent("PCBorad.Stop");
            #endregion 

       
        }

        #endregion 

        #region "Methods"

        #region "Signals"

        public void SetHOLD(Definitions.Enumerations.Enum_Signal Signal)
        {
        }

        public void SetHLDA(Definitions.Enumerations.Enum_Signal Signal)
        {
        }

        #endregion

        #region "Start Stop"

        /// <summary>
        /// Start a machine.
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stop a machine
        /// </summary>
        public virtual void Stop()
        {
            throw new NotImplementedException();
        }


        public virtual void Pause()
        {
            throw new NotImplementedException();
        }


        public virtual void SaveAndClose()
        {
            throw new NotImplementedException();
        }


      

        #endregion

        #region "Board Communication"
        #region "A20"
        public void SetEnableA20(Enum_Signal Value)
        {
            Enum_Signal  OldEnable = A20 ;

            if (Value == Enum_Signal.High)
            {
                A20 = Enum_Signal.High;
                A20_Mask = 0xffffffff;
            }
            else
            {
                A20 = Enum_Signal.Low ;
                A20_Mask = 0xffefffff;
            }
 
            // If there has been a transition, we need to notify the CPUs so
            // they can potentially invalidate certain cache info based on
            // A20-line-applied physical addresses.
            if (OldEnable != Value)
            {
                mCPU.OnA20Change();
            }
        }

        public void EvaluateA20Mask()
        {
            if (Enable_A20)
            {
                A20_Mask = 0xffffffff;
            }
            else
            {
                A20_Mask = 0x1fffff;
            }
        }

        /// <summary>
        /// Bochs: A20ADDR
        /// </summary>
        /// <param name="PhysicalAddress"></param>
        /// <returns></returns>
        public UInt64 A20Addr(UInt64 PhysicalAddress)
        {
            if (SupportA20)
            {
                return PhysicalAddress & this.mA20_Mask;
            }
            else
            {
                return PhysicalAddress;
            }

        }
        #endregion 

        #region "DMA"
        public virtual void RegisterDMA8Channel(uint Channel, Definitions.Delegates.delegate_DMAWrite DMAWrite, Definitions.Delegates.delegate_DMARead DMARead)
        {
            throw new NotImplementedException();
        }
        public virtual void RegisterDMA16Channel(uint Channel, Definitions.Delegates.delegate_DMAWrite DMAWrite, Definitions.Delegates.delegate_DMARead DMARead)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region "IRQ"

        public virtual void IRQRegister(IRQDeviceEntry oIRQDeviceEntry)
        {
            throw new NotImplementedException();
        }
        public virtual void IRQRaise(uint IRQ)
        {
            throw new NotImplementedException();
        }

        public virtual void IRQLow(uint IRQ)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #endregion
        #endregion

    }
}
