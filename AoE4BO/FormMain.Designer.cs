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
            this.lbState.Location = new System.Drawing.Point(91, 185);
            this.lbState.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(197, 20);
            this.lbState.TabIndex = 14;
            this.lbState.Text = "No build order selected...";
            this.lbState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOpenBO
            // 
            this.btnOpenBO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenBO.Location = new System.Drawing.Point(11, 208);
            this.btnOpenBO.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpenBO.Name = "btnOpenBO";
            this.btnOpenBO.Size = new System.Drawing.Size(87, 38);
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
            this.btnSettings.Location = new System.Drawing.Point(205, 208);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(83, 38);
            this.btnSettings.TabIndex = 7;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnBoFromClipboard
            // 
            this.btnBoFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBoFromClipboard.Location = new System.Drawing.Point(102, 208);
            this.btnBoFromClipboard.Margin = new System.Windows.Forms.Padding(2);
            this.btnBoFromClipboard.Name = "btnBoFromClipboard";
            this.btnBoFromClipboard.Size = new System.Drawing.Size(99, 38);
            this.btnBoFromClipboard.TabIndex = 7;
            this.btnBoFromClipboard.Text = "Open build order from clipboard";
            this.btnBoFromClipboard.UseVisualStyleBackColor = true;
            this.btnBoFromClipboard.Click += new System.EventHandler(this.btnBoFromClipboard_Click);
            // 
            // btnRestartBO
            // 
            this.btnRestartBO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestartBO.Location = new System.Drawing.Point(11, 184);
            this.btnRestartBO.Name = "btnRestartBO";
            this.btnRestartBO.Size = new System.Drawing.Size(69, 20);
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
            this.rtbBuildOrder.DetectUrls = false;
            this.rtbBuildOrder.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbBuildOrder.Location = new System.Drawing.Point(12, 12);
            this.rtbBuildOrder.Name = "rtbBuildOrder";
            this.rtbBuildOrder.ReadOnly = true;
            this.rtbBuildOrder.Size = new System.Drawing.Size(277, 167);
            this.rtbBuildOrder.TabIndex = 17;
            this.rtbBuildOrder.Text = "";
            this.rtbBuildOrder.WordWrap = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 257);
            this.Controls.Add(this.rtbBuildOrder);
            this.Controls.Add(this.btnRestartBO);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnBoFromClipboard);
            this.Controls.Add(this.btnOpenBO);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(317, 296);
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

