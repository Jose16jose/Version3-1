﻿namespace WinFormsTestHarness
{
    partial class FlexForm
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
            this.LoadStructure = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.EditStructure = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.ChangeBackground = new System.Windows.Forms.Button();
            this.ShowCarbons = new System.Windows.Forms.CheckBox();
            this.RemoveAtom = new System.Windows.Forms.Button();
            this.RandomElement = new System.Windows.Forms.Button();
            this.EditorType = new System.Windows.Forms.ComboBox();
            this.Serialize = new System.Windows.Forms.Button();
            this.Examine = new System.Windows.Forms.Button();
            this.Hex = new System.Windows.Forms.Button();
            this.Timing = new System.Windows.Forms.Button();
            this.Undo = new System.Windows.Forms.Button();
            this.Redo = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Information = new System.Windows.Forms.Label();
            this.DisplayHost = new System.Windows.Forms.Integration.ElementHost();
            this.Display = new Chem4Word.ACME.Display();
            this.RedoHost = new System.Windows.Forms.Integration.ElementHost();
            this.RedoStack = new WinFormsTestHarness.StackViewer();
            this.UndoHost = new System.Windows.Forms.Integration.ElementHost();
            this.UndoStack = new WinFormsTestHarness.StackViewer();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadStructure
            // 
            this.LoadStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LoadStructure.Location = new System.Drawing.Point(12, 486);
            this.LoadStructure.Name = "LoadStructure";
            this.LoadStructure.Size = new System.Drawing.Size(75, 23);
            this.LoadStructure.TabIndex = 0;
            this.LoadStructure.Text = "Load";
            this.LoadStructure.UseVisualStyleBackColor = true;
            this.LoadStructure.Click += new System.EventHandler(this.LoadStructure_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // EditStructure
            // 
            this.EditStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EditStructure.Enabled = false;
            this.EditStructure.Location = new System.Drawing.Point(608, 486);
            this.EditStructure.Name = "EditStructure";
            this.EditStructure.Size = new System.Drawing.Size(75, 23);
            this.EditStructure.TabIndex = 2;
            this.EditStructure.Text = "Edit";
            this.EditStructure.UseVisualStyleBackColor = true;
            this.EditStructure.Click += new System.EventHandler(this.EditStructure_Click);
            // 
            // ChangeBackground
            // 
            this.ChangeBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ChangeBackground.Location = new System.Drawing.Point(134, 486);
            this.ChangeBackground.Name = "ChangeBackground";
            this.ChangeBackground.Size = new System.Drawing.Size(75, 23);
            this.ChangeBackground.TabIndex = 3;
            this.ChangeBackground.Text = "Background";
            this.ChangeBackground.UseVisualStyleBackColor = true;
            this.ChangeBackground.Click += new System.EventHandler(this.ChangeBackground_Click);
            // 
            // ShowCarbons
            // 
            this.ShowCarbons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowCarbons.AutoSize = true;
            this.ShowCarbons.Enabled = false;
            this.ShowCarbons.Location = new System.Drawing.Point(215, 490);
            this.ShowCarbons.Name = "ShowCarbons";
            this.ShowCarbons.Size = new System.Drawing.Size(95, 17);
            this.ShowCarbons.TabIndex = 4;
            this.ShowCarbons.Text = "Show Carbons";
            this.ShowCarbons.UseVisualStyleBackColor = true;
            this.ShowCarbons.CheckedChanged += new System.EventHandler(this.ShowCarbons_CheckedChanged);
            // 
            // RemoveAtom
            // 
            this.RemoveAtom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveAtom.Enabled = false;
            this.RemoveAtom.Location = new System.Drawing.Point(496, 486);
            this.RemoveAtom.Name = "RemoveAtom";
            this.RemoveAtom.Size = new System.Drawing.Size(97, 23);
            this.RemoveAtom.TabIndex = 5;
            this.RemoveAtom.Text = "Remove Atom";
            this.RemoveAtom.UseVisualStyleBackColor = true;
            this.RemoveAtom.Click += new System.EventHandler(this.RemoveAtom_Click);
            // 
            // RandomElement
            // 
            this.RandomElement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RandomElement.Enabled = false;
            this.RandomElement.Location = new System.Drawing.Point(496, 517);
            this.RandomElement.Name = "RandomElement";
            this.RandomElement.Size = new System.Drawing.Size(97, 23);
            this.RandomElement.TabIndex = 6;
            this.RandomElement.Text = "Random Element";
            this.RandomElement.UseVisualStyleBackColor = true;
            this.RandomElement.Click += new System.EventHandler(this.RandomElement_Click);
            // 
            // EditorType
            // 
            this.EditorType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EditorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EditorType.Enabled = false;
            this.EditorType.Location = new System.Drawing.Point(608, 519);
            this.EditorType.Name = "EditorType";
            this.EditorType.Size = new System.Drawing.Size(75, 21);
            this.EditorType.TabIndex = 0;
            // 
            // Serialize
            // 
            this.Serialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Serialize.Location = new System.Drawing.Point(12, 517);
            this.Serialize.Name = "Serialize";
            this.Serialize.Size = new System.Drawing.Size(55, 23);
            this.Serialize.TabIndex = 7;
            this.Serialize.Text = "Serialize";
            this.Serialize.UseVisualStyleBackColor = true;
            this.Serialize.Click += new System.EventHandler(this.Serialize_Click);
            // 
            // Examine
            // 
            this.Examine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Examine.Location = new System.Drawing.Point(73, 517);
            this.Examine.Name = "Examine";
            this.Examine.Size = new System.Drawing.Size(55, 23);
            this.Examine.TabIndex = 8;
            this.Examine.Text = "Analyse";
            this.Examine.UseVisualStyleBackColor = true;
            this.Examine.Click += new System.EventHandler(this.Examine_Click);
            // 
            // Hex
            // 
            this.Hex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Hex.Location = new System.Drawing.Point(134, 517);
            this.Hex.Name = "Hex";
            this.Hex.Size = new System.Drawing.Size(55, 23);
            this.Hex.TabIndex = 9;
            this.Hex.Text = "Hex";
            this.Hex.UseVisualStyleBackColor = true;
            this.Hex.Click += new System.EventHandler(this.Hex_Click);
            // 
            // Timing
            // 
            this.Timing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Timing.Location = new System.Drawing.Point(195, 517);
            this.Timing.Name = "Timing";
            this.Timing.Size = new System.Drawing.Size(55, 23);
            this.Timing.TabIndex = 10;
            this.Timing.Text = "Timing";
            this.Timing.UseVisualStyleBackColor = true;
            this.Timing.Click += new System.EventHandler(this.Timing_Click);
            // 
            // Undo
            // 
            this.Undo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Undo.Enabled = false;
            this.Undo.Location = new System.Drawing.Point(302, 517);
            this.Undo.Name = "Undo";
            this.Undo.Size = new System.Drawing.Size(55, 23);
            this.Undo.TabIndex = 11;
            this.Undo.Text = "Undo";
            this.Undo.UseVisualStyleBackColor = true;
            this.Undo.Click += new System.EventHandler(this.Undo_Click);
            // 
            // Redo
            // 
            this.Redo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Redo.Enabled = false;
            this.Redo.Location = new System.Drawing.Point(363, 517);
            this.Redo.Name = "Redo";
            this.Redo.Size = new System.Drawing.Size(55, 23);
            this.Redo.TabIndex = 12;
            this.Redo.Text = "Redo";
            this.Redo.UseVisualStyleBackColor = true;
            this.Redo.Click += new System.EventHandler(this.Redo_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.DisplayHost, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.RedoHost, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.UndoHost, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(671, 435);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // Information
            // 
            this.Information.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Information.AutoSize = true;
            this.Information.Location = new System.Drawing.Point(149, 454);
            this.Information.Name = "Information";
            this.Information.Size = new System.Drawing.Size(16, 13);
            this.Information.TabIndex = 14;
            this.Information.Text = "...";
            // 
            // DisplayHost
            // 
            this.DisplayHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.DisplayHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisplayHost.Location = new System.Drawing.Point(137, 3);
            this.DisplayHost.Name = "DisplayHost";
            this.DisplayHost.Size = new System.Drawing.Size(396, 429);
            this.DisplayHost.TabIndex = 1;
            this.DisplayHost.Text = "elementHost1";
            this.DisplayHost.Child = this.Display;
            // 
            // RedoHost
            // 
            this.RedoHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RedoHost.Location = new System.Drawing.Point(539, 3);
            this.RedoHost.Name = "RedoHost";
            this.RedoHost.Size = new System.Drawing.Size(129, 429);
            this.RedoHost.TabIndex = 2;
            this.RedoHost.Text = "elementHost2";
            this.RedoHost.Child = this.RedoStack;
            // 
            // UndoHost
            // 
            this.UndoHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndoHost.Location = new System.Drawing.Point(3, 3);
            this.UndoHost.Name = "UndoHost";
            this.UndoHost.Size = new System.Drawing.Size(128, 429);
            this.UndoHost.TabIndex = 3;
            this.UndoHost.Text = "elementHost3";
            this.UndoHost.Child = this.UndoStack;
            // 
            // FlexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 544);
            this.Controls.Add(this.Information);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Redo);
            this.Controls.Add(this.Undo);
            this.Controls.Add(this.Timing);
            this.Controls.Add(this.Hex);
            this.Controls.Add(this.Examine);
            this.Controls.Add(this.Serialize);
            this.Controls.Add(this.EditorType);
            this.Controls.Add(this.ShowCarbons);
            this.Controls.Add(this.ChangeBackground);
            this.Controls.Add(this.EditStructure);
            this.Controls.Add(this.RandomElement);
            this.Controls.Add(this.RemoveAtom);
            this.Controls.Add(this.LoadStructure);
            this.Name = "FlexForm";
            this.Text = "Flexible Display";
            this.Load += new System.EventHandler(this.FlexForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadStructure;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Integration.ElementHost DisplayHost;
        private System.Windows.Forms.Button EditStructure;
        private Chem4Word.ACME.Display Display;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button ChangeBackground;
        private System.Windows.Forms.CheckBox ShowCarbons;
        private System.Windows.Forms.ComboBox EditorType;
        private System.Windows.Forms.Button RemoveAtom;
        private System.Windows.Forms.Button RandomElement;
        private System.Windows.Forms.Button Serialize;
        private System.Windows.Forms.Button Examine;
        private System.Windows.Forms.Button Hex;
        private System.Windows.Forms.Button Timing;
        private System.Windows.Forms.Button Undo;
        private System.Windows.Forms.Button Redo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Integration.ElementHost RedoHost;
        private System.Windows.Forms.Integration.ElementHost UndoHost;
        private System.Windows.Forms.Label Information;
        private StackViewer RedoStack;
        private StackViewer UndoStack;
    }
}

