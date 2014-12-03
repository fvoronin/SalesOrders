﻿namespace SalesOrdersReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnMasterFileBrowse = new System.Windows.Forms.Button();
            this.txtBoxFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNewOrderSheet = new System.Windows.Forms.Button();
            this.btnCreateEachSellerInvoice = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Excel files (*.xls)|*.xlsx";
            // 
            // btnMasterFileBrowse
            // 
            this.btnMasterFileBrowse.Location = new System.Drawing.Point(381, 51);
            this.btnMasterFileBrowse.Name = "btnMasterFileBrowse";
            this.btnMasterFileBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnMasterFileBrowse.TabIndex = 1;
            this.btnMasterFileBrowse.Text = "Browse";
            this.btnMasterFileBrowse.UseVisualStyleBackColor = true;
            this.btnMasterFileBrowse.Click += new System.EventHandler(this.btnMasterFileBrowse_Click);
            // 
            // txtBoxFileName
            // 
            this.txtBoxFileName.Location = new System.Drawing.Point(142, 53);
            this.txtBoxFileName.Name = "txtBoxFileName";
            this.txtBoxFileName.Size = new System.Drawing.Size(233, 20);
            this.txtBoxFileName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select Master List File";
            // 
            // btnNewOrderSheet
            // 
            this.btnNewOrderSheet.Location = new System.Drawing.Point(112, 119);
            this.btnNewOrderSheet.Name = "btnNewOrderSheet";
            this.btnNewOrderSheet.Size = new System.Drawing.Size(263, 73);
            this.btnNewOrderSheet.TabIndex = 2;
            this.btnNewOrderSheet.Text = "Add New Order Sheet";
            this.btnNewOrderSheet.UseVisualStyleBackColor = true;
            this.btnNewOrderSheet.Click += new System.EventHandler(this.btnNewOrderSheet_Click);
            // 
            // btnCreateEachSellerInvoice
            // 
            this.btnCreateEachSellerInvoice.Location = new System.Drawing.Point(112, 217);
            this.btnCreateEachSellerInvoice.Name = "btnCreateEachSellerInvoice";
            this.btnCreateEachSellerInvoice.Size = new System.Drawing.Size(263, 73);
            this.btnCreateEachSellerInvoice.TabIndex = 3;
            this.btnCreateEachSellerInvoice.Text = "Create Invoice for each Seller";
            this.btnCreateEachSellerInvoice.UseVisualStyleBackColor = true;
            this.btnCreateEachSellerInvoice.Click += new System.EventHandler(this.btnCreateEachSellerInvoice_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 386);
            this.Controls.Add(this.btnCreateEachSellerInvoice);
            this.Controls.Add(this.btnNewOrderSheet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoxFileName);
            this.Controls.Add(this.btnMasterFileBrowse);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnMasterFileBrowse;
        private System.Windows.Forms.TextBox txtBoxFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNewOrderSheet;
        private System.Windows.Forms.Button btnCreateEachSellerInvoice;

    }
}
