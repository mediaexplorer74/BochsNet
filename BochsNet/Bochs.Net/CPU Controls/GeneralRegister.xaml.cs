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
    /// Interaction logic for GeneralRegister.xaml
    /// </summary>
    public partial class GeneralRegister : UserControl
    {

        #region "Properties"

        public CPU.Registers.Register64  CPUGeneralRegister
        {
            set
            {
                lblName64.Content = value.Name64;
                lblValue64.Content  = String.Format ("0x{0:x16}",value.Value64);
                
            
                
                if (value.Name32 == string.Empty)
                {
                    lblName32.Visibility = System.Windows.Visibility.Hidden;
                    lblValue32.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    lblName32.Content = value.Name32;
                    lblValue32.Content = String.Format("0x{0:x16}", value.Value32);
                }

                if (value.Name16 == string.Empty)
                {
                    lblName16.Visibility = System.Windows.Visibility.Hidden;
                    lblValue16.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    lblName16.Content = value.Name16;
                    lblValue16.Content = String.Format("0x{0:x4}", value.Value16);
                }

                if (value.Name8H  == string.Empty)
                {
                    lblName8H.Visibility = System.Windows.Visibility.Hidden;
                    lblValue8H.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    lblName8H.Content = value.Name8H;
                    lblValue8H.Content = String.Format("0x{0:x2}", value.Value8H);
                }

                if (value.Name8 == string.Empty)
                {
                    lblName8L.Visibility = System.Windows.Visibility.Hidden;
                    lblValue8L.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    lblName8L.Content = value.Name8;
                    lblValue8L.Content = String.Format("0x{0:x2}", value.Value8);
                }
            }
        }

        #endregion 


        public GeneralRegister()
        {
            InitializeComponent();

            
        }

    }
}
