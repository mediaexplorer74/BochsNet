using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Bochs.MainGUI
{
    /// <summary>
    /// Interaction logic for CPUGeneralRegisters.xaml
    /// </summary>
    public partial class CPUGeneralRegisters : UserControl
    {
        public CPUGeneralRegisters()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Core.Monitor.EventManager.CallMeOn("CPU.OnExecuteInstruction", OnExecuteInstruction);
            }

        }
            

        public void OnExecuteInstruction(string EventName, Core.Component Sender, Core.Monitor.EventArgument Arg)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                CPU.CPU oCPU = ((CPU.CPU)Sender);
                GR1.CPUGeneralRegister = oCPU.RAX ;
                GR2.CPUGeneralRegister = oCPU.RBX ;
                GR3.CPUGeneralRegister = oCPU.RCX ;
                GR4.CPUGeneralRegister = oCPU.RDX ;
                GR5.CPUGeneralRegister = oCPU.RSP ;
                GR6.CPUGeneralRegister = oCPU.RBP ;
                GR7.CPUGeneralRegister = oCPU.RSI ;
                GR8.CPUGeneralRegister = oCPU.RDI ;
                GR9.CPUGeneralRegister = oCPU.R8 ;
                GR10.CPUGeneralRegister = oCPU.R9;
                GR11.CPUGeneralRegister = oCPU.R10;
                GR12.CPUGeneralRegister = oCPU.R11;
                GR13.CPUGeneralRegister = oCPU.R12;
                GR14.CPUGeneralRegister = oCPU.R13;
                GR15.CPUGeneralRegister = oCPU.R14;
                GR16.CPUGeneralRegister = oCPU.R15;
                GR17.CPUGeneralRegister = oCPU.RIP ;
           }
              ));
        }
        
    }
}
