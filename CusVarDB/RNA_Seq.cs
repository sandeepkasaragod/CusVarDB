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
    public partial class RNA_Seq : Form
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
        public string terminal_path = "";
        public string hisat2_index = "";
        
        public RNA_Seq()
        {
            InitializeComponent();
            terminal_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "terminal.exe";
            processor_count = Environment.ProcessorCount;
            Rna_chromosome_field.Enabled = false;
            Rna_dry_run_set_bed_file.Enabled = false;
            Rna_dry_run_browse_bed_file.Enabled = false;

            if (processor_count <= 2)
            {
                MessageBox.Show("The program may not run faster due to the limited threads!!!!!!!!!");
                processor_count = 1;
            }
            else
            {
                processor_count = processor_count - 2;
            }
            Rna_set_threads.Text = processor_count.ToString();
            Rna_dry_run_threads.Text = processor_count.ToString();
            var totalRAM = (Convert.ToInt32((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory) / (Math.Pow(1024, 3)) + 0.5) / 2);
            {

                Rna_dry_run_memory.Text = totalRAM.ToString();
                Rna_set_memory.Text = totalRAM.ToString();
            }
            string get_curr_dir = System.IO.Directory.GetCurrentDirectory();
        }

        public string download_SRA(string sequence_read_archive_id, string id)
        {
            string sra_id = sequence_read_archive_id.Trim();
            string url = "https://ftp.ncbi.nlm.nih.gov/sra/sra-instant/reads/ByRun/sra/" + id + "/" + sra_id.Substring(0, 6) + "/" + sra_id + "/" + sra_id + ".sra";
            string sra_file_download = " --wait -m wslx -p ubuntu1804 wget -c " + url + " -P " + convert_path(Rna_working_directory_path.Text);
            Process run_sra_download = Linux_ProcessRunner(sra_file_download);
            /* Use process.ExitCode and based on the code pass the message in return */
            //Rna_sra_file_browse.Text = run_sra_download.ExitCode.ToString();
            return run_sra_download.ExitCode.ToString();
        }

        public Process ProcessRunner(string cmd)
        {
            System.Diagnostics.Process prcs = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd;
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

        private void Rna_browse_reference_fasta_file_Click(object sender, EventArgs e)
        {
           
            OpenFileDialog ofd_reference_fasta = new OpenFileDialog();
            ofd_reference_fasta.Filter = "Fasta file (*.fasta)|*.fasta|Fasta file (*.fa)|*.fa|Fasta file (*.fna)|*.fna";
            if (ofd_reference_fasta.ShowDialog() == DialogResult.OK)
            {
                Rna_reference_fasta.Text = ofd_reference_fasta.FileName;
                hisat2_index = convert_path(System.IO.Path.GetDirectoryName(Rna_reference_fasta.Text)) + "/" + System.IO.Path.GetFileNameWithoutExtension(Rna_reference_fasta.Text);    
                
                string get_filename = System.IO.Path.GetDirectoryName(Rna_reference_fasta.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Rna_reference_fasta.Text); //reference file name with extension

                //Browse for reference file
                ref_path_converted = convert_path(Rna_reference_fasta.Text);

                if (System.IO.File.Exists(get_filename + ".1.ht2") && System.IO.File.Exists(get_filename + ".2.ht2") && System.IO.File.Exists(get_filename + ".3.ht2") && System.IO.File.Exists(get_filename + ".4.ht2") && System.IO.File.Exists(get_filename + ".5.ht2") && System.IO.File.Exists(get_filename + ".6.ht2") && System.IO.File.Exists(get_filename + ".7.ht2") && System.IO.File.Exists(get_filename + ".8.ht2"))
                {
                    MessageBox.Show("All the files exist");

                }
                else
                {
                    //Converting the root directory path to linux path
                    root_path = convert_path(System.IO.Directory.GetCurrentDirectory());
                    string cmd_idx = " --wait -m wslx -p ubuntu1804 " + " hisat2-build " + ref_path_converted + " " + hisat2_index + " -p " + Rna_set_threads.Text;
                    //Console.WriteLine(get_filename + ".1.ht2");
                    Process bwa_cmd = Linux_ProcessRunner(cmd_idx);
                } 
            }
        }

        private void Rna_browse_dbSNP_Click(object sender, EventArgs e)
        {
            if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Rna_dbSNP_path.Text = Rna_sra_file_browse_for_fq.FileName;
            }
        }

        private void Rna_browse_bed_file_Click(object sender, EventArgs e)
        {
            if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Rna_bed_file_path.Text = Rna_sra_file_browse_for_fq.FileName;
            }
        }

        private void Rna_browse_reference_file_Click(object sender, EventArgs e)
        {
            if (Rna_working_directory.ShowDialog() == DialogResult.OK)
            {
                Rna_working_directory_path.Text = Rna_working_directory.SelectedPath;
            }
        }


        private void Rna_sra_download_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Rna_working_directory_path.Text))
            { 
            if (Rna_sra_id_download.Text.Contains("SRR"))
            {
                /*Reduce the code to function and pass SRR, ERR and DRR to the function*/
                string sra_fl = download_SRA(Rna_sra_id_download.Text, "SRR");
            }
            else if (Rna_sra_id_download.Text.Contains("ERR"))
            {
                string sra_fl = download_SRA(Rna_sra_id_download.Text, "ERR");

            }
            else if (Rna_sra_id_download.Text.Contains("DRR"))
            {
                string sra_fl = download_SRA(Rna_sra_id_download.Text, "DRR");
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

        private void Rna_browse_sra_file_Click(object sender, EventArgs e)
        {
            if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {

                string extension = System.IO.Path.GetExtension(Rna_sra_file_browse_for_fq.FileName);
                if (extension != ".sra")
                {
                    MessageBox.Show("Please select the FASTQ file");
                }
                else
                {
                    Rna_sra_file_browse.Text = Rna_sra_file_browse_for_fq.FileName;
                }
            }
        }

        private void Rna_sra_convert_Click(object sender, EventArgs e)
        {
            string get_data_layout = Rna_comboBox.GetItemText(Rna_comboBox.SelectedItem);
            if (System.IO.Directory.Exists(Rna_working_directory_path.Text))
            {
                if (get_data_layout.Length != 0)
                {
                    //MessageBox.Show(get_data_layout + ", " + gatk_algorithm);
                    if (get_data_layout == "Single-end")
                    {
                        //Single-end command
                        string sra_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools" + "\\";
                        string get_srr_ids = System.IO.Path.GetFullPath(Rna_sra_file_browse.Text);
                        string fastq_dump_cmds = " -m wslx --wait -p ubuntu1804 " + "fastq-dump " + convert_path(get_srr_ids.Trim()) + " -O " + convert_path(Rna_working_directory_path.Text) + "/";
                        Process run_fastq_dumps = Linux_ProcessRunner(fastq_dump_cmds);
                        //Rna_sra_file_browse.Text = fastq_dump_cmds;
                    }
                    else if (get_data_layout == "Paired-end")
                    {
                        //Paired-end command
                        try
                        {
                            string sra_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools" + "\\";
                            string get_srr_ids = System.IO.Path.GetFullPath(Rna_sra_file_browse.Text);
                            string fastq_dump_cmds = " -m wslx --wait -p ubuntu1804 " + "fastq-dump --split-files " + convert_path(get_srr_ids.Trim()) + " -O " + convert_path(Rna_working_directory_path.Text) + "/";
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

        private void Rna_browse_fastq_file_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Rna_working_directory_path.Text))
            {
                if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
                {
                    Rna_fastq_file_path_qc.Text = Rna_sra_file_browse_for_fq.FileName;
                }
            }
            else
            {
                MessageBox.Show("Please set the working directory");
            }
        }

        private void Rna_run_fastx_Click(object sender, EventArgs e)
        {
            /*The process involves converting the fastq to fastq.zip (summary) and extracting the files and read the summary table, show the quality output in textbox*/
            if (System.IO.File.Exists(Rna_fastq_file_path_qc.Text))
            {
                /* conver the fastq file path to Linux path */
                string curr_dir = System.IO.Directory.GetCurrentDirectory() + "/" + "tools/FastQC/";
                string replace_slash = curr_dir.Substring(2).Replace("\\", "/");
                string get_drive_letter = System.IO.Directory.GetCurrentDirectory().Substring(0, 1).ToLower();
                string fastqc_path = "/mnt/" + get_drive_letter + replace_slash;

                /* conver the input sra file path to Linux path */
                string input_file_conveted_path = convert_path(Rna_fastq_file_path_qc.Text);

                /*Conver the working directory path to linux*/
                input_working_dir_converted_path = convert_path(Rna_working_directory_path.Text);

                /* Working code do not delete ; it will run the fastq command */
                string fastqc_cmd = " -m wslx -p ubuntu1804 " + fastqc_path + "fastqc " + input_file_conveted_path;
                Process run_fastqc_cmd = Linux_ProcessRunner(fastqc_cmd);

                /* Extract the FastQc zip files */
                string get_dirs = System.IO.Path.GetDirectoryName(Rna_fastq_file_path_qc.Text);
                string get_file_name = System.IO.Path.GetFileName(Rna_fastq_file_path_qc.Text);
                System.Diagnostics.Process prcs1 = new System.Diagnostics.Process();

                /*Creating working directory*/
                string wk_dir = System.IO.Path.GetFullPath(Rna_working_directory_path.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Rna_fastq_file_path_qc.Text) + "_fastqc";
                System.IO.Directory.CreateDirectory(wk_dir);
                //MessageBox.Show(fastq_file_path_qc.Text.Replace('.', '_') + 'c');
                String unzip_cmd = " --wait -m wslx -p ubuntu1804 unzip " + input_file_conveted_path.Replace('.', '_') + 'c' + ".zip" + " -d " + input_working_dir_converted_path + "/";

                Process run_unzip_cmd = Linux_ProcessRunner(unzip_cmd);

                /*Read the FastQC summary file to provide the statistics*/
                
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    using (System.IO.TextReader tr1 = System.IO.File.OpenText(Rna_fastq_file_path_qc.Text.Replace('.', '_') + 'c' + "\\" + "summary.txt"))
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
                        Rna_qc_report.Text = Rna_qc_report.Text + keypair.Key + '\t' + keypair.Value + Environment.NewLine;
                    }
                    Rna_pictureBox.Load(wk_dir + "\\Images\\per_base_quality.png");
                    Rna_pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    /* The files are extracted from fq.zip file */
                    //Exon_pictureBox.Image = Image.FromFile(working_directory_path.Text + "\\" + wk_dir + "\\Images\\per_base_quality.png");


            }
            else
            {
                MessageBox.Show("Please load the fastq file");
            }
            
        }

        private void Rna_summary_fastqc_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Rna_fastq_file_path_qc.Text.Replace('.', '_') + 'c' + "\\" + "summary.txt"))
            {
                System.Diagnostics.Process.Start(Rna_fastq_file_path_qc.Text.Split('.')[0] + "_fastqc.html");

            }
            else
            {
             MessageBox.Show("Please run the QC to get the summary");
            }
                
        }

        private void Rna_alignment_browse_read1_Click(object sender, EventArgs e)
        {
            if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Rna_alignment_fastq_read1.Text = Rna_sra_file_browse_for_fq.FileName;
            }
        }

        private void Rna_alignment_browse_read2_Click(object sender, EventArgs e)
        {
            if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Rna_alignment_fastq_read2.Text = Rna_sra_file_browse_for_fq.FileName;
            }
        }

        private void Rna_alignment_Click(object sender, EventArgs e)
        {
            string cmd_alignment = "";
            string cmd_samtool_faidx = "";
            string cmd_samtobam = "";
            string cmd_sort = "";
            string cmd_flagstat = "";
            string cmd_markduplicates = "";
            string cmd_createdictfile = "";
            string cmd_addorreplacereadgroup = "";
            string cmd_splitNCigarReads = "";
            string cmd_dbsnpidx = "";
            string cmd_baserecaliberator = "";
            string cmd_applyBQSR = "";
            string cmd_haplotypecaller = "";

            string add_pause = "";
            if (Genome_pause_each_command.Checked)
            {
                add_pause = "--wait ";
            }

            /*Convert the windows path to linux path [Reference fasta path]*/
            string ref_path = Rna_reference_fasta.Text.Substring(2).Replace("\\", "/");
            string ref_path_drv_ltr = Rna_reference_fasta.Text.Substring(0, 1).ToLower();
            string ref_path_converted = "/mnt/" + ref_path_drv_ltr + ref_path;

            /*Convert working directory path to linux */
            string alignment_working_dir_converted = convert_path(Rna_working_directory_path.Text);

            /* Convert Read1 path to linux path */
            read1_working_dir = convert_path(Rna_alignment_fastq_read1.Text);

            string get_data_layout = Rna_data_layout_combobox.GetItemText(Rna_data_layout_combobox.SelectedItem);
            string gatk_algorithm = Rna_gatk_combobox.GetItemText(Rna_gatk_combobox.SelectedItem);

            string output_file_name = System.IO.Path.GetFileNameWithoutExtension(Rna_alignment_fastq_read1.Text); // output file name

            var chk_bed_file = System.IO.File.Exists(Rna_bed_file_path.Text);
            string bed_fl_path = "";
            if (Rna_chromosome_field.Enabled == false)
            {
                if (chk_bed_file != false)
                {
                    bed_fl_path = " -L " + convert_path(Rna_bed_file_path.Text);
                }
                else
                {
                    bed_fl_path = "";
                }
            }
            else
            {
                if (Rna_chromosome_field.Text.Length >= 1)
                {
                    bed_fl_path = " -L " + Rna_chromosome_field.Text;
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
                    Rna_alignment_browse_read2.Enabled = false;
                    cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "hisat2 -x " + hisat2_index + " -1 " + read1_working_dir + " -S "+ alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam" + " -p " + Rna_set_threads.Text + " --summary-file " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_summary.txt";
                    //cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "hisat2 -x " + "-t " + Rna_set_threads.Text + " " + ref_path_converted + " " + read1_working_dir + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam";
                    //cmd_alignment = "/C terminal.py --wait -m wslx ls "+ "touch " + alignment_working_dir_converted + "/text.txt";
                }
                else
                {
                    //Convert read2 windows path to linux
                    string read2_working_dir = convert_path(Rna_alignment_fastq_read2.Text);

                    //Paired-end
                    //cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "bwa mem " + "-t " + Rna_set_threads.Text + " " + ref_path_converted + " " + read1_working_dir + " " + read2_working_dir + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam";
                    cmd_alignment = " -m wslx -p ubuntu1804 " + add_pause + "hisat2 -x " + hisat2_index + " -1 " + read1_working_dir + " -2 " + read2_working_dir +  " -S " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam" + " -p " + Rna_set_threads.Text + " --summary-file " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_summary.txt";
                }


                cmd_samtool_faidx = " -m wslx -p ubuntu1804 " + add_pause + "samtools faidx " + ref_path_converted;
                cmd_samtobam = " -m wslx -p ubuntu1804 " + add_pause + "samtools view -b -S " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.sam" + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.bam " + "-@ " + Rna_set_threads.Text;
                cmd_sort = " -m wslx -p ubuntu1804 " + add_pause + "samtools sort " + "-@ " + Rna_set_threads.Text + " -o " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam " + alignment_working_dir_converted + "/" + output_file_name + "_alignment.bam";
                cmd_flagstat = " -m wslx -p ubuntu1804 " + add_pause + "samtools flagstat " + "-@ " + Rna_set_threads.Text + " " + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam" + " > " + alignment_working_dir_converted + "Alignment_summary.txt" ;

                string picard_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/picard.jar");
                string gatk_path = "java -jar " + '"' + "-Xmx" + Rna_set_memory.Text + "G" + '"' + " " + convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/gatk-4.1.4.1/gatk-package-4.1.4.1-local.jar ");
                dbSNP = convert_path(Rna_dbSNP_path.Text);
                string bed_file = convert_path(Rna_bed_file_path.Text);
                string get_reference_file_name = System.IO.Path.GetFileNameWithoutExtension(Rna_reference_fasta.Text);
                if (System.IO.File.Exists(Rna_reference_fasta.Text.Split('.')[0] + ".dict"))
                {
                    cmd_createdictfile = " -m wslx -p ubuntu1804 exit";
                }
                else
                {
                    cmd_createdictfile = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "CreateSequenceDictionary " + "-R=" + ref_path_converted + " -O=" + alignment_working_dir_converted + "/" + System.IO.Path.GetFileNameWithoutExtension(Rna_reference_fasta.Text) + ".dict";
                }

                cmd_addorreplacereadgroup = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "AddOrReplaceReadGroups -I=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted.bam " + "-O=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted_rg.bam " + "-ID=" + Rna_rgid.Text + " " + "-PU=" + Rna_rgpu.Text + " " + "-SM=" + Rna_rgsm.Text + " " + "-PL=" + Rna_rgpl.Text + " " + "-LB=" + Rna_rglb.Text;

                cmd_markduplicates = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "MarkDuplicates -I=" + alignment_working_dir_converted + "/" + output_file_name + "_alignment_sorted_rg.bam " + "-O=" + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true --M=" + alignment_working_dir_converted + "/" + "demo.nodup";

                /*Edit the below splice N cigar code */
                cmd_splitNCigarReads = " -m wslx -p ubuntu1804 " + add_pause + gatk_path  + " SplitNCigarReads" + " -R " + ref_path_converted + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam -O " + alignment_working_dir_converted + "/" + "split.bam -RF AllowAllReadsReadFilter";


                //IndedFeature file here//
                if (!System.IO.File.Exists(Rna_dbSNP_path.Text + "\\" + ".idx"))
                {
                    cmd_dbsnpidx = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + "IndexFeatureFile -I " + convert_path(Rna_dbSNP_path.Text);
                }
                else
                {
                    cmd_dbsnpidx = " -m wslx -p ubuntu1804 exit";
                }
                cmd_baserecaliberator = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " BaseRecalibrator" + " -R " + ref_path_converted + " --known-sites " + dbSNP + " -I " + alignment_working_dir_converted + "/" + "split.bam" + " -O " + alignment_working_dir_converted + "/" + "recal_data.table";

                cmd_applyBQSR = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " ApplyBQSR" + " -R " + ref_path_converted + " -I " + alignment_working_dir_converted + "/" + output_file_name + "_sorted_nodup.bam " + "--bqsr-recal-file " + alignment_working_dir_converted + "/" + "recal_data.table" + " -O " + alignment_working_dir_converted + "/" + "outputApplyBQSR.bam";

                cmd_haplotypecaller = " -m wslx -p ubuntu1804 " + add_pause + gatk_path + " HaplotypeCaller " + "-I " + alignment_working_dir_converted + "/outputApplyBQSR.bam " + "-R " + ref_path_converted + " --dbsnp " + dbSNP + " -O " + alignment_working_dir_converted + "/" + "Variant_calling_output" + ".vcf" + bed_fl_path;

                string[] commands = new string[13];
                commands[0] = cmd_alignment;
                commands[1] = cmd_samtool_faidx;
                commands[2] = cmd_samtobam;
                commands[3] = cmd_sort;
                commands[4] = cmd_flagstat;
                commands[5] = cmd_addorreplacereadgroup;
                commands[6] = cmd_createdictfile;
                commands[7] = cmd_markduplicates;
                commands[8] = cmd_splitNCigarReads;
                commands[9] = cmd_dbsnpidx;
                commands[10]= cmd_baserecaliberator;
                commands[11] = cmd_applyBQSR;
                commands[12] = cmd_haplotypecaller;

                foreach (string p in commands)
                {

                    Process prcs = Linux_ProcessRunner(p);
                    textBox1.Text = textBox1.Text + p + Environment.NewLine;
                    //textBox1.Text = textBox1.Text + prcs.StandardOutput.ReadToEnd().ToString() + Environment.NewLine;
                }
            }
            else
            {
                MessageBox.Show("Data layout or Gatk algorithm has to be selected");
            }
        }

        private void Rna_dry_run_browse_working_dir_Click(object sender, EventArgs e)
        {
            if (Rna_dry_run_dir_browse.ShowDialog() == DialogResult.OK)
            {    
                Rna_dry_run_set_workig_dir.Text = Rna_dry_run_dir_browse.SelectedPath;
            }
        }

        private void Rna_dry_run_browse_dbsnp_Click(object sender, EventArgs e)
        {
            if (Rna_dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Rna_dry_run_set_dbsnp.Text = Rna_dry_run_file_browse.FileName;
            }
        }

        private void Rna_dry_run_browse_reference_genome_Click(object sender, EventArgs e)
        {
            if (Rna_dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Rna_dry_run_set_reference_genome.Text = Rna_dry_run_file_browse.FileName;
            }
        }

        private void Rna_dry_run_browse_bed_file_Click(object sender, EventArgs e)
        {
            if (Rna_dry_run_file_browse.ShowDialog() == DialogResult.OK)
            {
                Rna_dry_run_set_bed_file.Text = Rna_dry_run_file_browse.FileName;
            }
        }

        private void Rna_dry_run_browse_fq_dirs_Click(object sender, EventArgs e)
        {
            if (Rna_dry_run_dir_browse.ShowDialog() == DialogResult.OK)
            {
                Rna_dry_run_select_fq_directory.Text = Rna_dry_run_dir_browse.SelectedPath;
            }
        }

        private void Rna_dry_run_generate_command_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Rna_dry_run_set_workig_dir.Text))
            {
                string gatk_algorithm = Rna_dry_run_gatk_select_algorithm.GetItemText(this.Rna_dry_run_gatk_select_algorithm.SelectedItem);
                if ((!string.IsNullOrEmpty(Rna_dry_run_rgid_txtbx.Text)) && (!string.IsNullOrEmpty(Rna_dry_run_rgpl_txtbx.Text)) && (!string.IsNullOrEmpty(Rna_dry_run_rglb_txtbx.Text)) && (!string.IsNullOrEmpty(Rna_dry_run_rgsm_txtbx.Text)) && (!string.IsNullOrEmpty(Rna_dry_run_rgpu_txtbx.Text)))
                {
                    //Alignment
                    string cmd_dryrun_index = "";
                    string samtools_faidx = "";
                    string alignment_cmd = "";
                    string cmd_samtobam = "";
                    string cmd_sortbam = "";
                    string fastq_files = "";
                    string file_for_output = "";
                    string cmd_picard_create_dict = "";
                    string cmd_picard_addorreplace = "";
                    string cmd_splitNCigarReads = "";
                    string cmd_markduplicates = "";
                    string cmd_dbsnpidx = "";
                    string cmd_indelrealigner = "";
                    string cmd_baserecaliberator = "";
                    string cmd_applyBQSR = "";
                    string cmd_variant_calling = "";
                    string command_all = "";
                    string picard_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/picard.jar");
                    //string gatk_path = convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/");
                    string gatk_path = "java -jar " + '"' + "-Xmx" + Rna_set_memory.Text + "G" + '"' + " " + convert_path(System.IO.Directory.GetCurrentDirectory() + "/" + "tools/gatk/gatk-4.1.4.1/gatk-package-4.1.4.1-local.jar ");

                    System.IO.DirectoryInfo dir_info = new System.IO.DirectoryInfo(Rna_dry_run_select_fq_directory.Text);
                    System.IO.FileInfo[] file_list = dir_info.GetFiles("*.fastq");
                    foreach (System.IO.FileInfo files in file_list)
                    {
                        fastq_files = fastq_files + convert_path(dir_info.ToString()) + "/" + files + " ";
                        file_for_output = files.ToString().Split('.')[0];
                    }
                    
                    string hisat2_idx_path = convert_path(System.IO.Path.GetDirectoryName(Rna_dry_run_set_reference_genome.Text)) + "/" + System.IO.Path.GetFileNameWithoutExtension(Rna_dry_run_set_reference_genome.Text);
                    cmd_dryrun_index = "hisat2-build " + convert_path(Rna_dry_run_set_reference_genome.Text) + " " + hisat2_idx_path + " -p " + Rna_dry_run_threads.Text;
                    samtools_faidx = "samtools faidx " + convert_path(Rna_dry_run_set_reference_genome.Text);
                    //alignment_cmd = "bwa mem " + "-t " + Rna_dry_run_threads.Text + " " + convert_path(Rna_dry_run_set_reference_genome.Text) + " " + fastq_files + "> " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam";

                    if (fastq_files.Trim().Split(' ').Length > 1)
                    { 
                    alignment_cmd = "hisat2 -x " + hisat2_idx_path + " -1 " + fastq_files.Split(' ')[0] + " -2 " + fastq_files.Split(' ')[1] + " -S " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam" + " -p " + Rna_dry_run_threads.Text + " --summary-file " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment_summary.txt";
                    }
                    else
                    {
                        alignment_cmd = "hisat2 -x " + hisat2_idx_path + " -1 " + fastq_files.Split(' ')[0] + " -S " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam" + " -p " + Rna_dry_run_threads.Text + " --summary-file " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment_summary.txt";
                    }
                    cmd_samtobam = "samtools view -b -S " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.sam > " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.bam -@ " + Rna_dry_run_threads.Text;
                    cmd_sortbam = "samtools sort -@ " + Rna_dry_run_threads.Text + " -o " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_alignment.bam";

                    cmd_picard_create_dict = gatk_path + " CreateSequenceDictionary " + "-R=" + convert_path(Rna_dry_run_set_reference_genome.Text) + " -O=" + hisat2_idx_path + ".dict";
                    cmd_picard_addorreplace = gatk_path + " AddOrReplaceReadGroups -I=" + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment.bam " + "-O=" + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-ID=" + Rna_dry_run_rgid_txtbx.Text + " " + "-PU=" + Rna_dry_run_rgpu_txtbx.Text + " " + "-SM=" + Rna_dry_run_rgsm_txtbx.Text + " " + "-PL=" + Rna_dry_run_rgpl_txtbx.Text + " " + "-LB=" + Rna_dry_run_rglb_txtbx.Text;
                    cmd_markduplicates = gatk_path + " MarkDuplicates -I=" + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_alignment_rg.bam " + "-O=" + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam " + "--VALIDATION_STRINGENCY=SILENT --ASSUME_SORTED=true --CREATE_INDEX=true --M=" + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "demo.nodup";
                    cmd_dbsnpidx = gatk_path + "IndexFeatureFile -I " + convert_path(Rna_dry_run_set_dbsnp.Text);
                    cmd_splitNCigarReads = gatk_path + " SplitNCigarReads" + " -R " + convert_path(Rna_dry_run_set_reference_genome.Text) + " -I " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam -O " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "split.bam";
                    cmd_baserecaliberator = gatk_path + " BaseRecalibrator" + " -R " + convert_path(Rna_dry_run_set_reference_genome.Text) + " --known-sites " + convert_path(Rna_dry_run_set_dbsnp.Text) + " -I " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "split.bam" + " -O " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "recal_data.table";
                    cmd_applyBQSR = gatk_path + " ApplyBQSR" + " -R " + convert_path(Rna_dry_run_set_reference_genome.Text) + " -I " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + file_for_output + "_sorted_nodup.bam" + " --bqsr-recal-file " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "recal_data.table" + " -O " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "outputApplyBQSR.bam";
                    cmd_variant_calling = gatk_path + "HaplotypeCaller " + "-I " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/outputApplyBQSR.bam " + "-R " + convert_path(Rna_dry_run_set_reference_genome.Text) + " --dbsnp " + convert_path(Rna_dry_run_set_dbsnp.Text) + " -O " + convert_path(Rna_dry_run_set_workig_dir.Text) + "/" + "Variant_calling_output" + ".vcf";

                    //textBox7.Text = cmd_printreads;

                    string[] commands = new string[14];
                    commands[0] = cmd_dryrun_index;
                    commands[1] = samtools_faidx;
                    commands[2] = alignment_cmd;
                    commands[3] = cmd_samtobam;
                    commands[4] = cmd_sortbam;
                    commands[5] = cmd_picard_create_dict;
                    commands[6] = cmd_picard_addorreplace;
                    commands[7] = cmd_markduplicates;
                    commands[8] = cmd_dbsnpidx;
                    commands[9] = cmd_splitNCigarReads;
                    commands[10] = cmd_indelrealigner;
                    commands[11] = cmd_baserecaliberator;
                    commands[12] = cmd_applyBQSR;
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
                    Rna_dry_run_open_directory.Enabled = true;

                    System.IO.File.WriteAllText(Rna_dry_run_set_workig_dir.Text + "/" + "dry_run.sh", command_all);
                }
            }
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void Genome_annovar_annotate_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(Rna_working_directory_path.Text))
            {
                if ((Rna_sel_genome_version.SelectedIndex != -1) && (Rna_sel_db.Items.Count > 0) && (System.IO.Path.GetExtension(Rna_select_vcf_file.Text) == ".vcf"))
                {
                    string annovar_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\annovar" + "\\";
                    string perl_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + "tools\\perl" + "\\" + "bin" + "\\";
                    string create_DB_path = System.IO.Directory.GetCurrentDirectory() + "\\" + "tools\\DB_making\\";
                    List<string> list_db = new List<string>();
                    int cnt = Rna_sel_db.Items.Count;
                    for (int i = 0; i <= cnt - 1; i++)
                    {
                        if (Rna_sel_db.GetItemChecked(i) == true)
                        {
                            list_db.Add(Rna_sel_db.Items[i].ToString());
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
                    string genome = Rna_sel_genome_version.GetItemText(Rna_sel_genome_version.SelectedItem);
                    if (genome == "hg19")
                    {
                        set_genome = "hg19";
                    }
                    else if (genome == "hg38")
                    {
                        set_genome = "hg38";
                    }
                    // Write a code to set if genome version is not selected
                    string annovar_cmd = " --wait -m wslx -p ubuntu1804 perl " + convert_path(annovar_path) + "table_annovar.pl " + convert_path(System.IO.Path.GetFullPath(Rna_select_vcf_file.Text)) + " " + convert_path(annovar_path) + "humandb/" + " -buildver " + set_genome + " -out " + convert_path(Rna_working_directory_path.Text) + "/" + System.IO.Path.GetFileNameWithoutExtension(Rna_select_vcf_file.Text).Split('.')[0] + "_annotated.vcf " + database_add + " " + operations + " -nastring . -vcfinput";
                    Process run_annovar = Linux_ProcessRunner(annovar_cmd);
                    //textBox1.Text = annovar_cmd;
                    string annovar_result_file = "";
                    foreach (string i in System.IO.Directory.GetFiles(Rna_working_directory_path.Text))
                    {
                        if (i.ToString().Contains("_multianno.txt"))
                        {
                            annovar_result_file = i.ToString();
                        }
                    }
                    string create_db = "/C " + create_DB_path + "Create_DB.exe " + annovar_result_file + " " + create_DB_path + "NP_and_NM.txt" + " " + create_DB_path + set_genome + "_refGeneversion.txt " + create_DB_path + "homo_sapiens_RS81.fasta " + Rna_working_directory_path.Text + "\\" + "Variant_protein_DB.fasta " + Rna_working_directory_path.Text + "\\";
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

        private void Rna_browse_vcf_file_Click(object sender, EventArgs e)
        {
            if (Rna_sra_file_browse_for_fq.ShowDialog() == DialogResult.OK)
            {
                Rna_select_vcf_file.Text = Rna_sra_file_browse_for_fq.FileName;
            }
        }
        int flag = 0;
        private void Rna_chromosome_enable_disable_Click(object sender, EventArgs e)
        {
            if (flag==0)
            {
                Rna_bed_file_path.Enabled = false;
                Rna_browse_bed_file.Enabled = false;
                Rna_chromosome_field.Enabled = true;
                flag = 1;
            }
            else
            {
                Rna_bed_file_path.Enabled = true;
                Rna_browse_bed_file.Enabled = true;
                Rna_chromosome_field.Enabled = false;
                flag = 0;
            }
        }

        private void RNA_Seq_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Rna_annovar_download_label_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form annovar_download = new Annovar_download();
            annovar_download.Visible = true;
        }

        private void downloadAnnovarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Form annovar_download = new Annovar_download();
            annovar_download.Visible = true;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Rna_sel_genome_version_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Paste here
            string genome = Rna_sel_genome_version.GetItemText(Rna_sel_genome_version.SelectedItem);
            Rna_sel_db.Items.Clear();
            if (genome == "hg19")
            {
                Rna_sel_db.Items.Insert(0, "avsift");
                Rna_sel_db.Items.Insert(1, "ljb26_all");
                Rna_sel_db.Items.Insert(2, "dbnsfp30a");
                Rna_sel_db.Items.Insert(3, "dbnsfp31a_interpro");
                Rna_sel_db.Items.Insert(4, "dbnsfp33a");
                Rna_sel_db.Items.Insert(5, "dbnsfp35a");
                Rna_sel_db.Items.Insert(6, "dbnsfp35c");
                Rna_sel_db.Items.Insert(7, "dbscsnv11");
                Rna_sel_db.Items.Insert(8, "intervar_20170202");
                Rna_sel_db.Items.Insert(9, "intervar_20180118");
                Rna_sel_db.Items.Insert(10, "cg46");
                Rna_sel_db.Items.Insert(11, "cg69");
                Rna_sel_db.Items.Insert(12, "cosmic64");
                Rna_sel_db.Items.Insert(13, "cosmic65");
                Rna_sel_db.Items.Insert(14, "cosmic67");
                Rna_sel_db.Items.Insert(15, "cosmic67wgs");
                Rna_sel_db.Items.Insert(16, "cosmic68");
                Rna_sel_db.Items.Insert(17, "cosmic68wgs");
                Rna_sel_db.Items.Insert(18, "cosmic70");
                Rna_sel_db.Items.Insert(19, "esp6500siv2_ea");
                Rna_sel_db.Items.Insert(20, "esp6500siv2_aa");
                Rna_sel_db.Items.Insert(21, "esp6500siv2_all");
                Rna_sel_db.Items.Insert(22, "exac03");
                Rna_sel_db.Items.Insert(23, "exac03nontcga");
                Rna_sel_db.Items.Insert(24, "exac03nonpsych");
                Rna_sel_db.Items.Insert(25, "gnomad_exome");
                Rna_sel_db.Items.Insert(26, "gnomad_genome");
                Rna_sel_db.Items.Insert(27, "kaviar_20150923");
                Rna_sel_db.Items.Insert(28, "hrcr1");
                Rna_sel_db.Items.Insert(29, "abraom");
                Rna_sel_db.Items.Insert(30, "1000g2010nov");
                Rna_sel_db.Items.Insert(31, "1000g2011may");
                Rna_sel_db.Items.Insert(32, "1000g2012feb");
                Rna_sel_db.Items.Insert(33, "1000g2012apr");
                Rna_sel_db.Items.Insert(34, "1000g2014aug");
                Rna_sel_db.Items.Insert(35, "1000g2014sep");
                Rna_sel_db.Items.Insert(36, "1000g2014oct");
                Rna_sel_db.Items.Insert(37, "1000g2015aug");
                Rna_sel_db.Items.Insert(38, "gme");
                Rna_sel_db.Items.Insert(39, "mcap");
                Rna_sel_db.Items.Insert(40, "mcap13");
                Rna_sel_db.Items.Insert(41, "revel");
                Rna_sel_db.Items.Insert(42, "snp129");
                Rna_sel_db.Items.Insert(43, "snp130");
                Rna_sel_db.Items.Insert(44, "snp131");
                Rna_sel_db.Items.Insert(45, "snp132");
                Rna_sel_db.Items.Insert(46, "snp135");
                Rna_sel_db.Items.Insert(47, "snp137");
                Rna_sel_db.Items.Insert(48, "snp138");
                Rna_sel_db.Items.Insert(49, "avsnp138");
                Rna_sel_db.Items.Insert(50, "avsnp142");
                Rna_sel_db.Items.Insert(51, "avsnp144");
                Rna_sel_db.Items.Insert(52, "avsnp147");
                Rna_sel_db.Items.Insert(53, "avsnp150");
                Rna_sel_db.Items.Insert(54, "snp130NonFlagged");
                Rna_sel_db.Items.Insert(55, "snp131NonFlagged");
                Rna_sel_db.Items.Insert(56, "snp132NonFlagged");
                Rna_sel_db.Items.Insert(57, "snp135NonFlagged");
                Rna_sel_db.Items.Insert(58, "snp137NonFlagged");
                Rna_sel_db.Items.Insert(59, "snp138NonFlagged");
                Rna_sel_db.Items.Insert(60, "nci60");
                Rna_sel_db.Items.Insert(61, "icgc21");
                Rna_sel_db.Items.Insert(62, "clinvar_20131105");
                Rna_sel_db.Items.Insert(63, "clinvar_20140211");
                Rna_sel_db.Items.Insert(64, "clinvar_20140303");
                Rna_sel_db.Items.Insert(65, "clinvar_20140702");
                Rna_sel_db.Items.Insert(66, "clinvar_20140902");
                Rna_sel_db.Items.Insert(67, "clinvar_20140929");
                Rna_sel_db.Items.Insert(68, "clinvar_20150330");
                Rna_sel_db.Items.Insert(69, "clinvar_20150629");
                Rna_sel_db.Items.Insert(70, "clinvar_20151201");
                Rna_sel_db.Items.Insert(71, "clinvar_20160302");
                Rna_sel_db.Items.Insert(72, "clinvar_20161128");
                Rna_sel_db.Items.Insert(73, "clinvar_20170130");
                Rna_sel_db.Items.Insert(74, "clinvar_20170501");
                Rna_sel_db.Items.Insert(75, "clinvar_20170905");
                Rna_sel_db.Items.Insert(76, "clinvar_20180603");
                Rna_sel_db.Items.Insert(77, "clinvar_20190305");
                Rna_sel_db.Items.Insert(78, "popfreq_max_20150413");
                Rna_sel_db.Items.Insert(79, "popfreq_all_20150413");
                Rna_sel_db.Items.Insert(80, "mitimpact2");
                Rna_sel_db.Items.Insert(81, "mitimpact24");
                Rna_sel_db.Items.Insert(82, "regsnpintron");
                Rna_sel_db.Items.Insert(83, "gerp++elem");
                Rna_sel_db.Items.Insert(84, "gerp++gt2");
                Rna_sel_db.Items.Insert(85, "caddgt20");
                Rna_sel_db.Items.Insert(86, "caddgt10");
                Rna_sel_db.Items.Insert(87, "cadd");
                Rna_sel_db.Items.Insert(88, "cadd13");
                Rna_sel_db.Items.Insert(89, "cadd13gt10");
                Rna_sel_db.Items.Insert(90, "cadd13gt20");
                Rna_sel_db.Items.Insert(91, "caddindel");
                Rna_sel_db.Items.Insert(92, "fathmm");
                Rna_sel_db.Items.Insert(93, "gwava");
                Rna_sel_db.Items.Insert(94, "eigen");
            }
            else
            {
                Rna_sel_db.Items.Insert(0, "ljb26_all");
                Rna_sel_db.Items.Insert(1, "dbnsfp30a");
                Rna_sel_db.Items.Insert(2, "dbnsfp31a_interpro");
                Rna_sel_db.Items.Insert(3, "dbnsfp33a");
                Rna_sel_db.Items.Insert(4, "dbnsfp35a");
                Rna_sel_db.Items.Insert(5, "dbnsfp35c");
                Rna_sel_db.Items.Insert(6, "dbscsnv11");
                Rna_sel_db.Items.Insert(7, "intervar_20180118");
                Rna_sel_db.Items.Insert(8, "cosmic70");
                Rna_sel_db.Items.Insert(9, "esp6500siv2_ea");
                Rna_sel_db.Items.Insert(10, "esp6500siv2_aa");
                Rna_sel_db.Items.Insert(10, "esp6500siv2_all");
                Rna_sel_db.Items.Insert(12, "exac03");
                Rna_sel_db.Items.Insert(13, "exac03nontcga");
                Rna_sel_db.Items.Insert(14, "exac03nonpsych");
                Rna_sel_db.Items.Insert(15, "gnomad_exome");
                Rna_sel_db.Items.Insert(16, "gnomad_genome");
                Rna_sel_db.Items.Insert(17, "kaviar_20150923");
                Rna_sel_db.Items.Insert(18, "hrcr1");
                Rna_sel_db.Items.Insert(19, "abraom");
                Rna_sel_db.Items.Insert(20, "1000g2014oct");
                Rna_sel_db.Items.Insert(21, "1000g2015aug");
                Rna_sel_db.Items.Insert(22, "gme");
                Rna_sel_db.Items.Insert(23, "mcap");
                Rna_sel_db.Items.Insert(24, "revel");
                Rna_sel_db.Items.Insert(25, "avsnp144");
                Rna_sel_db.Items.Insert(26, "avsnp142");
                Rna_sel_db.Items.Insert(27, "avsnp144");
                Rna_sel_db.Items.Insert(28, "avsnp147");
                Rna_sel_db.Items.Insert(29, "avsnp150");
                Rna_sel_db.Items.Insert(30, "clinvar_20140702");
                Rna_sel_db.Items.Insert(31, "clinvar_20150330");
                Rna_sel_db.Items.Insert(32, "clinvar_20150629");
                Rna_sel_db.Items.Insert(33, "clinvar_20151201");
                Rna_sel_db.Items.Insert(34, "clinvar_20160302");
                Rna_sel_db.Items.Insert(35, "clinvar_20161128");
                Rna_sel_db.Items.Insert(36, "clinvar_20170130");
                Rna_sel_db.Items.Insert(37, "clinvar_20170501");
                Rna_sel_db.Items.Insert(38, "clinvar_20170905");
                Rna_sel_db.Items.Insert(39, "clinvar_20180603");
                Rna_sel_db.Items.Insert(40, "clinvar_20190305");
                Rna_sel_db.Items.Insert(41, "regsnpintron");
            }
        }

        private void Rna_select_all_db_Click(object sender, EventArgs e)
        {
            int cnt = Rna_sel_db.Items.Count;

            for (int i = 0; i <= cnt - 1; i++)
            {
                if (Rna_sel_db.GetItemChecked(i) == false)
                {
                    Rna_sel_db.SetItemCheckState(i, CheckState.Checked);
                }
            }
        }

        private void Rna_clear_db_list_Click(object sender, EventArgs e)
        {
            Rna_sel_db.Items.Clear();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form abt = new About();
            abt.Show();
        }

        private void Rna_data_layout_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Rna_data_layout_combobox.GetItemText(Rna_data_layout_combobox.SelectedItem) == "Single-end")
            {
                Rna_alignment_fastq_read2.Enabled = false;
                Rna_alignment_browse_read2.Enabled = false;
            }
            else
            {
                Rna_alignment_fastq_read2.Enabled = true; ;
                Rna_alignment_browse_read2.Enabled = true;
            }
        }

        private void RNA_Seq_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 fm1 = new Form1();
            fm1.Show();
        }

        private void Rna_dry_run_open_directory_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Rna_dry_run_set_workig_dir.Text);
        }
    }
}
