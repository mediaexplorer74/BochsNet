using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Memory;
using Core.IO;
using Core.PCBoard;
using Definitions.Enumerations;

namespace Devices.CMOS
{
    public class CMOS : DeviceBase
    {

        #region "Enumeration"
        public enum Enum_CMOSRegistery : byte
        {
            REG_SEC = 0x00,
            REG_SEC_ALARM = 0x01,
            REG_MIN = 0x02,
            REG_MIN_ALARM = 0x03,
            REG_HOUR = 0x04,
            REG_HOUR_ALARM = 0x05,
            REG_WEEK_DAY = 0x06,
            REG_MONTH_DAY = 0x07,
            REG_MONTH = 0x08,
            REG_YEAR = 0x09,
            REG_STAT_A = 0x0a,
            REG_STAT_B = 0x0b,
            REG_STAT_C = 0x0c,
            REG_STAT_D = 0x0d,
            REG_DIAGNOSTIC_STATUS = 0x0e, /* alternatives */
            REG_SHUTDOWN_STATUS = 0x0f,
            REG_EQUIPMENT_BYTE = 0x14,
            REG_CSUM_HIGH = 0x2e,
            REG_CSUM_LOW = 0x2f,
            REG_IBM_CENTURY_BYTE = 0x32, /* alternatives */
            REG_IBM_PS2_CENTURY_BYTE = 0x37  /* alternatives */
        }

        #endregion

        #region "Constants"

        public const int const_BX_NULL_TIMER_HANDLE = 10000;
        #endregion


        #region "Attributes"

        protected StateInformation mStateInformation = new StateInformation();

        #endregion

        #region "Properties"

        public StateInformation StateInformation
        {
            get
            {
                return mStateInformation;
            }
        }

        #endregion


        #region "Constructors"

        public CMOS(PCBoard PCBoard)
        {
            mPCBoard = PCBoard;
            mName = "CMOS";

            mStateInformation.PeriodicTimerIndex = new Core.Simulator.ScheduleEntry(this.PeriodicTimerHanlder, "CMOS PeriodicTimerHanlder", 0, true, 1000000, false);
            mStateInformation.OneSecondTimerIndex = new Core.Simulator.ScheduleEntry(this.OneSecondTimerHandler, "CMOS OneSecondTimerIndex", 0, true, 1000000, false);
            mStateInformation.UIPTimerIndex = new Core.Simulator.ScheduleEntry(this.UIP_TimerHandler, "CMOS UIPTimerIndex", 0, true, 244, false);

        }

        #endregion

        #region "Methods"

        public override void Initialize()
        {
            // BX_DEBUG(("Init $Id: cmos.cc,v 1.70 2009/04/23 18:28:17 sshwarts Exp $"));

            this.mListDeviceIO.Add(new IODeviceEntry(this, 0x0070, "CMOS RAM", this.DeviceWriteByte, this.DeviceReadByte));
            this.mListDeviceIO.Add(new IODeviceEntry(this, 0x0071, "CMOS RAM", this.DeviceWriteByte, this.DeviceReadByte));
            mPCBoard.IRQRegister(new IRQDeviceEntry(this, 8, "CMOS RTC"));


            // Register Ports on Board
            foreach (var Entry in mListDeviceIO)
            {
                mPCBoard.IOManager.DeviceList.Add(Entry.IOPortNubmer, Entry);
            }


            // CMOS values generated
            mStateInformation.REG_STAT_A = 0x26;
            mStateInformation.REG_STAT_B = 0x02;
            mStateInformation.REG_STAT_C = 0x00;
            mStateInformation.REG_STAT_D = 0x80;
            //#if BX_SUPPORT_FPU == 1
            mStateInformation.REG_EQUIPMENT_BYTE |= (byte)0x02;
            //#endif
            mStateInformation.RTCMode12Hour = false;
            mStateInformation.RTCModeBindary = false;

            mStateInformation.TimeVal = (UInt64)DateTime.Now.ToBinary();





        }

        /// <summary>
        /// <para> RESET affects the following registers:</para>  
        /// <para>  CRA: no effects</para>  
        /// <para>  CRB: bits 4,5,6 forced to 0</para>  
        /// <para>  CRC: bits 4,5,6,7 forced to 0</para>  
        /// <para>  CRD: no effects</para>  
        /// </summary>
        public override void Reset(Enum_ResetType Type)
        {
            mStateInformation.CMOSMemoryAddress = 0;

            // RESET affects the following registers:
            //  CRA: no effects
            //  CRB: bits 4,5,6 forced to 0
            //  CRC: bits 4,5,6,7 forced to 0
            //  CRD: no effects
            mStateInformation.REG_STAT_B &= 0x8f;
            mStateInformation.REG_STAT_C = 0;

            // One second timer for updating clock & alarm functions
            ////////bx_pc_system.activate_timer(mStateInformation.OneSecondTimerIndex ,1000000, 1);
            mStateInformation.OneSecondTimerIndex.Enabled = true;

            // handle periodic interrupt rate select
            CRA_Change();
        }

        public void CheckSUM()
        {
            UInt16 sum = 0;
            for (uint i = 0x10; i <= 0x2d; i++)
                sum += mStateInformation.Reg[i];
            mStateInformation.REG_CSUM_HIGH = (byte)((sum >> 8) & 0xff); /* checksum high */
            mStateInformation.REG_CSUM_LOW = (byte)((sum & 0xff));      /* checksum low */
        }

        public void CRA_Change()
        {
            byte nibble, dcc;

            // Periodic Interrupt timer
            nibble = (byte)(mStateInformation.REG_STAT_A & 0x0f);
            dcc = (byte)((mStateInformation.REG_STAT_A >> 4) & 0x07);
            if ((nibble == 0) || ((dcc & 0x06) == 0))
            {
                // No Periodic Interrupt Rate when 0, deactivate timer
                ////bx_pc_system.deactivate_timer(mStateInformation.PeriodicTimerIndex );
                mStateInformation.PeriodicTimerIndex.Enabled = false;
                mStateInformation.PeriodicIntervalUSec = (int)-1; // max value
            }
            else
            {
                // values 0001b and 0010b are the same as 1000b and 1001b
                if (nibble <= 2)
                    nibble += 7;
                mStateInformation.PeriodicIntervalUSec = (int)(1000000.0 / (32768.0 / (1 << (nibble - 1))));

                // if Periodic Interrupt Enable bit set, activate timer
                if ((mStateInformation.REG_STAT_B & 0x40) != 0)
                {
                    ////bx_pc_system.activate_timer(mStateInformation.PeriodicTimerIndex ,mStateInformation.PeriodicIntervalUSec , 1);
                    mStateInformation.PeriodicTimerIndex.Interval = (UInt64)(mStateInformation.PeriodicIntervalUSec);
                    mStateInformation.PeriodicTimerIndex.Enabled = true;
                }
                else
                {
                    /////bx_pc_system.deactivate_timer(mStateInformation.PeriodicTimerIndex );
                    mStateInformation.PeriodicTimerIndex.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Reads System Time & Updates BIOS registers
        /// <para>Note that system time is determined by calling the real
        /// system time</para>
        /// </summary>
        public void UpdateClock()
        {
            // ToDo: make time user intput.
            DateTime oNow = System.DateTime.Now; // you can change time from here.
            // update Seconds
            mStateInformation.REG_SEC = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Second), true);
            // Update Minutes
            mStateInformation.REG_MIN = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Minute), true);
            // Update Hours

            if (mStateInformation.RTCMode12Hour == true)
            {
                // [12 Hours]
                byte hour = (byte)oNow.Hour;
                byte val_bcd = (byte)((hour > 11) ? 0x80 : 0x00);
                if (hour > 11) hour -= 12;
                if (hour == 0) hour = 12;
                val_bcd |= Definitions.DataTypes.ConvertBinToBcd(hour, mStateInformation.RTCModeBindary);
                mStateInformation.REG_HOUR = val_bcd;
            }
            else
            {
                // [24 hour]
                mStateInformation.REG_SEC = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Hour), true);
            }

            // update day of the week
            mStateInformation.REG_WEEK_DAY = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.DayOfWeek), true);
            // update day of the month
            mStateInformation.REG_MONTH_DAY = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Day), true);
            // update month
            mStateInformation.REG_MONTH = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Month), true);
            // update year
            mStateInformation.REG_YEAR = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Year), true);
            // update Century
            mStateInformation.REG_IBM_CENTURY_BYTE = Definitions.DataTypes.ConvertBinToBcd((byte)(oNow.Year / 100 + 19), true);

            // Raul Hudea pointed out that some bioses also use reg 0x37 for the
            // century byte.  Tony Heller says this is critical in getting WinXP to run.
            mStateInformation.REG_IBM_PS2_CENTURY_BYTE = mStateInformation.REG_IBM_CENTURY_BYTE;
        }


        public void UpdateTimeVal()
        {

            byte Hours;
            byte val_bin, pmFlag;

            // update Seconds
            byte Seconds = Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_SEC, mStateInformation.RTCModeBindary);
            // Update Minutes
            byte Minutes = Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_MIN, mStateInformation.RTCModeBindary);
            // Update Hours 
            if (mStateInformation.RTCMode12Hour == true)
            {//[12 hour]

                pmFlag = (byte)(mStateInformation.REG_HOUR & 0x80);
                val_bin = Definitions.DataTypes.ConvertBcdToBin((byte)(mStateInformation.REG_HOUR & 0x70), mStateInformation.RTCModeBindary);
                if ((val_bin < 12) & (pmFlag > 0))
                {
                    val_bin += 12;
                }
                else if ((val_bin == 12) & (pmFlag == 0))
                {
                    val_bin = 0;
                }
                Hours = val_bin;
            }
            else
            {//[24 hour]
                Hours = Definitions.DataTypes.ConvertBinToBcd(mStateInformation.REG_HOUR, mStateInformation.RTCModeBindary);
            }



            // update day of the week
            byte week = Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_WEEK_DAY, mStateInformation.RTCModeBindary);
            // update day of the month
            byte day = Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_MONTH_DAY, mStateInformation.RTCModeBindary);
            // update month
            byte month = Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_MONTH, mStateInformation.RTCModeBindary);
            // update year
            val_bin = Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_IBM_CENTURY_BYTE, mStateInformation.RTCModeBindary);
            val_bin = (byte)((val_bin - 19) * 100);
            val_bin += Definitions.DataTypes.ConvertBcdToBin(mStateInformation.REG_YEAR, mStateInformation.RTCModeBindary);

            byte year = val_bin;

            System.DateTime oDT = new DateTime();
            oDT.AddSeconds(Seconds);
            oDT.AddMinutes(Minutes);
            oDT.AddHours(Hours);
            oDT.AddMonths(month);
            oDT.AddYears(year);
            mStateInformation.TimeVal = (UInt64)oDT.ToBinary();


        }


        #region "Timer Functions Handlers"

        public void PeriodicTimerHanlder()
        {
            // if periodic interrupts are enabled, trip IRQ 8, and
            // update status register C
            if ((mStateInformation.REG_STAT_B & 0x40) != 0)
            {
                mStateInformation.REG_STAT_C |= (byte)0xc0; // Interrupt Request, Periodic Int
                ///DEV_pic_raise_irq(8);
                mPCBoard.IRQRaise(8);
            }
        }

        public void OneSecondTimerHandler()
        {
            // divider chain reset - RTC stopped
            if ((mStateInformation.REG_STAT_A & 0x60) == 0x60)
                return;

            // update internal time/date buffer
            mStateInformation.TimeVal += 1;

            // Dont update CMOS user copy of time/date if CRB bit7 is 1
            // Nothing else do to
            if ((mStateInformation.REG_STAT_B & 0x80) != 0)
                return;

            mStateInformation.REG_STAT_A |= (byte)0x80; // Set UIP bit

            // UIP timer for updating clock & alarm functions
            ////bx_pc_system.activate_timer(BX_CMOS_THIS s.uip_timer_index, 244, 0);
            mStateInformation.UIPTimerIndex.Interval = 244;
            mStateInformation.UIPTimerIndex.Enabled = true;
        }

        public void UIP_TimerHandler()
        {
            UpdateClock();

            // if update interrupts are enabled, trip IRQ 8, and
            // update status register C
            if ((mStateInformation.REG_STAT_A & 0x10) != 0)
            {
                mStateInformation.REG_STAT_C |= (byte)0x90;
                mPCBoard.IRQRaise(8);
            }

            // compare CMOS user copy of time/date to alarm time/date here
            if ((mStateInformation.REG_STAT_B & 0x20) != 0)
            {
                // Alarm interrupts enabled
                bool alarm_match = true;
                if ((mStateInformation.REG_SEC_ALARM & 0xc0) != 0xc0)
                {
                    // seconds alarm not in dont care mode
                    if (mStateInformation.REG_SEC != mStateInformation.REG_SEC_ALARM)
                        alarm_match = false;
                }
                if ((mStateInformation.REG_MIN_ALARM & 0xc0) != 0xc0)
                {
                    // minutes alarm not in dont care mode
                    if (mStateInformation.REG_MIN != mStateInformation.REG_MIN_ALARM)
                        alarm_match = false;
                }
                if ((mStateInformation.REG_HOUR_ALARM & 0xc0) != 0xc0)
                {
                    // hours alarm not in dont care mode
                    if (mStateInformation.REG_HOUR != mStateInformation.REG_HOUR_ALARM)
                        alarm_match = false;
                }
                if (alarm_match)
                {
                    mStateInformation.REG_STAT_C |= 0xa0; // Interrupt Request, Alarm Int
                    mPCBoard.IRQRaise(8);
                }
            }
            mStateInformation.REG_STAT_A &= 0x7f; // clear UIP bit
        }

        #endregion


        #region "RW Functions"

        public override void DeviceWriteByte(UInt64 Address, byte Value)
        {

            // BX_DEBUG(("CMOS write to address: 0x%04x = 0x%02x", address, value));

            switch (Address)
            {
                case 0x0070:
                    mStateInformation.CMOSMemoryAddress = (byte)(Value & 0x7F);
                    break;

                case 0x0071:
                    switch ((Enum_CMOSRegistery)(mStateInformation.CMOSMemoryAddress))
                    {
                        case Enum_CMOSRegistery.REG_SEC_ALARM:             // seconds alarm
                        case Enum_CMOSRegistery.REG_MIN_ALARM:             // minutes alarm
                        case Enum_CMOSRegistery.REG_HOUR_ALARM:            // hours alarm
                            mStateInformation.Reg[(mStateInformation.CMOSMemoryAddress)] = Value;
                            //BX_DEBUG(("alarm time changed to %02x:%02x:%02x", BX_CMOS_THIS s.reg[REG_HOUR_ALARM],
                            //        BX_CMOS_THIS s.reg[REG_MIN_ALARM], BX_CMOS_THIS s.reg[REG_SEC_ALARM]));
                            break;

                        case Enum_CMOSRegistery.REG_SEC:                   // seconds
                        case Enum_CMOSRegistery.REG_MIN:                   // minutes
                        case Enum_CMOSRegistery.REG_HOUR:                  // hours
                        case Enum_CMOSRegistery.REG_WEEK_DAY:              // day of the week
                        case Enum_CMOSRegistery.REG_MONTH_DAY:             // day of the month
                        case Enum_CMOSRegistery.REG_MONTH:                 // month
                        case Enum_CMOSRegistery.REG_YEAR:                  // year
                        case Enum_CMOSRegistery.REG_IBM_CENTURY_BYTE:      // century
                        case Enum_CMOSRegistery.REG_IBM_PS2_CENTURY_BYTE:  // century (PS/2)
                            mStateInformation.Reg[(mStateInformation.CMOSMemoryAddress)] = Value;
                            if ((Enum_CMOSRegistery)(mStateInformation.CMOSMemoryAddress) == Enum_CMOSRegistery.REG_IBM_PS2_CENTURY_BYTE)
                            {
                                mStateInformation.REG_IBM_CENTURY_BYTE = Value;
                            }
                            if ((mStateInformation.REG_STAT_B & 0x80) != 0)
                            {
                                mStateInformation.TimerValueChange = true;
                            }
                            else
                            {
                                UpdateTimeVal();
                            }
                            break;

                        case Enum_CMOSRegistery.REG_STAT_A: // Control Register A
                            // bit 7: Update in Progress (read-only)
                            //   1 = signifies time registers will be updated within 244us
                            //   0 = time registers will not occur before 244us
                            //   note: this bit reads 0 when CRB bit 7 is 1
                            // bit 6..4: Divider Chain Control
                            //   000 oscillator disabled
                            //   001 oscillator disabled
                            //   010 Normal operation
                            //   011 TEST
                            //   100 TEST
                            //   101 TEST
                            //   110 Divider Chain RESET
                            //   111 Divider Chain RESET
                            // bit 3..0: Periodic Interrupt Rate Select
                            //   0000 None
                            //   0001 3.90625  ms
                            //   0010 7.8125   ms
                            //   0011 122.070  us
                            //   0100 244.141  us
                            //   0101 488.281  us
                            //   0110 976.562  us
                            //   0111 1.953125 ms
                            //   1000 3.90625  ms
                            //   1001 7.8125   ms
                            //   1010 15.625   ms
                            //   1011 31.25    ms
                            //   1100 62.5     ms
                            //   1101 125      ms
                            //   1110 250      ms
                            //   1111 500      ms

                            byte dcc;
                            dcc = (byte)((Value >> 4) & 0x07);
                            if ((dcc & 0x06) == 0x06)
                            {
                                //BX_INFO(("CRA: divider chain RESET"));
                            }
                            else if (dcc > 0x02)
                            {
                                throw new InvalidOperationException("CRA: divider chain control ");
                                //BX_PANIC(("CRA: divider chain control 0x%02x", dcc));
                            }
                            mStateInformation.REG_STAT_A &= (byte)(0x80);
                            mStateInformation.REG_STAT_A |= (byte)(Value & 0x7f);
                            CRA_Change();
                            break;

                        case Enum_CMOSRegistery.REG_STAT_B: // Control Register B
                            // bit 0: Daylight Savings Enable
                            //   1 = enable daylight savings
                            //   0 = disable daylight savings
                            // bit 1: 24/12 hour mode
                            //   1 = 24 hour format
                            //   0 = 12 hour format
                            // bit 2: Data Mode
                            //   1 = binary format
                            //   0 = BCD format
                            // bit 3: "square wave enable"
                            //   Not supported and always read as 0
                            // bit 4: Update Ended Interrupt Enable
                            //   1 = enable generation of update ended interrupt
                            //   0 = disable
                            // bit 5: Alarm Interrupt Enable
                            //   1 = enable generation of alarm interrupt
                            //   0 = disable
                            // bit 6: Periodic Interrupt Enable
                            //   1 = enable generation of periodic interrupt
                            //   0 = disable
                            // bit 7: Set mode
                            //   1 = user copy of time is "frozen" allowing time registers
                            //       to be accessed without regard for an occurance of an update
                            //   0 = time updates occur normally

                            if ((Value & 0x01) != 0)
                            {
                                //BX_ERROR(("write status reg B, daylight savings unsupported"));
                            }
                            Value &= 0xf7; // bit3 always 0
                            // Note: setting bit 7 clears bit 4
                            if ((Value & 0x80) != 0)
                                Value &= 0xef;

                            byte prev_CRB;
                            prev_CRB = mStateInformation.REG_STAT_B;
                            mStateInformation.REG_STAT_B = Value;
                            if ((prev_CRB & 0x02) != (Value & 0x02))
                            {
                                mStateInformation.RTCMode12Hour = ((Value & 0x02) == 0);
                                UpdateClock();
                            }
                            if ((prev_CRB & 0x04) != (Value & 0x04))
                            {
                                mStateInformation.RTCModeBindary = ((Value & 0x04) != 0);
                                UpdateClock();
                            }
                            if ((prev_CRB & 0x40) != (Value & 0x40))
                            {
                                // Periodic Interrupt Enabled changed
                                if ((prev_CRB & 0x40) != 0)
                                {
                                    // transition from 1 to 0, deactivate timer
                                    ////bx_pc_system.deactivate_timer(BX_CMOS_THIS s.periodic_timer_index);
                                    mStateInformation.PeriodicTimerIndex.Enabled = false;
                                }
                                else
                                {
                                    // transition from 0 to 1
                                    // if rate select is not 0, activate timer
                                    if ((mStateInformation.REG_STAT_A & 0x0f) != 0)
                                    {
                                        //// bx_pc_system.activate_timer(                  BX_CMOS_THIS s.periodic_timer_index,                  BX_CMOS_THIS s.periodic_interval_usec, 1);
                                        mStateInformation.PeriodicTimerIndex.Enabled = true;
                                    }
                                }
                            }
                            if ((prev_CRB >= 0x80) && (Value < 0x80) && mStateInformation.TimerValueChange)
                            {
                                UpdateTimeVal();
                                mStateInformation.TimerValueChange = false;
                            }
                            break;

                        case Enum_CMOSRegistery.REG_STAT_C: // Control Register C
                        case Enum_CMOSRegistery.REG_STAT_D: // Control Register D
                            //BX_ERROR(("write to control register 0x%02x ignored (read-only)", mStateInformation.CMOSMemoryAddress ));
                            break;

                        case Enum_CMOSRegistery.REG_DIAGNOSTIC_STATUS:
                            //BX_DEBUG(("write register 0x0e: 0x%02x", Value));
                            mStateInformation.REG_DIAGNOSTIC_STATUS = Value;
                            break;

                        case Enum_CMOSRegistery.REG_SHUTDOWN_STATUS:
                            switch (Value)
                            {
                                case 0x00: /* proceed with normal POST (soft reset) */
                                    // BX_DEBUG(("Reg 0Fh(00): shutdown action = normal POST"));
                                    break;
                                case 0x01: /* shutdown after memory size check */
                                    //BX_DEBUG(("Reg 0Fh(01): request to change shutdown action" " to shutdown after memory size check"));
                                    break;
                                case 0x02: /* shutdown after successful memory test */
                                    //BX_DEBUG(("Reg 0Fh(02): request to change shutdown action"" to shutdown after successful memory test"));
                                    break;
                                case 0x03: /* shutdown after failed memory test */
                                    //BX_DEBUG(("Reg 0Fh(03): request to change shutdown action"" to shutdown after successful memory test"));
                                    break;
                                case 0x04: /* jump to disk bootstrap routine */
                                    //BX_DEBUG(("Reg 0Fh(04): request to change shutdown action ""to jump to disk bootstrap routine."));
                                    break;
                                case 0x05: /* flush keyboard (issue EOI) and jump via 40h:0067h */
                                    //BX_DEBUG(("Reg 0Fh(05): request to change shutdown action ""to flush keyboard (issue EOI) and jump via 40h:0067h."));
                                    break;
                                case 0x06:
                                    //BX_DEBUG(("Reg 0Fh(06): Shutdown after memory test !"));
                                    break;
                                case 0x07: /* reset (after failed test in virtual mode) */
                                    //BX_DEBUG(("Reg 0Fh(07): request to change shutdown action ""to reset (after failed test in virtual mode)."));
                                    break;
                                case 0x08: /* used by POST during protected-mode RAM test (return to POST) */
                                    //BX_DEBUG(("Reg 0Fh(08): request to change shutdown action ""to return to POST (used by POST during protected-mode RAM test)."));
                                    break;
                                case 0x09: /* return to BIOS extended memory block move
                  (interrupt 15h, func 87h was in progress) */
                                    //BX_DEBUG(("Reg 0Fh(09): request to change shutdown action "  "to return to BIOS extended memory block move."));
                                    break;
                                case 0x0a: /* jump to DWORD pointer at 40:67 */
                                    //BX_DEBUG(("Reg 0Fh(0a): request to change shutdown action"
                                    //        " to jump to DWORD at 40:67"));
                                    break;
                                case 0x0b: /* iret to DWORD pointer at 40:67 */
                                    //BX_DEBUG(("Reg 0Fh(0b): request to change shutdown action"
                                    //        " to iret to DWORD at 40:67"));
                                    break;
                                case 0x0c: /* retf to DWORD pointer at 40:67 */
                                    //BX_DEBUG(("Reg 0Fh(0c): request to change shutdown action"
                                    //        " to retf to DWORD at 40:67"));
                                    break;
                                default:
                                    //BX_ERROR(("unsupported shutdown status: 0x%02x!", Value));
                                    throw new InvalidOperationException("unsupported shutdown status");
                            }
                            mStateInformation.REG_SHUTDOWN_STATUS = Value;
                            break;

                        default:
                            //BX_DEBUG(("write reg 0x%02x: Value = 0x%02x",mStateInformation.CMOSMemoryAddress , Value));
                            mStateInformation.Reg[mStateInformation.CMOSMemoryAddress] = Value;
                            break;
                    }
                    break;
            }
        }

        public override byte DeviceReadByte(UInt64 Address)
        {
            byte ret8;

            ///BX_DEBUG(("CMOS read of CMOS register 0x%02x", (unsigned) BX_CMOS_THIS s.cmos_mem_address));

            switch (Address)
            {
                case 0x0070:
                    // this register is write-only on most machines
                    //BX_DEBUG(("read of index port 0x70. returning 0xff"));
                    return (0xff);
                case 0x0071:
                    // Read register value [port 0x0071,RegNum]
                    ret8 = mStateInformation.Reg[mStateInformation.CMOSMemoryAddress];

                    // all bits of Register C are cleared after a read occurs.
                    if (mStateInformation.CMOSMemoryAddress == (byte)(Enum_CMOSRegistery.REG_STAT_C))
                    {
                        mStateInformation.REG_STAT_C = 0x00;
                        mPCBoard.IRQLow(8);
                    }
                    return (ret8);

                default:
                    //BX_PANIC(("unsupported cmos read, address=0x%04x!", (unsigned) address));
                    throw new InvalidOperationException("unsupported cmos read, address=0x%04x!");

            }
        }

        #endregion

        #endregion


    }
}
