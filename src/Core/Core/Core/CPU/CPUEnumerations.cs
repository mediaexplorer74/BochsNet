using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.CPU
{
    public enum Enum_CPUModes : short
    {
        /// <summary>
        /// <see cref="http://en.wikipedia.org/wiki/Real_mode"/>
        /// CR0.PE=0 
        /// </summary>
        RealMode = 0x00,
        /// <summary>
        /// <see cref="http://en.wikipedia.org/wiki/Protected_mode"/>
        /// CR0.PE=1, EFLAGS.VM=0   |
        /// </summary>
        ProtectedMode = 0x02,
        /// <summary>
        /// <see cref="http://en.wikipedia.org/wiki/Unreal_mode"/>
        /// </summary>
        UnRealMode = 0x98, // not defined value
        /// <summary>
        /// <see cref="http://en.wikipedia.org/wiki/Virtual_8086_mode"/>
        /// CR0.PE=1, EFLAGS.VM=1   | EFER.LMA=0
        /// </summary>
        Virtual8086Mode = 0x01,
        /// <summary>
        /// <see cref="http://en.wikipedia.org/wiki/System_Management_Mode"/>
        /// </summary>
        SystemManagementMode = 0x99, // not defined value
        /// <summary>
        /// <see cref="http://en.wikipedia.org/wiki/Long_mode"/>
        /// EFER.LMA = 1, CR0.PE=1, CS.L=1
        /// </summary>
        LongMode = 0x4,
        /// <summary>
        ///  EFER.LMA = 1, CR0.PE=1, CS.L=0
        /// </summary>
        LongCompatMode = 0x3
    };
}
