namespace Jeu_Comm
{
    partial class frmStartup
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
            this.btnHost = new System.Windows.Forms.Button();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lbJoin = new System.Windows.Forms.ListBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblNom = new System.Windows.Forms.Label();
            this.dlgPlayerColor = new System.Windows.Forms.ColorDialog();
            this.gbPlayer1 = new System.Windows.Forms.GroupBox();
            this.btnValidate1 = new System.Windows.Forms.Button();
            this.panelColorJoueur1 = new System.Windows.Forms.Panel();
            this.btnPickColorJoueur1 = new System.Windows.Forms.Button();
            this.panelImageJoueur1 = new System.Windows.Forms.Panel();
            this.txtIPJoueur1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNomJoueur1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbPlayer2 = new System.Windows.Forms.GroupBox();
            this.btnValidate2 = new System.Windows.Forms.Button();
            this.panelColorJoueur2 = new System.Windows.Forms.Panel();
            this.btnPickColorJoueur2 = new System.Windows.Forms.Button();
            this.panelImageJoueur2 = new System.Windows.Forms.Panel();
            this.txtIPJoueur2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNomJoueur2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.gbPlayer3 = new System.Windows.Forms.GroupBox();
            this.btnValidate3 = new System.Windows.Forms.Button();
            this.panelColorJoueur3 = new System.Windows.Forms.Panel();
            this.btnPickColorJoueur3 = new System.Windows.Forms.Button();
            this.panelImageJoueur3 = new System.Windows.Forms.Panel();
            this.txtIPJoueur3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNomJoueur3 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.gbPlayer4 = new System.Windows.Forms.GroupBox();
            this.btnValidate4 = new System.Windows.Forms.Button();
            this.panelColorJoueur4 = new System.Windows.Forms.Panel();
            this.btnPickColorJoueur4 = new System.Windows.Forms.Button();
            this.panelImageJoueur4 = new System.Windows.Forms.Panel();
            this.txtIPJoueur4 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtNomJoueur4 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.gbPlayer1.SuspendLayout();
            this.gbPlayer2.SuspendLayout();
            this.gbPlayer3.SuspendLayout();
            this.gbPlayer4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnHost
            // 
            this.btnHost.Location = new System.Drawing.Point(12, 12);
            this.btnHost.Name = "btnHost";
            this.btnHost.Size = new System.Drawing.Size(151, 23);
            this.btnHost.TabIndex = 1;
            this.btnHost.Text = "Host";
            this.btnHost.UseVisualStyleBackColor = true;
            this.btnHost.Click += new System.EventHandler(this.btnHost_Click);
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(13, 42);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(64, 13);
            this.lblIP.TabIndex = 2;
            this.lblIP.Text = "Your IP is :  ";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(12, 59);
            this.txtIP.Name = "txtIP";
            this.txtIP.ReadOnly = true;
            this.txtIP.Size = new System.Drawing.Size(151, 20);
            this.txtIP.TabIndex = 4;
            // 
            // lbJoin
            // 
            this.lbJoin.FormattingEnabled = true;
            this.lbJoin.Location = new System.Drawing.Point(12, 205);
            this.lbJoin.Name = "lbJoin";
            this.lbJoin.Size = new System.Drawing.Size(151, 303);
            this.lbJoin.TabIndex = 9;
            this.lbJoin.DoubleClick += new System.EventHandler(this.lbJoin_DoubleClick);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(12, 100);
            this.txtName.MaxLength = 31;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(151, 20);
            this.txtName.TabIndex = 11;
            // 
            // lblNom
            // 
            this.lblNom.AutoSize = true;
            this.lblNom.Location = new System.Drawing.Point(13, 84);
            this.lblNom.Name = "lblNom";
            this.lblNom.Size = new System.Drawing.Size(75, 13);
            this.lblNom.TabIndex = 10;
            this.lblNom.Text = "Game Name : ";
            // 
            // dlgPlayerColor
            // 
            this.dlgPlayerColor.Color = System.Drawing.Color.Blue;
            this.dlgPlayerColor.SolidColorOnly = true;
            // 
            // gbPlayer1
            // 
            this.gbPlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlayer1.Controls.Add(this.btnValidate1);
            this.gbPlayer1.Controls.Add(this.panelColorJoueur1);
            this.gbPlayer1.Controls.Add(this.btnPickColorJoueur1);
            this.gbPlayer1.Controls.Add(this.panelImageJoueur1);
            this.gbPlayer1.Controls.Add(this.txtIPJoueur1);
            this.gbPlayer1.Controls.Add(this.label2);
            this.gbPlayer1.Controls.Add(this.txtNomJoueur1);
            this.gbPlayer1.Controls.Add(this.label1);
            this.gbPlayer1.Enabled = false;
            this.gbPlayer1.Location = new System.Drawing.Point(170, 12);
            this.gbPlayer1.Name = "gbPlayer1";
            this.gbPlayer1.Size = new System.Drawing.Size(375, 120);
            this.gbPlayer1.TabIndex = 12;
            this.gbPlayer1.TabStop = false;
            this.gbPlayer1.Text = "Joueur 1";
            // 
            // btnValidate1
            // 
            this.btnValidate1.Location = new System.Drawing.Point(164, 67);
            this.btnValidate1.Name = "btnValidate1";
            this.btnValidate1.Size = new System.Drawing.Size(91, 23);
            this.btnValidate1.TabIndex = 8;
            this.btnValidate1.Text = "Validate";
            this.btnValidate1.UseVisualStyleBackColor = true;
            this.btnValidate1.Click += new System.EventHandler(this.btnValidateClick);
            // 
            // panelColorJoueur1
            // 
            this.panelColorJoueur1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelColorJoueur1.Enabled = false;
            this.panelColorJoueur1.Location = new System.Drawing.Point(235, 43);
            this.panelColorJoueur1.Name = "panelColorJoueur1";
            this.panelColorJoueur1.Size = new System.Drawing.Size(20, 20);
            this.panelColorJoueur1.TabIndex = 7;
            // 
            // btnPickColorJoueur1
            // 
            this.btnPickColorJoueur1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickColorJoueur1.Enabled = false;
            this.btnPickColorJoueur1.Location = new System.Drawing.Point(164, 15);
            this.btnPickColorJoueur1.Name = "btnPickColorJoueur1";
            this.btnPickColorJoueur1.Size = new System.Drawing.Size(91, 23);
            this.btnPickColorJoueur1.TabIndex = 6;
            this.btnPickColorJoueur1.Text = "Choose Color";
            this.btnPickColorJoueur1.UseVisualStyleBackColor = true;
            this.btnPickColorJoueur1.Click += new System.EventHandler(this.btnPickColorJoueur2_Click);
            // 
            // panelImageJoueur1
            // 
            this.panelImageJoueur1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImageJoueur1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelImageJoueur1.Enabled = false;
            this.panelImageJoueur1.Location = new System.Drawing.Point(261, 6);
            this.panelImageJoueur1.Name = "panelImageJoueur1";
            this.panelImageJoueur1.Size = new System.Drawing.Size(114, 114);
            this.panelImageJoueur1.TabIndex = 0;
            // 
            // txtIPJoueur1
            // 
            this.txtIPJoueur1.Enabled = false;
            this.txtIPJoueur1.Location = new System.Drawing.Point(51, 43);
            this.txtIPJoueur1.Name = "txtIPJoueur1";
            this.txtIPJoueur1.ReadOnly = true;
            this.txtIPJoueur1.Size = new System.Drawing.Size(100, 20);
            this.txtIPJoueur1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP : ";
            // 
            // txtNomJoueur1
            // 
            this.txtNomJoueur1.Enabled = false;
            this.txtNomJoueur1.Location = new System.Drawing.Point(51, 17);
            this.txtNomJoueur1.MaxLength = 31;
            this.txtNomJoueur1.Name = "txtNomJoueur1";
            this.txtNomJoueur1.ReadOnly = true;
            this.txtNomJoueur1.Size = new System.Drawing.Size(100, 20);
            this.txtNomJoueur1.TabIndex = 1;
            this.txtNomJoueur1.TextChanged += new System.EventHandler(this.NameTextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nom : ";
            // 
            // gbPlayer2
            // 
            this.gbPlayer2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlayer2.Controls.Add(this.btnValidate2);
            this.gbPlayer2.Controls.Add(this.panelColorJoueur2);
            this.gbPlayer2.Controls.Add(this.btnPickColorJoueur2);
            this.gbPlayer2.Controls.Add(this.panelImageJoueur2);
            this.gbPlayer2.Controls.Add(this.txtIPJoueur2);
            this.gbPlayer2.Controls.Add(this.label5);
            this.gbPlayer2.Controls.Add(this.txtNomJoueur2);
            this.gbPlayer2.Controls.Add(this.label6);
            this.gbPlayer2.Enabled = false;
            this.gbPlayer2.Location = new System.Drawing.Point(170, 139);
            this.gbPlayer2.Name = "gbPlayer2";
            this.gbPlayer2.Size = new System.Drawing.Size(375, 120);
            this.gbPlayer2.TabIndex = 13;
            this.gbPlayer2.TabStop = false;
            this.gbPlayer2.Text = "Joueur 2";
            // 
            // btnValidate2
            // 
            this.btnValidate2.Location = new System.Drawing.Point(164, 67);
            this.btnValidate2.Name = "btnValidate2";
            this.btnValidate2.Size = new System.Drawing.Size(91, 23);
            this.btnValidate2.TabIndex = 9;
            this.btnValidate2.Text = "Validate";
            this.btnValidate2.UseVisualStyleBackColor = true;
            this.btnValidate2.Click += new System.EventHandler(this.btnValidateClick);
            // 
            // panelColorJoueur2
            // 
            this.panelColorJoueur2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelColorJoueur2.Enabled = false;
            this.panelColorJoueur2.Location = new System.Drawing.Point(235, 43);
            this.panelColorJoueur2.Name = "panelColorJoueur2";
            this.panelColorJoueur2.Size = new System.Drawing.Size(20, 20);
            this.panelColorJoueur2.TabIndex = 7;
            // 
            // btnPickColorJoueur2
            // 
            this.btnPickColorJoueur2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickColorJoueur2.Enabled = false;
            this.btnPickColorJoueur2.Location = new System.Drawing.Point(164, 15);
            this.btnPickColorJoueur2.Name = "btnPickColorJoueur2";
            this.btnPickColorJoueur2.Size = new System.Drawing.Size(91, 23);
            this.btnPickColorJoueur2.TabIndex = 6;
            this.btnPickColorJoueur2.Text = "Choose Color";
            this.btnPickColorJoueur2.UseVisualStyleBackColor = true;
            this.btnPickColorJoueur2.Click += new System.EventHandler(this.btnPickColorJoueur2_Click);
            // 
            // panelImageJoueur2
            // 
            this.panelImageJoueur2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImageJoueur2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelImageJoueur2.Enabled = false;
            this.panelImageJoueur2.Location = new System.Drawing.Point(261, 6);
            this.panelImageJoueur2.Name = "panelImageJoueur2";
            this.panelImageJoueur2.Size = new System.Drawing.Size(114, 114);
            this.panelImageJoueur2.TabIndex = 0;
            // 
            // txtIPJoueur2
            // 
            this.txtIPJoueur2.Enabled = false;
            this.txtIPJoueur2.Location = new System.Drawing.Point(51, 43);
            this.txtIPJoueur2.Name = "txtIPJoueur2";
            this.txtIPJoueur2.ReadOnly = true;
            this.txtIPJoueur2.Size = new System.Drawing.Size(100, 20);
            this.txtIPJoueur2.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(7, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "IP : ";
            // 
            // txtNomJoueur2
            // 
            this.txtNomJoueur2.Enabled = false;
            this.txtNomJoueur2.Location = new System.Drawing.Point(51, 17);
            this.txtNomJoueur2.MaxLength = 31;
            this.txtNomJoueur2.Name = "txtNomJoueur2";
            this.txtNomJoueur2.ReadOnly = true;
            this.txtNomJoueur2.Size = new System.Drawing.Size(100, 20);
            this.txtNomJoueur2.TabIndex = 1;
            this.txtNomJoueur2.TextChanged += new System.EventHandler(this.NameTextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(7, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Nom : ";
            // 
            // gbPlayer3
            // 
            this.gbPlayer3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlayer3.Controls.Add(this.btnValidate3);
            this.gbPlayer3.Controls.Add(this.panelColorJoueur3);
            this.gbPlayer3.Controls.Add(this.btnPickColorJoueur3);
            this.gbPlayer3.Controls.Add(this.panelImageJoueur3);
            this.gbPlayer3.Controls.Add(this.txtIPJoueur3);
            this.gbPlayer3.Controls.Add(this.label8);
            this.gbPlayer3.Controls.Add(this.txtNomJoueur3);
            this.gbPlayer3.Controls.Add(this.label9);
            this.gbPlayer3.Enabled = false;
            this.gbPlayer3.Location = new System.Drawing.Point(170, 265);
            this.gbPlayer3.Name = "gbPlayer3";
            this.gbPlayer3.Size = new System.Drawing.Size(375, 120);
            this.gbPlayer3.TabIndex = 13;
            this.gbPlayer3.TabStop = false;
            this.gbPlayer3.Text = "Joueur 3";
            // 
            // btnValidate3
            // 
            this.btnValidate3.Location = new System.Drawing.Point(164, 67);
            this.btnValidate3.Name = "btnValidate3";
            this.btnValidate3.Size = new System.Drawing.Size(91, 23);
            this.btnValidate3.TabIndex = 10;
            this.btnValidate3.Text = "Validate";
            this.btnValidate3.UseVisualStyleBackColor = true;
            this.btnValidate3.Click += new System.EventHandler(this.btnValidateClick);
            // 
            // panelColorJoueur3
            // 
            this.panelColorJoueur3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelColorJoueur3.Enabled = false;
            this.panelColorJoueur3.Location = new System.Drawing.Point(235, 43);
            this.panelColorJoueur3.Name = "panelColorJoueur3";
            this.panelColorJoueur3.Size = new System.Drawing.Size(20, 20);
            this.panelColorJoueur3.TabIndex = 7;
            // 
            // btnPickColorJoueur3
            // 
            this.btnPickColorJoueur3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickColorJoueur3.Enabled = false;
            this.btnPickColorJoueur3.Location = new System.Drawing.Point(164, 15);
            this.btnPickColorJoueur3.Name = "btnPickColorJoueur3";
            this.btnPickColorJoueur3.Size = new System.Drawing.Size(91, 23);
            this.btnPickColorJoueur3.TabIndex = 6;
            this.btnPickColorJoueur3.Text = "Choose Color";
            this.btnPickColorJoueur3.UseVisualStyleBackColor = true;
            this.btnPickColorJoueur3.Click += new System.EventHandler(this.btnPickColorJoueur2_Click);
            // 
            // panelImageJoueur3
            // 
            this.panelImageJoueur3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImageJoueur3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelImageJoueur3.Enabled = false;
            this.panelImageJoueur3.Location = new System.Drawing.Point(261, 6);
            this.panelImageJoueur3.Name = "panelImageJoueur3";
            this.panelImageJoueur3.Size = new System.Drawing.Size(114, 114);
            this.panelImageJoueur3.TabIndex = 0;
            // 
            // txtIPJoueur3
            // 
            this.txtIPJoueur3.Enabled = false;
            this.txtIPJoueur3.Location = new System.Drawing.Point(51, 43);
            this.txtIPJoueur3.Name = "txtIPJoueur3";
            this.txtIPJoueur3.ReadOnly = true;
            this.txtIPJoueur3.Size = new System.Drawing.Size(100, 20);
            this.txtIPJoueur3.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(7, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "IP : ";
            // 
            // txtNomJoueur3
            // 
            this.txtNomJoueur3.Enabled = false;
            this.txtNomJoueur3.Location = new System.Drawing.Point(51, 17);
            this.txtNomJoueur3.MaxLength = 31;
            this.txtNomJoueur3.Name = "txtNomJoueur3";
            this.txtNomJoueur3.ReadOnly = true;
            this.txtNomJoueur3.Size = new System.Drawing.Size(100, 20);
            this.txtNomJoueur3.TabIndex = 1;
            this.txtNomJoueur3.TextChanged += new System.EventHandler(this.NameTextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(7, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Nom : ";
            // 
            // gbPlayer4
            // 
            this.gbPlayer4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlayer4.Controls.Add(this.btnValidate4);
            this.gbPlayer4.Controls.Add(this.panelColorJoueur4);
            this.gbPlayer4.Controls.Add(this.btnPickColorJoueur4);
            this.gbPlayer4.Controls.Add(this.panelImageJoueur4);
            this.gbPlayer4.Controls.Add(this.txtIPJoueur4);
            this.gbPlayer4.Controls.Add(this.label11);
            this.gbPlayer4.Controls.Add(this.txtNomJoueur4);
            this.gbPlayer4.Controls.Add(this.label12);
            this.gbPlayer4.Enabled = false;
            this.gbPlayer4.Location = new System.Drawing.Point(170, 391);
            this.gbPlayer4.Name = "gbPlayer4";
            this.gbPlayer4.Size = new System.Drawing.Size(375, 120);
            this.gbPlayer4.TabIndex = 13;
            this.gbPlayer4.TabStop = false;
            this.gbPlayer4.Text = "Joueur 4";
            // 
            // btnValidate4
            // 
            this.btnValidate4.Location = new System.Drawing.Point(164, 67);
            this.btnValidate4.Name = "btnValidate4";
            this.btnValidate4.Size = new System.Drawing.Size(91, 23);
            this.btnValidate4.TabIndex = 11;
            this.btnValidate4.Text = "Validate";
            this.btnValidate4.UseVisualStyleBackColor = true;
            this.btnValidate4.Click += new System.EventHandler(this.btnValidateClick);
            // 
            // panelColorJoueur4
            // 
            this.panelColorJoueur4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelColorJoueur4.Enabled = false;
            this.panelColorJoueur4.Location = new System.Drawing.Point(235, 43);
            this.panelColorJoueur4.Name = "panelColorJoueur4";
            this.panelColorJoueur4.Size = new System.Drawing.Size(20, 20);
            this.panelColorJoueur4.TabIndex = 7;
            // 
            // btnPickColorJoueur4
            // 
            this.btnPickColorJoueur4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPickColorJoueur4.Enabled = false;
            this.btnPickColorJoueur4.Location = new System.Drawing.Point(164, 15);
            this.btnPickColorJoueur4.Name = "btnPickColorJoueur4";
            this.btnPickColorJoueur4.Size = new System.Drawing.Size(91, 23);
            this.btnPickColorJoueur4.TabIndex = 6;
            this.btnPickColorJoueur4.Text = "Choose Color";
            this.btnPickColorJoueur4.UseVisualStyleBackColor = true;
            this.btnPickColorJoueur4.Click += new System.EventHandler(this.btnPickColorJoueur2_Click);
            // 
            // panelImageJoueur4
            // 
            this.panelImageJoueur4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImageJoueur4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelImageJoueur4.Enabled = false;
            this.panelImageJoueur4.Location = new System.Drawing.Point(261, 6);
            this.panelImageJoueur4.Name = "panelImageJoueur4";
            this.panelImageJoueur4.Size = new System.Drawing.Size(114, 114);
            this.panelImageJoueur4.TabIndex = 0;
            // 
            // txtIPJoueur4
            // 
            this.txtIPJoueur4.Enabled = false;
            this.txtIPJoueur4.Location = new System.Drawing.Point(51, 43);
            this.txtIPJoueur4.Name = "txtIPJoueur4";
            this.txtIPJoueur4.ReadOnly = true;
            this.txtIPJoueur4.Size = new System.Drawing.Size(100, 20);
            this.txtIPJoueur4.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Enabled = false;
            this.label11.Location = new System.Drawing.Point(7, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "IP : ";
            // 
            // txtNomJoueur4
            // 
            this.txtNomJoueur4.Enabled = false;
            this.txtNomJoueur4.Location = new System.Drawing.Point(51, 17);
            this.txtNomJoueur4.MaxLength = 31;
            this.txtNomJoueur4.Name = "txtNomJoueur4";
            this.txtNomJoueur4.ReadOnly = true;
            this.txtNomJoueur4.Size = new System.Drawing.Size(100, 20);
            this.txtNomJoueur4.TabIndex = 1;
            this.txtNomJoueur4.TextChanged += new System.EventHandler(this.NameTextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Enabled = false;
            this.label12.Location = new System.Drawing.Point(7, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Nom : ";
            // 
            // frmStartup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 522);
            this.Controls.Add(this.gbPlayer4);
            this.Controls.Add(this.gbPlayer3);
            this.Controls.Add(this.gbPlayer2);
            this.Controls.Add(this.gbPlayer1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblNom);
            this.Controls.Add(this.lbJoin);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.btnHost);
            this.Name = "frmStartup";
            this.Text = "Bomberman - Menu Multijoueur";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmStartup_FormClosed);
            this.gbPlayer1.ResumeLayout(false);
            this.gbPlayer1.PerformLayout();
            this.gbPlayer2.ResumeLayout(false);
            this.gbPlayer2.PerformLayout();
            this.gbPlayer3.ResumeLayout(false);
            this.gbPlayer3.PerformLayout();
            this.gbPlayer4.ResumeLayout(false);
            this.gbPlayer4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnHost;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.ListBox lbJoin;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblNom;
        private System.Windows.Forms.ColorDialog dlgPlayerColor;
        private System.Windows.Forms.GroupBox gbPlayer1;
        private System.Windows.Forms.Panel panelColorJoueur1;
        private System.Windows.Forms.Button btnPickColorJoueur1;
        private System.Windows.Forms.Panel panelImageJoueur1;
        private System.Windows.Forms.TextBox txtIPJoueur1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNomJoueur1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbPlayer2;
        private System.Windows.Forms.Panel panelColorJoueur2;
        private System.Windows.Forms.Button btnPickColorJoueur2;
        private System.Windows.Forms.Panel panelImageJoueur2;
        private System.Windows.Forms.TextBox txtIPJoueur2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNomJoueur2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox gbPlayer3;
        private System.Windows.Forms.Panel panelColorJoueur3;
        private System.Windows.Forms.Button btnPickColorJoueur3;
        private System.Windows.Forms.Panel panelImageJoueur3;
        private System.Windows.Forms.TextBox txtIPJoueur3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtNomJoueur3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox gbPlayer4;
        private System.Windows.Forms.Panel panelColorJoueur4;
        private System.Windows.Forms.Button btnPickColorJoueur4;
        private System.Windows.Forms.Panel panelImageJoueur4;
        private System.Windows.Forms.TextBox txtIPJoueur4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtNomJoueur4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnValidate1;
        private System.Windows.Forms.Button btnValidate2;
        private System.Windows.Forms.Button btnValidate3;
        private System.Windows.Forms.Button btnValidate4;
    }
}