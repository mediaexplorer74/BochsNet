using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU.Registers
{
    /// <summary>
    /// Each segment register is really four registers:
    /// <para> * A selector register</para>
    /// <para> * A base register</para>
    /// <para> * A limit register</para>
    /// <para> * An attribute register</para>
    /// <para> In all modes, every access to memory that uses a segment register uses the base, </para>
    /// <para> limit, and attribute portions of the segment register and does not use the selector portion.</para>
    /// <para> Every direct access to a segment register (PUSHing it on the stack, MOVing it to a general register etc.) uses only the selector portion.</para>
    /// <para> The base, limit, and attribute portions are either very hard or impossible to read (depending on CPU type). </para>
    /// <para> They are often called the "hidden" part of the segment register because they are so hard to read.</para>
    /// <para> Intel documentation refers to the hidden part of the segment register as a "descriptor Cache". This name obscures the actual behavior of the "hidden" part.</para>
    /// <see cref="http://geezer.osdevbrasil.net/johnfine/segments.htm"/>
    /// </summary>
    public class SegmentRegister : Register64
    {

        //
        // |---------------------------------------------|
        // |             Segment Descriptor              |
        // |---------------------------------------------|
        // |33222222|2|2|2|2| 11 11 |1|11|1|11  |        |
        // |10987654|3|2|1|0| 98 76 |5|43|2|1098|76543210|
        // |--------|-|-|-|-|-------|-|--|-|----|--------|
        // |Base    |G|D|L|A|Limit  |P|D |S|Type|Base    |
        // |[31-24] | |/| |V|[19-16]| |P | |    |[23-16] |
        // |        | |B| |L|       | |L | |    |        |
        // |------------------------|--------------------|
        // |       Base [15-0]      |    Limit [15-0]    |
        // |------------------------|--------------------|
        //


        #region "Enumeration"

        public enum Enum_AccessStatus : int
        {
            /// <summary>
            /// =1
            /// </summary>
            SegValidCache = (0x01),
            /// <summary>
            /// =2
            /// </summary>
            SegAccessROK = (0x02),
            /// <summary>
            /// =4
            /// </summary>
            SegAccessWOK = (0x04)
        }


        public enum Enum_SegmentType
        {
            /// <summary>
            /// =0
            /// </summary>
            SystemGate = 0,
            /// <summary>
            /// =1
            /// </summary>
            DataOrCode = 1
        }


        public enum Enum_Granularity
        {
            /// <summary>
            /// =0
            /// </summary>
            gradularity_byte = 0,
            /// <summary>
            /// =1
            /// </summary>
            gradularity_4k = 1
        }

        public enum Enum_SegmentDB
        {
            /// <summary>
            /// =0
            /// </summary>
            bit16 = 0,
            /// <summary>
            /// =1
            /// </summary>
            bit64 = 1
        }

        public enum Enum_LongMode
        {
            /// <summary>
            /// =0
            /// </summary>
            compat = 0,
            /// <summary>
            /// =1
            /// </summary>
            bit64 = 1
        }
        #endregion

        #region "Attributes"




        #region "Selector"

        protected Selector mSelector = new Selector ();
        
        #endregion

        #region "Cache"

        /// <summary>
        /// For system & gate descriptors:
        ///   0 = invalid descriptor (reserved)
        ///   1 = 286 available Task State Segment (TSS)
        ///   2 = LDT descriptor
        ///   3 = 286 busy Task State Segment (TSS)
        ///   4 = 286 call gate
        ///   5 = task gate
        ///   6 = 286 interrupt gate
        ///   7 = 286 trap gate
        ///   8 = (reserved)
        ///   9 = 386 available TSS
        ///  10 = (reserved)
        ///  11 = 386 busy TSS
        ///  12 = 386 call gate
        ///  13 = (reserved)
        ///  14 = 386 interrupt gate
        ///  15 = 386 trap gate
        /// </summary>
        public enum ENUM_SystemGateDescription : int
        {

            // For system & gate descriptors:
            /* 
                         *  0 = invalid descriptor (reserved)
                         *  1 = 286 available Task State Segment (TSS)
                         *  2 = LDT descriptor
                         *  3 = 286 busy Task State Segment (TSS)
                         *  4 = 286 call gate
                         *  5 = task gate
                         *  6 = 286 interrupt gate
                         *  7 = 286 trap gate
                         *  8 = (reserved)
                         *  9 = 386 available TSS
                         * 10 = (reserved)
                         * 11 = 386 busy TSS
                         * 12 = 386 call gate
                         * 13 = (reserved)
                         * 14 = 386 interrupt gate
                         * 15 = 386 trap gate */
            InavlidDescriptor = 0,
            AvailableTaskStateSegment_286_TSS = 1,
            LDTDescriptor = 2,
            BusyTaskStateSegment_286_TSS = 3,
            //CallGate_286 = 4,
            //TaskGate = 5,
            //InterruptGate_286 = 6,
            //TrapGate_286 = 7,
            Reserved_8 = 8,
            Available_386_TSS = 9,
            Reserved_10 = 10,
            Busy_386_TSS = 11,
            //CallGate_386 = 12,
            Reserved_13 = 13,
            //InterruptGate_386 = 14,
            //TrapGate_386 = 15,


            #region "For system & gate descriptors"
            // For system & gate descriptors:
            /// <summary>
            /// = 0
            /// </summary>
            GateTypeNon = (0x0),
            /// <summary>
            /// =1
            /// </summary>
            SysSegmentAvail_286_TSS = (0x1),
            /// <summary>
            /// =2
            /// </summary>
            SysSegment_LDT = (0x2),
            /// <summary>
            /// =3
            /// </summary>
            SysSegmentBusy_286_TSS = (0x3),
            /// <summary>
            /// =4
            /// </summary>
            CallGate_286 = (0x4),
            /// <summary>
            /// =5
            /// </summary>
            TaskGate = (0x5),
            /// <summary>
            /// =6
            /// </summary>
            InterruptGate_286 = (0x6),
            /// <summary>
            /// = 7
            /// </summary>
            TrapGate_286 = (0x7),
            /* 0x8 reserved */
            /// <summary>
            /// = 9
            /// </summary>
            SysSegmentAvail_386_TSS = (0x9),
            /* 0xa reserved */
            /// <summary>
            /// = 0xb
            /// </summary>
            SysSegmentBusy_386_TSS = (0xb),
            /// <summary>
            /// = 0xc
            /// </summary>
            CallGate_386 = (0xc),
            /* 0xd reserved */
            /// <summary>
            /// = 0xe
            /// </summary>
            InterruptGate_386 = (0xe),
            /// <summary>
            /// = 0xf
            /// </summary>
            TrapGate_386 = (0xf),

            #endregion 

            #region "For data/code descriptors"
            // For data/code descriptors:
            DataReadOnly = (0x0),
            DataReadOnlyAccessed = (0x1),
            DataReadWrite = (0x2),
            DataReadWriteAccessed = (0x3),
            DataReadOnlyExpandDown = (0x4),
            DataReadOnlyExpandDownAccessed = (0x5),
            DataReadWriteExpandDown = (0x6),
            DataReadExpandDownAccessed = (0x7),
            CodeExecOnly = (0x8),
            CodeExecOnlyAccessed = (0x9),
            CodeExecRead = (0xa),
            CodeExecReadAccess = (0xb),
            CodeExecOnlyConfirming = (0xc),
            CodeExecOnlyConfirmingAccessed = (0xd),
            CodeExecReadConfirming = (0xe),
            CodeExecReadConfirmingAccessed = (0xf)
            #endregion 
        };

        /// <summary>
        /// Holds above values, Or'd together.  Used to
        /// hold only 0 or 1.
        /// </summary>
        protected UInt32 mCache_Valid;

        protected bool mCache_Present;

        /// <summary>
        /// Which Ring : descriptor privilege level 0..3
        /// </summary>
        protected ushort mCache_DPL;

        /// <summary>
        /// 0 = system/gate, 1 = data/code segment 
        /// </summary>
        protected Enum_SegmentType mCache_Segment;

        /// <summary>
        /// For system & gate descriptors
        /// </summary>
        protected ENUM_SystemGateDescription mCache_Type;

        #region "Cache_u_Segment"

        /// <summary>
        /// base address: 286=24bits, 386=32bits, long=64 
        /// </summary>
        protected UInt64 mCache_u_Segment_Base;

        /// <summary>
        /// for efficiency, this contrived field is set to
        /// limit for byte granular, and
        /// (limit SHL 12) | 0xfff for page granular seg's
        /// </summary>
        protected UInt32 mCache_u_Segment_LimitScaled;

        /// <summary>
        /// Granularity: 0=byte, 1=4K (page)
        /// </summary>
        protected Enum_Granularity mCache_u_Segment_G = Enum_Granularity.gradularity_byte;

        /// <summary>
        /// Default Size: 0=16bit, 1=32bit
        /// </summary>
        protected Enum_SegmentDB mCache_u_Segment_D_B = Enum_SegmentDB.bit16;

        /// <summary>
        /// long mode: 0=compat, 1=64 bit
        /// <para>64 bit</para>
        /// </summary>
        protected Enum_LongMode mCache_u_Segment_LongMode = Enum_LongMode.compat;

        /// <summary>
        /// Available for use by system 
        /// </summary>
        protected UInt32 mCache_u_Segment_AVL = 0;

        #endregion

        #endregion

        #endregion



        #region "Properties"

        public string Name
        {
            get
            {
                return mName64;
            }
        }

        #region "Selector"

         public Selector Selector
        {
            get
            {
                return mSelector;
            }
            set
            {
                mSelector =value;
            }
        }

        #endregion

        #region "Cache"

        /// <summary>
        /// Holds above values, Or'd together.  Used to
        /// hold values 0 ,1
        /// </summary>
        public UInt32 Cache_Valid
        {
            get
            {
                return mCache_Valid;
            }
            set
            {
                mCache_Valid = value;
            }
        }

        public bool Cache_Present
        {
            get
            {
                return mCache_Present;
            }
            set
            {
                mCache_Present = value;
            }
        }


        /// <summary>
        /// Which Ring : descriptor privilege level 0..3
        /// </summary>
        public ushort Cache_DPL
        {
            get
            {
                return mCache_DPL;
            }
            set
            {
                mCache_DPL = value;
            }
        }

        /// <summary>
        /// 0 = system/gate, 1 = data/code segment 
        /// </summary>
        public Enum_SegmentType Cache_Segment
        {
            get
            {
                return mCache_Segment;
            }
            set
            {
                mCache_Segment = value;
            }
        }

        /// <summary>
        /// For system & gate descriptors
        /// </summary>
        public ENUM_SystemGateDescription Cache_Type
        {
            get
            {
                return mCache_Type;
            }
            set
            {
                mCache_Type = value;
            }
        }


        #region "Cache_Union"

        #region "Cache_u_Segment"

        /// <summary>
        /// base address: 286=24bits, 386=32bits, long=64 
        /// </summary>
        public UInt64 Cache_u_Segment_Base
        {
            get
            {
                return mCache_u_Segment_Base;
            }

            set
            {
                mCache_u_Segment_Base = value;
            }
        }

        /// <summary>
        /// for efficiency, this contrived field is set to
        /// limit for byte granular, and
        /// (limit SHL 12) | 0xfff for page granular seg's
        /// </summary>
        public UInt32 Cache_u_Segment_LimitScaled
        {
            get
            {
                return mCache_u_Segment_LimitScaled;
            }
            set
            {
                mCache_u_Segment_LimitScaled = value;
            }
        }



        /// <summary>
        /// Granularity: 0=byte, 1=4K (page)
        /// </summary>
        public Enum_Granularity Cache_u_Segment_G
        {
            get
            {
                return mCache_u_Segment_G;
            }
            set
            {
                mCache_u_Segment_G = value;
            }
        }

        /// <summary>
        /// Default Size: 0=16bit, 1=32bit
        /// </summary>
        public Enum_SegmentDB Cache_u_Segment_D_B
        {
            get
            {
                return mCache_u_Segment_D_B;
            }
            set
            {
                mCache_u_Segment_D_B = value;
            }
        }

        /// <summary>
        /// available for use by system 
        /// </summary>
        public UInt32 Cache_u_Segment_AVL
        {
            get
            {
                return mCache_u_Segment_AVL;
            }
            set
            {
                mCache_u_Segment_AVL = value;
            }
        }

        /// <summary>
        /// long mode: 0=compat, 1=64 bit
        /// <para>64 bit</para>
        /// </summary>
        public Enum_LongMode Cache_u_Segment_LongMode
        {
            get
            {
                return mCache_u_Segment_LongMode;
            }
            set
            {
                mCache_u_Segment_LongMode = value;
            }
        }

        #endregion

        #region "Cache_u_Gate"

        /// <summary>
        /// bite [0..8] of Base
        /// 5bits (0..31) #words/dword to copy from caller's
        /// stack to called procedure's stack.
        /// </summary>
        public byte Cache_u_Gate_Param_Count
        {
            get
            {
                return (byte)mCache_u_Segment_Base;
            }
        }

        /// <summary>
        /// bits [16..32] of Base
        /// </summary>
        public UInt16 Cache_u_Gate_Param_Dest_Selector
        {
            get
            {
                return (UInt16)((mCache_u_Segment_Base & 0x00000000ffff0000) >> 0x10);
            }
        }

        /// <summary>
        /// bits [32..64] of Base
        /// </summary>
        public UInt32 Cache_u_Gate_Param_Dest_Offset
        {
            get
            {
                return (UInt32)(mCache_u_Segment_Base >> 0x20);
            }
        }

        #endregion

        #region "Cache_u_TaskGate"

        /// <summary>
        /// bits [0..16] of Base
        /// </summary>
        public UInt16 Cache_u_TaskGate_Tss_Selector
        {
            get
            {
                return (UInt16)mCache_u_Segment_Base;
            }
        }

        #endregion

        #endregion
        #endregion






        #endregion

        #region "Constructor"

        public SegmentRegister()
        {
        }

        public SegmentRegister(string Name64)
            : base(Name64)
        {
            mName64 = Name64;
        }

        #endregion


        #region "Method"

        /// <summary>
        /// This reset is valid for all segment registers except CS.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            this.Selector.Selector_Value = 0x00;

            this.Cache_u_Segment_Base = 0x00000000ffff0000;
            this.Cache_u_Segment_LimitScaled = 0x0000ffff;
            this.Cache_Valid = (UInt16)(SegmentRegister.Enum_AccessStatus.SegValidCache | SegmentRegister.Enum_AccessStatus.SegAccessROK | SegmentRegister.Enum_AccessStatus.SegAccessWOK);
            this.Cache_Present = true;
            this.Cache_DPL = 0;
            this.Cache_Segment = SegmentRegister.Enum_SegmentType.DataOrCode;
            this.Cache_Type = SegmentRegister.ENUM_SystemGateDescription.DataReadWriteAccessed;
            this.Cache_u_Segment_Base = 0x00000000;
            this.Cache_u_Segment_LimitScaled = 0xFFFF;

            this.Cache_u_Segment_G = SegmentRegister.Enum_Granularity.gradularity_byte; /* byte granular */
            this.Cache_u_Segment_D_B = SegmentRegister.Enum_SegmentDB.bit16; /* 16bit default size */
            this.Cache_u_Segment_LongMode = SegmentRegister.Enum_LongMode.compat; /* 16bit default size */
            this.Cache_u_Segment_AVL = 0;
        }


        protected  void ParseSelector(UInt16 RawSelector)
        {
            this.Selector.Selector_Value = RawSelector;
            this.Selector.Selector_Index = (UInt16)(RawSelector >> 3);
            this.Selector.Selector_TI = (byte)((RawSelector >> 2) & 0x01);
            this.Selector.Selector_RPL = (byte)(RawSelector & 0x03);
        }
        #endregion

    }
}
