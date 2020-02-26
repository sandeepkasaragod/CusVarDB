using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.Devices;

namespace varcDB
{
    public partial class Form1 : Form
    {
        public string terminal_path = "";
        public Form1()
        {
            
            InitializeComponent();
            terminal_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "terminal.exe";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2();
            fm2.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form r_seq = new RNA_Seq();
            r_seq.Show();
            this.Hide();
        }
        public Process Linux_ProcessRunner(string cmd)
        {
            System.Diagnostics.Process prcs = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = terminal_path;
            startInfo.Arguments = cmd;
            prcs.StartInfo = startInfo;
            prcs.Start();
            prcs.WaitForExit();
            return prcs;
        }

        private void Genome_sequencing_Click(object sender, EventArgs e)
        {
            Form genome_seq = new Genome();
            genome_seq.Show();
            this.Hide();
        }

        private void Linux_dependency_Click(object sender, EventArgs e)
        {
            string add_java_to_repo = " -m wslx -p ubuntu1804 --wait " + "sudo add-apt-repository ppa:webupd8team/java";
            string update_linux = " -m wslx -p ubuntu1804 --wait " + "sudo apt-get update";
            string open_jdk = " -m wslx -p ubuntu1804 --wait " + "sudo apt-get install openjdk-8-jre";
            string bwa = " -m wslx -p ubuntu1804 --wait " + "sudo apt-get install bwa";
            string samtools = " -m wslx -p ubuntu1804 --wait " + "sudo apt-get install samtools";
            string unzip = " -m wslx -p ubuntu1804 --wait " + "sudo apt install unzip";
            string sra_toolkit = " -m wslx -p ubuntu1804 --wait " + "sudo apt-get install sra-toolkit";
            string hisat2 = " -m wslx -p ubuntu1804 --wait " + "sudo apt install hisat2";

            string[] commands = new string[8];
            commands[0] = add_java_to_repo;
            commands[1] = update_linux;
            commands[2] = open_jdk;
            commands[3] = bwa;
            commands[4] = samtools;
            commands[5] = unzip;
            commands[6] = sra_toolkit;
            commands[7] = hisat2;

            foreach (string p in commands)
            {

                Process prcs = Linux_ProcessRunner(p);
                //textBox1.Text = textBox1.Text + prcs.ToString() + Environment.NewLine;
            }

            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Process proc in Process.GetProcessesByName("CusVarDB"))
            {
                proc.Kill();
            }
        }
    }
}
