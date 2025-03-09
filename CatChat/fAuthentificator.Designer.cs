namespace CatChat
{
    partial class fAuthentificator
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
            this.lNameTitle = new System.Windows.Forms.Label();
            this.lAuthentificatorTitle = new System.Windows.Forms.Label();
            this.lIPTitle = new System.Windows.Forms.Label();
            this.tbNameValue = new System.Windows.Forms.TextBox();
            this.lIPValue = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lNameTitle
            // 
            this.lNameTitle.AutoSize = true;
            this.lNameTitle.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lNameTitle.Location = new System.Drawing.Point(12, 69);
            this.lNameTitle.Name = "lNameTitle";
            this.lNameTitle.Size = new System.Drawing.Size(142, 23);
            this.lNameTitle.TabIndex = 1;
            this.lNameTitle.Text = "Connect as:";
            this.lNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lAuthentificatorTitle
            // 
            this.lAuthentificatorTitle.AutoSize = true;
            this.lAuthentificatorTitle.Font = new System.Drawing.Font("Courier New", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lAuthentificatorTitle.Location = new System.Drawing.Point(145, 18);
            this.lAuthentificatorTitle.Name = "lAuthentificatorTitle";
            this.lAuthentificatorTitle.Size = new System.Drawing.Size(236, 27);
            this.lAuthentificatorTitle.TabIndex = 2;
            this.lAuthentificatorTitle.Text = "Aithentification";
            this.lAuthentificatorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lIPTitle
            // 
            this.lIPTitle.AutoSize = true;
            this.lIPTitle.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lIPTitle.Location = new System.Drawing.Point(12, 106);
            this.lIPTitle.Name = "lIPTitle";
            this.lIPTitle.Size = new System.Drawing.Size(118, 23);
            this.lIPTitle.TabIndex = 3;
            this.lIPTitle.Text = "Your IP: ";
            this.lIPTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbNameValue
            // 
            this.tbNameValue.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbNameValue.Location = new System.Drawing.Point(169, 63);
            this.tbNameValue.Name = "tbNameValue";
            this.tbNameValue.Size = new System.Drawing.Size(366, 30);
            this.tbNameValue.TabIndex = 4;
            // 
            // lIPValue
            // 
            this.lIPValue.AutoSize = true;
            this.lIPValue.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lIPValue.Location = new System.Drawing.Point(174, 106);
            this.lIPValue.Name = "lIPValue";
            this.lIPValue.Size = new System.Drawing.Size(0, 23);
            this.lIPValue.TabIndex = 5;
            this.lIPValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Snow;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.Location = new System.Drawing.Point(12, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(167, 36);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.Snow;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnConfirm.Location = new System.Drawing.Point(373, 166);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(167, 36);
            this.btnConfirm.TabIndex = 7;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // fAuthentificator
            // 
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(552, 233);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lIPValue);
            this.Controls.Add(this.tbNameValue);
            this.Controls.Add(this.lIPTitle);
            this.Controls.Add(this.lAuthentificatorTitle);
            this.Controls.Add(this.lNameTitle);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(570, 280);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(570, 280);
            this.Name = "fAuthentificator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CatChat | Authentification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lNameTitle;
        private System.Windows.Forms.Label lAuthentificatorTitle;
        private System.Windows.Forms.Label lIPTitle;
        private System.Windows.Forms.TextBox tbNameValue;
        private System.Windows.Forms.Label lIPValue;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
    }
}