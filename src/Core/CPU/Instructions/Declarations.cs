using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Instructions
{



    public enum Enum_Resolve : byte
    {
        BX_RESOLVE16 = 0,
        BX_RESOLVE32_BASE = 1,
        BX_RESOLVE32_BASE_INDEX = 2,
        BX_RESOLVE_NONE = 3
    };

    // If the BxImmediate mask is set, the lowest 4 bits of the attribute
    // specify which kinds of immediate data a required by instruction.
    public enum DecodingAttribute : uint
    {
        BXZero = 0x0000,
        BxImmediate = 0x000f, // bits 3..0: any immediate
        BxImmediate_I1 = 0x0001, // imm8 = 1
        BxImmediate_Ib = 0x0002, // 8 bit
        BxImmediate_Ib_SE = 0x0003, // sign extend to OS size
        BxImmediate_Iw = 0x0004, // 16 bit
        BxImmediate_IbIb = 0x0005, // SSE4A
        BxImmediate_IwIb = 0x0006, // enter_IwIb
        BxImmediate_IwIw = 0x0007, // call_Ap, not encodable in 64-bit mode
        BxImmediate_IdIw = 0x0008, // call_Ap, not encodable in 64-bit mode
        BxImmediate_Id = 0x0009, // 32 bit
        BxImmediate_O = 0x000A, // MOV_ALOd, mov_OdAL, mov_eAXOv, mov_OveAX
        BxImmediate_BrOff8 = 0x000B, // Relative branch offset byte

        BxImmediate_Iq = 0x000C, // 64 bit override


        BxImmediate_BrOff16 = BxImmediate_Iw, // Relative branch offset word, not encodable in 64-bit mode
        BxImmediate_BrOff32 = BxImmediate_Id,// Relative branch offset dword

        // Lookup for opcode and attributes in another opcode tables
        // Totally 15 opcode groups supported
        BxGroupX = 0x00f0, // bits 7..4: opcode groups definition
        BxGroupN = 0x0010, // Group encoding: 0001
        BxPrefixSSE = 0x0020, // Group encoding: 0010
        BxPrefixSSE66 = 0x0030, // Group encoding: 0011
        BxFPEscape = 0x0040, // Group encoding: 0100
        BxRMGroup = 0x0050, // Group encoding: 0101
        Bx3ByteOp = 0x0060, // Group encoding: 0110
        BxOSizeGrp = 0x0070, // Group encoding: 0111

        BxLockable = 0x0100, // bit 8
        BxArithDstRM = 0x0200, // bit 9


        BxTraceEnd = 0x0400, // bit 10
        BxTraceJCC = 0x0,
        BxGroup1 = BxGroupN,
        BxGroup1A = BxGroupN,
        BxGroup2 = BxGroupN,
        BxGroup3 = BxGroupN,
        BxGroup4 = BxGroupN,
        BxGroup5 = BxGroupN,
        BxGroup6 = BxGroupN,
        BxGroup7 = BxGroupN,
        BxGroup8 = BxGroupN,
        BxGroup9 = BxGroupN,

        BxGroup11 = BxGroupN,
        BxGroup12 = BxGroupN,
        BxGroup13 = BxGroupN,
        BxGroup14 = BxGroupN,
        BxGroup15 = BxGroupN,
        BxGroup16 = BxGroupN,

        BxGroupFP = BxGroupN
    };


    
}
