using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IO.PCI
{
    public class PCI
    {

        public byte RD_MemoryType(UInt32 Address)
        {
            switch ((Address & 0xFC000) >> 12)
            {
                case 0xFC:
                    // return ((BX_PCI_THIS s.i440fx.pci_conf[0x59] >> 4) & 0x1);
                    return 0x0; //hard coded;
            }

            return 0;
        }




    }
}
