using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BxImage.Tools
{
    public partial class BxImagectrl : UserControl
    {
        private bool bHardDisk;

        public BxImagectrl()
        {
            InitializeComponent();
        }

        private void BxImage_Load(object sender, EventArgs e)
        {
            bHardDisk = true;
            EnableFD(false);
            EnableHD(true);
            this.cbFloppyDiskSize.SelectedIndex = 0;
        }

        private void radioHD_CheckedChanged(object sender, EventArgs e)
        {
            EnableFD(false);
            EnableHD(true);
        }

        private void radioFD_CheckedChanged(object sender, EventArgs e)
        {
            EnableFD(true);
            EnableHD(false);
        }


        #region "GUI Handling"

        private void EnableFD(bool bEnable)
        {
            this.cbFloppyDiskSize.Enabled = bEnable;
        }


        private void EnableHD(bool bEnable)
        {
            this.cbHDType.Enabled = bEnable;
            this.txtHDSize.Enabled = bEnable;
        }
        #endregion

        private void btnMakeDisk_Click(object sender, EventArgs e)
        {
            long Cylender = 0, Heads = 0, SectorPerTrack = 0;
            DiskBase oDiskBase = null;

            if (bHardDisk)
            {
                
                long HDSize=0;
                if (long.TryParse(this.txtHDSize.Text, out HDSize) == true)
                {
                    Cylender = (long)(HDSize * 1024.0 * 1024.0 / 16.0 / 63.0 / 512.0);
                    Heads = 16;
                    SectorPerTrack = 63;
                    
                    oDiskBase = (DiskBase)new HDMaker(Cylender, Heads, SectorPerTrack,(HDMaker.EnumHDType) (cbHDType.SelectedIndex));
                    MessageBox.Show("Task Completed");
                }
                else
                {
                    MessageBox.Show("Invalid Cylender value");
                }
            }
            else
            {

                switch (this.cbFloppyDiskSize.SelectedIndex)
                {
                    case 0: Cylender = 40; Heads = 1; SectorPerTrack = 8; break;  /* 0.16 meg */
                    case 1: Cylender = 40; Heads = 1; SectorPerTrack = 9; break;  /* 0.18 meg */
                    case 2: Cylender = 40; Heads = 2; SectorPerTrack = 8; break;  /* 0.32 meg */
                    case 3: Cylender = 40; Heads = 2; SectorPerTrack = 9; break;  /* 0.36 meg */
                    case 4: Cylender = 80; Heads = 2; SectorPerTrack = 9; break;  /* 0.72 meg */
                    case 5: Cylender = 80; Heads = 2; SectorPerTrack = 15; break; /* 1.2 meg */
                    case 6: Cylender = 80; Heads = 2; SectorPerTrack = 18; break; /* 1.44 meg */
                    case 7: Cylender = 80; Heads = 2; SectorPerTrack = 21; break; /* 1.68 meg */
                    case 8: Cylender = 82; Heads = 2; SectorPerTrack = 21; break; /* 1.72 meg */
                    case 9: Cylender = 80; Heads = 2; SectorPerTrack = 36; break; /* 2.88 meg */
                    default:
                        System.Windows.Forms.MessageBox.Show("Flobby disk size is out of range");
                        return;

                }

                oDiskBase = (DiskBase)new FloppyDiskMaker(Cylender, Heads, SectorPerTrack);
            }

            SaveFileDialog oSaveFileDlg = new SaveFileDialog();
            oSaveFileDlg.Title = "Select Files";
            oSaveFileDlg.Filter="*.img|(*.img)|*.*|(*.*) All Files";
            if (oSaveFileDlg.ShowDialog() == DialogResult.OK)
            {
                 
                //System.IO.StreamWriter oSW = new System.IO.StreamWriter(System.IO.File.Create(oSaveFileDlg.FileName));
                oDiskBase.WriteDisk(oSaveFileDlg.FileName);
                

            }
        }



    }
}
