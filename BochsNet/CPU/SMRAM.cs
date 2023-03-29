using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace CPU
{
    public class SMRAM : Dictionary<SMRAM.SMMRAM_Fields, UInt16>
    {

        #region "Enumeration"

        public enum SMMRAM_Fields
        {
            SMRAM_FIELD_SMBASE_OFFSET = 0,
            SMRAM_FIELD_SMM_REVISION_ID,
            SMRAM_FIELD_RAX_HI32,
            SMRAM_FIELD_EAX,
            SMRAM_FIELD_RCX_HI32,
            SMRAM_FIELD_ECX,
            SMRAM_FIELD_RDX_HI32,
            SMRAM_FIELD_EDX,
            SMRAM_FIELD_RBX_HI32,
            SMRAM_FIELD_EBX,
            SMRAM_FIELD_RSP_HI32,
            SMRAM_FIELD_ESP,
            SMRAM_FIELD_RBP_HI32,
            SMRAM_FIELD_EBP,
            SMRAM_FIELD_RSI_HI32,
            SMRAM_FIELD_ESI,
            SMRAM_FIELD_RDI_HI32,
            SMRAM_FIELD_EDI,
            SMRAM_FIELD_R8_HI32,
            SMRAM_FIELD_R8,
            SMRAM_FIELD_R9_HI32,
            SMRAM_FIELD_R9,
            SMRAM_FIELD_R10_HI32,
            SMRAM_FIELD_R10,
            SMRAM_FIELD_R11_HI32,
            SMRAM_FIELD_R11,
            SMRAM_FIELD_R12_HI32,
            SMRAM_FIELD_R12,
            SMRAM_FIELD_R13_HI32,
            SMRAM_FIELD_R13,
            SMRAM_FIELD_R14_HI32,
            SMRAM_FIELD_R14,
            SMRAM_FIELD_R15_HI32,
            SMRAM_FIELD_R15,
            SMRAM_FIELD_RIP_HI32,
            SMRAM_FIELD_EIP,
            SMRAM_FIELD_RFLAGS_HI32,  // always zero
            SMRAM_FIELD_EFLAGS,
            SMRAM_FIELD_DR6_HI32,     // always zero
            SMRAM_FIELD_DR6,
            SMRAM_FIELD_DR7_HI32,     // always zero
            SMRAM_FIELD_DR7,
            SMRAM_FIELD_CR0_HI32,     // always zero
            SMRAM_FIELD_CR0,
            SMRAM_FIELD_CR3_HI32,     // zero when physical address size 32-bit
            SMRAM_FIELD_CR3,
            SMRAM_FIELD_CR4_HI32,     // always zero
            SMRAM_FIELD_CR4,
            SMRAM_FIELD_EFER_HI32,    // always zero
            SMRAM_FIELD_EFER,
            SMRAM_FIELD_IO_INSTRUCTION_RESTART,
            SMRAM_FIELD_AUTOHALT_RESTART,
            SMRAM_FIELD_NMI_MASK,
            SMRAM_FIELD_TR_BASE_HI32,
            SMRAM_FIELD_TR_BASE,
            SMRAM_FIELD_TR_LIMIT,
            SMRAM_FIELD_TR_SELECTOR_AR,
            SMRAM_FIELD_LDTR_BASE_HI32,
            SMRAM_FIELD_LDTR_BASE,
            SMRAM_FIELD_LDTR_LIMIT,
            SMRAM_FIELD_LDTR_SELECTOR_AR,
            SMRAM_FIELD_IDTR_BASE_HI32,
            SMRAM_FIELD_IDTR_BASE,
            SMRAM_FIELD_IDTR_LIMIT,
            SMRAM_FIELD_GDTR_BASE_HI32,
            SMRAM_FIELD_GDTR_BASE,
            SMRAM_FIELD_GDTR_LIMIT,
            SMRAM_FIELD_ES_BASE_HI32,
            SMRAM_FIELD_ES_BASE,
            SMRAM_FIELD_ES_LIMIT,
            SMRAM_FIELD_ES_SELECTOR_AR,
            SMRAM_FIELD_CS_BASE_HI32,
            SMRAM_FIELD_CS_BASE,
            SMRAM_FIELD_CS_LIMIT,
            SMRAM_FIELD_CS_SELECTOR_AR,
            SMRAM_FIELD_SS_BASE_HI32,
            SMRAM_FIELD_SS_BASE,
            SMRAM_FIELD_SS_LIMIT,
            SMRAM_FIELD_SS_SELECTOR_AR,
            SMRAM_FIELD_DS_BASE_HI32,
            SMRAM_FIELD_DS_BASE,
            SMRAM_FIELD_DS_LIMIT,
            SMRAM_FIELD_DS_SELECTOR_AR,
            SMRAM_FIELD_FS_BASE_HI32,
            SMRAM_FIELD_FS_BASE,
            SMRAM_FIELD_FS_LIMIT,
            SMRAM_FIELD_FS_SELECTOR_AR,
            SMRAM_FIELD_GS_BASE_HI32,
            SMRAM_FIELD_GS_BASE,
            SMRAM_FIELD_GS_LIMIT,
            SMRAM_FIELD_GS_SELECTOR_AR,
            SMRAM_FIELD_LAST
        };

        #endregion

        #region "Attributes"

        protected bool mReady;
        protected bool mIn_SMM;
        protected bool mSMM_Mode;

        #endregion


        #region "Properties"

        public bool Ready
        {
            get
            {
                return mReady;
            }
        }

        public bool In_SMM
        {
            get
            {
                return mIn_SMM;
            }

            set
            {
                mIn_SMM = value;
            }
        }

        public bool SMM_Mode
        {
            get
            {
                return mSMM_Mode;
            }

            set
            {
                mSMM_Mode = value;
            }
        }


        #endregion

        #region "Constructors"

        public SMRAM()
        {
            mIn_SMM = true;
            mReady = false;
        }

        #endregion

        #region "Methods"

        public void Add(SMMRAM_Fields Field, UInt16 Address)
        {
            base.Add(Field, SMRAM_Translate(Address));
        }

        public void InitSMMRAM()
        {
            mReady = false;

            this.Clear();

            this.Add(SMMRAM_Fields.SMRAM_FIELD_SMBASE_OFFSET, SMRAM_Translate(0x7f00));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_SMM_REVISION_ID, SMRAM_Translate(0x7efc));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RAX_HI32, SMRAM_Translate(0x7ffc));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EAX, SMRAM_Translate(0x7ff8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RCX_HI32, SMRAM_Translate(0x7ff4));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ECX, SMRAM_Translate(0x7ff0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RDX_HI32, SMRAM_Translate(0x7fec));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EDX, SMRAM_Translate(0x7fe8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RBX_HI32, SMRAM_Translate(0x7fe4));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EBX, SMRAM_Translate(0x7fe0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RSP_HI32, SMRAM_Translate(0x7fdc));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ESP, SMRAM_Translate(0x7fd8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RBP_HI32, SMRAM_Translate(0x7fd4));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EBP, SMRAM_Translate(0x7fd0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RSI_HI32, SMRAM_Translate(0x7fcc));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ESI, SMRAM_Translate(0x7fc8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RDI_HI32, SMRAM_Translate(0x7fc4));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EDI, SMRAM_Translate(0x7fc0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R8_HI32, SMRAM_Translate(0x7fbc));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R8, SMRAM_Translate(0x7fb8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R9_HI32, SMRAM_Translate(0x7fb4));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R9, SMRAM_Translate(0x7fb0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R10_HI32, SMRAM_Translate(0x7fac));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R10, SMRAM_Translate(0x7fa8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R11_HI32, SMRAM_Translate(0x7fa4));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R11, SMRAM_Translate(0x7fa0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R12_HI32, SMRAM_Translate(0x7f9c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R12, SMRAM_Translate(0x7f98));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R13_HI32, SMRAM_Translate(0x7f94));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R13, SMRAM_Translate(0x7f90));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R14_HI32, SMRAM_Translate(0x7f8c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R14, SMRAM_Translate(0x7f88));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R15_HI32, SMRAM_Translate(0x7f84));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_R15, SMRAM_Translate(0x7f80));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RIP_HI32, SMRAM_Translate(0x7f7c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EIP, SMRAM_Translate(0x7f78));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_RFLAGS_HI32, SMRAM_Translate(0x7f74)); // always zero
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EFLAGS, SMRAM_Translate(0x7f70));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DR6_HI32, SMRAM_Translate(0x7f6c));    // always zero
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DR6, SMRAM_Translate(0x7f68));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DR7_HI32, SMRAM_Translate(0x7f64));    // always zero
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DR7, SMRAM_Translate(0x7f60));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CR0_HI32, SMRAM_Translate(0x7f5c));    // always zero
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CR0, SMRAM_Translate(0x7f58));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CR3_HI32, SMRAM_Translate(0x7f54));    // zero when physical address size 32-bit
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CR3, SMRAM_Translate(0x7f50));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CR4_HI32, SMRAM_Translate(0x7f4c));    // always zero
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CR4, SMRAM_Translate(0x7f48));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EFER_HI32, SMRAM_Translate(0x7ed4));   // always zero
            this.Add(SMMRAM_Fields.SMRAM_FIELD_EFER, SMRAM_Translate(0x7ed0));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_IO_INSTRUCTION_RESTART, SMRAM_Translate(0x7ec8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_AUTOHALT_RESTART, SMRAM_Translate(0x7ec8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_NMI_MASK, SMRAM_Translate(0x7ec8));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_TR_BASE_HI32, SMRAM_Translate(0x7e9c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_TR_BASE, SMRAM_Translate(0x7e98));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_TR_LIMIT, SMRAM_Translate(0x7e94));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_TR_SELECTOR_AR, SMRAM_Translate(0x7e90));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_IDTR_BASE_HI32, SMRAM_Translate(0x7e8c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_IDTR_BASE, SMRAM_Translate(0x7e88));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_IDTR_LIMIT, SMRAM_Translate(0x7e84));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_LDTR_BASE_HI32, SMRAM_Translate(0x7e7c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_LDTR_BASE, SMRAM_Translate(0x7e78));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_LDTR_LIMIT, SMRAM_Translate(0x7e74));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_LDTR_SELECTOR_AR, SMRAM_Translate(0x7e70));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GDTR_BASE_HI32, SMRAM_Translate(0x7e6c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GDTR_BASE, SMRAM_Translate(0x7e68));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GDTR_LIMIT, SMRAM_Translate(0x7e64));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ES_BASE_HI32, SMRAM_Translate(0x7e0c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ES_BASE, SMRAM_Translate(0x7e08));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ES_LIMIT, SMRAM_Translate(0x7e04));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_ES_SELECTOR_AR, SMRAM_Translate(0x7e00));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CS_BASE_HI32, SMRAM_Translate(0x7e1c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CS_BASE, SMRAM_Translate(0x7e18));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CS_LIMIT, SMRAM_Translate(0x7e14));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_CS_SELECTOR_AR, SMRAM_Translate(0x7e10));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_SS_BASE_HI32, SMRAM_Translate(0x7e2c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_SS_BASE, SMRAM_Translate(0x7e28));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_SS_LIMIT, SMRAM_Translate(0x7e24));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_SS_SELECTOR_AR, SMRAM_Translate(0x7e20));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DS_BASE_HI32, SMRAM_Translate(0x7e3c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DS_BASE, SMRAM_Translate(0x7e38));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DS_LIMIT, SMRAM_Translate(0x7e34));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_DS_SELECTOR_AR, SMRAM_Translate(0x7e30));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_FS_BASE_HI32, SMRAM_Translate(0x7e4c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_FS_BASE, SMRAM_Translate(0x7e48));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_FS_LIMIT, SMRAM_Translate(0x7e44));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_FS_SELECTOR_AR, SMRAM_Translate(0x7e40));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GS_BASE_HI32, SMRAM_Translate(0x7e5c));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GS_BASE, SMRAM_Translate(0x7e58));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GS_LIMIT, SMRAM_Translate(0x7e54));
            this.Add(SMMRAM_Fields.SMRAM_FIELD_GS_SELECTOR_AR, SMRAM_Translate(0x7e50));

            mReady = true;
        }

        /// <summary>
        /// Translate address to SMM (((0x8000 - (addr)) >> 2) - 1)
        /// The menimum amount of SMRAM that can be implemented is 
        /// SMRAM base + 0x8000 to SMRAM base + 0xffff
        /// <see>http://books.google.com/books?id=TVzjEZg1--YC&pg=PA226&lpg=PA226&dq=what+is+CPU+SMRAM&source=bl&ots=iz8zMJu2G-&sig=aJsqxCiDLhM9mNhhkwaI8fb5a5I&hl=en&ei=LaxuTNKHD8TJ4Aacnq2_Cw&sa=X&oi=book_result&ct=result&resnum=7&ved=0CDcQ6AEwBg#v=onepage&q=what%20is%20CPU%20SMRAM&f=false</see>
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        protected UInt16 SMRAM_Translate(UInt16 Address)
        {
            return (UInt16)(((0x8000 - (Address)) >> 2) - 1);
        }



        #endregion
    }
}
