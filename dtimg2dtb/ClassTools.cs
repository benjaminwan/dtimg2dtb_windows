using System;
using System.Collections.Generic;
using System.Text;

namespace dtimg2dtb
{

    /*
     * Format:
                                     size
   x      +------------------+
   |      | MAGIC ("QCDT")   |   4B
   |      +------------------+
 header   | VERSION          |   uint32 (version 3)
   |      +------------------+
   |      | num of DTBs      |   uint32 (number of DTB entries)
   x      +------------------+
   |      | platform id #1   |   uint32 (e.g. ID for MSM8974)
   |      +------------------+
   |      | variant id #1    |   uint32 (e.g. ID for CDP, MTP)
   |      +------------------+
   |      | subtype id #1    |   uint32 (e.g. ID for subtype) (QCDT v2)
 device   +------------------+
  #1      | soc rev #1       |   uint32 (e.g. MSM8974 v2)
 entry    +------------------+
   |      | pmic0 #1         |   uint32 (pmic0-> first smallest SID of existing pmic)
   |      +------------------+
   |      | pmic1 #1         |   uint32 (pmic1-> secondary smallest SID of existing pmic)
   |      +------------------+
   |      | pmic2 #1         |   uint32 (pmic2-> third smallest SID of existing pmic)
   |      +------------------+
   |      | pmic3 #1         |   uint32 (pmic3-> fourth smallest SID of existing pmic)
   |      +------------------+
   |      | offset #1        |   uint32 (byte offset from start/before MAGIC
   |      +------------------+           to DTB entry)
   |      | size #1          |   uint32 (size in bytes of DTB blob)
   x      +------------------+
   .              .
   .              .  (repeat)
   .              .

   x      +------------------+
   |      | platform id #Z   |   uint32 (e.g. ID for MSM8974)
   |      +------------------+
  device  | variant id #Z    |   uint32 (e.g. ID for CDP, MTP)
  #Z      +------------------+
  entry   | subtype id #Z    |   uint32 (e.g. ID for subtype) (QCDT v2)
  (last)  +------------------+
   |      | soc rev #Z       |   uint32 (e.g. MSM8974 v2)
   |      +------------------+
   |      | pmic0 #1         |   uint32 (pmic0-> first smallest SID of existing pmic)
   |      +------------------+
   |      | pmic1 #1         |   uint32 (pmic1-> secondary smallest SID of existing pmic)
   |      +------------------+
   |      | pmic2 #1         |   uint32 (pmic2-> third smallest SID of existing pmic)
   |      +------------------+
   |      | pmic3 #1         |   uint32 (pmic3-> fourth smallest SID of existing pmic)
   |      +------------------+
   |      | offset #Z        |   uint32 (byte offset from start/before MAGIC
   x      +------------------+           to DTB entry)
          | 0 ("zero")       |   uint32 (end of list delimiter)
          +------------------+           to DTB entry)
          | padding          |   variable length for next DTB to start on
          +------------------+           page boundary
          | DTB #1           |   variable (start is page aligned)
          |                  |
          |                  |
          +------------------+
          | padding          |   variable length for next DTB to start on
          +------------------+           page boundary
                  .
                  .
                  .

          +------------------+
          | DTB #Z (last)    |   variable (start is page aligned)
          |                  |
          |                  |
          +------------------+
    */
    public static class MyFunc
    {
        public static string GetTimeFileName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(System.DateTime.Now.Year.ToString("D4"));
            sb.Append(System.DateTime.Now.Month.ToString("D2"));
            sb.Append(System.DateTime.Now.Day.ToString("D2"));
            sb.Append(System.DateTime.Now.Hour.ToString("D2"));
            sb.Append(System.DateTime.Now.Minute.ToString("D2"));
            sb.Append(System.DateTime.Now.Second.ToString("D2"));
            return sb.ToString();
        }
        public static long GetFileSize(string FileName)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(FileName);
            return file.Length;
        }
    }

    public class DTB_single
    {
        public int No { get; set; }
        public String dtbfile { get; set; }
        public UInt32 iOffset { get; set; }
        public UInt32 iSize { get; set; }
    }

    /*
    public class DTB_entryV2
    {
        public int No { get; set; }
        public String dtbfile { get; set; }
        public String Platform_id { get; set; }
        public UInt32 iPlatform_id { get; set; }
        private byte[] bPlatform_id = new byte[4];
        public String Variant_id { get; set; }
        public UInt32 iVariant_id { get; set; }
        private byte[] bVariant_id = new byte[4];
        public String Subtype_id { get; set; }
        public UInt32 iSubtype_id { get; set; }
        private byte[] bSubtype_id = new byte[4];
        public String Soc_rev { get; set; }
        public UInt32 iSoc_rev { get; set; }
        private byte[] bSoc_rev = new byte[4];

        public String Offset { get; set; }
        public UInt32 iOffset { get; set; }
        private byte[] bOffset = new byte[4];
        public String Size { get; set; }
        public UInt32 iSize { get; set; }
        private byte[] bSize = new byte[4];
        public DTB_entryV2(byte[] bDTB_entry)
        {
            if (bDTB_entry.Length==24)
            {
                bPlatform_id[0] = bDTB_entry[0];
                bPlatform_id[1] = bDTB_entry[1];
                bPlatform_id[2] = bDTB_entry[2];
                bPlatform_id[3] = bDTB_entry[3];
                iPlatform_id = BitConverter.ToUInt32(bPlatform_id, 0);
                Platform_id = "0x" + iPlatform_id.ToString("x8").ToUpper();

                bVariant_id[0] = bDTB_entry[4];
                bVariant_id[1] = bDTB_entry[5];
                bVariant_id[2] = bDTB_entry[6];
                bVariant_id[3] = bDTB_entry[7];
                iVariant_id = BitConverter.ToUInt32(bVariant_id, 0);
                Variant_id = "0x" + iVariant_id.ToString("x8").ToUpper();

                bSubtype_id[0] = bDTB_entry[8];
                bSubtype_id[1] = bDTB_entry[9];
                bSubtype_id[2] = bDTB_entry[10];
                bSubtype_id[3] = bDTB_entry[11];
                iSubtype_id = BitConverter.ToUInt32(bSubtype_id, 0);
                Subtype_id = "0x" + iSubtype_id.ToString("x8").ToUpper();

                bSoc_rev[0] = bDTB_entry[12];
                bSoc_rev[1] = bDTB_entry[13];
                bSoc_rev[2] = bDTB_entry[14];
                bSoc_rev[3] = bDTB_entry[15];
                iSoc_rev = BitConverter.ToUInt32(bSoc_rev, 0);
                Soc_rev = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bOffset[0] = bDTB_entry[16];
                bOffset[1] = bDTB_entry[17];
                bOffset[2] = bDTB_entry[18];
                bOffset[3] = bDTB_entry[19];
                iOffset = BitConverter.ToUInt32(bOffset, 0);
                Offset = "0x" + iOffset.ToString("x8").ToUpper();

                bSize[0] = bDTB_entry[20];
                bSize[1] = bDTB_entry[21];
                bSize[2] = bDTB_entry[22];
                bSize[3] = bDTB_entry[23];
                iSize = BitConverter.ToUInt32(bSize, 0);
                Size = "0x" + iSize.ToString("x8").ToUpper();
            }
            else if (bDTB_entry.Length==40)
            {
                
            }
            

        }
    }
    */
    public class DTB_entryV3
    {
        private byte[] bTemp = new byte[4];
        public int No { get; set; }
        public String dtbfile { get; set; }
        public String Platform_id { get; set; }
        public UInt32 iPlatform_id { get; set; }
        public String Variant_id { get; set; }
        public UInt32 iVariant_id { get; set; }
        public String Subtype_id { get; set; }
        public UInt32 iSubtype_id { get; set; }
        public String Soc_rev { get; set; }
        public UInt32 iSoc_rev { get; set; }
        public String Pmic0 { get; set; }
        public UInt32 iPmic0 { get; set; }
        public String Pmic1 { get; set; }
        public UInt32 iPmic1 { get; set; }
        public String Pmic2 { get; set; }
        public UInt32 iPmic2 { get; set; }
        public String Pmic3 { get; set; }
        public UInt32 iPmic3 { get; set; }
        public String Offset { get; set; }
        public UInt32 iOffset { get; set; }
        public String Size { get; set; }
        public UInt32 iSize { get; set; }
         public DTB_entryV3(byte[] bDTB_entry)
        {
            if (bDTB_entry.Length == 24)//version 2
            {
                bTemp[0] = bDTB_entry[0];
                bTemp[1] = bDTB_entry[1];
                bTemp[2] = bDTB_entry[2];
                bTemp[3] = bDTB_entry[3];
                iPlatform_id = BitConverter.ToUInt32(bTemp, 0);
                Platform_id = "0x" + iPlatform_id.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[4];
                bTemp[1] = bDTB_entry[5];
                bTemp[2] = bDTB_entry[6];
                bTemp[3] = bDTB_entry[7];
                iVariant_id = BitConverter.ToUInt32(bTemp, 0);
                Variant_id = "0x" + iVariant_id.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[8];
                bTemp[1] = bDTB_entry[9];
                bTemp[2] = bDTB_entry[10];
                bTemp[3] = bDTB_entry[11];
                iSubtype_id = BitConverter.ToUInt32(bTemp, 0);
                Subtype_id = "0x" + iSubtype_id.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[12];
                bTemp[1] = bDTB_entry[13];
                bTemp[2] = bDTB_entry[14];
                bTemp[3] = bDTB_entry[15];
                iSoc_rev = BitConverter.ToUInt32(bTemp, 0);
                Soc_rev = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[16];
                bTemp[1] = bDTB_entry[17];
                bTemp[2] = bDTB_entry[18];
                bTemp[3] = bDTB_entry[19];
                iOffset = BitConverter.ToUInt32(bTemp, 0);
                Offset = "0x" + iOffset.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[20];
                bTemp[1] = bDTB_entry[21];
                bTemp[2] = bDTB_entry[22];
                bTemp[3] = bDTB_entry[23];
                iSize = BitConverter.ToUInt32(bTemp, 0);
                Size = "0x" + iSize.ToString("x8").ToUpper();
            }
            else if (bDTB_entry.Length == 40)//version 3
            {
                bTemp[0] = bDTB_entry[0];
                bTemp[1] = bDTB_entry[1];
                bTemp[2] = bDTB_entry[2];
                bTemp[3] = bDTB_entry[3];
                iPlatform_id = BitConverter.ToUInt32(bTemp, 0);
                Platform_id = "0x" + iPlatform_id.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[4];
                bTemp[1] = bDTB_entry[5];
                bTemp[2] = bDTB_entry[6];
                bTemp[3] = bDTB_entry[7];
                iVariant_id = BitConverter.ToUInt32(bTemp, 0);
                Variant_id = "0x" + iVariant_id.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[8];
                bTemp[1] = bDTB_entry[9];
                bTemp[2] = bDTB_entry[10];
                bTemp[3] = bDTB_entry[11];
                iSubtype_id = BitConverter.ToUInt32(bTemp, 0);
                Subtype_id = "0x" + iSubtype_id.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[12];
                bTemp[1] = bDTB_entry[13];
                bTemp[2] = bDTB_entry[14];
                bTemp[3] = bDTB_entry[15];
                iSoc_rev = BitConverter.ToUInt32(bTemp, 0);
                Soc_rev = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[16];
                bTemp[1] = bDTB_entry[17];
                bTemp[2] = bDTB_entry[18];
                bTemp[3] = bDTB_entry[19];
                iPmic0 = BitConverter.ToUInt32(bTemp, 0);
                Pmic0 = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[20];
                bTemp[1] = bDTB_entry[21];
                bTemp[2] = bDTB_entry[22];
                bTemp[3] = bDTB_entry[23];
                iPmic1 = BitConverter.ToUInt32(bTemp, 0);
                Pmic1 = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[24];
                bTemp[1] = bDTB_entry[25];
                bTemp[2] = bDTB_entry[26];
                bTemp[3] = bDTB_entry[27];
                iPmic2 = BitConverter.ToUInt32(bTemp, 0);
                Pmic2 = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[28];
                bTemp[1] = bDTB_entry[29];
                bTemp[2] = bDTB_entry[30];
                bTemp[3] = bDTB_entry[31];
                iPmic3 = BitConverter.ToUInt32(bTemp, 0);
                Pmic3 = "0x" + iSoc_rev.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[32];
                bTemp[1] = bDTB_entry[33];
                bTemp[2] = bDTB_entry[34];
                bTemp[3] = bDTB_entry[35];
                iOffset = BitConverter.ToUInt32(bTemp, 0);
                Offset = "0x" + iOffset.ToString("x8").ToUpper();

                bTemp[0] = bDTB_entry[36];
                bTemp[1] = bDTB_entry[37];
                bTemp[2] = bDTB_entry[38];
                bTemp[3] = bDTB_entry[39];
                iSize = BitConverter.ToUInt32(bTemp, 0);
                Size = "0x" + iSize.ToString("x8").ToUpper();
            }

        }
    }
}
