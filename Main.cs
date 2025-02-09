﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Globalization;

namespace DtcRemover
{
    public partial class DtcRemover : Form
    {
        public DtcRemover()
        {
            InitializeComponent();
        }
        //Create byte arrays and lists outside of functions
        byte[] bytes;
        byte[] DFES_DTCO;
        byte[] DFES_Cls;
        byte[] DFC_DisblMsk2;
        byte[] AvailableErrorCodes;

        List<int> potentialDFES_DTCO;
        List<int> potentialDFES_Cls;
        List<int> potentialDFC_DisblMsk2;
        List<string> dtcFromList;

        int lengthErrorCodes8bit;
        int lengthErrorCodes16bit;

        bool hiLoSwitch = false;

        string substringBytesSwapped;
        string pCodeBinary;
        string pCodeBinary1;
        string pCodeBinary2;

        //Create datatables 
        //Removed P-Code table
        DataTable dtMain = new DataTable();
        //List of available DTC's
        DataTable dtAvailableCodes = new DataTable();

        private void ResetDtcBlocks()
        {
            DFES_DTCO = null;
            DFES_Cls = null;
            DFC_DisblMsk2 = null;
        }


        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //Enable DTC Remove button
            btnRemoveDtc.Enabled = true;
            //Enable Open File button
            btnOpenDtc.Enabled = true;
            //Disable Open File button
            btnOpenFile.Enabled = false;
            //Enable Close File button
            btnCloseFile.Enabled = true;

            //Reset datatable Main
            dtMain.Reset();
            dtAvailableCodes.Reset();

            //Add columns to datatable Main
            dtMain.Columns.Add("P-Code");
            dtMain.Columns.Add("Removed");
            //Add columns to datatable available codes
            dtAvailableCodes.Columns.Add("P-Code");

            //OpenFileDialog to select file to modify
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Create byte array of the file in binary format
                bytes = File.ReadAllBytes(openFileDialog.FileName);

                //Add 4G0907589F_0004
                if (DFES_DTCO == null && DFES_Cls == null && DFC_DisblMsk2 == null)
                {
                    //block length is 1552 8 bit, 3104 16 bit error codes.
                    lengthErrorCodes8bit = 1552;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 19, 209, 18, 209, 11, 21, 11, 21 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 00, 00, 00, 03, 11 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 253, 03, 255, 255, 255, 255, 253, 03, 253, 03, 253 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("(EDCP17CP44 Based on 4G0907589F_0004 Algorithm Detected", "EDCP17CP44");
                    }
                    ResetDtcBlocks();
                }
                //Add 7P0907401_0006
                if (DFES_DTCO == null && DFES_Cls == null && DFC_DisblMsk2 == null)
                {
                    //block length is 1036 8 bit, 2072 16 bit error codes.
                    lengthErrorCodes8bit = 1036;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 19, 209, 18, 209, 11, 21, 11, 21 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 03, 03, 03, 03, 01, 01, 01 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 00, 00, 255, 255, 253 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("EDCP17CP44 Based on 7P0907401_0006 Algorithm Detected", "EDCP17CP44");
                    }
                    ResetDtcBlocks();
                }

                //Add 04E906016DE_9022
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 838 8 bit, 1676 16 bit error codes.
                    lengthErrorCodes8bit = 838;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 08, 208, 58, 16 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 01, 11, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 00, 00, 255, 255, 255, 255, 255, 255, 255, 255, 253, 03 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MED17.5.21 Based on 04E906016DE_9022 Algorithm Detected", "MED17.5.21");
                    }
                    ResetDtcBlocks();
                }

                //Add 04C906026_3537
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 900 8 bit, 1800 16 bit error codes.
                    lengthErrorCodes8bit = 900;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 50, 05 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 1, 1, 11, 2, 0 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 253, 03, 255, 255, 00, 00, 255 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MED17.5.21 Based on 04C906026_3537 Algorithm Detected", "MED17.5.21");
                    }
                    ResetDtcBlocks();
                }

                //Add 04C906025F_6355
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 960 8 bit, 1920 16 bit error codes.
                    lengthErrorCodes8bit = 960;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 50, 05, 08 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 01, 01, 11, 11, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 253, 03, 255, 255, 255, 255, 255 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MED17.1.27 Based on 04C906025F_6355 Algorithm Detected", "MED17.1.27");
                    }
                    ResetDtcBlocks();
                }
                //Add 04E906027JT_4145
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 992 8 bit, 1984 16 bit error codes.
                    lengthErrorCodes8bit = 992;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 08, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 1, 11, 2, 0 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 253, 03, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 253 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MED17.5.25 Based on 04E906027JT_4145 Algorithm Detected", "MED17.5.25");
                    }
                    ResetDtcBlocks();
                }
                //Add 04E906027HB_4418
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 986 8 bit, 1972 16 bit error codes.
                    lengthErrorCodes8bit = 986;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 08, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 1, 11, 2, 0 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 253, 03, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 253 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MED17.5.21 Based on 04E906027HB_4418 Algorithm Detected", "MED17.5.21");
                    }
                    ResetDtcBlocks();
                }
                //Add 05E906012Q_1319
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1096 8 bit, 2192 16 bit error codes.
                    lengthErrorCodes8bit = 1096;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 09, 22 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 01, 01, 01, 11, 04 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 00, 00, 00, 00, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 204 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MG1CS011 Based on 05E906012Q_1319 Algorithm Detected", "MG1CS011");
                    }
                    ResetDtcBlocks();
                }
                //Add 2G0907115_0004
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1286 8 bit, 2572 16 bit error codes.
                    lengthErrorCodes8bit = 1286;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 16, 58, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 02, 00, 01, 03 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 03, 252, 03, 252, 03, 252, 255, 255, 03 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = true;
                        MessageBox.Show("MG1CS001 Based on 2G0907115_0004 Algorithm Detected", "MG1CS001");
                    }
                    ResetDtcBlocks();
                }
                //Add 81A907115A_0001
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1346 8 bit, 2692 16 bit error codes.
                    lengthErrorCodes8bit = 1346;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 16, 58, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 02, 00 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 02, 233, 255, 255, 03, 252 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = true;
                        MessageBox.Show("MG1CS111 Based on 81A907115A_0001 Algorithm Detected", "MG1CS111");
                    }
                    ResetDtcBlocks();
                }
                //Add 8W0907115Q_0003
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1346 8 bit, 2692 16 bit error codes.
                    lengthErrorCodes8bit = 1346;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 22, 09, 10 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 01, 11, 11, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 03, 224 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = true;
                        MessageBox.Show("MG1CS001 Based on 8W0907115Q_0003 Algorithm Detected", "MG1CS001");
                    }
                    ResetDtcBlocks();
                }
                //Add 8P0907115BA_0020
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 694 8 bit, 1388 16 bit error codes.
                    lengthErrorCodes8bit = 694;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 23, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 01, 11, 01, 01, 01, 01, 01, 11, 02, 02, 01, 01, 01, 11 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 00, 00, 255, 128, 255, 128, 255, 128, 255 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MED17.5 Based on 8P0907115BA_0020 Algorithm Detected", "MED17.5");
                    }
                    ResetDtcBlocks();
                }
                //Add 04L906026AD_1970
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1396 8 bit, 2792 16 bit error codes.
                    lengthErrorCodes8bit = 1396;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 07, 06, 54 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 03, 04, 11, 11 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 05, 08 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("EDC17C74 Based on 04L906026AD_1970 Algorithm Detected", "EDC17C74");
                    }
                    ResetDtcBlocks();
                }
                //Add 8K5907401T_0001
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1440 8 bit, 2880 16 bit error codes.
                    lengthErrorCodes8bit = 1440;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 19, 209 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 00, 03 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 00, 00, 253, 03 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("EDC17CP44 Based on 8K5907401T_0001 Algorithm Detected", "EDC17CP44");
                    }
                    ResetDtcBlocks();
                }
                //Add 992906020AS_0002
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1635 8 bit, 3270 16 bit error codes.
                    lengthErrorCodes8bit = 1635;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 08, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 00, 11, 01, 11, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 252, 03, 252, 03, 255, 255, 233 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MG1CP007 Based on 992906020AS_0002 Algorithm Detected", "MG1CP007");
                    }
                    ResetDtcBlocks();
                }
                //Add 982906033T_0001
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1355 8 bit, 2710 16 bit error codes.
                    lengthErrorCodes8bit = 1355;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 35, 21 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 11, 11, 11, 01, 11, 11, 01 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 00, 00, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 252, 03, 252, 03, 255 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MG1CP007 Based on 982906033T_0001 Algorithm Detected", "MG1CP007");
                    }
                    ResetDtcBlocks();
                }
                //Add 4ML907589C_0001
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1760 8 bit, 3520 16 bit error codes.
                    lengthErrorCodes8bit = 1760;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 09, 22 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 11, 01, 11, 01, 02, 01, 01 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 253, 03, 255, 255, 252 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MG1CS008 Based on 4ML907589C_0001 Algorithm Detected", "MG1CS008");
                    }
                    ResetDtcBlocks();
                }
                //Add 4K0907557D_0002
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1842 8 bit, 3684 16 bit error codes.
                    lengthErrorCodes8bit = 1842;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 09, 22 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 01, 01, 11, 02, 02, 00, 01, 11 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 224, 03, 255, 255, 253, 03, 253, 03 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MG1CS008 Based on 4K0907557D_0002 Algorithm Detected", "MG1CS008");
                    }
                    ResetDtcBlocks();
                }
                //Add 4K0907557R_0001
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1955 16 bit, 3910 16 bit error codes.
                    lengthErrorCodes8bit = 1955;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 09, 22 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 01, 11, 11, 02, 02, 00, 01, 11 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 00, 00, 255, 255, 252, 03, 252, 03, 255 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MG1CS008 Based on 4K0907557R_0001 Algorithm Detected", "MG1CS008");
                    }
                    ResetDtcBlocks();
                }
                //Add 05L906023AD_1067
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length
                    lengthErrorCodes8bit = 1697;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block                   

                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    //Block based on A2L
                    DFES_DTCO = new byte[] { 7, 6, 54, 2, 0, 0, 0 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 03, 04, 11, 11 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 253, 07, 253, 07, 253, 07, 253, 07, 253, 07 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MD1_CS004 Based on 05L906023AD_1067 Algorithm Detected", "MD1_CS004");
                    }
                    ResetDtcBlocks();
                }

                //Add 4G2907311C_0007
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length
                    lengthErrorCodes8bit = 1560;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block                   
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 11, 04, 00, 00, 00, 00, 08, 208, 23 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 01, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 253, 03, 252 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("EDC17CP54 Based on 4G2907311C_0007 Algorithm Detected", "EDC17CP54");
                    }
                    ResetDtcBlocks();
                }
                //Add 8W0907311B_0003
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1552 8 bit, 3104 16 bit error codes.
                    lengthErrorCodes8bit = 1552;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 04, 11 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 03, 252 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count !=0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = true;
                        MessageBox.Show("(MD1_CP004 Based on 8W0907311B_0003 Algorithm Detected", "MD1_CP004");
                    }
                    ResetDtcBlocks();
                }
                //Add 4G0906560_0013
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length is 1392 8 bit, 2784 16 bit error codes.
                    lengthErrorCodes8bit = 1392;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 19, 209, 18, };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 01, 01, 01, 01, 01, 01, 11, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 253, 03, 255, 255, 255, 255, 253, 03 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("(MED17.1.1 Based on 4G0906560_0013 Algorithm Detected", "MED17.1.1");
                    }
                    ResetDtcBlocks();
                }
                //Add 4K0907401K_0003
                if (potentialDFES_DTCO.Count != 1 || potentialDFES_Cls.Count != 1 || potentialDFC_DisblMsk2.Count != 1)
                {
                    //block length
                    lengthErrorCodes8bit = 1864;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 11, 04 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 01, 02 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 224 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    //Speed up the search proces by skipping the next algorithms when potentialDFES_DTCO is empty
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                        potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    }

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count == 1 && potentialDFES_Cls.Count == 1 && potentialDFC_DisblMsk2.Count == 1)
                    {
                        hiLoSwitch = false;
                        MessageBox.Show("MD1_CP004 Based on 4K0907401K_0003 Algorithm Detected", "MD1_CP004");
                    }
                    else
                    {
                        MessageBox.Show("Firmware not supported, please contact support.", "Firmware not supported");  
                        dgvAvailableCodes.Refresh();
                        dgvMain.Refresh();
                        tbRemoveDtc.Text = "";
                        btnSaveFile.Enabled = false;
                        btnRemoveDtc.Enabled = false;
                        btnOpenFile.Enabled = true;
                        btnOpenDtc.Enabled = false;
                        btnCloseFile.Enabled = false;
                        System.Windows.Forms.Application.Restart();
                        return;
                    }
                }

                //Create DTC P-code table to show which DTC are available in the ECU
                AvailableErrorCodes = bytes.Skip(potentialDFES_DTCO[0]).Take(lengthErrorCodes16bit).ToArray();

                //Create hex string of Available Error Codes
                string hexStringAvailableErrorCodes = ByteArrayToString(AvailableErrorCodes);
                
                //Take ever 4 characters of a string and put in an array
                int countHexStringAvailableErrorCodes = hexStringAvailableErrorCodes.Length;
                for (int i = 0; i < countHexStringAvailableErrorCodes; i += 4)
                {
                    string substring = hexStringAvailableErrorCodes.Substring(i, 4);
                    //swap the first 2 characters with the last 2 (HiLo Change)
                    if (hiLoSwitch == false)
                    {
                        substringBytesSwapped = substring.Remove(0, 2) + substring.Substring(0, 2);
                    }
                    if (hiLoSwitch == true)
                    {
                        substringBytesSwapped = substring;
                    }
                    
                    //UpperCase for P-codes
                    string substringBytesSwappedUpper = substringBytesSwapped.ToUpper();

                    //Add P-codes to list
                    if (substringBytesSwappedUpper != "")
                    {
                        //listAvailableErrorCodesInHex.Add(substringBytesSwappedUpper);

                        //Add row to datatable available dtc's
                        DataRow _pCodeString = dtAvailableCodes.NewRow();
                        //Add row to column
                        _pCodeString["P-Code"] = substringBytesSwappedUpper;
                        dtAvailableCodes.Rows.Add(_pCodeString);
                    }                    
                }
                //Show list with available errorcodes in datagridview
                dgvAvailableCodes.DataSource = dtAvailableCodes;
            }
            else
            {
                MessageBox.Show("Firmware not supported, please contact support.", "Firmware not supported");
                return;
            }
        }
        //ByteArray to String
        public static string ByteArrayToString(byte[] ba)
            {
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }

        //SearchFunction for pattern search in byte array
        static public List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
        {
            List<int> positions = new List<int>();
            int patternLength = pattern.Length;
            int totalLength = bytes.Length;
            byte firstMatchByte = pattern[0];
            for (int i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
                {
                    byte[] match = new byte[patternLength];
                    Array.Copy(bytes, i, match, 0, patternLength);
                    if (match.SequenceEqual<byte>(pattern))
                    {
                        positions.Add(i);
                        i += patternLength - 1;
                    }
                }
            }
            return positions;
        }
        public void btnRemoveDtc_Click(object sender, EventArgs e)
        {
            //Search for P-Code
            //Test P0087 Hex to Dec
            string pCodeHex = tbRemoveDtc.Text;

            if (tbRemoveDtc.Text.Length  < 4)
            {
                MessageBox.Show("Please enter the full P-Code including zero's. E.g. for P0401, enter 0401", "Error");
                return;
            }
            var pcode1 = pCodeHex.Substring(0, 2);
            var pcode2 = pCodeHex.Substring(2, 2);

            if (hiLoSwitch == false)
            {
                pCodeBinary = Convert.ToInt32(pCodeHex, 16).ToString();
                pCodeBinary1 = Convert.ToInt32(pcode2, 16).ToString();
                pCodeBinary2 = Convert.ToInt32(pcode1, 16).ToString();
            }
            if (hiLoSwitch == true)
            {
                pCodeBinary = Convert.ToInt32(pCodeHex, 16).ToString();
                pCodeBinary2 = Convert.ToInt32(pcode2, 16).ToString();
                pCodeBinary1 = Convert.ToInt32(pcode1, 16).ToString();
            }

            string decimal_numbers = pCodeBinary1 + "," + pCodeBinary2;

            string[] decimal_values = decimal_numbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            byte[] pCodeArray = new byte[decimal_values.Length];
            for (int i = 0; i < decimal_values.Length; i++)
            {
                pCodeArray[i] = byte.Parse(decimal_values[i]);
            }

            //Extract the DTC Array's from the file to start counting the DTC position
            if (potentialDFES_DTCO.Count != 0)
            {
                //Search for all DTC's
                var DFES_DTCOCuttedEightBit = bytes.Skip(potentialDFES_DTCO[0]).Take(lengthErrorCodes16bit).ToArray();
                List<int> pCodeLocation = SearchBytePattern(pCodeArray, DFES_DTCOCuttedEightBit);
                foreach (var item in pCodeLocation)
                    //Make sure only the even and not the odd start addresses are used, since it's 16-bit.
                    if (item % 2 == 0)
                    {
                        {
                            int addressDFES_DTCO = item + potentialDFES_DTCO[0];

                            //txtBoxMain.Text = addressDFES_DTCO.ToString();
                            //16 bit
                            if (potentialDFC_DisblMsk2.Count != 0)
                            {
                                //Calculate the address of the DisableMask
                                var DFC_DisblMsk2Cutted = bytes.Skip(potentialDFC_DisblMsk2[0]).Take(lengthErrorCodes16bit).ToArray();
                                int addressDFC_DisblMsk2 = item + potentialDFC_DisblMsk2[0];

                                //txtBoxMain.Text = addressDFC_DisblMsk2.ToString();

                                //Set bytes to 0
                                bytes[addressDFC_DisblMsk2] = 255;
                                bytes[addressDFC_DisblMsk2 + 1] = 255;
                            }
                            //8bit, item count divided by 2 to get the right address
                            if (potentialDFES_Cls.Count != 0)
                            {
                                int EightBitItem = item / 2;
                                //Calculate the address of the Fehlerklass
                                var DFES_ClsCutted = bytes.Skip(potentialDFES_Cls[0]).Take(lengthErrorCodes8bit).ToArray();
                                int addressDFES_Cls = EightBitItem + potentialDFES_Cls[0];
                                //Set byte to 0
                                bytes[addressDFES_Cls] = 0;

                                //txtBoxMain.Text = bytes.ToString();
                            }
                        }
                    }
                MessageBox.Show("DTC removed succesfully.", "DTC Removed");

                //Enable Save File button
                btnSaveFile.Enabled = true;

                DataRow _itemString = dtMain.NewRow();
                //Add row to column
                _itemString["P-Code"] = tbRemoveDtc.Text;
                _itemString["Removed"] = "Yes";
                dtMain.Rows.Add(_itemString);

                dgvMain.DataSource = dtMain;
            }
            else
            {
                MessageBox.Show("Firmware not supported, please contact support.", "Firmware not supported");
                return;
            }
        }
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            //Write bytes to file.
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = desktop + "\\OutputFileDtcRemover.bin";
            File.WriteAllBytes(path, bytes);
            dgvMain.Refresh();
            dgvAvailableCodes.Refresh();
            tbRemoveDtc.Text = "";
            btnSaveFile.Enabled = false;
            btnRemoveDtc.Enabled = false;
            btnOpenFile.Enabled = true;
            btnOpenDtc.Enabled = false;
            btnCloseFile.Enabled = false;
            MessageBox.Show("Modified file succesfully saved.", "File saved");
            System.Windows.Forms.Application.Restart();
        }
        //Accept only numbers in textbox.
        private void tbRemoveDtc_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void btnOpenDtc_Click(object sender, EventArgs e)
        {
            //OpenFileDialog to select file with dtc list
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                //Convert P-Codes to list
                dtcFromList = System.IO.File.ReadLines(path).ToList();
                   
                for (int j = 0; j < dtcFromList.Count; j++)
                    {
                    string pCodeHex = dtcFromList[j].ToString();

                    if (pCodeHex.Length < 4)
                    {
                        MessageBox.Show("Please Check the list.", "Error");
                        return;
                    }
                    var pcode1 = pCodeHex.Substring(0, 2);
                    var pcode2 = pCodeHex.Substring(2, 2);

                    if (hiLoSwitch == false)
                    {
                        pCodeBinary = Convert.ToInt32(pCodeHex, 16).ToString();
                        pCodeBinary1 = Convert.ToInt32(pcode2, 16).ToString();
                        pCodeBinary2 = Convert.ToInt32(pcode1, 16).ToString();
                    }
                    if (hiLoSwitch == true)
                    {
                        pCodeBinary = Convert.ToInt32(pCodeHex, 16).ToString();
                        pCodeBinary2 = Convert.ToInt32(pcode2, 16).ToString();
                        pCodeBinary1 = Convert.ToInt32(pcode1, 16).ToString();
                    }

                    string decimal_numbers = pCodeBinary1 + "," + pCodeBinary2;

                    string[] decimal_values = decimal_numbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    byte[] pCodeArray = new byte[decimal_values.Length];
                    for (int i = 0; i < decimal_values.Length; i++)
                    {
                        pCodeArray[i] = byte.Parse(decimal_values[i]);
                    }

                    //Extract the DTC Array's from the file to start counting the DTC position
                    if (potentialDFES_DTCO.Count != 0)
                    {
                        //Search for all DTC's
                        var DFES_DTCOCuttedEightBit = bytes.Skip(potentialDFES_DTCO[0]).Take(lengthErrorCodes16bit).ToArray();
                        List<int> pCodeLocation = SearchBytePattern(pCodeArray, DFES_DTCOCuttedEightBit);
                        foreach (var item in pCodeLocation)
                            //Make sure only the even and not the odd start addresses are used, since it's 16-bit.
                            if (item % 2 == 0)
                            {
                                {
                                    int addressDFES_DTCO = item + potentialDFES_DTCO[0];

                                    //txtBoxMain.Text = addressDFES_DTCO.ToString();
                                    //16 bit
                                    if (potentialDFC_DisblMsk2.Count != 0)
                                    {
                                        //Calculate the address of the DisableMask
                                        var DFC_DisblMsk2Cutted = bytes.Skip(potentialDFC_DisblMsk2[0]).Take(lengthErrorCodes16bit).ToArray();
                                        int addressDFC_DisblMsk2 = item + potentialDFC_DisblMsk2[0];

                                        //txtBoxMain.Text = addressDFC_DisblMsk2.ToString();

                                        //Set bytes to 0
                                        bytes[addressDFC_DisblMsk2] = 255;
                                        bytes[addressDFC_DisblMsk2 + 1] = 255;
                                    }
                                    //8bit, item count divided by 2 to get the right address
                                    if (potentialDFES_Cls.Count != 0)
                                    {
                                        int EightBitItem = item / 2;
                                        //Calculate the address of the Fehlerklass
                                        var DFES_ClsCutted = bytes.Skip(potentialDFES_Cls[0]).Take(lengthErrorCodes8bit).ToArray();
                                        int addressDFES_Cls = EightBitItem + potentialDFES_Cls[0];
                                        //Set byte to 0
                                        bytes[addressDFES_Cls] = 0;

                                        //txtBoxMain.Text = bytes.ToString();
                                    }
                                }
                            }
                        
                        //Enable Save File button
                        btnSaveFile.Enabled = true;

                        DataRow _itemString = dtMain.NewRow();
                        //Add row to column
                        _itemString["P-Code"] = pCodeHex;
                        _itemString["Removed"] = "Yes";
                        dtMain.Rows.Add(_itemString);

                        dgvMain.DataSource = dtMain;
                    }                    
                }
                MessageBox.Show("DTC's from list removed succesfully.", "DTC Removed");
            }
        }

        private void btnCloseFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
        }

        private void DtcRemover_Load(object sender, EventArgs e)
        {

        }
    }
}
