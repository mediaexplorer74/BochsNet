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
    /// Interaction logic for CPUInstructions.xaml
    /// </summary>
    public partial class CPUInstructions : UserControl
    {

        ObservableCollection<LVItem> LVItem = new ObservableCollection<LVItem>();
        

        public CPUInstructions()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Core.Monitor.EventManager.CallMeOn("CPU.OnExecuteInstruction", OnExecuteInstruction);
            }
        }


        #region "Methods"

      


        public void OnExecuteInstruction(string EventName, Core.Component Sender, Core.Monitor.EventArgument Arg)
        {
            this.lstInstruction.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, 
            new Action(
            delegate(){
                int Index = this.lstInstruction.Items.Add(new LVItem
                {
                    Address =string.Format("0x{0:x16}", ((CPU.Event_Arguments.InstructionEventArgument)Arg).Instruction.InstructionAddress),
                    Instruction = ((CPU.Event_Arguments.InstructionEventArgument)Arg).Description
                });
                this.lstInstruction.Items.MoveCurrentToFirst();
            }));
            
            
        }

        #endregion 

        private void UserControl_Initialized(object sender, EventArgs e)
        {
           
        }

       
       
    }


    public class LVItem
    {
        public string Address { get; set; }
        public string Instruction { get; set; }
    }
}
