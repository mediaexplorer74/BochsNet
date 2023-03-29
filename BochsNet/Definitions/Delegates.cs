using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Definitions.Delegates
{
    /// <summary>
    /// used when implementing a function to be used by IO
    /// </summary>
    /// <param name="Data"></param>
    public delegate void DeviceWriteByte(UInt64 Address, byte Value);
    
    /// <summary>
    /// used when implementing a function to be used by IO
    /// </summary>
    /// <param name="Data"></param>
    public delegate byte DeviceReadByte(UInt64 Address);


    /// <summary>
    /// used when implementing a function to be used by IO
    /// </summary>
    /// <param name="Data"></param>
    public delegate void DeviceWrite(UInt64 Address, UInt64 Length , byte[] Data, UInt64 StartDataIndex);

    /// <summary>
    /// used when implementing a function to be used by IO
    /// </summary>
    /// <param name="Data"></param>
    public delegate byte[] DeviceRead(UInt64 Address, UInt64 Length);
   

    /// <summary>
    /// used when implementing a function to be used by DMA
    /// </summary>
    /// <param name="Data"></param>
    public delegate void delegate_DMARead(byte[] Data);
    /// <summary>
    /// used when implementing a function to be used by DMA
    /// </summary>
    /// <param name="Data"></param>
    public delegate void delegate_DMAWrite(byte[] Data);

    /// <summary>
    /// Used When implementing a function to be called by scheduler
    /// </summary>
    public delegate void Fire();

    public delegate void IRQRegister(uint IRQ);
    public delegate void IRQRaise(uint IRQ);
    public delegate void IRQLow(uint IRQ);



    
}
