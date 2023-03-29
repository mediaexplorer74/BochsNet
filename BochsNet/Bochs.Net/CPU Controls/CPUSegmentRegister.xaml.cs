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
    /// Interaction logic for CPUSegmentRegister.xaml
    /// </summary>
    public partial class CPUSegmentRegister : UserControl
    {

        #region "GUI Properties"

        [System.ComponentModel.Category ("Extra")]
        [System.ComponentModel.Browsable (true)]
        public string RegistryName
        {
            get
            {
                return lblRegName.Content.ToString();
            }

            set
            {
                lblRegName.Content = value;
            }
        }

        #endregion

        #region "Properties"

        public CPU.Registers.SegmentRegister CPUGeneralRegister
        {
            set
            {
                //lblName64.Content = value.Name64;
                lblValueBase.Content = String.Format("0x{0:x16}", value.Cache_u_Segment_Base);
                lblValueLimit.Content = String.Format("0x{0:x16}", value.Cache_u_Segment_LimitScaled);
            }


        }

        #endregion 


        public CPUSegmentRegister()
        {
            InitializeComponent();
        }
    }
}
