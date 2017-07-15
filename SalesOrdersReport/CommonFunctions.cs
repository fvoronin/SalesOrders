﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;

namespace SalesOrdersReport
{
    class CommonFunctions
    {
        public static List<ProductLine> ListProductLines;
        public static Int32 SelectedProductLineIndex;
        public static List<String> ListLines, ListSelectedSellers, ListSelectedVendors;
        public static String AppDataFolder;
        public static String MasterFilePath;
        public static ToolStripProgressBar ToolStripProgressBarMainForm;
        public static ToolStripLabel ToolStripProgressBarMainFormStatus;
        public static Form CurrentForm = null;

        public static void Initialize()
        {
            try
            {
                AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SalesOrders";
                if (!Directory.Exists(AppDataFolder)) Directory.CreateDirectory(AppDataFolder);
                SettingsFilePath = CommonFunctions.AppDataFolder + @"\Settings.xml";

                ListProductLines = new List<ProductLine>();
                SelectedProductLineIndex = 1;

                LoadSettingsFile();

                ListSelectedSellers = new List<String>();
                ListSelectedVendors = new List<String>();
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.Initialize()", ex);
            }
        }

        public static void ShowErrorDialog(String Method, Exception ex)
        {
            if (CurrentForm != null)
                MessageBox.Show(CurrentForm, "Following Error Occured in " + Method + ":\n" + ex.Message, "Exception occured", MessageBoxButtons.OK);
            else
                MessageBox.Show("Following Error Occured in " + Method + ":\n" + ex.Message, "Exception occured", MessageBoxButtons.OK);
        }

        public static void ReleaseCOMObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                ShowErrorDialog("ReleaseCOMObject", ex);
            }
            finally
            {
                GC.Collect();
            }
        }

        #region "Get DataTable from Worksheet of an Excel file "
        public static DataTable ReturnDataTableFromExcelWorksheet(String SheetName, String FilePath, String ColumnNames, String UsedRange = "")
        {
            try
            {
                String strConnectionString, strCommandText;
                OleDbConnection conOleDbCon;
                OleDbCommand cmdCommand;

                if (System.IO.Path.GetExtension(FilePath).Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                {
                    strConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1';";
                }
                else
                {
                    strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
                }

                //strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=Excel 14.0";
                strCommandText = "Select " + ColumnNames + " from [" + SheetName + "$" + UsedRange + "]";
                conOleDbCon = new OleDbConnection(strConnectionString);
                cmdCommand = new OleDbCommand(strCommandText, conOleDbCon);
                conOleDbCon.Open();

                OleDbDataAdapter dapAdapter = new OleDbDataAdapter(strCommandText, conOleDbCon);
                DataTable dtbInput = new DataTable();
                dapAdapter.Fill(dtbInput);
                conOleDbCon.Close();
                return dtbInput;
            }
            catch (Exception)
            {
                Console.WriteLine("Error Occured in CommonFunctions.ReturnDataTableFromExcelWorksheet()");
                return null;
            }
        }

        public static Excel.Worksheet GetWorksheet(Excel.Workbook ObjWorkbook, String Sheetname)
        {
            try
            {
                for (int i = 1; i <= ObjWorkbook.Sheets.Count; i++)
                {
                    if (ObjWorkbook.Worksheets[i].Name.Equals(Sheetname, StringComparison.InvariantCultureIgnoreCase))
                        return ObjWorkbook.Worksheets[i];
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.GetWorksheet", ex);
            }
            return null;
        }
        #endregion

        #region "Settings file related methods"
        static XmlDocument SettingXmlDoc;
        static String SettingsFilePath;
        //static Boolean SettingsFileEntryModified = false;
        static XmlNode ProductLinesNode;
        public static ApplicationSettings ObjApplicationSettings;
        public static GeneralSettings ObjGeneralSettings;
        public static ReportSettings ObjInvoiceSettings, ObjQuotationSettings, ObjPurchaseOrderSettings;
        public static ProductMaster ObjProductMaster;
        public static SellerMaster ObjSellerMaster;
        public static VendorMaster ObjVendorMaster;

        public static void LoadSettingsFile()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"\Settings.xml", SettingsFilePath, false);
                }

                SettingXmlDoc = new XmlDocument();
                SettingXmlDoc.Load(SettingsFilePath);

                XmlNode SettingsNode;
                XMLFileUtils.GetChildNode(SettingXmlDoc, "Settings", out SettingsNode);

                XmlNode ApplicationNode;
                XMLFileUtils.GetChildNode(SettingsNode, "Application", out ApplicationNode);
                ObjApplicationSettings = new ApplicationSettings();
                ObjApplicationSettings.ReadSettingsFromNode(ApplicationNode);

                ListProductLines.Clear();
                XMLFileUtils.GetChildNode(SettingsNode, "ProductLines", out ProductLinesNode);
                foreach (XmlNode item in ProductLinesNode.ChildNodes)
                {
                    if (item.NodeType == XmlNodeType.CDATA || item.NodeType == XmlNodeType.Comment) continue;
                    ProductLine ObjProductLine = new ProductLine();
                    if (ObjProductLine.LoadDetailsFromNode(item))
                    {
                        ListProductLines.Add(ObjProductLine);
                    }
                }

                SelectProductLine(SelectedProductLineIndex);

                SettingXmlDoc.NodeChanged += new XmlNodeChangedEventHandler(SettingXmlDoc_NodeChanged);
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.LoadSettingsFile()", ex);
            }
        }

        static void SettingXmlDoc_NodeChanged(object sender, XmlNodeChangedEventArgs e)
        {
            try
            {
                if (e.OldValue.Equals(e.NewValue)) return;
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.SettingXmlDoc_NodeChanged()", ex);
            }
        }

        public static void SelectProductLine(Int32 Index, Boolean Temporary = false)
        {
            try
            {
                ProductLine CurrProductLine;
                if (Temporary)
                {
                    CurrProductLine = ListProductLines[Index];
                }
                else
                {
                    SelectedProductLineIndex = Index;
                    CurrProductLine = ListProductLines[SelectedProductLineIndex];
                }

                ObjGeneralSettings = CurrProductLine.ObjSettings.GeneralSettings;
                ObjInvoiceSettings = CurrProductLine.ObjSettings.InvoiceSettings;
                ObjQuotationSettings = CurrProductLine.ObjSettings.QuotationSettings;
                ObjPurchaseOrderSettings = CurrProductLine.ObjSettings.PurchaseOrderSettings;
                ObjProductMaster = CurrProductLine.ObjProductMaster;
                ObjSellerMaster = CurrProductLine.ObjSellerMaster;
                ObjVendorMaster = CurrProductLine.ObjVendorMaster;
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.SelectProductLine()", ex);
                throw;
            }
        }

        public static void AddNewProductLine(String Name, Int32 UseSettingsOfProductLineIndex)
        {
            try
            {
                XmlNode ProductLineNode = ListProductLines[UseSettingsOfProductLineIndex].ProductLineNode.CloneNode(true);
                ProductLine ObjProductLine = new ProductLine();
                XMLFileUtils.SetAttributeValue(ProductLineNode, "Name", Name);
                ObjProductLine.LoadDetailsFromNode(ProductLineNode);
                ListProductLines.Add(ObjProductLine);
                ProductLinesNode.AppendChild(ProductLineNode);
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.AddNewProductLine()", ex);
            }
        }

        public static Color GetColor(String ColorArgb)
        {
            if (String.IsNullOrEmpty(ColorArgb))
                return Color.Black;
            else
                return Color.FromArgb(Int32.Parse(ColorArgb));
        }

        public static void WriteToSettingsFile()
        {
            try
            {
                if (SettingXmlDoc == null) return;

                ObjApplicationSettings.UpdateSettingsToNode();
                for (int i = 0; i < ListProductLines.Count; i++)
                {
                    ListProductLines[i].ObjSettings.UpdateSettingsToNode();
                }

                SettingXmlDoc.Save(SettingsFilePath);
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.WriteToSettingsFile()", ex);
            }
        }
        #endregion

        public static string GetColorHexCode(Color color)
        {
            return ColorTranslator.ToHtml(color).Replace("#", "");
        }

        public static void ResetProgressBar(Int32 ProgressState = 0, Int32 Maximum = 100, Int32 Step = 1, String Status = "")
        {
            try
            {
                ToolStripProgressBarMainForm.Maximum = Maximum;
                ToolStripProgressBarMainForm.Step = Step;
                ToolStripProgressBarMainForm.Value = ProgressState;
                ToolStripProgressBarMainFormStatus.Text = Status;
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.ResetProgressBar()", ex);
            }
        }

        public static void UpdateProgressBar(Int32 ProgressState)
        {
            try
            {
                if (ToolStripProgressBarMainForm == null || ToolStripProgressBarMainFormStatus == null) return;

                ToolStripProgressBarMainForm.Value = Math.Min(ProgressState, 100);
                ToolStripProgressBarMainFormStatus.Text = Math.Min(ProgressState, 100).ToString() + "%";
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.UpdateProgressBar()", ex);
            }
        }

        public static Boolean ValidateFile_Overwrite_TakeBackup(Form ParentForm, String DirectoryPath, ref String FilePath, 
                                                String ValidFileName, String FileDesc)
        {
            try
            {
                if (FilePath.Length == 0)
                {
                    FilePath = DirectoryPath + @"\" + ValidFileName;
                    if (File.Exists(FilePath))
                    {
                        DialogResult result = MessageBox.Show(ParentForm, "\"" + FilePath + "\" already exist.\nDo you want to append " + FileDesc + " to this file?\nYes - Append to it\nNo - Backup it & Create new",
                                                FileDesc, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        switch (result)
                        {
                            case DialogResult.Cancel: return false;
                            case DialogResult.No:
                                String BackupFilePath = DirectoryPath + @"\" + ValidFileName + ".bkp";
                                if (File.Exists(BackupFilePath)) File.Delete(BackupFilePath);
                                File.Move(FilePath, BackupFilePath);
                                break;
                            case DialogResult.Yes:
                            default: break;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.ValidateFile_Overwrite_TakeBackup()", ex);
                throw;
            }
        }

        public static void ToggleEnabledPropertyOfAllControls(Form CurrentForm, Boolean Override = false, Boolean Enabled = false)
        {
            try
            {
                foreach (Control item in CurrentForm.Controls)
                {
                    item.Enabled = (Override ? Enabled : !item.Enabled);
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog("CommonFunctions.ToggleEnabledPropertyOfAllControls()", ex);
                throw;
            }
        }
    }
}
