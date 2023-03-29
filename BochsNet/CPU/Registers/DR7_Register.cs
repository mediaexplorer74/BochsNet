using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// DR7 - Debug control
    /// <para>The low-order eight bits of DR7 (0,2,4,6 and 1,3,5,7) selectively enable the four address breakpoint conditions.</para>
    /// <para>There are two levels of enabling: the local (0,2,4,6) and global (1,3,5,7) levels. </para>
    /// <para>The local enable bits are automatically reset by the processor at every task switch to avoid unwanted breakpoint conditions in the new task.</para>
    /// <para>The global enable bits are not reset by a task switch; therefore, they can be used for conditions that are global to all tasks.</para>
    /// <para>Bits 16-17 (DR0), 20-21 (DR1), 24-25 (DR2), 28-29 (DR3), define when breakpoints trigger.</para>
    /// <para> Each breakpoint has a two-bit entry that specifies whether they break on execution (00b), data write (01b), data read or write (11b). </para>
    /// <para>10b is defined to mean break on IO read or write but no hardware supports it. Bits 18-19 (DR0), 22-23 (DR1), 26-27 (DR2), 30-31 (DR3), define how large area of memory is watched by breakpoints. Again each breakpoint has a two-bit entry that specifies whether they watch one (00b), two (01b), eight (10b) or four (11b) bytes.</para>
    /// <see cref="http://en.wikipedia.org/wiki/X86_debug_register"/>
    /// </summary>
    public class DR7_Register: DR_Register 
    {

        
        #region "Attributes"


        #endregion 

        #region "Properties"


        #endregion 

        #region "Constructor"

        public DR7_Register()
        {
        }

        public DR7_Register(string Name32)
            : base(Name32)
        {
            
        }

        #endregion 
    }
}
