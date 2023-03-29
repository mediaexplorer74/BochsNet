using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU
{
    #region "Enumeration"


    public enum Enum_PageFaults : byte
    {
        /*
        // #define ERROR_NOT_PRESENT       0x00
        // #define ERROR_PROTECTION        0x01
        // #define ERROR_RESERVED          0x08
        // #define ERROR_CODE_ACCESS       0x10
         * */
        ERROR_NOT_PRESENT = 0x00,
        ERROR_PROTECTION =0x01,
        ERROR_RESERVED = 0x08,
        ERROR_CODE_ACCESS = 0x10
    }

    public enum Enum_CPUActivityState : byte
    {
        BX_Activity_State_ACTIVE = 0,
        BX_Activity_State_HLT = 1,
        BX_Activity_State_SHUTDOWN = 2,
        BX_Activity_State_WAIT_FOR_SIPI = 3,
        BX_Activity_State_MWAIT = 4,
        BX_Activity_State_MWAIT_IF = 5
    }


   

    public  enum Enum_SegmentReg : byte
    {
        REG_ES = 0,
        REG_CS = 1,
        REG_SS = 2,
        REG_DS = 3,
        REG_FS = 4,
        REG_GS = 5,
        // NULL now has to fit in 3 bits.
        BX_SEG_REG_NULL = 7
    };

    public enum Enum_16BitReg : byte
    {
       REG_AX =0,
       REG_CX =1,
       REG_DX = 2,
       REG_BX = 3,
       REG_SP = 4,
       REG_BP = 5,
       REG_SI = 6,
       REG_DI = 7
    };


    public enum Enum_32BitReg : byte
    {
        REG_EAX = 0,
        REG_ECX = 1,
        REG_EDX = 2,
        REG_EBX = 3,
        REG_ESP = 4,
        REG_EBP = 5,
        REG_ESI = 6,
        REG_EDI = 7
    }

    public enum Enum_64BitReg : byte
    {
        REG_RAX = 0x0,
        REG_RCX = 0x1,
        REG_RDX = 0x2,
        REG_RBX = 0x3,
        REG_RSP = 0x4,
        REG_RBP = 0x5,
        REG_RSI = 0x6,
        REG_RDI = 0x7,

        REG_R8 = 0x8,
        REG_R9 = 0x9,
        REG_R10 = 0xa,
        REG_R11 = 0xb,
        REG_R12 = 0xc,
        REG_R13 = 0xd,
        REG_R14 = 0xe,
        REG_R15 = 0xf
        
    };

    public enum Enum_GeneralReg : byte
    {
        GENERAL_REGISTERS = 16,
        BX_16BIT_REG_IP = GENERAL_REGISTERS,
        BX_32BIT_REG_EIP = GENERAL_REGISTERS,
        BX_64BIT_REG_RIP = GENERAL_REGISTERS,
        BX_NIL_REGISTER = GENERAL_REGISTERS+2,
        BX_TMP_REGISTER = GENERAL_REGISTERS
    };


    #endregion

}
