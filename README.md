CusVarDB generates variant protein database from NGS-datasets. It is a windows based tool for creating a variant protein database from Next-generation sequencing datasets. The programs supports variant calling for Genome, RNA-Seq and exome datasets. 4 major modules of the tool are listed below.

1. Align the datasets with reference database using BWA / HISAT2
2. Perform the variant calling using Genome Analysis Toolkit (GATK)
3. Annotate the variant using ANNOVAR
4. Create the variant protein database

The program supports additional function such as

1. Download the SRA file
2. Convert the SRA file to fastq file format
3. Download the annotation (ANNOVAR) database
4. Dry-run concept to customize the commands


The manual in available inside the zip folder. The CusVarDB can be also downloaded at http://bioinfo-tools.com/Downloads/CusVarDB/V1.0.0/


The test datasets are available at http://bioinfo-tools.com/Downloads/CusVarDB/V1.0.0/test_dataset.rar