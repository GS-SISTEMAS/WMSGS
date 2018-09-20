namespace WMS.Tareo
{
    partial class frmTareo
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
            this.components = new System.ComponentModel.Container();
            this.chkTrasladoGuia = new System.Windows.Forms.CheckBox();
            this.UpInterval_TrasladoGuia = new System.Windows.Forms.DomainUpDown();
            this.tmTrasladoGuia = new System.Windows.Forms.Timer(this.components);
            this.btnTrasladoGuia = new System.Windows.Forms.Button();
            this.lblTrasladoGuia = new System.Windows.Forms.Label();
            this.chkSincronizarLotes = new System.Windows.Forms.CheckBox();
            this.UpInterval_sincronizarlotes = new System.Windows.Forms.DomainUpDown();
            this.btnSincronizaLotes = new System.Windows.Forms.Button();
            this.lblSincronzarLotes = new System.Windows.Forms.Label();
            this.tmSincronizacionLotes = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // chkTrasladoGuia
            // 
            this.chkTrasladoGuia.AutoSize = true;
            this.chkTrasladoGuia.Checked = true;
            this.chkTrasladoGuia.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrasladoGuia.Location = new System.Drawing.Point(12, 35);
            this.chkTrasladoGuia.Name = "chkTrasladoGuia";
            this.chkTrasladoGuia.Size = new System.Drawing.Size(94, 17);
            this.chkTrasladoGuia.TabIndex = 0;
            this.chkTrasladoGuia.Text = "Traslado Guía";
            this.chkTrasladoGuia.UseVisualStyleBackColor = true;
            this.chkTrasladoGuia.Visible = false;
            this.chkTrasladoGuia.CheckStateChanged += new System.EventHandler(this.chkTrasladoGuia_CheckStateChanged);
            // 
            // UpInterval_TrasladoGuia
            // 
            this.UpInterval_TrasladoGuia.AllowDrop = true;
            this.UpInterval_TrasladoGuia.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UpInterval_TrasladoGuia.Items.Add("1");
            this.UpInterval_TrasladoGuia.Items.Add("5");
            this.UpInterval_TrasladoGuia.Items.Add("10");
            this.UpInterval_TrasladoGuia.Items.Add("15");
            this.UpInterval_TrasladoGuia.Items.Add("20");
            this.UpInterval_TrasladoGuia.Items.Add("25");
            this.UpInterval_TrasladoGuia.Items.Add("30");
            this.UpInterval_TrasladoGuia.Location = new System.Drawing.Point(179, 32);
            this.UpInterval_TrasladoGuia.Name = "UpInterval_TrasladoGuia";
            this.UpInterval_TrasladoGuia.ReadOnly = true;
            this.UpInterval_TrasladoGuia.Size = new System.Drawing.Size(40, 20);
            this.UpInterval_TrasladoGuia.TabIndex = 1;
            this.UpInterval_TrasladoGuia.Text = "1";
            this.UpInterval_TrasladoGuia.Visible = false;
            this.UpInterval_TrasladoGuia.SelectedItemChanged += new System.EventHandler(this.UpInterval_TrasladoGuia_SelectedItemChanged);
            // 
            // tmTrasladoGuia
            // 
            this.tmTrasladoGuia.Interval = 60000;
            this.tmTrasladoGuia.Tick += new System.EventHandler(this.tmTrasladoGuia_Tick);
            // 
            // btnTrasladoGuia
            // 
            this.btnTrasladoGuia.Location = new System.Drawing.Point(231, 30);
            this.btnTrasladoGuia.Name = "btnTrasladoGuia";
            this.btnTrasladoGuia.Size = new System.Drawing.Size(82, 23);
            this.btnTrasladoGuia.TabIndex = 2;
            this.btnTrasladoGuia.Text = "Iniciar";
            this.btnTrasladoGuia.UseVisualStyleBackColor = true;
            this.btnTrasladoGuia.Visible = false;
            this.btnTrasladoGuia.Click += new System.EventHandler(this.btnTrasladoGuia_Click);
            // 
            // lblTrasladoGuia
            // 
            this.lblTrasladoGuia.AutoSize = true;
            this.lblTrasladoGuia.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrasladoGuia.ForeColor = System.Drawing.Color.Red;
            this.lblTrasladoGuia.Location = new System.Drawing.Point(319, 36);
            this.lblTrasladoGuia.Name = "lblTrasladoGuia";
            this.lblTrasladoGuia.Size = new System.Drawing.Size(117, 13);
            this.lblTrasladoGuia.TabIndex = 3;
            this.lblTrasladoGuia.Text = "[ Estado de Ejecución ]";
            this.lblTrasladoGuia.Visible = false;
            // 
            // chkSincronizarLotes
            // 
            this.chkSincronizarLotes.AutoSize = true;
            this.chkSincronizarLotes.Checked = true;
            this.chkSincronizarLotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSincronizarLotes.Location = new System.Drawing.Point(12, 12);
            this.chkSincronizarLotes.Name = "chkSincronizarLotes";
            this.chkSincronizarLotes.Size = new System.Drawing.Size(107, 17);
            this.chkSincronizarLotes.TabIndex = 4;
            this.chkSincronizarLotes.Text = "Sincronizar Lotes";
            this.chkSincronizarLotes.UseVisualStyleBackColor = true;
            // 
            // UpInterval_sincronizarlotes
            // 
            this.UpInterval_sincronizarlotes.AllowDrop = true;
            this.UpInterval_sincronizarlotes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UpInterval_sincronizarlotes.Items.Add("1");
            this.UpInterval_sincronizarlotes.Items.Add("5");
            this.UpInterval_sincronizarlotes.Items.Add("10");
            this.UpInterval_sincronizarlotes.Items.Add("15");
            this.UpInterval_sincronizarlotes.Items.Add("20");
            this.UpInterval_sincronizarlotes.Items.Add("25");
            this.UpInterval_sincronizarlotes.Items.Add("30");
            this.UpInterval_sincronizarlotes.Location = new System.Drawing.Point(179, 9);
            this.UpInterval_sincronizarlotes.Name = "UpInterval_sincronizarlotes";
            this.UpInterval_sincronizarlotes.ReadOnly = true;
            this.UpInterval_sincronizarlotes.Size = new System.Drawing.Size(40, 20);
            this.UpInterval_sincronizarlotes.TabIndex = 5;
            this.UpInterval_sincronizarlotes.Text = "15";
            this.UpInterval_sincronizarlotes.SelectedItemChanged += new System.EventHandler(this.UpInterval_sincronizarlotes_SelectedItemChanged);
            // 
            // btnSincronizaLotes
            // 
            this.btnSincronizaLotes.Location = new System.Drawing.Point(231, 7);
            this.btnSincronizaLotes.Name = "btnSincronizaLotes";
            this.btnSincronizaLotes.Size = new System.Drawing.Size(82, 23);
            this.btnSincronizaLotes.TabIndex = 6;
            this.btnSincronizaLotes.Text = "Iniciar";
            this.btnSincronizaLotes.UseVisualStyleBackColor = true;
            this.btnSincronizaLotes.Click += new System.EventHandler(this.btnSincronizaLotes_Click);
            // 
            // lblSincronzarLotes
            // 
            this.lblSincronzarLotes.AutoSize = true;
            this.lblSincronzarLotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSincronzarLotes.ForeColor = System.Drawing.Color.Red;
            this.lblSincronzarLotes.Location = new System.Drawing.Point(319, 13);
            this.lblSincronzarLotes.Name = "lblSincronzarLotes";
            this.lblSincronzarLotes.Size = new System.Drawing.Size(117, 13);
            this.lblSincronzarLotes.TabIndex = 7;
            this.lblSincronzarLotes.Text = "[ Estado de Ejecución ]";
            // 
            // tmSincronizacionLotes
            // 
            this.tmSincronizacionLotes.Interval = 60000;
            this.tmSincronizacionLotes.Tick += new System.EventHandler(this.tmSincronizacionLotes_Tick);
            // 
            // frmTareo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 76);
            this.Controls.Add(this.lblSincronzarLotes);
            this.Controls.Add(this.btnSincronizaLotes);
            this.Controls.Add(this.UpInterval_sincronizarlotes);
            this.Controls.Add(this.chkSincronizarLotes);
            this.Controls.Add(this.lblTrasladoGuia);
            this.Controls.Add(this.btnTrasladoGuia);
            this.Controls.Add(this.UpInterval_TrasladoGuia);
            this.Controls.Add(this.chkTrasladoGuia);
            this.Name = "frmTareo";
            this.Text = "Tareo Interfaces WMS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTrasladoGuia;
        private System.Windows.Forms.DomainUpDown UpInterval_TrasladoGuia;
        private System.Windows.Forms.Timer tmTrasladoGuia;
        private System.Windows.Forms.Button btnTrasladoGuia;
        private System.Windows.Forms.Label lblTrasladoGuia;
        private System.Windows.Forms.CheckBox chkSincronizarLotes;
        private System.Windows.Forms.DomainUpDown UpInterval_sincronizarlotes;
        private System.Windows.Forms.Button btnSincronizaLotes;
        private System.Windows.Forms.Label lblSincronzarLotes;
        private System.Windows.Forms.Timer tmSincronizacionLotes;
    }
}