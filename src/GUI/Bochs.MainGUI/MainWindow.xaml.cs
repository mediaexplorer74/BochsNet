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

namespace Bochs.MainGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Threading.Thread mMachineThread;

        CPUDialog oCPUDialog;
        PCMachine.Machine oMachine;

        public MainWindow()
        {
            InitializeComponent();

            oCPUDialog = new CPUDialog();
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Init();
            }
        }

        private void Init()
        {
            oMachine = new PCMachine.Machine();
            oMachine.Initialize();
        }



        private void RunThread()
        {
            if (mMachineThread != null) mMachineThread.Abort();

            mMachineThread = new System.Threading.Thread(this.Start);
            mMachineThread.SetApartmentState(System.Threading.ApartmentState.STA);
            mMachineThread.Start();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Please use single step if you are not debugging in VS2010.\r\nPlease use single step.\r\nDo you want to Continue ?", "Warning Message", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }
            RunThread();
        }


        public void Start ()
        {
           
            oMachine.Start();
        }

        private void btnCPU_Click(object sender, RoutedEventArgs e)
        {
         oCPUDialog.Show();    
        }

        private void btnDebugSingleStep_Click(object sender, RoutedEventArgs e)
        {
            if (mMachineThread == null)
            {
                oMachine.CPU.SingleDebugStep = Core.CPU.CPUBase.CPUDebugMode.SingleStep;
                RunThread ();
            }
            oMachine.CPU.SingleStep();
        }

        private void btnShutdown_Click(object sender, RoutedEventArgs e)
        {
            oMachine.CPU.Shutdown();
            Application.Current.Shutdown();
        }
    }
}
