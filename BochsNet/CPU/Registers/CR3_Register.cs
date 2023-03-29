using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Definitions.Enumerations;

namespace CPU.Registers
{
    /// <summary>
    /// CR3 Register
    /// <para>The upper 20 bits of CR3 are called the Page Directory Base Register (PDBR).</para>
    /// <para>The PDBR holds the physical address of the page directory.</para>
    /// <see>http://en.wikibooks.org/wiki/X86_Assembly/Protected_Mode</see>
    /// </summary>
    public class CR3_Register: CR_Register 
    {

        
        #region "Attributes"


        #endregion 

        #region "Properties"


        #endregion 

        #region "Constructor"

        public CR3_Register()
        {
            mName = "CR3";
        }


        #endregion 
    }
}
