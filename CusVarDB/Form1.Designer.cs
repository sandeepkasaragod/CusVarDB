namespace varcDB
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.rna_seq = new System.Windows.Forms.Button();
            this.Genome_sequencing = new System.Windows.Forms.Button();
            this.Linux_dependency = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(954, 31);
            this.label1.TabIndex = 1;
            this.label1.Text = "Variant database creator from Genome, RNA and Exome sequencing datasets";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(873, 412);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(174, 43);
            this.button4.TabIndex = 7;
            this.button4.Text = "Exit";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(699, 122);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(348, 169);
            this.button3.TabIndex = 3;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // rna_seq
            // 
            this.rna_seq.BackColor = System.Drawing.Color.Transparent;
            this.rna_seq.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rna_seq.BackgroundImage")));
            this.rna_seq.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rna_seq.FlatAppearance.BorderSize = 0;
            this.rna_seq.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rna_seq.Location = new System.Drawing.Point(368, 117);
            this.rna_seq.Margin = new System.Windows.Forms.Padding(4);
            this.rna_seq.Name = "rna_seq";
            this.rna_seq.Size = new System.Drawing.Size(320, 170);
            this.rna_seq.TabIndex = 2;
            this.rna_seq.UseVisualStyleBackColor = false;
            this.rna_seq.Click += new System.EventHandler(this.button2_Click);
            // 
            // Genome_sequencing
            // 
            this.Genome_sequencing.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Genome_sequencing.BackgroundImage")));
            this.Genome_sequencing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Genome_sequencing.FlatAppearance.BorderSize = 0;
            this.Genome_sequencing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Genome_sequencing.Location = new System.Drawing.Point(16, 123);
            this.Genome_sequencing.Margin = new System.Windows.Forms.Padding(4);
            this.Genome_sequencing.Name = "Genome_sequencing";
            this.Genome_sequencing.Size = new System.Drawing.Size(325, 160);
            this.Genome_sequencing.TabIndex = 0;
            this.Genome_sequencing.UseVisualStyleBackColor = true;
            this.Genome_sequencing.Click += new System.EventHandler(this.Genome_sequencing_Click);
            // 
            // Linux_dependency
            // 
            this.Linux_dependency.Location = new System.Drawing.Point(9, 412);
            this.Linux_dependency.Name = "Linux_dependency";
            this.Linux_dependency.Size = new System.Drawing.Size(217, 43);
            this.Linux_dependency.TabIndex = 8;
            this.Linux_dependency.Text = "Install Linux Dependenies";
            this.Linux_dependency.UseVisualStyleBackColor = true;
            this.Linux_dependency.Click += new System.EventHandler(this.Linux_dependency_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 468);
            this.Controls.Add(this.Linux_dependency);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.rna_seq);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Genome_sequencing);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CusVarDB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Genome_sequencing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button rna_seq;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button Linux_dependency;
    }
}

