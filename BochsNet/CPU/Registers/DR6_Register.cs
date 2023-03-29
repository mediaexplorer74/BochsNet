using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// DR6 - Debug status
    /// <para>The debug status register permits the debugger to determine which debug conditions have occurred. </para>
    /// <para>When the processor detects an enabled debug exception, it sets the low-order bits of this register (0,1,2,3) before entering the debug exception handler.</para>
    /// <para>Note that the bits of DR6 are never cleared by the processor. To avoid any confusion in identifying the next debug exception, </para>
    /// <para>the debug handler should move zeros to DR6 immediately before returning.</para>
    /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
    /// </summary>
    public class DR6_Register: DR_Register 
    {

        
        #region "Attributes"


        #endregion 

        #region "Properties"


        #endregion 

        #region "Constructor"

        public DR6_Register()
        {
        }

        public DR6_Register(string Name32)
            : base(Name32)
        {
            
        }

        #endregion 
    }
}
