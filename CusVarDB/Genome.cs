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
    public partial class Genome : Form
    {
        // Working directory path
        public string working_dir_path = "";
        public string working_dir_path_drv_ltr = "";
        public string input_working_dir_converted_path = "";

        // Currrent directory path
        public string curr_dir = "";
        public string root_dir_drv_path = "";
        public string root_path = "";

        // Read1 path 
        public string read1_path = "";
        public string read1_path_drv_ltr = "";
        public string read1_working_dir = "";

        // Read2 path
        public string read2_path = "";
        public string read2_path_drv_ltr = "";
        public string read2_working_dir = "";

        // Reference path
        public string ref_path = "";
        public string ref_path_drv_ltr = "";
        public string ref_path_converted = "";
        public string dbSNP = "";
        public int processor_count;
        public string terminal_path;
        public Genome()
        {
            InitializeComponent();
            terminal_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "terminal.exe";
            Genome_set_chromosomes.Enabled = false;
            processor_count = Environment.ProcessorCount;
            if (processor_count <= 2)
            {
                MessageBox.Show("The program may not run faster due to the limited threads!!!!!!!!!");
                processor_count = 1;
            }
            else
            {
                processor_count = processor_count - 2;
            }
            Genome_set_threads.Text = processor_count.ToString();
            Genome_dry_run_threads.Text = processor_count.ToString();
            var totalRAM = (Convert.ToInt32((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory) / (Math.Pow(1024, 3)) + 0.5) / 2);
            {

                Genome_dry_run_memory.Text = totalRAM.ToString();
                Genome_set_memory.Text = totalRAM.ToString();
            }
            string get_curr_dir = System.IO.Directory.GetCurrentDirectory();
            /* if (System.IO.File.Exists(get_curr_dir + "\\" + "config.txt"))
             {

                 using (System.IO.TextReader tr = System.IO.File.OpenText(get_curr_dir + "\\" + "config.txt"))
                 {
                     string line = "";
                     while ((line = tr.ReadLine()) != null)
                     {
                         string[] items = line.Split('\t');
                         working_directory_path.Text = items[1];
                     }
                 }
             } 
             else
             {
                 MessageBox.Show("Please set the working directory");
             }
             */
        }
        public string download_SRA(string sequence_read_archive_id, string id)
        {
            string sra_id = sequence_read_archive_id.Trim();
            string url = "https://ftp.ncbi.nlm.nih.gov/sra/sra-instant/reads/ByRun/sra/" + id + "/" + sra_id.Substring(0, 6) + "/" + sra_id + "/" + sra_id + ".sra";
            string sra_file_download = " --wait -m wslx -p ubuntu1804 wget -c " + url + " -P " + convert_path(Genome_working_directory_path.Text) + "/";
            Process run_sra_download = Linux_ProcessRunner(sra_file_download);
            /* Use process.ExitCode and based on the code pass the message in return */
            //Genome_sra_file_browse.Text = run_sra_download.ExitCode.ToString();
            return run_sra_download.ExitCode.ToString();
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

        public string convert_path(string windows_path)
        {
            string win_path = windows_path.Substring(2).Replace("\\", "/");
            string drv_ltr = windows_path.Substring(0, 1).ToLower();
            string linux_path = "/mnt/" + drv_ltr + win_path;
            return linux_path;
        }

        private void Genome_browse_reference_fasta_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd_reference_fasta = new OpenFileDialog();
            ofd_reference_fasta.Filter = "Fasta file (*.fasta)|*.fasta|Fasta file (*.fa)|*.fa|Fasta file (*.fna)|*.fna";
            if (ofd_reference_fasta.ShowDialog() == DialogResult.OK)
            {
                Genome_reference_fasta.Text = ofd_reference_fasta.FileName;
                string get_filename = System.IO.Path.GetFullPath(Genome_reference_fasta.Text); //reference file name with extension

                //Browse for reference file
                ref_path_converted = convert_path(Genome_reference_fasta.Text);

                if (System.IO.File.Exists(get_filename + ".amb") && System.IO.File.Exists(get_filename + ".ann") && System.IO.File.Exists(get_filename + ".bwt") && System.IO.File.Exists(get_filename + ".pac") && System.IO.File.Exists(get_filename + ".sa"))
                {
                    MessageBox.Show("All the files exist");

                }
                else
                {
                    //Converting the root directory path to linux path
                    root_path = convert_path(System.IO.Directory.GetCurrentDirectory());
                    /*
                    //MessageBox.Show("Index files are missing. BWA is performing the Indexing!!!!" + root_path + System.IO.Path.GetFileName(reference_fasta.Text));
                    System.Diagnostics.Process prcs = new System.Diagnostics.Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C terminal.py -m wslx " + root_path + '/' + "bwa index " + ref_path_converted;
                    Genome_sra_file_browse.Text = startInfo.Arguments;
                    prcs.StartInfo = startInfo;
                    prcs.Start();
                    prcs.WaitForExit(); */
                    String idx_cmd = " -m wslx -p ubuntu1804 " + root_path + '/' + "bwa index " + ref_path_converted;
                    Process bwa_idx = Linux_ProcessRunner(idx_cmd);
                }
            }
        }

        private void Genome_browse_dbSNP_Click(object sender, EventArgs e)
        {
            if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Genome_dbSNP_path.Text = Genome_sra_file_browse_for_fq.FileName;
            }
        }

        private void Genome_browse_bed_file_Click(object sender, EventArgs e)
        {
            if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Genome_bed_file_path.Text = Genome_sra_file_browse_for_fq.FileName;
            }
        }

        private void Genome_browse_reference_file_Click(object sender, EventArgs e)
        {
            if (Genome_working_directory.ShowDialog() == DialogResult.OK)
            {
                Genome_working_directory_path.Text = Genome_working_directory.SelectedPath;
            }
        }

        private void Genome_sra_download_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Genome_working_directory_path.Text))
            { 
            if (Genome_sra_id_download.Text.Contains("SRR"))
            {
                /*Reduce the code to function and pass SRR, ERR and DRR to the function*/
                string sra_fl = download_SRA(Genome_sra_id_download.Text, "SRR");
            }
            else if (Genome_sra_id_download.Text.Contains("ERR"))
            {
                string sra_fl = download_SRA(Genome_sra_id_download.Text, "ERR");

            }
            else if (Genome_sra_id_download.Text.Contains("DRR"))
            {
                string sra_fl = download_SRA(Genome_sra_id_download.Text, "DRR");
            }
            else
            {
                MessageBox.Show("Invalid SRR ID");
            }
            }
            else
            {
                MessageBox.Show("Please set the working directory");
            }
        }

        private void Genome_browse_sra_file_Click(object sender, EventArgs e)
        {
            if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {

                string extension = System.IO.Path.GetExtension(Genome_sra_file_browse_for_fq.FileName);
                if (extension != ".sra")
                {
                    MessageBox.Show("Please select the FASTQ file");
                }
                else
                {
                    Genome_sra_file_browse.Text = Genome_sra_file_browse_for_fq.FileName;
                }
            }
        }

        private void Genome_sra_convert_Click(object sender, EventArgs e)
        {
            string get_data_layout = Genome_comboBox.GetItemText(Genome_comboBox.SelectedItem);
            if (System.IO.Directory.Exists(Genome_working_directory_path.Text))
            {

                if (get_data_layout.Length != 0)
                {
                    //MessageBox.Show(get_data_layout + ", " + gatk_algorithm);
                    if (get_data_layout == "Single-end")
                    {
                        //Single-end command
                        string sra_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools" + "\\";
                        string get_srr_ids = System.IO.Path.GetFullPath(Genome_sra_file_browse.Text);
                        string fastq_dump_cmds = " -m wslx --wait -p ubuntu1804 " + "fastq-dump " + convert_path(get_srr_ids.Trim()) + " -O " + convert_path(Genome_working_directory_path.Text) + "/";
                        MessageBox.Show(fastq_dump_cmds);
                        Process run_fastq_dumps = Linux_ProcessRunner(fastq_dump_cmds);
                        //Genome_sra_file_browse.Text = fastq_dump_cmds;
                    }
                    else if (get_data_layout == "Paired-end")
                    {
                        //Paired-end command
                        try
                        {
                            string sra_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools" + "\\";
                            string get_srr_ids = System.IO.Path.GetFullPath(Genome_sra_file_browse.Text);
                            string fastq_dump_cmds = " -m wslx --wait -p ubuntu1804 " + "fastq-dump --split-files " + convert_path(get_srr_ids.Trim()) + " -O " + convert_path(Genome_working_directory_path.Text) + "/";
                            Process run_fastq_dumps = Linux_ProcessRunner(fastq_dump_cmds);
                            if (run_fastq_dumps.ExitCode.ToString() == "0")
                            {
                                MessageBox.Show("Conversion complete");
                            }
                        }
                        catch (Exception v)
                        {
                            MessageBox.Show(v.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select the layout");
                }
            }
            else
            {
                MessageBox.Show("Please set the working directory");
            }
        }

        private void Genome_annovar_download_label_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form annovar_download = new Annovar_download();
            annovar_download.Visible = true;
        }

        private void Genome_browse_fastq_file_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Genome_working_directory_path.Text))
            {
                if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
                {
                    Genome_fastq_file_path_qc.Text = Genome_sra_file_browse_for_fq.FileName;
                }
            }
            else
            {
                MessageBox.Show("Please set the working directory");
            }
        }

        private void Genome_run_fastx_Click(object sender, EventArgs e)
        {
            /*The process involves converting the fastq to fastq.zip (summary) and extracting the files and read the summary table, show the quality output in textbox*/
            if (System.IO.File.Exists(Genome_fastq_file_path_qc.Text))
            {
                /* conver the fastq file path to Linux path */
                string curr_dir = System.IO.Directory.GetCurrentDirectory() + "/" + "/tools/FastQC/";
            string replace_slash = curr_dir.Substring(2).Replace("\\", "/");
            string get_drive_letter = System.IO.Directory.GetCurrentDirectory().Substring(0, 1).ToLower();
            string fastqc_path = "/mnt/" + get_drive_letter + replace_slash;

            /* conver the input sra file path to Linux path */
            string input_file_conveted_path = convert_path(Genome_fastq_file_path_qc.Text);

            /*Conver the working directory path to linux*/
            input_working_dir_converted_path = convert_path(Genome_working_directory_path.Text);

            /* Working code do not delete ; it will run the fastq command */
            string fastqc_cmd = " -m wslx -p ubuntu1804 " + fastqc_path + "fastqc " + input_file_conveted_path;
            Process run_fastqc_cmd = Linux_ProcessRunner(fastqc_cmd);

            /* Extract the FastQc zip files */
            string get_dirs = System.IO.Path.GetDirectoryName(Genome_fastq_file_path_qc.Text);
            string get_file_name = System.IO.Path.GetFileName(Genome_fastq_file_path_qc.Text);
            System.Diagnostics.Process prcs1 = new System.Diagnostics.Process();

            /*Creating working directory*/
            string wk_dir = System.IO.Path.GetFullPath(Genome_working_directory_path.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Genome_fastq_file_path_qc.Text) + "_fastqc";
            System.IO.Directory.CreateDirectory(wk_dir);
            //MessageBox.Show(fastq_file_path_qc.Text.Replace('.', '_') + 'c');
            String unzip_cmd = " -m wslx -p ubuntu1804 unzip " + input_file_conveted_path.Replace('.', '_') + 'c' + ".zip" + " -d " + input_working_dir_converted_path + "/";

            Process run_unzip_cmd = Linux_ProcessRunner(unzip_cmd);

            /*Read the FastQC summary file to provide the statistics*/
            Dictionary<string, string> dict = new Dictionary<string, string>();
            using (System.IO.TextReader tr1 = System.IO.File.OpenText(Genome_fastq_file_path_qc.Text.Replace('.', '_') + 'c' + "\\" + "summary.txt"))
            {
                string line1 = "";
                while ((line1 = tr1.ReadLine()) != null)
                {
                    string[] items1 = line1.Split('\t');
                    dict.Add(items1[1], items1[0]);
                }
            }
            foreach (KeyValuePair<string, string> keypair in dict)
            {
                    Genome_qc_report.Text = Genome_qc_report.Text + keypair.Key + '\t' + keypair.Value + Environment.NewLine;
            }
            Genome_pictureBox.Load(wk_dir + "\\Images\\per_base_quality.png");
            Genome_pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                /* The files are extracted from fq.zip file */
                //Exon_pictureBox.Image = Image.FromFile(working_directory_path.Text + "\\" + wk_dir + "\\Images\\per_base_quality.png");
            }
            else
            {
                MessageBox.Show("Please load the fastq file");
            }
        }

        private void Genome_alignment_browse_read1_Click(object sender, EventArgs e)
        {
            if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Genome_alignment_fastq_read1.Text = Genome_sra_file_browse_for_fq.FileName;
            }
        }

        private void Genome_alignment_browse_read2_Click(object sender, EventArgs e)
        {
            if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Genome_alignment_fastq_read2.Text = Genome_sra_file_browse_for_fq.FileName;
            }
        }

        private void Genome_alignment_Click(object sender, EventArgs e)
        {
            string cmd_alignment = "";
            string cmd_samtobam = "";
            string cmd_sort = "";
            string cmd_flagstat = "";
            string cmd_markduplicates = "";
            string cmd_createdictfile = "";
            string cmd_addorreplacereadgroup = "";
            string cmd_indexfeaturefile = "";
            string cmd_baserecaliberator = "";
            string cmd_applyBQSR = "";
            string cmd_samtool_faidx = "";

            string add_pause = "";
            if (Genome_pause.Checked)
            {
                add_pause = "--wait ";
            }

            /*Convert the windows path to linux path [Reference fasta path]*/
            string ref_path = Genome_reference_fasta.Text.Substring(2).Replace("\\", "/");
            string ref_path_drv_ltr = Genome_reference_fasta.Text.Substring(0, 1).ToLower();
            string ref_path_converted = "/mnt/" + ref_path_drv_ltr + ref_path;

            /*Convert working directory path to linux */
            string alignment_working_dir_converted = convert_path(Genome_working_directory_path.Text);

            /* Convert Read1 path to linux path */
            read1_working_dir = convert_path(Genome_alignment_fastq_read1.Text);

            string get_data_layout = Genome_data_layout_combobox.GetItemText(Genome_data_layout_combobox.SelectedItem);
            string gatk_algorithm = Genome_gatk_combobox.GetItemText(Genome_gatk_combobox.SelectedItem);

            string output_file_name = System.IO.Path.GetFileNameWithoutExtension(Genome_alignment_fastq_read1.Text); // output file name
            var chk_bed_file = System.IO.File.Exists(Genome_bed_file_path.Text);
            string bed_fl_path = "";
            if (Genome_set_chromosomes.Enabled == false)
            {
                if (chk_bed_file != false)
                {
                    bed_fl_path = " -L " + convert_path(Genome_bed_file_path.Text);
                }
                else
                {
                    bed_fl_path = "";
                }
            }
            else
            {
                if (Genome_set_chromosomes.Text.Length >= 1)
                {
                    bed_fl_path = " -L " + Genome_set_chromosomes.Text;
                }
                else
                {
                    bed_fl_path = "";
                }
            }

            if (get_data_layout.Length != 0 && gatk_algorithm.Length != 0)
            {
                //MessageBox.Show(get_data_layout + ", " + gatk_algorithm);
                if (get_data_layout == "Single-end")
                {
                    // Single-end 
                    Genome_alignment_browse_read2.Enabled = false;
                    Genome_alignment_browse_read2.Enabled = false;

                    cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "bwa mem " + "-t " + Genome_set_threads.Text + " " + ref_path_converted + " " + read1_working_dir + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam";
                    //cmd_alignment = "/C terminal.py --wait -m wslx ls "+ "touch " + alignment_working_dir_converted + "/text.txt";
                }
                else
                {
                    //Convert read2 windows path to linux
                    string read2_working_dir = convert_path(Genome_alignment_fastq_read2.Text);

                    //Paired-end
                    cmd_alignment = "  -m wslx -p ubuntu1804 " + add_pause + "bwa mem " + "-t " + Genome_set_threads.Text + " " + ref_path_converted + " " + read1_working_dir + " " + read2_working_dir + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam";
                }

                cmd_samtool_faidx = " -m wslx -p ubuntu1804 " + add_pause + "samtools faidx " + ref_path_converted;
                cmd_samtobam = " -m wslx -p ubuntu1804 " + add_pause + "samtools view -b -S " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam" + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.bam " + "-@ " + Genome_set_threads.Text;
                cmd_sort = " -m wslx -p ubuntu1804 " + add_pause + "samtools sort " + "-@ " + Genome_set_threads.Text + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.bam";
                cmd_flagstat = " -m wslx -p ubuntu1804 " + add_pause + "samtools flagstat " + "-@ " + Genome_set_threads.Text + " " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam" + " > " + alignment_working_dir_converted + "Alignment_summary.txt";
                string picard_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/picard.jar");
                //string gatk_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/GenomeAnalysisTK.jar");
                string gatk_path = "java -jar " + '"' + "-Xmx" + Genome_set_memory.Text + "G" + '"' + " " + convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/gatk-4.1.4.1/gatk-package-4.1.4.1-local.jar ");
                dbSNP = convert_path(Genome_dbSNP_path.Text);
                //string bed_file = convert_path(bed_file_path.Text);
                string get_reference_file_name = System.IO.Path.GetFileNameWithoutExtension(Genome_reference_fasta.Text);
               
                if (!System.IO.File.Exists(get_reference_file_name.Split('.')[0] + ".dict"))
                {

                    cmd_createdictfile = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " CreateSequenceDictionary " + "-R=" + ref_path_converted + " -O=" + alignment_working_dir_converted + "/" + System.IO.Path.GetFileNameWithoutExtension(Genome_reference_fasta.Text) + ".dict";
                }
                else
                {
                    cmd_createdictfile = " -m wslx -p ubuntu1804 exit";
                }
                cmd_addorreplacereadgroup = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " AddOrReplaceReadGroups -I=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam " + "-O=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted_rg.bam " + "-ID=" + Genome_rgid.Text + " " + "-PU=" + Genome_rgpu.Text + " " + "-SM=" + Genome_rgsm.Text + " " + "-PL=" + Genome_rgpl.Text + " " + "-LB=" + Genome_rglb.Text;
                cmd_markduplicates = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " MarkDuplicates -I=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted_rg.bam " + "-O=" + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true -M=" + alignment_working_dir_converted + "/" + "demo.nodup";
                cmd_indexfeaturefile = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "IndexFeatureFile -I " + dbSNP;
                cmd_baserecaliberator = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "BaseRecalibrator" + " -R " + ref_path_converted + " --known-sites " + dbSNP + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "-O " + alignment_working_dir_converted + "/" + "recal_data.table";
                cmd_applyBQSR = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "ApplyBQSR" + " -R " + ref_path_converted + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam -O " + alignment_working_dir_converted + "/" + "outputApplyBQSR.bam" + " --bqsr-recal-file " + alignment_working_dir_converted + "/" + "recal_data.table";

                string variant_call = " ";
                variant_call = " -m wslx -p ubuntu1804 " + add_pause +  gatk_path + "HaplotypeCaller " + "-I " + alignment_working_dir_converted + "/outputApplyBQSR.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP + " -O " + alignment_working_dir_converted + "/" + "Variant_calling_output" + ".vcf";
                /*if (gatk_algorithm == "UnifiedGenotyper")
                {
                    variant_call = " -m wslx -p ubuntu1804 " + add_pause + "java -jar -Xmx" + Genome_set_memory.Text + "g " + gatk_path + " -T " + "UnifiedGenotyper " + "-I " + alignment_working_dir_converted + "/ReadyForVariantCall.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP + " -o " + alignment_working_dir_converted + "/" + output_file_name + ".vcf" + " -glm SNP -A QualByDepth -A HaplotypeScore -A MappingQualityRankSumTest -A ReadPosRankSumTest -A FisherStrand -A GCContent -A AlleleBalanceBySample -A Coverage --baq CALCULATE_AS_NECESSARY";
                }
                else
                {
                    variant_call = " -m wslx -p ubuntu1804 " + add_pause + "java -jar -Xmx" + Genome_set_memory.Text + "g " + gatk_path + " -T " + "HaplotypeCaller " + "-I " + alignment_working_dir_converted + "/ReadyForVariantCall.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP  + " -o " + alignment_working_dir_converted + "/" + "Variant_calling_output" + ".vcf";
                }*/
                string[] commands = new string[13];
                commands[0] = cmd_alignment;
                commands[1] = cmd_samtool_faidx;
                commands[2] = cmd_samtobam;
                commands[3] = cmd_sort;
                commands[4] = cmd_flagstat;
                commands[5] = cmd_createdictfile;
                commands[6] = cmd_addorreplacereadgroup;
                commands[7] = cmd_markduplicates;
                commands[8] = cmd_indexfeaturefile;
                commands[10] = cmd_baserecaliberator;
                commands[11] = cmd_applyBQSR;
                commands[12] = variant_call;

                foreach (string p in commands)
                {

                    Process prcs = Linux_ProcessRunner(p);
                    textBox1.Text = textBox1.Text + p + Environment.NewLine;
                    //textBox1.Text = textBox1.Text + prcs.ToString() + Environment.NewLine;
                }

            }
            else
            {
                MessageBox.Show("Data layout or Gatk algorithm has to be selected");
            }
        }

        private void Genome_browse_vcf_file_Click(object sender, EventArgs e)
        {
            if (Genome_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Genome_select_vcf_file.Text = Genome_sra_file_browse_for_fq.FileName;
            }
        }

        private void Genome_select_all_db_Click(object sender, EventArgs e)
        {
            int cnt = Genome_sel_db.Items.Count;
            
            for (int i = 0; i <= cnt - 1; i++)
            {
                if (Genome_sel_db.GetItemChecked(i) == false)
                {
                    Genome_sel_db.SetItemCheckState(i, CheckState.Checked);
                }
            }
        }

        private void Genome_clear_db_list_Click(object sender, EventArgs e)
        {
            Genome_sel_db.Items.Clear();
        }

        private void Genome_annovar_annotate_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Genome_working_directory_path.Text))
            {
                if ((Genome_sel_genome_version.SelectedIndex != -1) && (Genome_sel_db.Items.Count > 0) && (System.IO.Path.GetExtension(Genome_select_vcf_file.Text) == ".vcf"))
                {
                    string annovar_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\annovar" + "\\";
                    string perl_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\perl" + "\\" + "bin" + "\\";
                    string create_DB_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools\\DB_making\\";
                    List<string> list_db = new List<string>();
                    int cnt = Genome_sel_db.Items.Count;
                    for (int i = 0; i <= cnt - 1; i++)
                    {
                        if (Genome_sel_db.GetItemChecked(i) == true)
                        {
                            list_db.Add(Genome_sel_db.Items[i].ToString());
                        }
                    }

                    //count the database to be annotated
                    int cnt_db = list_db.Count - 1;
                    string database_add = "-remove -protocol refGene,cytoBand";
                    for (int j = 0; j <= cnt_db; j++)
                    {
                        database_add = database_add + "," + list_db[j];
                        //MessageBox.Show(list_db[j] + " " + j.ToString() + " " + cnt_db.ToString());
                    }

                    //adding operations
                    string operations = "-operation g,r";
                    for (int k = 0; k <= cnt_db; k++)
                    {
                        operations = operations + "," + "f";
                    }

                    string set_genome = "";
                    string genome = Genome_sel_genome_version.GetItemText(Genome_sel_genome_version.SelectedItem);
                    if (genome == "hg19")
                    {
                        set_genome = "hg19";
                    }
                    else if (genome == "hg38")
                    {
                        set_genome = "hg38";
                    }
                    // Write a code to set if genome version is not selected
                    string annovar_cmd = " -m wslx -p ubuntu1804 perl " + convert_path(annovar_path) + "table_annovar.pl " + convert_path(System.IO.Path.GetFullPath(Genome_select_vcf_file.Text)) + " " + convert_path(annovar_path) + "humandb/" + " -buildver " + set_genome + " -out " + convert_path(Genome_working_directory_path.Text) + "/" + System.IO.Path.GetFileNameWithoutExtension(Genome_select_vcf_file.Text).Split('.')[0] + "_annotated.vcf " + database_add + " " + operations + " -nastring . -vcfinput";
                    Process run_annovar = Linux_ProcessRunner(annovar_cmd);
                    //textBox1.Text = annovar_cmd;


                    string annovar_result_file = "";
                    foreach (string i in System.IO.Directory.GetFiles(Genome_working_directory_path.Text))
                    {
                        if (i.ToString().Contains("_multianno.txt"))
                        {
                            annovar_result_file = i.ToString();
                        }
                    }
                    string create_db = "/C " + create_DB_path + "Create_DB.exe " + annovar_result_file + " " + create_DB_path + "NP_and_NM.txt" + " " + create_DB_path + set_genome + "_refGeneversion.txt " + create_DB_path + "homo_sapiens_RS81.fasta " + Genome_working_directory_path.Text + "\\" + "Variant_protein_DB.fasta " + Genome_working_directory_path.Text + "\\";
                    Process run_db_mkaing = ProcessRunner(create_db);
                    MessageBox.Show("Variant database created sucessfully");
                    //textBox1.Text = textBox1.Text + create_db;
                }
                else
                {
                    MessageBox.Show("Either of these may be missing: It can be either Genome version or Annovar database or vcf input file. Please check");
                }
            }
            else
            {
                MessageBox.Show("Please set the working directory");
            }
        }

        private void Genome_dry_run_browse_working_dir_Click(object sender, EventArgs e)
        {
            if (Genome_dry_run_dir_browse.ShowDialog() == DialogResult.OK)
            {
                Genome_dry_run_set_workig_dir.Text = Genome_dry_run_dir_browse.SelectedPath;
            }
        }

        private void Genome_dry_run_browse_dbsnp_Click(object sender, EventArgs e)
        {
            if (Genome_dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Genome_dry_run_set_dbsnp.Text = Genome_dry_run_file_browse.FileName;
            }
        }

        private void Genome_dry_run_browse_reference_genome_Click(object sender, EventArgs e)
        {
            if (Genome_dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Genome_dry_run_set_reference_genome.Text = Genome_dry_run_file_browse.FileName;
            }
        }

        private void Genome_dry_run_browse_bed_file_Click(object sender, EventArgs e)
        {
            if (Genome_dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Genome_dry_run_set_bed_file.Text = Genome_dry_run_file_browse.FileName;
            }
        }

        private void Genome_dry_run_browse_fq_dirs_Click(object sender, EventArgs e)
        {
            if (Genome_dry_run_dir_browse.ShowDialog() == DialogResult.OK)
            {
                Genome_dry_run_select_fq_directory.Text = Genome_dry_run_dir_browse.SelectedPath;
            }
        }

        private void Genome_dry_run_generate_command_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Genome_dry_run_set_workig_dir.Text))
            {
                string gatk_algorithm = Genome_dry_run_gatk_select_algorithm.GetItemText(this.Genome_dry_run_gatk_select_algorithm.SelectedItem);
                if ((!string.IsNullOrEmpty(Genome_dry_run_rgid_txtbx.Text)) && (!string.IsNullOrEmpty(Genome_dry_run_rgpl_txtbx.Text)) && (!string.IsNullOrEmpty(Genome_dry_run_rglb_txtbx.Text)) && (!string.IsNullOrEmpty(Genome_dry_run_rgsm_txtbx.Text)) && (!string.IsNullOrEmpty(Genome_dry_run_rgpu_txtbx.Text)))
                {
                    //Alignment
                    string dbSNP = convert_path(Genome_dry_run_set_dbsnp.Text);
                    string cmd_dryrun_index = "";
                    string samtools_faidx = "";
                    string alignment_cmd = "";
                    string cmd_samtobam = "";
                    string cmd_sortbam = "";
                    string cmd_flagstat = "";
                    string fastq_files = "";
                    string file_for_output = "";
                    string cmd_picard_create_dict = "";
                    string cmd_picard_addorreplace = "";
                    string cmd_markduplicates = "";
                    string cmd_baserecaliberator = "";
                    string cmd_indexfeaturefile = "";
                    string cmd_applyBQSR = "";
                    string variant_call = "";
                    string command_all = "";
                    string picard_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/picard.jar");
                    //string gatk_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/GenomeAnalysisTK.jar");
                    string gatk_path = "java -jar " + '"' + "-Xmx" + Genome_dry_run_memory.Text + "G" + '"' + " " + convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/gatk-4.1.4.1/gatk-package-4.1.4.1-local.jar ");

                    System.IO.DirectoryInfo dir_info = new System.IO.DirectoryInfo(Genome_dry_run_select_fq_directory.Text);
                    System.IO.FileInfo[] file_list = dir_info.GetFiles("*.fastq");
                    foreach (System.IO.FileInfo files in file_list)
                    {
                        fastq_files = fastq_files + convert_path(dir_info.ToString()) + "/" + files + " ";
                        file_for_output = files.ToString().Split('.')[0];
                    }
                    cmd_dryrun_index = "bwa index " + convert_path(Genome_dry_run_set_reference_genome.Text);
                    samtools_faidx = "samtools faidx " + convert_path(Genome_dry_run_set_reference_genome.Text);
                    alignment_cmd = "bwa mem " + "-t " + Genome_dry_run_threads.Text + " " + convert_path(Genome_dry_run_set_reference_genome.Text) + " " + fastq_files + "> " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam";
                    cmd_samtobam = "samtools view -b -S " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam > " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.bam -@ " + Genome_dry_run_threads.Text;
                    cmd_sortbam = "samtools sort -@ " + Genome_dry_run_threads.Text + " -o " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.bam";
                    cmd_flagstat = "samtools flagstat -@ " + Genome_dry_run_threads.Text + " " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam > " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_Alignment_summary.txt";
                    cmd_indexfeaturefile = gatk_path + "IndexFeatureFile -I " + dbSNP;
                    /*cmd_picard_create_dict = gatk_path + " CreateSequenceDictionary " + "-R=" + convert_path(Genome_dry_run_set_reference_genome.Text) + " -O=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + System.IO.Path.GetFileNameWithoutExtension(Genome_dry_run_set_reference_genome.Text) + ".dict";
                    cmd_picard_addorreplace = gatk_path + " AddOrReplaceReadGroups -I=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + "-O=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-ID=" + Genome_dry_run_rgid_txtbx.Text + " " + "-PU=" + Genome_dry_run_rgpu_txtbx.Text + " " + "-SM=" + Genome_dry_run_rgsm_txtbx.Text + " " + "-PL=" + Genome_dry_run_rgpl_txtbx.Text + " " + "-LB=" + Genome_dry_run_rglb_txtbx.Text;

                    cmd_markduplicates = gatk_path + " MarkDuplicates -I=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-O=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true --M=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "demo.nodup";
                    cmd_Realignertargetcreator = "java -jar -Xmx" + Genome_dry_run_memory.Text + "g " + gatk_path + " -T " + "RealignerTargetCreator" + " -R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " --known " + convert_path(Genome_dry_run_set_dbsnp.Text) + " -I " + convert_path(Genome_dry_run_set_workig_dir.Text) + '/' +  file_for_output + "_sorted_nodup.bam -o " + convert_path(Genome_dry_run_set_workig_dir.Text)  + "/" + "demo.intervals";

                    cmd_indelrealigner = "java -jar -Xmx" + Genome_dry_run_memory.Text + "g " + gatk_path + " -T " + "IndelRealigner " + "-R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " -known " + convert_path(Genome_dry_run_set_dbsnp.Text) + " -targetIntervals " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "demo.intervals" + " -I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam" + " -o " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup_realigned.bam";
                    cmd_baserecaliberator = "java -jar -Xmx" + Genome_dry_run_memory.Text + "g " + gatk_path + " -T " + "BaseRecalibrator" + " -R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " --knownSites " + convert_path(Genome_dry_run_set_dbsnp.Text) + " -I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup_realigned.bam " + "-o " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup_realigned_baserecaliberated.bam";
                    cmd_printreads = "java -jar -Xmx" + Genome_dry_run_memory.Text + "g " + gatk_path + " -T " + "PrintReads" + " -R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " -BQSR " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup_realigned_baserecaliberated.bam " + "-I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup_realigned.bam -o " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/ReadyForVariantCall.bam";
                    */
                    cmd_picard_create_dict = gatk_path + " CreateSequenceDictionary " + "-R=" + convert_path(Genome_dry_run_set_reference_genome.Text) + " -O=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + System.IO.Path.GetFileNameWithoutExtension(Genome_dry_run_set_reference_genome.Text) + ".dict";

                    cmd_picard_addorreplace = gatk_path + " AddOrReplaceReadGroups -I=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + "-O=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-ID=" + Genome_dry_run_rgid_txtbx.Text + " " + "-PU=" + Genome_dry_run_rgpu_txtbx.Text + " " + "-SM=" + Genome_dry_run_rgsm_txtbx.Text + " " + "-PL=" + Genome_dry_run_rgpl_txtbx.Text + " " + "-LB=" + Genome_dry_run_rglb_txtbx.Text;

                    cmd_markduplicates = gatk_path + " MarkDuplicates -I=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-O=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true --M=" + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "demo.nodup";

                    cmd_baserecaliberator = gatk_path + "BaseRecalibrator" + " -R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " --known-sites " + convert_path(Genome_dry_run_set_dbsnp.Text) + " -I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam " + "-O " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "recal_data.table";


                    cmd_applyBQSR =  gatk_path + "ApplyBQSR" + " -R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " -I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam -O " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "outputApplyBQSR.bam" + " --bqsr-recal-file " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "recal_data.table";

                    variant_call =  gatk_path + "HaplotypeCaller " + "-I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/outputApplyBQSR.bam " + "-R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " --dbsnp " + dbSNP + " -O " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + "Variant_calling_output" + ".vcf";

                    /*if (gatk_algorithm == "UnifiedGenotyper")
                    {
                        variant_call = "java -jar -Xmx" + Genome_dry_run_memory.Text + "g " + gatk_path + " -T " + gatk_algorithm + " " + "-I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/ReadyForVariantCall.bam " + "-R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " --dbsnp " + convert_path(Genome_dry_run_set_dbsnp.Text) + " -o " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + ".vcf" + " -glm SNP -A QualByDepth -A HaplotypeScore -A MappingQualityRankSumTest -A ReadPosRankSumTest -A FisherStrand -A GCContent -A AlleleBalanceBySample -A Coverage --baq CALCULATE_AS_NECESSARY";
                    }
                    else
                    {
                        variant_call = "java -jar -Xmx" + Genome_dry_run_memory.Text + "g " + gatk_path + " -T " + gatk_algorithm + " " + "-I " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/ReadyForVariantCall.bam " + "-R " + convert_path(Genome_dry_run_set_reference_genome.Text) + " --dbsnp " + convert_path(Genome_dry_run_set_dbsnp.Text) + " -o " + convert_path(Genome_dry_run_set_workig_dir.Text) + "/" + file_for_output + ".vcf";
                    }*/
                    //textBox7.Text = cmd_printreads;

                    string[] commands = new string[14];
                    commands[0] = cmd_dryrun_index;
                    commands[1] = samtools_faidx;
                    commands[2] = alignment_cmd;
                    commands[3] = cmd_samtobam;
                    commands[4] = cmd_sortbam;
                    commands[5] = cmd_flagstat;
                    commands[6] = cmd_indexfeaturefile;
                    commands[7] = cmd_picard_create_dict;
                    commands[8] = cmd_picard_addorreplace;
                    commands[9] = cmd_markduplicates;
                    commands[10] = cmd_baserecaliberator;
                    commands[11] = cmd_baserecaliberator;
                    commands[12] = cmd_applyBQSR;
                    commands[13] = variant_call;

                    //StreamWriter OurStream;
                    //OurStream = System.IO.File.CreateText(Exome_dry_run_set_workig_dir.Text + "/" + "dry_run.sh");

                    foreach (string p in commands)
                    {
                        //Process prcs = ProcessRunner(p);
                        //textBox7.Text T= textBox1.Text + prcs.StandardOutput.ReadToEnd().ToString() + Environment.NewLine;

                        command_all = command_all + p + '\n';
                        //OurStream.WriteLine(p.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("'$", "").Replace("'$'\r'", '');
                    }
                    Genome_dry_run_open_directory.Enabled = true;

                    System.IO.File.WriteAllText(Genome_dry_run_set_workig_dir.Text + "/" + "dry_run.sh", command_all);
                }
            }
        }

        private void Genome_dry_run_open_directory_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Genome_dry_run_set_workig_dir.Text);
        }

        private void Genome_summary_fastqc_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Genome_fastq_file_path_qc.Text.Replace('.', '_') + 'c' + "\\" + "summary.txt"))
            {
                System.Diagnostics.Process.Start(Genome_fastq_file_path_qc.Text.Split('.')[0] + "_fastqc.html");
            }
            else
            {
                MessageBox.Show("Please run the QC to get the summary");
            }
        }

        int flag = 0;
        private void Genome_enable_chromosome_field_Click(object sender, EventArgs e)
        {
            if (flag==0)
            {
                Genome_bed_file_path.Enabled = false;
                Genome_browse_bed_file.Enabled = false;
                Genome_set_chromosomes.Enabled = true;
                flag = 1;
            
            }
            else
            {
                Genome_bed_file_path.Enabled = true;
                Genome_browse_bed_file.Enabled = true;
                Genome_set_chromosomes.Enabled = false;
                flag = 0;
            }
        }

        private void downloadAnnovarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form annovar_download = new Annovar_download();
            annovar_download.Visible = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Genome_sel_genome_version_SelectedIndexChanged(object sender, EventArgs e)
        {
            string genome = Genome_sel_genome_version.GetItemText(Genome_sel_genome_version.SelectedItem);
            Genome_sel_db.Items.Clear();
            if (genome == "hg19")
            {
                Genome_sel_db.Items.Insert(0, "avsift");
                Genome_sel_db.Items.Insert(1, "ljb26_all");
                Genome_sel_db.Items.Insert(2, "dbnsfp30a");
                Genome_sel_db.Items.Insert(3, "dbnsfp31a_interpro");
                Genome_sel_db.Items.Insert(4, "dbnsfp33a");
                Genome_sel_db.Items.Insert(5, "dbnsfp35a");
                Genome_sel_db.Items.Insert(6, "dbnsfp35c");
                Genome_sel_db.Items.Insert(7, "dbscsnv11");
                Genome_sel_db.Items.Insert(8, "intervar_20170202");
                Genome_sel_db.Items.Insert(9, "intervar_20180118");
                Genome_sel_db.Items.Insert(10, "cg46");
                Genome_sel_db.Items.Insert(11, "cg69");
                Genome_sel_db.Items.Insert(12, "cosmic64");
                Genome_sel_db.Items.Insert(13, "cosmic65");
                Genome_sel_db.Items.Insert(14, "cosmic67");
                Genome_sel_db.Items.Insert(15, "cosmic67wgs");
                Genome_sel_db.Items.Insert(16, "cosmic68");
                Genome_sel_db.Items.Insert(17, "cosmic68wgs");
                Genome_sel_db.Items.Insert(18, "cosmic70");
                Genome_sel_db.Items.Insert(19, "esp6500siv2_ea");
                Genome_sel_db.Items.Insert(20, "esp6500siv2_aa");
                Genome_sel_db.Items.Insert(21, "esp6500siv2_all");
                Genome_sel_db.Items.Insert(22, "exac03");
                Genome_sel_db.Items.Insert(23, "exac03nontcga");
                Genome_sel_db.Items.Insert(24, "exac03nonpsych");
                Genome_sel_db.Items.Insert(25, "gnomad_exome");
                Genome_sel_db.Items.Insert(26, "gnomad_genome");
                Genome_sel_db.Items.Insert(27, "kaviar_20150923");
                Genome_sel_db.Items.Insert(28, "hrcr1");
                Genome_sel_db.Items.Insert(29, "abraom");
                Genome_sel_db.Items.Insert(30, "1000g2010nov");
                Genome_sel_db.Items.Insert(31, "1000g2011may");
                Genome_sel_db.Items.Insert(32, "1000g2012feb");
                Genome_sel_db.Items.Insert(33, "1000g2012apr");
                Genome_sel_db.Items.Insert(34, "1000g2014aug");
                Genome_sel_db.Items.Insert(35, "1000g2014sep");
                Genome_sel_db.Items.Insert(36, "1000g2014oct");
                Genome_sel_db.Items.Insert(37, "1000g2015aug");
                Genome_sel_db.Items.Insert(38, "gme");
                Genome_sel_db.Items.Insert(39, "mcap");
                Genome_sel_db.Items.Insert(40, "mcap13");
                Genome_sel_db.Items.Insert(41, "revel");
                Genome_sel_db.Items.Insert(42, "snp129");
                Genome_sel_db.Items.Insert(43, "snp130");
                Genome_sel_db.Items.Insert(44, "snp131");
                Genome_sel_db.Items.Insert(45, "snp132");
                Genome_sel_db.Items.Insert(46, "snp135");
                Genome_sel_db.Items.Insert(47, "snp137");
                Genome_sel_db.Items.Insert(48, "snp138");
                Genome_sel_db.Items.Insert(49, "avsnp138");
                Genome_sel_db.Items.Insert(50, "avsnp142");
                Genome_sel_db.Items.Insert(51, "avsnp144");
                Genome_sel_db.Items.Insert(52, "avsnp147");
                Genome_sel_db.Items.Insert(53, "avsnp150");
                Genome_sel_db.Items.Insert(54, "snp130NonFlagged");
                Genome_sel_db.Items.Insert(55, "snp131NonFlagged");
                Genome_sel_db.Items.Insert(56, "snp132NonFlagged");
                Genome_sel_db.Items.Insert(57, "snp135NonFlagged");
                Genome_sel_db.Items.Insert(58, "snp137NonFlagged");
                Genome_sel_db.Items.Insert(59, "snp138NonFlagged");
                Genome_sel_db.Items.Insert(60, "nci60");
                Genome_sel_db.Items.Insert(61, "icgc21");
                Genome_sel_db.Items.Insert(62, "clinvar_20131105");
                Genome_sel_db.Items.Insert(63, "clinvar_20140211");
                Genome_sel_db.Items.Insert(64, "clinvar_20140303");
                Genome_sel_db.Items.Insert(65, "clinvar_20140702");
                Genome_sel_db.Items.Insert(66, "clinvar_20140902");
                Genome_sel_db.Items.Insert(67, "clinvar_20140929");
                Genome_sel_db.Items.Insert(68, "clinvar_20150330");
                Genome_sel_db.Items.Insert(69, "clinvar_20150629");
                Genome_sel_db.Items.Insert(70, "clinvar_20151201");
                Genome_sel_db.Items.Insert(71, "clinvar_20160302");
                Genome_sel_db.Items.Insert(72, "clinvar_20161128");
                Genome_sel_db.Items.Insert(73, "clinvar_20170130");
                Genome_sel_db.Items.Insert(74, "clinvar_20170501");
                Genome_sel_db.Items.Insert(75, "clinvar_20170905");
                Genome_sel_db.Items.Insert(76, "clinvar_20180603");
                Genome_sel_db.Items.Insert(77, "clinvar_20190305");
                Genome_sel_db.Items.Insert(78, "popfreq_max_20150413");
                Genome_sel_db.Items.Insert(79, "popfreq_all_20150413");
                Genome_sel_db.Items.Insert(80, "mitimpact2");
                Genome_sel_db.Items.Insert(81, "mitimpact24");
                Genome_sel_db.Items.Insert(82, "regsnpintron");
                Genome_sel_db.Items.Insert(83, "gerp++elem");
                Genome_sel_db.Items.Insert(84, "gerp++gt2");
                Genome_sel_db.Items.Insert(85, "caddgt20");
                Genome_sel_db.Items.Insert(86, "caddgt10");
                Genome_sel_db.Items.Insert(87, "cadd");
                Genome_sel_db.Items.Insert(88, "cadd13");
                Genome_sel_db.Items.Insert(89, "cadd13gt10");
                Genome_sel_db.Items.Insert(90, "cadd13gt20");
                Genome_sel_db.Items.Insert(91, "caddindel");
                Genome_sel_db.Items.Insert(92, "fathmm");
                Genome_sel_db.Items.Insert(93, "gwava");
                Genome_sel_db.Items.Insert(94, "eigen");
            }
            else
            {
                Genome_sel_db.Items.Insert(0, "ljb26_all");
                Genome_sel_db.Items.Insert(1, "dbnsfp30a");
                Genome_sel_db.Items.Insert(2, "dbnsfp31a_interpro");
                Genome_sel_db.Items.Insert(3, "dbnsfp33a");
                Genome_sel_db.Items.Insert(4, "dbnsfp35a");
                Genome_sel_db.Items.Insert(5, "dbnsfp35c");
                Genome_sel_db.Items.Insert(6, "dbscsnv11");
                Genome_sel_db.Items.Insert(7, "intervar_20180118");
                Genome_sel_db.Items.Insert(8, "cosmic70");
                Genome_sel_db.Items.Insert(9, "esp6500siv2_ea");
                Genome_sel_db.Items.Insert(10, "esp6500siv2_aa");
                Genome_sel_db.Items.Insert(10, "esp6500siv2_all");
                Genome_sel_db.Items.Insert(12, "exac03");
                Genome_sel_db.Items.Insert(13, "exac03nontcga");
                Genome_sel_db.Items.Insert(14, "exac03nonpsych");
                Genome_sel_db.Items.Insert(15, "gnomad_exome");
                Genome_sel_db.Items.Insert(16, "gnomad_genome");
                Genome_sel_db.Items.Insert(17, "kaviar_20150923");
                Genome_sel_db.Items.Insert(18, "hrcr1");
                Genome_sel_db.Items.Insert(19, "abraom");
                Genome_sel_db.Items.Insert(20, "1000g2014oct");
                Genome_sel_db.Items.Insert(21, "1000g2015aug");
                Genome_sel_db.Items.Insert(22, "gme");
                Genome_sel_db.Items.Insert(23, "mcap");
                Genome_sel_db.Items.Insert(24, "revel");
                Genome_sel_db.Items.Insert(25, "avsnp144");
                Genome_sel_db.Items.Insert(26, "avsnp142");
                Genome_sel_db.Items.Insert(27, "avsnp144");
                Genome_sel_db.Items.Insert(28, "avsnp147");
                Genome_sel_db.Items.Insert(29, "avsnp150");
                Genome_sel_db.Items.Insert(30, "clinvar_20140702");
                Genome_sel_db.Items.Insert(31, "clinvar_20150330");
                Genome_sel_db.Items.Insert(32, "clinvar_20150629");
                Genome_sel_db.Items.Insert(33, "clinvar_20151201");
                Genome_sel_db.Items.Insert(34, "clinvar_20160302");
                Genome_sel_db.Items.Insert(35, "clinvar_20161128");
                Genome_sel_db.Items.Insert(36, "clinvar_20170130");
                Genome_sel_db.Items.Insert(37, "clinvar_20170501");
                Genome_sel_db.Items.Insert(38, "clinvar_20170905");
                Genome_sel_db.Items.Insert(39, "clinvar_20180603");
                Genome_sel_db.Items.Insert(40, "clinvar_20190305");
                Genome_sel_db.Items.Insert(41, "regsnpintron");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form abt = new About();
            abt.Show();
        }

        private void Genome_data_layout_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Genome_data_layout_combobox.GetItemText(Genome_data_layout_combobox.SelectedItem) == "Single-end")
            {
              
                Genome_alignment_fastq_read2.Enabled = false;
            }
            else
            {
                Genome_alignment_fastq_read2.Enabled = true; ;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Genome_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 fm1 = new Form1();
            fm1.Show();
            
        }
    }
 }

