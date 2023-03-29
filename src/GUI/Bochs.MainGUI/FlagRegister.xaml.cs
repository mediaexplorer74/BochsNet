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
    /// Interaction logic for FlagRegister.xaml
    /// </summary>
    public partial class FlagRegister : UserControl
    {


        /*
        /// The FLAGS register is the status register in Intel x86 microprocessors that contains the current state of the processor. 
        /// <para>This register is 16 bits wide. Its successors, the EFLAGS and RFLAGS registers are 32 bits and 64 bits wide, respectively. </para>
        /// <para>The wider registers retain compatibility with their smaller predecessors.</para>
        /// <see cref="http://en.wikipedia.org/wiki/FLAGS_register"/></para>
        /// <para>31|30|29|28| 27|26|25|24| 23|22|21|20| 19|18|17|16</para>
        /// <para> ==|==|=====| ==|==|==|==| ==|==|==|==| ==|==|==|==</para>
        /// <para>  0| 0| 0| 0|  0| 0| 0| 0|  0| 0|ID|VP| VF|AC|VM|RF</para>
        /// <para></para>
        /// <para> 15|14|13|12| 11|10| 9| 8|  7| 6| 5| 4|  3| 2| 1| 0</para>
        /// <para> ==|==|=====| ==|==|==|==| ==|==|==|==| ==|==|==|==</para>
        /// <para>  0|NT| IOPL| OF|DF|IF|TF| SF|ZF| 0|AF|  0|PF| 1|CF</para>   
        */


        public FlagRegister()
        {
            InitializeComponent();
            DesignInit();
            

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Core.Monitor.EventManager.CallMeOn("CPU.OnExecuteInstruction", OnExecuteInstruction);
            }
        }

        protected void DesignInit()
        {
            txtCF.Foreground = Brushes.Gray;
            txtPF.Foreground = Brushes.Gray;
            txtAF.Foreground = Brushes.Gray;
            txtZF.Foreground = Brushes.Gray;
            txtSF.Foreground = Brushes.Gray;
            lblValue.Content = "RFlags: N/A";
        }

        public void OnExecuteInstruction(string EventName, Core.Component Sender, Core.Monitor.EventArgument Arg)
        {
            this.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                CPU.CPU oCPU = ((CPU.CPU)Sender);

                lblValue.Content = String.Format ("Flag: 0x{0:x16}", oCPU.RFlags.Value64);

                if (oCPU.RFlags.CF == Definitions.Enumerations.Enum_Signal.High)
                    txtCF.Foreground = Brushes.Yellow;
                else
                    txtCF.Foreground = Brushes.Gray;

                if (oCPU.RFlags.PF == Definitions.Enumerations.Enum_Signal.High)
                    txtPF.Foreground = Brushes.Yellow;
                else
                    txtPF.Foreground = Brushes.Gray;
                
                if (oCPU.RFlags.AF == Definitions.Enumerations.Enum_Signal.High)
                    txtAF.Foreground = Brushes.Yellow;
                else
                    txtAF.Foreground = Brushes.Gray;

                if (oCPU.RFlags.ZF == Definitions.Enumerations.Enum_Signal.High)
                    txtZF.Foreground = Brushes.Yellow;
                else
                    txtZF.Foreground = Brushes.Gray;


                if (oCPU.RFlags.SF == Definitions.Enumerations.Enum_Signal.High)
                    txtSF.Foreground = Brushes.Yellow;
                else
                    txtSF.Foreground = Brushes.Gray;


                if (oCPU.RFlags.TF == Definitions.Enumerations.Enum_Signal.High)
                    txtTF.Foreground = Brushes.Yellow;
                else
                    txtTF.Foreground = Brushes.Gray;
                
            }));


        }
    }
}
