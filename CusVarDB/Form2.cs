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
    public partial class Form2 : Form
    {
        //working directory for tab check

        public string wkdir_tab = "";
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
        public string terminal_path = "";

        public int processor_count;
        public Form2()
        {
            InitializeComponent();
            terminal_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "terminal.exe";
            processor_count = Environment.ProcessorCount;
            Set_chromosome_field.Enabled = false;
            if (processor_count <= 2)
            {
                MessageBox.Show("The program may not run faster due to the limited threads!!!!!!!!!");
                processor_count = 1;
            }
            else
            {
                processor_count = processor_count - 2;
            }
            threads.Text = processor_count.ToString();
            Exome_dry_run_threads.Text = processor_count.ToString();
            var totalRAM = (Convert.ToInt32((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory) / (Math.Pow(1024, 3)) + 0.5) / 2);
            {

                Exome_dry_run_memory.Text = totalRAM.ToString();
                Memory.Text = totalRAM.ToString();
            }
            string get_curr_dir = System.IO.Directory.GetCurrentDirectory();
            Set_chromosome_field.Enabled = false;
            Exome_dry_run_set_chromosome.Enabled = false;
            /*
            Exome_alignment_btn.Visible = false;
            Exome_sambam_btn.Visible = false;
            Exome_sortbam_btn.Visible = false;
            Exome_addreplace_btn.Visible = false;
            Exome_createdict_btn.Visible = false;
            Exome_markdup_btn.Visible = false;
            Exome_realignertarget_btn.Visible = false;
            Exome_indelrealigner_btn.Visible = false;
            Exome_baserecaliberator_btn.Visible = false;
            Exome_printreads_btn.Visible = false;
            Exome_gatk_btn.Visible = false; */
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

        /* function takes SRR10000, SRR*/
        public string download_SRA(string sequence_read_archive_id, string id)
        {
            string sra_id = sequence_read_archive_id.Trim();
            string url = "https://ftp.ncbi.nlm.nih.gov/sra/sra-instant/reads/ByRun/sra/" + id + "/" + sra_id.Substring(0, 6) + "/" + sra_id + "/" + sra_id + ".sra";
            //string sra_file_download = "/C terminal.py --wait -m wslx wget -c " + url + " -P " + convert_path(working_directory_path.Text) + "/";
            string sra_file_download = " --wait -m wslx -p ubuntu1804 wget -c " + url + " -P " + convert_path(working_directory_path.Text) + "/";
            //sra_id_download.Text = sra_file_download;
            Process run_sra_download = Linux_ProcessRunner(sra_file_download);
            /* Use process.ExitCode and based on the code pass the message in return */
            //sra_file_browse.Text = run_sra_download.ExitCode.ToString();
            return run_sra_download.ExitCode.ToString();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void sra_download_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(working_directory_path.Text))
            { 
            if (sra_id_download.Text.Contains("SRR"))
            {
                /*Reduce the code to function and pass SRR, ERR and DRR to the function*/
                string sra_fl = download_SRA(sra_id_download.Text, "SRR");
            }
            else if (sra_id_download.Text.Contains("ERR"))
            {
                string sra_fl = download_SRA(sra_id_download.Text, "ERR");

            }
            else if (sra_id_download.Text.Contains("DRR"))
            {
                string sra_fl = download_SRA(sra_id_download.Text, "DRR");
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

        private void browse_sra_file_Click(object sender, EventArgs e)
        {
            if (sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {

                string extension = System.IO.Path.GetExtension(sra_file_browse_for_fq.FileName);
                if (extension != ".sra")
                {
                    MessageBox.Show("Please select the FASTQ file");
                }
                else
                {
                    sra_file_browse.Text = sra_file_browse_for_fq.FileName;
                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {

            string get_data_layout = comboBox1.GetItemText(comboBox1.SelectedItem);
            if (System.IO.Directory.Exists(working_directory_path.Text))
            {
                if (get_data_layout.Length != 0)
                {
                    //MessageBox.Show(get_data_layout + ", " + gatk_algorithm);
                    if (get_data_layout == "Single-end")
                    {
                        //Single-end command
                        string sra_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools" + "\\";
                        string get_srr_ids = System.IO.Path.GetFullPath(sra_file_browse.Text);
                        string fastq_dump_cmds = " -m wslx --wait -p ubuntu1804 " + "fastq-dump " + convert_path(get_srr_ids.Trim()) + " -O " + convert_path(working_directory_path.Text) + "/";
                        Process run_fastq_dumps = Linux_ProcessRunner(fastq_dump_cmds);
                        
                        //sra_file_browse.Text = fastq_dump_cmds;
                    }
                    else if (get_data_layout == "Paired-end")
                    {
                        //Paired-end command
                        try
                        {
                            string sra_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools" + "\\";
                            string get_srr_ids = System.IO.Path.GetFullPath(sra_file_browse.Text);
                            string fastq_dump_cmds = " -m wslx --wait -p ubuntu1804 " + "fastq-dump --split-files " + convert_path(get_srr_ids.Trim()) + " -O " + convert_path(working_directory_path.Text) + "/";
                            Process run_fastq_dumps = Linux_ProcessRunner(fastq_dump_cmds);
                            //MessageBox.Show(fastq_dump_cmds);
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

        private void browse_reference_file_Click(object sender, EventArgs e)
        {
            if (working_directory.ShowDialog() == DialogResult.OK)
            {
                working_directory_path.Text = working_directory.SelectedPath;
            }
        }


        private void browse_fastq_file_Click(object sender, EventArgs e)
        {

            if (System.IO.Directory.Exists(working_directory_path.Text))
            {
                if (sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
                {
                    fastq_file_path_qc.Text = sra_file_browse_for_fq.FileName;
                }
            }
            else
            {
                MessageBox.Show("Please set the working directory");
            }
        }

        private void run_fastx_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(fastq_file_path_qc.Text))
            {
                    /*The process involves converting the fastq to fastq.zip (summary) and extracting the files and read the summary table, show the quality output in textbox*/
                    /* conver the fastq file path to Linux path */
                    string curr_dir = System.IO.Directory.GetCurrentDirectory() + "/" + "tools/FastQC/";
                    string replace_slash = curr_dir.Substring(2).Replace("\\", "/");
                    string get_drive_letter = System.IO.Directory.GetCurrentDirectory().Substring(0, 1).ToLower();
                    string fastqc_path = "/mnt/" + get_drive_letter + replace_slash;

                    /* conver the input sra file path to Linux path */
                    string input_file_conveted_path = convert_path(fastq_file_path_qc.Text);

                    /*Conver the working directory path to linux*/
                    input_working_dir_converted_path = convert_path(working_directory_path.Text);

                    /* Working code do not delete ; it will run the fastq command */
                    string fastqc_cmd = " --wait -m wslx -p ubuntu1804 " + fastqc_path + "fastqc " + input_file_conveted_path;
                    //qc_report.Text = fastqc_cmd;
                    Process run_fastqc_cmd = Linux_ProcessRunner(fastqc_cmd);

                    /* Extract the FastQc zip files */
                    string get_dirs = System.IO.Path.GetDirectoryName(fastq_file_path_qc.Text);
                    string get_file_name = System.IO.Path.GetFileName(fastq_file_path_qc.Text);
                    System.Diagnostics.Process prcs1 = new System.Diagnostics.Process();

                    /*Creating working directory*/
                    string wk_dir = System.IO.Path.GetFullPath(working_directory_path.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(fastq_file_path_qc.Text) + "_fastqc";
                    System.IO.Directory.CreateDirectory(wk_dir);
                    //MessageBox.Show(fastq_file_path_qc.Text.Replace('.', '_') + 'c');
                    String unzip_cmd = " --wait -m wslx -p ubuntu1804 unzip " + input_file_conveted_path.Replace('.', '_') + 'c' + ".zip" + " -d " + input_working_dir_converted_path + "/";

                    Process run_unzip_cmd = Linux_ProcessRunner(unzip_cmd);

                    /*Read the FastQC summary file to provide the statistics*/
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    using (System.IO.TextReader tr1 = System.IO.File.OpenText(fastq_file_path_qc.Text.Replace('.', '_') + 'c' + "\\" + "summary.txt"))
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
                        qc_report.Text = qc_report.Text + keypair.Key + '\t' + keypair.Value + Environment.NewLine;

                    }
                    Exon_pictureBox.Load(wk_dir + "\\Images\\per_base_quality.png");
                    Exon_pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    /* The files are extracted from fq.zip file */
                    //Exon_pictureBox.Image = Image.FromFile(working_directory_path.Text + "\\" + wk_dir + "\\Images\\per_base_quality.png");
                
            }
            else
            {
                MessageBox.Show("Please load fastq file");
            }


        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            alignment_fastq_read2.Enabled = false;
            alignment_browse_read2.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            alignment_fastq_read2.Enabled = true;
            alignment_browse_read2.Enabled = true;
        }

        private void alignment_Click(object sender, EventArgs e)
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
            if (pause.Checked)
            {
                add_pause = "--wait ";
            }

            /*Convert the windows path to linux path [Reference fasta path]*/
            string ref_path = reference_fasta.Text.Substring(2).Replace("\\", "/");
            string ref_path_drv_ltr = reference_fasta.Text.Substring(0, 1).ToLower();
            string ref_path_converted = "/mnt/" + ref_path_drv_ltr + ref_path;

            /*Convert working directory path to linux */
            string alignment_working_dir_converted = convert_path(working_directory_path.Text);

            /* Convert Read1 path to linux path */
            read1_working_dir = convert_path(alignment_fastq_read1.Text);

            string get_data_layout = data_layout_combobox.GetItemText(data_layout_combobox.SelectedItem);
            string gatk_algorithm = gatk_combobox.GetItemText(gatk_combobox.SelectedItem);

            string output_file_name = System.IO.Path.GetFileNameWithoutExtension(alignment_fastq_read1.Text); // output file name
            var chk_bed_file = System.IO.File.Exists(bed_file_path.Text);
            string bed_fl_path = "";
            if (Set_chromosome_field.Enabled ==false)
            {
                if (chk_bed_file != false)
                {
                    bed_fl_path = " -L " + convert_path(bed_file_path.Text);
                }
                else
                {
                    bed_fl_path = "";
                }
            }
            else
            { 
                if (Set_chromosome_field.Text.Length >=1)
                { 
                    bed_fl_path =  " -L " + Set_chromosome_field.Text;
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
                        alignment_browse_read2.Enabled = false;

                        cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "bwa mem " + "-t " + threads.Text + " " + ref_path_converted + " " + read1_working_dir + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam";
                        //cmd_alignment = "/C terminal.py --wait -m wslx ls "+ "touch " + alignment_working_dir_converted + "/text.txt";
                    }
                    else
                    {
                        //Convert read2 windows path to linux
                        string read2_working_dir = convert_path(alignment_fastq_read2.Text);

                        //Paired-end
                        cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "bwa mem " + "-t " + threads.Text + " " + ref_path_converted + " " + read1_working_dir + " " + read2_working_dir + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam";
                    }

                        cmd_samtool_faidx = " -m wslx -p ubuntu1804 " + add_pause + "samtools faidx " + ref_path_converted;
                        cmd_samtobam = " -m wslx -p ubuntu1804 " + add_pause + "samtools view -b -S " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam" + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.bam " + "-@ " + threads.Text;
                        cmd_sort = " -m wslx -p ubuntu1804 " + add_pause + "samtools sort " + "-@ " + threads.Text + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.bam";
                        cmd_flagstat = "  -m wslx -p ubuntu1804 " + add_pause + "samtools flagstat " + "-@ " + threads.Text + " " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam" + " > " + alignment_working_dir_converted + "/" + "Alignment_summary.txt";
                        string picard_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/picard.jar");
                        //string gatk_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/GenomeAnalysisTK.jar");
                        string gatk_path = "java -jar " + '"' + "-Xmx" + Memory.Text + "G" + '"' + " " + convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/gatk-4.1.4.1/gatk-package-4.1.4.1-local.jar ");
                        dbSNP = convert_path(dbSNP_path.Text);
                        //string bed_file = convert_path(bed_file_path.Text);
                        string get_reference_file_name = System.IO.Path.GetFileNameWithoutExtension(reference_fasta.Text);
                        if (System.IO.File.Exists(reference_fasta.Text.Split('.')[0] + ".dict"))
                            {
                                cmd_createdictfile = " -m wslx -p ubuntu1804 exit";
                }
                         else
                            {
                                
                                cmd_createdictfile = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " CreateSequenceDictionary " + "-R=" + ref_path_converted + " -O=" + alignment_working_dir_converted + "/" + System.IO.Path.GetFileNameWithoutExtension(reference_fasta.Text) + ".dict" + " ";
                            }
                            cmd_addorreplacereadgroup = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " AddOrReplaceReadGroups -I=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam " + "-O=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted_rg.bam " + "-ID=" + rgid.Text + " " + "-PU=" + rgpu.Text + " " + "-SM=" + rgsm.Text + " " + "-PL=" + rgpl.Text + " " + "-LB=" + rglb.Text;
                            cmd_markduplicates = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " MarkDuplicates -I=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted_rg.bam " + "-O=" + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true --M=" + alignment_working_dir_converted + "/" + "demo.nodup";
                            cmd_indexfeaturefile = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "IndexFeatureFile -I " + dbSNP;
                            //cmd_indelrealigner = " -m wslx -p ubuntu1804 " + add_pause + "java -jar -Xmx" + Memory.Text + "g " + gatk_path + " -T " + "IndelRealigner " + "-R " + ref_path_converted + " -known " + dbSNP + " -targetIntervals " + alignment_working_dir_converted + "/" + "demo.intervals" + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam" + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup_realigned.bam";
                            //cmd_baserecaliberator = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "BaseRecalibrator" + " -R " + ref_path_converted + " --known-sites " + dbSNP + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "-O " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup_realigned_baserecaliberated.bam" + " ";
                            //cmd_printreads = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "PrintReads" + " -R " + ref_path_converted + " --BQSR " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup_realigned_baserecaliberated.bam " + "-I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup_realigned.bam -O " + alignment_working_dir_converted + "/" + "ReadyForVariantCall.bam";
                            cmd_baserecaliberator = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "BaseRecalibrator" + " -R " + ref_path_converted + " --known-sites " + dbSNP + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "-O " + alignment_working_dir_converted + "/" + "recal_data.table";
                            cmd_applyBQSR = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "ApplyBQSR" + " -R " + ref_path_converted + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam -O " + alignment_working_dir_converted + "/" + "outputApplyBQSR.bam" + " --bqsr-recal-file " + alignment_working_dir_converted + "/" + "recal_data.table";
                            string variant_call = " ";
                            variant_call = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "HaplotypeCaller " + "-I " + alignment_working_dir_converted + "/outputApplyBQSR.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP + " -O " + alignment_working_dir_converted + "/" + "Variant_calling_output" + ".vcf";
                /* 
                if (gatk_algorithm == "UnifiedGenotyper")
                     {
                        variant_call = " -m wslx -p ubuntu1804 " + add_pause + "java -jar -Xmx" + Memory.Text + "g " + gatk_path + " -T " + "UnifiedGenotyper " + "-I " + alignment_working_dir_converted + "/ReadyForVariantCall.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP + " -o " + alignment_working_dir_converted + "/" + output_file_name + ".vcf" + bed_fl_path + " -glm SNP -A QualByDepth -A HaplotypeScore -A MappingQualityRankSumTest -A ReadPosRankSumTest -A FisherStrand -A GCContent -A AlleleBalanceBySample -A Coverage --baq CALCULATE_AS_NECESSARY";
                     }
                else
                    {
                        variant_call = " -m wslx -p ubuntu1804 " + add_pause + "java -jar -Xmx" + Memory.Text + "g " + gatk_path + " -T " + "HaplotypeCaller " + "-I " + alignment_working_dir_converted + "/ReadyForVariantCall.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP + bed_fl_path + " -o " + alignment_working_dir_converted + "/" + "Variant_calling_output" + ".vcf";
                    }
                    */
                string[] commands = new string[12];
                commands[0] = cmd_alignment;
                commands[1] = cmd_samtool_faidx;
                commands[2] = cmd_samtobam;
                commands[3] = cmd_sort;
                commands[4] = cmd_flagstat;
                commands[5] = cmd_createdictfile;
                commands[6] = cmd_addorreplacereadgroup;
                commands[7] = cmd_markduplicates;
                commands[8] = cmd_indexfeaturefile;
                commands[9] = cmd_baserecaliberator;
                commands[10] = cmd_applyBQSR;
                commands[11] = variant_call;

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

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {           
            OpenFileDialog ofd_reference_fasta = new OpenFileDialog();
            ofd_reference_fasta.Filter = "Fasta file (*.fasta)|*.fasta|Fasta file (*.fa)|*.fa|Fasta file (*.fna)|*.fna";
            if(ofd_reference_fasta.ShowDialog() == DialogResult.OK)
            {
                reference_fasta.Text = ofd_reference_fasta.FileName;
                string get_filename = System.IO.Path.GetFullPath(reference_fasta.Text); //reference file name with extension

                //Browse for reference file
                ref_path_converted = convert_path(reference_fasta.Text);

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
                    startInfo.FileName = terminal_path;
                    //startInfo.Arguments = "/C terminal.py -m wslx " + root_path + '/' + "bwa index " + ref_path_converted;
                    startInfo.Arguments = " -m wslx " + root_path + '/' + "bwa index " + ref_path_converted;
                    sra_file_browse.Text = startInfo.Arguments;
                    prcs.StartInfo = startInfo;
                    prcs.Start();
                    prcs.WaitForExit(); */
                    string cmd_idx = "-m wslx " + root_path + '/' + "bwa index " + ref_path_converted;
                    Linux_ProcessRunner(cmd_idx);
                    MessageBox.Show("The indexing finished");
                }
            }
        }

        private void alignment_browse_read1_Click(object sender, EventArgs e)
        {
            if(sra_file_browse_for_fq.ShowDialog()== DialogResult.OK)
            {
                alignment_fastq_read1.Text = sra_file_browse_for_fq.FileName;
            }
        }

        private void alignment_browse_read2_Click(object sender, EventArgs e)
        {
            if(sra_file_browse_for_fq.ShowDialog()==DialogResult.OK)
            {
                alignment_fastq_read2.Text = sra_file_browse_for_fq.FileName;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void reference_fasta_TextChanged(object sender, EventArgs e)
        {

        }

        private void data_layout_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (data_layout_combobox.GetItemText(data_layout_combobox.SelectedItem) == "Single-end")
            {
                alignment_fastq_read2.Enabled = false;
                alignment_browse_read2.Enabled = false;
            }
            else
            {
                alignment_fastq_read2.Enabled = true; ;
                alignment_browse_read2.Enabled = true;
            }
        }

        private void browse_dbSNP_Click(object sender, EventArgs e)
        {
            if(sra_file_browse_for_fq.ShowDialog()==DialogResult.OK)
            {
                dbSNP_path.Text = sra_file_browse_for_fq.FileName;
            }
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void downloadAnnovarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form annovar = new Annovar_download();
            annovar.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(working_directory_path.Text))
            {

                if ((comboBox2.SelectedIndex != -1) && (checkedListBox1.Items.Count > 0) && (System.IO.Path.GetExtension(select_vcf_file.Text) == ".vcf"))
                {
                    string annovar_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\annovar" + "\\";
                    string perl_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\perl" + "\\" + "bin" + "\\";
                    string create_DB_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools\\DB_making\\";
                    List<string> list_db = new List<string>();
                    int cnt = checkedListBox1.Items.Count;
                    for (int i = 0; i <= cnt - 1; i++)
                    {
                        if (checkedListBox1.GetItemChecked(i) == true)
                        {
                            list_db.Add(checkedListBox1.Items[i].ToString());
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
                    string genome = comboBox2.GetItemText(comboBox2.SelectedItem);
                    if (genome == "hg19")
                    {
                        set_genome = "hg19";
                    }
                    else if (genome == "hg38")
                    {
                        set_genome = "hg38";
                    }

                    // Write a code to set if genome version is not selected
                    string annovar_cmd = " --wait -m wslx -p ubuntu1804 perl  " + convert_path(annovar_path) + "table_annovar.pl " + convert_path(System.IO.Path.GetFullPath(select_vcf_file.Text)) + " " + convert_path(annovar_path) + "humandb/" + " -buildver " + set_genome + " -out " + convert_path(working_directory_path.Text) + "/" + System.IO.Path.GetFileNameWithoutExtension(select_vcf_file.Text).Split('.')[0] + "_annotated.vcf " + database_add + " " + operations + " -nastring . -vcfinput";
                    Process run_annovar = Linux_ProcessRunner(annovar_cmd);
                    //textBox1.Text = annovar_cmd;
                    string annovar_result_file = "";
                    foreach (string i in System.IO.Directory.GetFiles(working_directory_path.Text))
                    {
                        if (i.ToString().Contains("_multianno.txt"))
                        {
                            annovar_result_file = i.ToString();
                        }
                    }
                    string create_db = "/C " + create_DB_path + "Create_DB.exe " + annovar_result_file + " " + create_DB_path + "NP_and_NM.txt" + " " + create_DB_path + set_genome + "_refGeneversion.txt " + create_DB_path + "homo_sapiens_RS81.fasta " + working_directory_path.Text + "\\" + "Variant_protein_DB.fasta " + working_directory_path.Text + "\\";
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

        private void browse_vcf_file_Click(object sender, EventArgs e)
        {
            if(sra_file_browse_for_fq.ShowDialog()==DialogResult.OK)
            {
                select_vcf_file.Text = sra_file_browse_for_fq.FileName;
            }
        }

        private void annovar_download_label_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form annovar_download = new Annovar_download();
            annovar_download.Visible = true;  
        }

        private void rgid_TextChanged(object sender, EventArgs e)
        {

        }

        private void browse_bed_file_Click(object sender, EventArgs e)
        {
            if(sra_file_browse_for_fq.ShowDialog()==DialogResult.OK)
            {
                bed_file_path.Text = sra_file_browse_for_fq.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
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

        private void Delete_Click(object sender, EventArgs e)
        {
            /* add the link and test it works or not */
        }

        private void Exome_dry_run_set_reference_genome_TextChanged(object sender, EventArgs e)
        {

        }

        private void Exome_dry_run_set_dbsnp_TextChanged(object sender, EventArgs e)
        {

        }

        private void Exome_dry_run_generate_command_Click(object sender, EventArgs e)
        {      
             if (System.IO.Directory.Exists(Exome_dry_run_set_workig_dir.Text))
             {
                     string gatk_algorithm = Exome_dry_run_gatk_select_algorithm.GetItemText(this.Exome_dry_run_gatk_select_algorithm.SelectedItem);
                     if ((!string.IsNullOrEmpty(Exome_dry_run_rgid_txtbx.Text)) && (!string.IsNullOrEmpty(Exome_dry_run_rgpl_txtbx.Text)) && (!string.IsNullOrEmpty(Exome_dry_run_rglb_txtbx.Text)) && (!string.IsNullOrEmpty(Exome_dry_run_rgsm_txtbx.Text)) && (!string.IsNullOrEmpty(Exome_dry_run_rgpu_txtbx.Text)))
                     {
                        //Alignment
                        string cmd_dryrun_index = "";
                        string samtools_faidx = "";
                        string alignment_cmd = "";
                        string cmd_samtobam = "";
                        string cmd_sortbam = "";
                        string cmd_flagstat = "";
                        string cmd_indexfeaturefile = "";
                        string fastq_files = "";
                        string file_for_output = "";
                        string cmd_picard_create_dict = "";
                        string cmd_picard_addorreplace = "";
                        string cmd_markduplicates = "";
                        string cmd_baserecaliberator = "";
                        string cmd_applyBQSR = "";
                        string cmd_variant_calling = "";
                        string command_all = "";
                        string picard_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/picard.jar");
                        //string gatk_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/GenomeAnalysisTK.jar");
                        string gatk_path = "java -jar " + '"' + "-Xmx" + Exome_dry_run_memory.Text + "G" + '"' + " " + convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/gatk-4.1.4.1/gatk-package-4.1.4.1-local.jar ");
                    var chk_bed_file = System.IO.File.Exists(Exome_dry_run_set_bed_file.Text);
                        string bed_fl_path1 = "";
                    if (Exome_dry_run_set_chromosome.Enabled == false)
                    {
                        if (chk_bed_file != false)
                        {
                            bed_fl_path1 = " -L " + convert_path(Exome_dry_run_set_bed_file.Text);
                        }
                        else
                        {
                            bed_fl_path1 = "";
                        }
                    }
                    else
                    {
                        if (Exome_dry_run_set_chromosome.Text.Length >= 1)
                        {
                            bed_fl_path1 = " -L " + Exome_dry_run_set_chromosome.Text;
                        }
                        else
                        {
                            bed_fl_path1 = "";
                        }
                    }
                    System.IO.DirectoryInfo dir_info = new System.IO.DirectoryInfo(Exome_dry_run_select_fq_directory.Text);
                        System.IO.FileInfo[] file_list = dir_info.GetFiles("*.fastq");
                        foreach (System.IO.FileInfo files in file_list)
                        {
                            fastq_files = fastq_files + convert_path(dir_info.ToString()) + "/" + files + " ";
                            file_for_output = files.ToString().Split('.')[0];
                        }
                        cmd_dryrun_index = "bwa index " + convert_path(Exome_dry_run_set_reference_genome.Text);
                        samtools_faidx = "samtools faidx " + convert_path(Exome_dry_run_set_reference_genome.Text);
                        alignment_cmd = "bwa mem " + "-t " + Exome_dry_run_threads.Text + " " + convert_path(Exome_dry_run_set_reference_genome.Text) + " " + fastq_files + "> " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam";

                        
                        cmd_samtobam = "samtools view -b -S " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam > " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.bam -@ " + Exome_dry_run_threads.Text;
                        cmd_sortbam = "samtools sort -@ " + Exome_dry_run_threads.Text + " -o " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.bam";
                        cmd_flagstat = "samtools flagstat -@ " + Exome_dry_run_threads.Text + " " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam > " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/"  + "Alignment_summary.txt";
                        cmd_indexfeaturefile = gatk_path + "IndexFeatureFile -I " + convert_path(Exome_dry_run_set_dbsnp.Text);
                        cmd_picard_create_dict = gatk_path + " CreateSequenceDictionary " + "-R=" + convert_path(Exome_dry_run_set_reference_genome.Text) + " -O=" + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + System.IO.Path.GetFileNameWithoutExtension(Exome_dry_run_set_reference_genome.Text) + ".dict";
                        cmd_picard_addorreplace = gatk_path + " AddOrReplaceReadGroups -I=" + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + "-O=" + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-ID=" + Exome_dry_run_rgid_txtbx.Text + " " + "-PU=" + Exome_dry_run_rgpu_txtbx.Text + " " + "-SM=" + Exome_dry_run_rgsm_txtbx.Text + " " + "-PL=" + Exome_dry_run_rgpl_txtbx.Text + " " + "-LB=" + Exome_dry_run_rglb_txtbx.Text;

                        cmd_markduplicates = gatk_path + " MarkDuplicates -I=" + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-O=" + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true -M=" + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + "demo.nodup";

                        cmd_baserecaliberator = gatk_path + "BaseRecalibrator" + " -R " + convert_path(Exome_dry_run_set_reference_genome.Text) + " --known-sites " + convert_path(Exome_dry_run_set_dbsnp.Text) + " -I " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam " + "-O " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + "recal_data.table";

                        cmd_applyBQSR = gatk_path + "ApplyBQSR" + " -R " + convert_path(Exome_dry_run_set_reference_genome.Text) + " -I " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam -O " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + "outputApplyBQSR.bam" + " --bqsr-recal-file " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + "recal_data.table";

                        cmd_variant_calling = gatk_path + "HaplotypeCaller" + " -I " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/outputApplyBQSR.bam " + "-R " + convert_path(Exome_dry_run_set_reference_genome.Text) + " --dbsnp " + convert_path(Exome_dry_run_set_dbsnp.Text) + " -O " + convert_path(Exome_dry_run_set_workig_dir.Text) + "/" + "Variant_calling_output" + ".vcf" + bed_fl_path1;


                    //textBox7.Text = cmd_printreads;

                        string[] commands = new string[14];
                        commands[0] = cmd_dryrun_index;
                        commands[1] = samtools_faidx;
                        commands[2] = alignment_cmd;
                        commands[3] = cmd_samtobam;
                        commands[4] = cmd_sortbam;
                        commands[5] = cmd_flagstat;
                        commands[5] = cmd_indexfeaturefile;
                        commands[6] = cmd_picard_create_dict;
                        commands[7] = cmd_picard_addorreplace;
                        commands[8] = cmd_markduplicates;
                        commands[9] = cmd_baserecaliberator;
                        commands[11] = cmd_applyBQSR;
                        commands[13] = cmd_variant_calling;

                        //StreamWriter OurStream;
                        //OurStream = System.IO.File.CreateText(Exome_dry_run_set_workig_dir.Text + "/" + "dry_run.sh");

                        foreach (string p in commands)
                        {
                            //Process prcs = ProcessRunner(p);
                            //textBox7.Text T= textBox1.Text + prcs.StandardOutput.ReadToEnd().ToString() + Environment.NewLine;
                            
                            command_all = command_all + p + '\n';
                             //OurStream.WriteLine(p.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("'$", "").Replace("'$'\r'", '');
                        }
                        Exome_dry_run_open_directory.Enabled = true;
                        
                        System.IO.File.WriteAllText(Exome_dry_run_set_workig_dir.Text + "/" + "dry_run.sh", command_all);
                    }
            }
        }

        private void Exome_dry_run_browse_working_dir_Click(object sender, EventArgs e)
        {
            if (Dry_run_dir_browse.ShowDialog() == DialogResult.OK)
            {
                Exome_dry_run_set_workig_dir.Text = Dry_run_dir_browse.SelectedPath;
            }
        }

        private void Exome_dry_run_browse_dbsnp_Click(object sender, EventArgs e)
        {
            if (Dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Exome_dry_run_set_dbsnp.Text = Dry_run_file_browse.FileName;
            }
        }

        private void Exome_dry_run_browse_reference_genome_Click(object sender, EventArgs e)
        {
            if (Dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Exome_dry_run_set_reference_genome.Text = Dry_run_file_browse.FileName;
            }
        }

        private void Exome_dry_run_browse_bed_file_Click(object sender, EventArgs e)
        {
            if (Dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Exome_dry_run_set_bed_file.Text = Dry_run_file_browse.FileName;
            }
        }

        private void Exome_annovar_dir_TextChanged(object sender, EventArgs e)
        {

        }

        private void Exome_dry_run_browse_annovar_directory_Click(object sender, EventArgs e)
        {
            
        }

        private void Exome_dry_run_browse_fq_dirs_Click(object sender, EventArgs e)
        {
            if (Dry_run_dir_browse.ShowDialog() == DialogResult.OK)
            {
                Exome_dry_run_select_fq_directory.Text = Dry_run_dir_browse.SelectedPath;
            }
        }

        private void Exome_dry_run_open_directory_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Exome_dry_run_set_workig_dir.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(fastq_file_path_qc.Text.Split('.')[0] + "_fastqc.html");
            if (System.IO.File.Exists(fastq_file_path_qc.Text.Replace('.', '_') + 'c' + "\\" + "summary.txt"))
            {
                System.Diagnostics.Process.Start(fastq_file_path_qc.Text.Split('.')[0] + "_fastqc.html");
            }
            else
            {
                MessageBox.Show("Please run the QC to get the summary");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
        }
        int flag = 0;
        private void Enable_chromosome_field_Click(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                bed_file_path.Text = "Set the BED interval file path";
                bed_file_path.Enabled = true;
                browse_bed_file.Enabled = true;
                Set_chromosome_field.Enabled = false;
                flag = 0;
            }
            else
            {
                bed_file_path.Text = "";
                Set_chromosome_field.Enabled = true;
                browse_bed_file.Enabled = false;
                bed_file_path.Enabled = false;
                Set_chromosome_field.Text = "Enter the chromosome (Empty will consider all the chrosomes)";
                flag = 1;
            }
    }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string genome = comboBox2.GetItemText(comboBox2.SelectedItem);
            checkedListBox1.Items.Clear();
            if (genome == "hg19")
            {
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
            else
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form abt = new About();
            abt.Show();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            Form1 fm1 = new Form1();
            fm1.Show();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {

        }
        int flag1 = 0;
        private void Exome_dry_run_enable_chromosome_field_Click(object sender, EventArgs e)
        {
            

            if (flag1 == 1)
            {
                Exome_dry_run_set_bed_file.Text = "Set the BED interval file path";
                Exome_dry_run_set_bed_file.Enabled = true;
                Exome_dry_run_browse_bed_file.Enabled = true;
                Exome_dry_run_set_chromosome.Enabled = false;
                flag1 = 0;
            }
            else
            {
                Exome_dry_run_set_bed_file.Text = "";
                Exome_dry_run_set_chromosome.Enabled = true;
                Exome_dry_run_set_bed_file.Enabled = false;
                Exome_dry_run_browse_bed_file.Enabled = false;
                Exome_dry_run_set_chromosome.Text = "Enter the chromosome (Empty will consider all the chrosomes)";
                flag1 = 1;
            }
        }
    }
}
