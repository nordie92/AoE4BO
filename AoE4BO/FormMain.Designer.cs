namespace AoE4BO
{
    partial class FormMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbState = new System.Windows.Forms.Label();
            this.btnOpenBO = new System.Windows.Forms.Button();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnBoFromClipboard = new System.Windows.Forms.Button();
            this.btnRestartBO = new System.Windows.Forms.Button();
            this.rtbBuildOrder = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lbState
            // 
            this.lbState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbState.Location = new System.Drawing.Point(137, 285);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(295, 31);
            this.lbState.TabIndex = 14;
            this.lbState.Text = "No build order selected...";
            this.lbState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOpenBO
            // 
            this.btnOpenBO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenBO.Location = new System.Drawing.Point(16, 320);
            this.btnOpenBO.Name = "btnOpenBO";
            this.btnOpenBO.Size = new System.Drawing.Size(130, 58);
            this.btnOpenBO.TabIndex = 7;
            this.btnOpenBO.Text = "Open build order file";
            this.btnOpenBO.UseVisualStyleBackColor = true;
            this.btnOpenBO.Click += new System.EventHandler(this.btnOpenBO_Click);
            // 
            // timerUI
            // 
            this.timerUI.Enabled = true;
            this.timerUI.Interval = 500;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSettings.Location = new System.Drawing.Point(308, 320);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(124, 58);
            this.btnSettings.TabIndex = 7;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnBoFromClipboard
            // 
            this.btnBoFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBoFromClipboard.Location = new System.Drawing.Point(153, 320);
            this.btnBoFromClipboard.Name = "btnBoFromClipboard";
            this.btnBoFromClipboard.Size = new System.Drawing.Size(148, 58);
            this.btnBoFromClipboard.TabIndex = 7;
            this.btnBoFromClipboard.Text = "Open build order from clipboard";
            this.btnBoFromClipboard.UseVisualStyleBackColor = true;
            this.btnBoFromClipboard.Click += new System.EventHandler(this.btnBoFromClipboard_Click);
            // 
            // btnRestartBO
            // 
            this.btnRestartBO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestartBO.Location = new System.Drawing.Point(17, 283);
            this.btnRestartBO.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRestartBO.Name = "btnRestartBO";
            this.btnRestartBO.Size = new System.Drawing.Size(104, 31);
            this.btnRestartBO.TabIndex = 16;
            this.btnRestartBO.Text = "Restart BO";
            this.btnRestartBO.UseVisualStyleBackColor = true;
            this.btnRestartBO.Click += new System.EventHandler(this.btnRestartBO_Click);
            // 
            // rtbBuildOrder
            // 
            this.rtbBuildOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbBuildOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rtbBuildOrder.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbBuildOrder.Location = new System.Drawing.Point(18, 18);
            this.rtbBuildOrder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbBuildOrder.Name = "rtbBuildOrder";
            this.rtbBuildOrder.ReadOnly = true;
            this.rtbBuildOrder.Size = new System.Drawing.Size(414, 255);
            this.rtbBuildOrder.TabIndex = 17;
            this.rtbBuildOrder.Text = "";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 395);
            this.Controls.Add(this.rtbBuildOrder);
            this.Controls.Add(this.btnRestartBO);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnBoFromClipboard);
            this.Controls.Add(this.btnOpenBO);
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.Text = "AoE4 Build Order";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Button btnOpenBO;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnBoFromClipboard;
        private System.Windows.Forms.Button btnRestartBO;
        private System.Windows.Forms.RichTextBox rtbBuildOrder;
    }
}

