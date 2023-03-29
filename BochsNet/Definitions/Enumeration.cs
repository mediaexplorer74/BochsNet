using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Definitions.Enumerations
{

    public enum  Enum_Signal
    {
        Low = 0,
        High = 1
    };

    public enum Enum_MemoryAccessType : byte
    {
        Read = 0,
        Write = 1,
        Execute = 2,
        RW = 3
    }

    public enum Enum_PortAccessType : byte
    {
        Read = 0,
        Write = 1,
        RW = 3
    }

    public enum Enum_ResetType : byte
    {
        HardwareReset=0,
        SoftwareReset=1
    }


    public enum InstructionType: byte
    {
        Undefined=0,
        Flag_Ctrl=1,
        Logical08=2,
        Logical16 = 3,
        Logical32 = 4,
        Logical64 = 5,
        Data_Xfer08 =6,
        Data_Xfer16 = 7,
        Data_Xfer32 = 8,
        Data_Xfer64 = 9,
        Ctrl_Xfer08=10,
        Ctrl_Xfer16 = 11,
        Ctrl_Xfer32 = 12,
        Ctrl_Xfer64 = 13,
        Arith08=14,
        Arith16 = 15,
        Arith32 = 16,
        Arith64 = 17,
        IO=18,
        String =19,
        Stack16=20
    }

}
