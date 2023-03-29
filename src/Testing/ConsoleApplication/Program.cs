using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core;
using CPU;
using CPU.Registers;
using CPU.Instructions;
using Definitions.Enumerations;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            ////////// Register Event
            Core.Monitor.EventRegisterar.AddEvent("CPU.ExecuteInstruction");

            ////////// Listeners
            Core.Monitor.EventManager.CallMeOn("CPU.ExecuteInstruction", CallMe);

            ////////// Raise Event 
            Core.Monitor.EventRegisterar.RaiseEvent ("CPU.ExecuteInstruction",null,new Core.Monitor.EventArgument("Allo"));


            //TestRegister();
           // TestInstruction();
           // TestFlags();
           // TestRegister();
            PCMachine.Machine oMachine = new PCMachine.Machine();
            oMachine.Start();

        }

        public static void CallMe (string EventName,Core.Component Sender ,Core.Monitor.EventArgument Arg)
        {
            int a = 3;
        }

        static protected void TestInstruction()
        {
            Instruction oIns = new Instruction(null);
            int a = 2;
        }

        static protected void TestFlags()
        {
            RFlagsRegister RFlags = new RFlagsRegister();

            RFlags.PF = Enum_Signal.High;
            RFlags.PF = Enum_Signal.Low;
            RFlags.PF = Enum_Signal.High;

            RFlags.CF = Enum_Signal.High;
            RFlags.CF = Enum_Signal.Low;
            RFlags.CF = Enum_Signal.High;

            RFlags.AF = Enum_Signal.High;
            RFlags.AF = Enum_Signal.Low;
            RFlags.AF = Enum_Signal.High;

            RFlags.ZF = Enum_Signal.High;
            RFlags.ZF = Enum_Signal.Low;
            RFlags.ZF = Enum_Signal.High;


            RFlags.SF = Enum_Signal.High;
            RFlags.SF = Enum_Signal.Low;
            RFlags.SF = Enum_Signal.High;

            RFlags.TF = Enum_Signal.High;
            RFlags.TF = Enum_Signal.Low;
            RFlags.TF = Enum_Signal.High;

            RFlags.IF = Enum_Signal.High;
            RFlags.IF = Enum_Signal.Low;
            RFlags.IF = Enum_Signal.High;

            RFlags.DF = Enum_Signal.High;
            RFlags.DF = Enum_Signal.Low;
            RFlags.DF = Enum_Signal.High;

            RFlags.OF = Enum_Signal.High;
            RFlags.OF = Enum_Signal.Low;
            RFlags.OF = Enum_Signal.High;

            RFlags.IOPL0  = Enum_Signal.High;
            RFlags.IOPL0 = Enum_Signal.Low;
            RFlags.IOPL0 = Enum_Signal.High;

            RFlags.IOPL1 = Enum_Signal.High;
            RFlags.IOPL1 = Enum_Signal.Low;
            RFlags.IOPL1 = Enum_Signal.High;
           

            RFlags.NT = Enum_Signal.High;
            RFlags.NT = Enum_Signal.Low;
            RFlags.NT = Enum_Signal.High;


            RFlags.RF = Enum_Signal.High;
            RFlags.RF = Enum_Signal.Low;
            RFlags.RF = Enum_Signal.High;


            RFlags.VM = Enum_Signal.High;
            RFlags.VM = Enum_Signal.Low;
            RFlags.VM = Enum_Signal.High;


            RFlags.AC = Enum_Signal.High;
            RFlags.AC = Enum_Signal.Low;
            RFlags.AC = Enum_Signal.High;


            RFlags.VIF = Enum_Signal.High;
            RFlags.VIF = Enum_Signal.Low;
            RFlags.VIF = Enum_Signal.High;


            RFlags.VIP = Enum_Signal.High;
            RFlags.VIP = Enum_Signal.Low;
            RFlags.VIP = Enum_Signal.High;



            RFlags.ID = Enum_Signal.High;
            RFlags.ID = Enum_Signal.Low;
            RFlags.ID = Enum_Signal.High;

        }

        static protected void TestRegister()
        {

            SegmentRegister SR =  new SegmentRegister("CS");
            SR.Selector.Selector_Value = 0xf000;

            CR0_Register CR0 = new CR0_Register();

            Register8 R8 = null;

            Register16 R16 = null;

            Register32 R32 = null;

            Register64 R64 = null;

            R64 = new Register64("dsd", 1);

            for (byte i = 0; i <= 255; ++i)
            {
                R8 = new Register8("A" + i.ToString(), i);

                R16 = new Register16("A" + i.ToString(), i);

            }

            for (UInt16 i = 255; i <= 40255; ++i)
            {

                R16 = new Register16("A" + i.ToString(), i);
                R32 = new Register32("A" + i.ToString(), i);
            }
        }
    }
}
