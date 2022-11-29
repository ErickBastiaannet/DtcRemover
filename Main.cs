using System;
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

        int lengthErrorCodes8bit;
        int lengthErrorCodes16bit;

        //Create datatables 
        //Removed P-Code table
        DataTable dtMain = new DataTable();
        //List of available DTC's
        DataTable dtAvailableCodes=new DataTable();
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //Enable DTC Remove button
            btnRemoveDtc.Enabled = true;
            //Disable Open File button
            btnOpenFile.Enabled = false;

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

                //4G0907589F_0004
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
                    DFC_DisblMsk2 = new byte[] { 255, 255, 255, 255, 253, 03, 255, 255, 255, 255, 253 };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                    potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);

                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count != 0 && potentialDFES_DTCO.Count != 0 && potentialDFC_DisblMsk2.Count != 0)
                    {
                        MessageBox.Show("EDCP17CP44 Algorithm Detected", "EDCP17CP44");
                    }  
                }

                //Add 04E906027JT_4145
                if (potentialDFES_DTCO.Count == 0 && potentialDFES_DTCO.Count == 0 && potentialDFC_DisblMsk2.Count == 0)
                {
                    //block length is 1552 8 bit, 3104 16 bit error codes.
                    lengthErrorCodes8bit = 992;
                    lengthErrorCodes16bit = lengthErrorCodes8bit * 2;
                    //Pcode Block
                    //Start of DFES_DTCO 16 bit (DFES_DTCO.DFC_Unused_C) 
                    DFES_DTCO = new byte[] { 00, 00, 08, 208 };
                    //Start of Fehlerklasse 8 bit
                    DFES_Cls = new byte[] { 11, 1, 11, 2, 0 };
                    //Start of DisableMask 16 bit
                    DFC_DisblMsk2 = new byte[] { 00, 255, 255, 253, 03, 255, };

                    //Find locations of DTC tables
                    potentialDFES_DTCO = SearchBytePattern(DFES_DTCO, bytes);
                    potentialDFES_Cls = SearchBytePattern(DFES_Cls, bytes);
                    potentialDFC_DisblMsk2 = SearchBytePattern(DFC_DisblMsk2, bytes);
                    
                    //Show Messagebox with detected ECU Type
                    if (potentialDFES_DTCO.Count != 0 && potentialDFES_DTCO.Count != 0 && potentialDFC_DisblMsk2.Count != 0)
                    {
                        MessageBox.Show("MED17.5.25 Algorithm Detected", "MED17.5.25");
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
                    string substringBytesSwapped = substring.Remove(0, 2) + substring.Substring(0, 2);
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
                //dtAvailableCodes.Columns.Add("P-Code");
                dgvAvailableCodes.DataSource = dtAvailableCodes;
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

        private void btnRemoveDtc_Click(object sender, EventArgs e)
        {
            //Search for P-Code
            //Test P0087 Hex to Dec
            string pCodeHex = tbRemoveDtc.Text;
            var pcode1 = pCodeHex.Substring(0, 2);
            var pcode2 = pCodeHex.Substring(2, 2);

            string pCodeBinary = Convert.ToInt32(pCodeHex, 16).ToString();
            string pCodeBinary1 = Convert.ToInt32(pcode2, 16).ToString();
            string pCodeBinary2 = Convert.ToInt32(pcode1, 16).ToString();

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
            var path = @"file.bin";
            File.WriteAllBytes(path, bytes);
            dtMain.Clear();
            dtAvailableCodes.Clear();
            dgvAvailableCodes.Refresh();
            dgvAvailableCodes.Refresh();
            tbRemoveDtc.Text = "";
            btnSaveFile.Enabled = false;
            btnRemoveDtc.Enabled = false;
            btnOpenFile.Enabled = true;
            MessageBox.Show("Modified file succesfully saved.", "File saved");
        }
    }
}
