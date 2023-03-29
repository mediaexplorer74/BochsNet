namespace BxImage.Tools
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bxImage1 = new BxImage.Tools.BxImagectrl();
            this.SuspendLayout();
            // 
            // bxImage1
            // 
            this.bxImage1.Location = new System.Drawing.Point(12, 12);
            this.bxImage1.Name = "bxImage1";
            this.bxImage1.Size = new System.Drawing.Size(364, 172);
            this.bxImage1.TabIndex = 0;
            this.bxImage1.Load += new System.EventHandler(this.bxImage1_Load);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 200);
            this.Controls.Add(this.bxImage1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "BxImage";
            this.ResumeLayout(false);

        }

        #endregion

        private BxImagectrl bxImage1;

      
    }
}

