using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.IO;
using Core.Memory;
using Core.PCBoard;
using Definitions.Delegates;
using Definitions.Enumerations;


namespace Devices.DMA
{

    /// <summary>
    /// 8237 DMA controller 
    /// <see cref="http://www.eie.polyu.edu.hk/~enyhchan/csf04_12.pdf"/>
    /// </summary>
    public class DMA : DeviceBase
    {

        #region "Attributes"
        protected DMARegister[] mS;
        protected Enum_Signal mHLDA;
        protected Enum_Signal mTC;         // TerminalCount;
        protected byte[] mExtPageReg; // Extra page registers (unused)

        // index to find channel from register number (only [0],[1],[2],[6] used)
        byte[] mChannelIndex = new byte[7] { 2, 3, 1, 0, 0, 0, 0 };

        #endregion


        #region"Properties"

        #endregion

        #region "Constructors"

        public DMA(Core.PCBoard.PCBoard PCBoard)
        {
            mPCBoard = PCBoard;
            mName = "8237 DMA controller";
            mExtPageReg = new byte[16];
            mS = new DMARegister[2];
            mS[0] = new DMARegister();
            mS[1] = new DMARegister();

        }

        #endregion


        #region "Methods"




        public override void Initialize()
        {


            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    mS[i].DRQ[j] = Enum_Signal.Low;
                    mS[i].DACK[j] = Enum_Signal.Low;
                }
            }

            mHLDA = Enum_Signal.Low;
            mTC = Enum_Signal.Low;


            // 0000..000F
            for (UInt64 i = 0x0000; i <= 0x000f; i++)
            {
                this.mListDeviceIO.Add(new IODeviceEntry(this, i, this.DeviceWriteByte, this.DeviceReadByte));
            }

            // 00080..008F
            for (UInt64 i = 0x0080; i <= 0x008f; i++)
            {
                this.mListDeviceIO.Add(new IODeviceEntry(this, i, this.DeviceWriteByte, this.DeviceReadByte));
            }

            // 000C0..00DE
            for (UInt64 i = 0x00C0; i <= 0x00DE; i++)
            {
                this.mListDeviceIO.Add(new IODeviceEntry(this, i, this.DeviceWriteByte, this.DeviceReadByte));
            }

            // Register Ports on Board
            foreach (var Entry in mListDeviceIO)
            {
                mPCBoard.IOManager.DeviceList.Add(Entry.IOPortNubmer, Entry);
            }

            for (int i = 0; i < 2; ++i)
            {
                for (int c = 0; c < 4; ++c)
                {
                    mS[i].Channel[c].Mode_ModeType = 0;             // demand mode
                    mS[i].Channel[c].Mode_AddressDecrement = 0;     // address increment
                    mS[i].Channel[c].Mode_AutoInitEnable = 0;       // autoinit disable
                    mS[i].Channel[c].Mode_TransferType = 0;         // verify
                    mS[i].Channel[c].CurrentAddress = 0;
                    mS[i].Channel[c].BaseCount = 0;
                    mS[i].Channel[c].CurrentCount = 0;
                    mS[i].Channel[c].PageReg = 0;
                    mS[i].Channel[c].Used = false;
                }
            }

            mS[1].Channel[0].Used = true; // Cascade channel in use.

        }

        public override void Reset(Enum_ResetType Type)
        {
            ResetController(0);
            ResetController(1);
        }

        protected void ResetController(uint Selector)
        {
            mS[Selector].Mask[0] = true;
            mS[Selector].Mask[1] = true;
            mS[Selector].Mask[2] = true;
            mS[Selector].Mask[3] = true;
            mS[Selector].ControlDisabled = false;
            mS[Selector].CommandRegister = 0;
            mS[Selector].StatusRegister = 0;
            mS[Selector].FlipFlop = Enum_Signal.Low;
        }


        /// <summary>
        /// HRQ: DMA request output
        /// </summary>
        /// <param name="Selector"></param>
        protected void ControlHRQ(int Selector)
        {
            // do nothing if controller is disabled
            if (mS[Selector].ControlDisabled == true)
                return;

            // deassert HRQ if no DRQ is pending
            if ((mS[Selector].StatusRegister & 0xf0) == 0)
            {
                if (Selector == 1)
                {
                    mPCBoard.SetHOLD(Enum_Signal.Low);
                }
                else
                {
                    SetDRQ(4, false);
                }
                return;
            }
            // find highest priority channel
            for (int channel = 0; channel < 4; channel++)
            {
                if (((mS[Selector].StatusRegister & (1 << (channel + 4))) != 0) &&
                    (mS[Selector].Mask[channel] == false))
                {
                    if (Selector == 1)
                    {
                        // assert Hold ReQuest line to CPU
                        mPCBoard.SetHOLD(Enum_Signal.High);
                    }
                    else
                    {
                        // send DRQ to cascade channel of the master
                        SetDRQ(4, true);
                    }
                    break;
                }
            }


        }



        protected void SetDRQ(uint Channel, bool Value)
        {
            UInt32 dma_base, dma_roof;
            int mSelect;

            if (Channel > 7)
            {
                throw new Exception("set_DRQ() channel > 7");
            }
            mSelect = (Channel > 3) ? 1 : 0;

            mS[mSelect].DRQ[Channel & 0x03] = (Value == true) ? Enum_Signal.High : Enum_Signal.Low;
            if (mS[mSelect].Channel[Channel & 0x03].Used == true)
            {
                throw new Exception("set_DRQ(): channel %d not connected to device");

            }
            Channel &= 0x03;
            if (Value == false)
            {
                // clear bit in status reg
                mS[mSelect].StatusRegister &= (byte)(~((byte)1 << ((byte)(Channel + 4))));

                ControlHRQ(mSelect);
                return;
            }
        }


        #region "Register RW DMA Functions"
        public void RegisterDMA8Channel(uint Channel, delegate_DMAWrite DMAWrite, delegate_DMARead DMARead)
        {
            if (Channel > 3)
            {
                //BX_PANIC(("registerDMA8Channel: invalid channel number(%u).", channel));
                throw new InvalidOperationException("registerDMA8Channel: invalid channel ");
            }
            if (mS[0].Channel[Channel].Used == true)
            {
                //BX_PANIC(("registerDMA8Channel: channel(%u) already in use.", channel));
                throw new InvalidOperationException("registerDMA8Channel: channel(%u) already in use");
            }
            //BX_INFO(("channel %u used by %s", channel, name));
            mS[0].Channel[Channel].Used = true;
            mS[0].Channel[Channel].DMAWrite = DMAWrite;
            mS[0].Channel[Channel].DMARead = DMARead;

            return;
        }

        public void RegisterDMA16Channel(uint Channel, delegate_DMAWrite DMAWrite, delegate_DMARead DMARead)
        {
            if ((Channel < 4) || (Channel > 7))
            {
                //BX_PANIC(("registerDMA8Channel: invalid channel number(%u).", channel));
                throw new InvalidOperationException("registerDMA16Channel: invalid channel ");
            }
            if (mS[1].Channel[Channel & 0x03].Used == true)
            {
                //BX_PANIC(("registerDMA8Channel: channel(%u) already in use.", channel));
                throw new InvalidOperationException("registerDMA16Channel: channel(%u) already in use");
            }
            //BX_INFO(("channel %u used by %s", channel, name));
            Channel &= 0x03;
            mS[0].Channel[Channel].Used = true;
            mS[0].Channel[Channel].DMAWrite = DMAWrite;
            mS[0].Channel[Channel].DMARead = DMARead;

            return;
        }

        #endregion 

        #region "RW IO Functions"
        public override void DeviceWriteByte(UInt64 Address, byte Value)
        {
            byte ChannelIdx;
            byte SetMaskBit;
            int mSelect = 0;
            if (Address >= 0xc0) mSelect = 1;
            switch (Address)
            {
                case 0x00:
                case 0x02:
                case 0x04:
                case 0x06:
                case 0xc0:
                case 0xc4:
                case 0xc8:
                case 0xcc:
                    ChannelIdx = (byte)((Address >> (1 + mSelect)) & 0x03);
                    //BX_DEBUG(("  DMA-%d base and current address, channel %d", mSelect+1, channel));
                    if (mS[mSelect].FlipFlop == Enum_Signal.Low)
                    { /* 1st byte */
                        mS[mSelect].Channel[ChannelIdx].BaseAddress = Value;
                        mS[mSelect].Channel[ChannelIdx].CurrentAddress = Value;
                    }
                    else
                    { /* 2nd byte */
                        mS[mSelect].Channel[ChannelIdx].BaseAddress |= (byte)(Value << 8);
                        mS[mSelect].Channel[ChannelIdx].CurrentAddress |= (byte)(Value << 8);
                        //BX_DEBUG(("    base = %04x",(unsigned)mS[mSelect].Channel[ChannelIdx].base_address));
                        //BX_DEBUG(("    curr = %04x",(unsigned)mS[mSelect].Channel[channel].current_address));
                    }
                    mS[mSelect].FlipFlop = (mS[mSelect].FlipFlop == Enum_Signal.High) ? Enum_Signal.Low : Enum_Signal.High;
                    break;

                case 0x01:
                case 0x03:
                case 0x05:
                case 0x07:
                case 0xc2:
                case 0xc6:
                case 0xca:
                case 0xce:
                    ChannelIdx = (byte)((Address >> (1 + mSelect)) & 0x03);
                    //BX_DEBUG(("  DMA-%d base and current count, channel %d", mSelect+1, channel));
                    if (mS[mSelect].FlipFlop == Enum_Signal.Low)
                    { /* 1st byte */
                        mS[mSelect].Channel[ChannelIdx].BaseAddress = Value;
                        mS[mSelect].Channel[ChannelIdx].CurrentAddress = Value;
                    }
                    else
                    { /* 2nd byte */
                        mS[mSelect].Channel[ChannelIdx].BaseAddress |= (byte)(Value << 8);
                        mS[mSelect].Channel[ChannelIdx].CurrentAddress |= (byte)(Value << 8);
                        //BX_DEBUG(("    base = %04x",(unsigned)mS[mSelect].Channel[ChannelIdx].base_count));
                        //BX_DEBUG(("    curr = %04x",(unsigned)mS[mSelect].Channel[ChannelIdx].current_count));
                    }
                    mS[mSelect].FlipFlop = (mS[mSelect].FlipFlop == Enum_Signal.High) ? Enum_Signal.Low : Enum_Signal.High;
                    break;

                case 0x08: /* DMA-1: command register */
                case 0xd0: /* DMA-2: command register */
                    if ((Value & 0xfb) != 0x00)
                    {
                        throw new Exception("write to command register: Value 0x%02x not supported");
                    }
                    mS[mSelect].CommandRegister = Value;
                    mS[mSelect].ControlDisabled = (((Value >> 2) & 0x01) != 0);
                    ControlHRQ(mSelect);
                    break;

                case 0x09: // DMA-1: request register
                case 0xd2: // DMA-2: request register
                    ChannelIdx = (byte)(Value & 0x03);
                    // note: write to 0x0d / 0xda clears this register
                    if ((Value & 0x04) != 0)
                    {
                        // set request bit
                        mS[mSelect].StatusRegister |= (byte)((1 << (ChannelIdx + 4)));
                        //BX_DEBUG(("DMA-%d: set request bit for channel %u", ma_sl+1, (unsigned) channel));
                    }
                    else
                    {
                        // clear request bit
                        mS[mSelect].StatusRegister &= (byte)(~(1 << (ChannelIdx + 4)));
                        //BX_DEBUG(("DMA-%d: cleared request bit for channel %u", ma_sl+1, (unsigned) channel));
                    }
                    ControlHRQ(mSelect);
                    break;

                case 0x0a:
                case 0xd4:
                    SetMaskBit = (byte)(Value & 0x04);
                    ChannelIdx = (byte)(Value & 0x03);
                    mS[mSelect].Mask[ChannelIdx] = (SetMaskBit > 0);
                    //BX_DEBUG(("DMA-%d: set_mask_bit=%u, channel=%u, mask now=%02xh", ma_sl+1,         (unsigned) set_mask_bit, (unsigned) channel, (unsigned) BX_DMA_THIS s[ma_sl].mask[channel]));
                    ControlHRQ(mSelect);
                    break;


                case 0x0b: /* DMA-1 mode register */
                case 0xd6: /* DMA-2 mode register */
                    ChannelIdx = (byte)(Value & 0x03);
                    mS[mSelect].Channel[ChannelIdx].Mode_ModeType = (byte)((Value >> 6) & 0x03);
                    mS[mSelect].Channel[ChannelIdx].Mode_AddressDecrement = (byte)((Value >> 5) & 0x01);
                    mS[mSelect].Channel[ChannelIdx].Mode_AutoInitEnable = (byte)((Value >> 4) & 0x01);
                    mS[mSelect].Channel[ChannelIdx].Mode_TransferType = (byte)((Value >> 2) & 0x03);
                    //BX_DEBUG(("DMA-%d: mode register[%u] = %02x", ma_sl+1,  (unsigned) channel, (unsigned) Value));
                    break;

                case 0x0c: /* DMA-1 clear byte flip/flop */
                case 0xd8: /* DMA-2 clear byte flip/flop */
                    //BX_DEBUG(("DMA-%d: clear flip/flop", mSelect+1));
                    mS[mSelect].FlipFlop = Enum_Signal.Low;
                    break;

                case 0x0d: // DMA-1: master clear
                case 0xda: // DMA-2: master clear
                    //BX_DEBUG(("DMA-%d: master clear", mSelect+1));
                    // writing any Value to this port resets DMA controller 1 / 2
                    // same action as a hardware reset
                    // mask register is set (chan 0..3 disabled)
                    // command, status, request, temporary, and byte flip-flop are all cleared
                    ResetController((uint)mSelect);
                    break;

                case 0x0e: // DMA-1: clear mask register
                case 0xdc: // DMA-2: clear mask register
                    //BX_DEBUG(("DMA-%d: clear mask register", mSelect+1));
                    mS[mSelect].Mask[0] = false;
                    mS[mSelect].Mask[1] = false;
                    mS[mSelect].Mask[2] = false;
                    mS[mSelect].Mask[3] = false;
                    ControlHRQ(mSelect);
                    break;

                case 0x0f: // DMA-1: write all mask bits
                case 0xde: // DMA-2: write all mask bits
                    //BX_DEBUG(("DMA-%d: write all mask bits", mSelect+1));
                    mS[mSelect].Mask[0] = (bool)((Value & 0x01) == 1); Value >>= 1;
                    mS[mSelect].Mask[1] = (bool)((Value & 0x01) == 1); Value >>= 1;
                    mS[mSelect].Mask[2] = (bool)((Value & 0x01) == 1); Value >>= 1;
                    mS[mSelect].Mask[3] = (bool)((Value & 0x01) == 1);
                    ControlHRQ(mSelect);
                    break;

                case 0x81: /* DMA-1 page register, channel 2 */
                case 0x82: /* DMA-1 page register, channel 3 */
                case 0x83: /* DMA-1 page register, channel 1 */
                case 0x87: /* DMA-1 page register, channel 0 */
                    /* address bits A16-A23 for DMA channel */
                    ChannelIdx = mChannelIndex[Address - 0x81];
                    mS[0].Channel[ChannelIdx].PageReg = Value;
                    //BX_DEBUG(("DMA-1: page register %d = %02x", channel, (unsigned) Value));
                    break;


                case 0x0089: // DMA-2 page register, channel 2
                case 0x008a: // DMA-2 page register, channel 3
                case 0x008b: // DMA-2 page register, channel 1
                case 0x008f: // DMA-2 page register, channel 0
                    ChannelIdx = mChannelIndex[Address - 0x89];
                    mS[0].Channel[ChannelIdx].PageReg = Value;
                    // BX_DEBUG(("DMA-2: page register %d = %02x", channel + 4, (unsigned) Value));
                    break;

                case 0x0080:
                case 0x0084:
                case 0x0085:
                case 0x0086:
                case 0x0088:
                case 0x008c:
                case 0x008d:
                case 0x008e:
                    //BX_DEBUG(("write: extra page register 0x%04x (unused)", (unsigned) address));
                    this.mExtPageReg[Address & 0x0f] = Value;
                    break;

                default:
                    throw new InvalidOperationException("write ignored: %04xh = %02xh");

            }
        }

        public override byte DeviceReadByte(UInt64 Address)
        {
            return 0;
        }
        #endregion
        #endregion
    }
}
