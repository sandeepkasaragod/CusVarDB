using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace varcDB
{
    public partial class Annovar_download : Form
    {
        public string terminal_path = "";
        public Annovar_download()
        {
            InitializeComponent();
            terminal_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "terminal.exe";
        }

        public string convert_path(string windows_path)
        {
            string win_path = windows_path.Substring(2).Replace("\\", "/");
            string drv_ltr = windows_path.Substring(0, 1).ToLower();
            string linux_path = "/mnt/" + drv_ltr + win_path;
            return linux_path;
        }

        public Process ProcessRunner(string cmd)
        {
            System.Diagnostics.Process prcs = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd;
            //sra_file_browse.Text = startInfo.Arguments;
            prcs.StartInfo = startInfo;
            prcs.Start();
            prcs.WaitForExit();
            return prcs;

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

        private void button1_Click(object sender, EventArgs e)
        {
            //Setting the genome version
            string set_genome = "";
            string genome = comboBox1.GetItemText(comboBox1.SelectedItem);
            if (genome == "hg19")
            {
                set_genome = "hg19";
                
            }
            else if (genome == "hg38")
            {
                set_genome = "hg38";
            }

            //Downloading the database

            if(set_genome!="")
            {

            string perl_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\perl" + "\\" + "bin" + "\\";
            string annovar_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\annovar" + "\\";
            textBox1.Text = "Downloading cytoband and refgene database" + Environment.NewLine;
            Process refgene = Linux_ProcessRunner(" --wait -m wslx -p ubuntu1804 perl " + convert_path(annovar_path) + "annotate_variation.pl -buildver " + set_genome + " -downdb -webfrom annovar refGene " + convert_path(annovar_path) + "humandb");
            Process cytoband = Linux_ProcessRunner(" --wait -m wslx -p ubuntu1804 perl " + convert_path(annovar_path) + "annotate_variation.pl -buildver " + set_genome + " -downdb cytoBand " + convert_path(annovar_path) + "humandb");
            textBox1.Text = textBox1.Text + "Download Fiinished" + Environment.NewLine;
            textBox1.Text = textBox1.Text + "Downloading User selected database" + Environment.NewLine;
            int cnt = checkedListBox1.Items.Count;
            for (int i = 0; i <= cnt - 1; i++)
            {
                if (checkedListBox1.GetItemChecked(i) == true)
                {
                        try
                        {
                            string get_DB = checkedListBox1.Items[i].ToString();
                            //Process refgene = ProcessRunner("/C terminal.py -m wslx perl " + convert_path(annovar_path) + "annotate_variation.pl -buildver " + set_genome + " -downdb -webfrom annovar refGene " + convert_path(annovar_path) + "humandb");
                            //Process cytoband = ProcessRunner("/C terminal.py -m wslx perl " + convert_path(annovar_path) + "annotate_variation.pl -buildver " + set_genome + " -downdb cytoBand " + convert_path(annovar_path) + "humandb");
                            string annovar_download_cmd = " --wait -m wslx -p ubuntu1804 perl " + convert_path(annovar_path) + "annotate_variation.pl -buildver " + set_genome + " -downdb -webfrom annovar " + get_DB + " " + convert_path(annovar_path) + "humandb";

                            Process Ps = Linux_ProcessRunner(annovar_download_cmd);
                            textBox1.Text = textBox1.Text + get_DB + " finished downloading" + Environment.NewLine;
                        }
                        catch
                        {
                            MessageBox.Show("Please check the internet connetion");
                        }

                }

            }
                MessageBox.Show("All the database downloaded successfully");
            }
            else
            {
                MessageBox.Show("Please select the genome build");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            int cnt = checkedListBox1.Items.Count;
            for(int i=0; i<=cnt - 1; i++)
            {
                if (checkedListBox1.GetItemChecked(i) == true)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }

        private void select_all_checkboxitem_Click(object sender, EventArgs e)
        {
            int cnt = checkedListBox1.Items.Count;
            for (int i = 0; i <= cnt - 1; i++)
            {
                if (checkedListBox1.GetItemChecked(i) == false)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                }
            }
        }

        private void Annovar_download_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form exome_seq = new Form2();
            exome_seq.Visible = true;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string genome = comboBox1.GetItemText(comboBox1.SelectedItem);
            checkedListBox1.Items.Clear();
            if (genome == "hg19")
            {

                checkedListBox1.Items.Clear();
                checkedListBox1.Items.Insert(0, "avsift");
                checkedListBox1.Items.Insert(1, "ljb26_all");
                checkedListBox1.Items.Insert(2, "dbnsfp30a");
                checkedListBox1.Items.Insert(3, "dbnsfp31a_interpro");
                checkedListBox1.Items.Insert(4, "dbnsfp33a");
                checkedListBox1.Items.Insert(5, "dbnsfp35a");
                checkedListBox1.Items.Insert(6, "dbnsfp35c");
                checkedListBox1.Items.Insert(7, "dbscsnv11");
                checkedListBox1.Items.Insert(8, "intervar_20170202");
                checkedListBox1.Items.Insert(9, "intervar_20180118");
                checkedListBox1.Items.Insert(10, "cg46");
                checkedListBox1.Items.Insert(11, "cg69");
                checkedListBox1.Items.Insert(12, "cosmic64");
                checkedListBox1.Items.Insert(13, "cosmic65");
                checkedListBox1.Items.Insert(14, "cosmic67");
                checkedListBox1.Items.Insert(15, "cosmic67wgs");
                checkedListBox1.Items.Insert(16, "cosmic68");
                checkedListBox1.Items.Insert(17, "cosmic68wgs");
                checkedListBox1.Items.Insert(18, "cosmic70");
                checkedListBox1.Items.Insert(19, "esp6500siv2_ea");
                checkedListBox1.Items.Insert(20, "esp6500siv2_aa");
                checkedListBox1.Items.Insert(21, "esp6500siv2_all");
                checkedListBox1.Items.Insert(22, "exac03");
                checkedListBox1.Items.Insert(23, "exac03nontcga");
                checkedListBox1.Items.Insert(24, "exac03nonpsych");
                checkedListBox1.Items.Insert(25, "gnomad_exome");
                checkedListBox1.Items.Insert(26, "gnomad_genome");
                checkedListBox1.Items.Insert(27, "kaviar_20150923");
                checkedListBox1.Items.Insert(28, "hrcr1");
                checkedListBox1.Items.Insert(29, "abraom");
                checkedListBox1.Items.Insert(30, "1000g2010nov");
                checkedListBox1.Items.Insert(31, "1000g2011may");
                checkedListBox1.Items.Insert(32, "1000g2012feb");
                checkedListBox1.Items.Insert(33, "1000g2012apr");
                checkedListBox1.Items.Insert(34, "1000g2014aug");
                checkedListBox1.Items.Insert(35, "1000g2014sep");
                checkedListBox1.Items.Insert(36, "1000g2014oct");
                checkedListBox1.Items.Insert(37, "1000g2015aug");
                checkedListBox1.Items.Insert(38, "gme");
                checkedListBox1.Items.Insert(39, "mcap");
                checkedListBox1.Items.Insert(40, "mcap13");
                checkedListBox1.Items.Insert(41, "revel");
                checkedListBox1.Items.Insert(42, "snp129");
                checkedListBox1.Items.Insert(43, "snp130");
                checkedListBox1.Items.Insert(44, "snp131");
                checkedListBox1.Items.Insert(45, "snp132");
                checkedListBox1.Items.Insert(46, "snp135");
                checkedListBox1.Items.Insert(47, "snp137");
                checkedListBox1.Items.Insert(48, "snp138");
                checkedListBox1.Items.Insert(49, "avsnp138");
                checkedListBox1.Items.Insert(50, "avsnp142");
                checkedListBox1.Items.Insert(51, "avsnp144");
                checkedListBox1.Items.Insert(52, "avsnp147");
                checkedListBox1.Items.Insert(53, "avsnp150");
                checkedListBox1.Items.Insert(54, "snp130NonFlagged");
                checkedListBox1.Items.Insert(55, "snp131NonFlagged");
                checkedListBox1.Items.Insert(56, "snp132NonFlagged");
                checkedListBox1.Items.Insert(57, "snp135NonFlagged");
                checkedListBox1.Items.Insert(58, "snp137NonFlagged");
                checkedListBox1.Items.Insert(59, "snp138NonFlagged");
                checkedListBox1.Items.Insert(60, "nci60");
                checkedListBox1.Items.Insert(61, "icgc21");
                checkedListBox1.Items.Insert(62, "clinvar_20131105");
                checkedListBox1.Items.Insert(63, "clinvar_20140211");
                checkedListBox1.Items.Insert(64, "clinvar_20140303");
                checkedListBox1.Items.Insert(65, "clinvar_20140702");
                checkedListBox1.Items.Insert(66, "clinvar_20140902");
                checkedListBox1.Items.Insert(67, "clinvar_20140929");
                checkedListBox1.Items.Insert(68, "clinvar_20150330");
                checkedListBox1.Items.Insert(69, "clinvar_20150629");
                checkedListBox1.Items.Insert(70, "clinvar_20151201");
                checkedListBox1.Items.Insert(71, "clinvar_20160302");
                checkedListBox1.Items.Insert(72, "clinvar_20161128");
                checkedListBox1.Items.Insert(73, "clinvar_20170130");
                checkedListBox1.Items.Insert(74, "clinvar_20170501");
                checkedListBox1.Items.Insert(75, "clinvar_20170905");
                checkedListBox1.Items.Insert(76, "clinvar_20180603");
                checkedListBox1.Items.Insert(77, "clinvar_20190305");
                checkedListBox1.Items.Insert(78, "popfreq_max_20150413");
                checkedListBox1.Items.Insert(79, "popfreq_all_20150413");
                checkedListBox1.Items.Insert(80, "mitimpact2");
                checkedListBox1.Items.Insert(81, "mitimpact24");
                checkedListBox1.Items.Insert(82, "regsnpintron");
                checkedListBox1.Items.Insert(83, "gerp++elem");
                checkedListBox1.Items.Insert(84, "gerp++gt2");
                checkedListBox1.Items.Insert(85, "caddgt20");
                checkedListBox1.Items.Insert(86, "caddgt10");
                checkedListBox1.Items.Insert(87, "cadd");
                checkedListBox1.Items.Insert(88, "cadd13");
                checkedListBox1.Items.Insert(89, "cadd13gt10");
                checkedListBox1.Items.Insert(90, "cadd13gt20");
                checkedListBox1.Items.Insert(91, "caddindel");
                checkedListBox1.Items.Insert(92, "fathmm");
                checkedListBox1.Items.Insert(93, "gwava");
                checkedListBox1.Items.Insert(94, "eigen");
            }
            else if (genome == "hg38")
            {
                checkedListBox1.Items.Insert(0, "ljb26_all");
                checkedListBox1.Items.Insert(1, "dbnsfp30a");
                checkedListBox1.Items.Insert(2, "dbnsfp31a_interpro");
                checkedListBox1.Items.Insert(3, "dbnsfp33a");
                checkedListBox1.Items.Insert(4, "dbnsfp35a");
                checkedListBox1.Items.Insert(5, "dbnsfp35c");
                checkedListBox1.Items.Insert(6, "dbscsnv11");
                checkedListBox1.Items.Insert(7, "intervar_20180118");
                checkedListBox1.Items.Insert(8, "cosmic70");
                checkedListBox1.Items.Insert(9, "esp6500siv2_ea");
                checkedListBox1.Items.Insert(10, "esp6500siv2_aa");
                checkedListBox1.Items.Insert(10, "esp6500siv2_all");
                checkedListBox1.Items.Insert(12, "exac03");
                checkedListBox1.Items.Insert(13, "exac03nontcga");
                checkedListBox1.Items.Insert(14, "exac03nonpsych");
                checkedListBox1.Items.Insert(15, "gnomad_exome");
                checkedListBox1.Items.Insert(16, "gnomad_genome");
                checkedListBox1.Items.Insert(17, "kaviar_20150923");
                checkedListBox1.Items.Insert(18, "hrcr1");
                checkedListBox1.Items.Insert(19, "abraom");
                checkedListBox1.Items.Insert(20, "1000g2014oct");
                checkedListBox1.Items.Insert(21, "1000g2015aug");
                checkedListBox1.Items.Insert(22, "gme");
                checkedListBox1.Items.Insert(23, "mcap");
                checkedListBox1.Items.Insert(24, "revel");
                checkedListBox1.Items.Insert(25, "avsnp144");
                checkedListBox1.Items.Insert(26, "avsnp142");
                checkedListBox1.Items.Insert(27, "avsnp144");
                checkedListBox1.Items.Insert(28, "avsnp147");
                checkedListBox1.Items.Insert(29, "avsnp150");
                checkedListBox1.Items.Insert(30, "clinvar_20140702");
                checkedListBox1.Items.Insert(31, "clinvar_20150330");
                checkedListBox1.Items.Insert(32, "clinvar_20150629");
                checkedListBox1.Items.Insert(33, "clinvar_20151201");
                checkedListBox1.Items.Insert(34, "clinvar_20160302");
                checkedListBox1.Items.Insert(35, "clinvar_20161128");
                checkedListBox1.Items.Insert(36, "clinvar_20170130");
                checkedListBox1.Items.Insert(37, "clinvar_20170501");
                checkedListBox1.Items.Insert(38, "clinvar_20170905");
                checkedListBox1.Items.Insert(39, "clinvar_20180603");
                checkedListBox1.Items.Insert(40, "clinvar_20190305");
                checkedListBox1.Items.Insert(41, "regsnpintron");
            }
        }
    }
}
