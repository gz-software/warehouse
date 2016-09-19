using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Text;
using System.Collections.Generic;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.Util;


using WHL.Models.Virtual;
using WHL.Models.Entity.FileDatas;
using WHL.Models.Entity.MapRules;


namespace WHL.Helpers
{
    /// <summary>
    ///  Excel Helpers: handling all excel import and export methods here. 
    /// </summary>
    public class ExcelHelper{
       

        /// <summary>
        /// Import the excel and return specifid List
        /// </summary>
        /// <param name="entityType">the data entity type</param>
        /// <param name="s">excel stream</param>
        /// <param name="ruleItemList">mapping rule list</param>
        /// <param name="processCount">the ref imported process count</param>
        /// <returns></returns>
        public static List<Object> GetImportDataList(Type entityType, FileData fileData, out int processCount)
        {
            ICollection<MapRuleItem> ruleItemList=fileData.MapRule.RuleItemList;
            List<Object> dataList = new List<Object>(); // the return data List
            processCount = 0;
            try
            {
                IWorkbook workbook = null;
                ISheet sheet = null;
                int startRow = 1; // 我们认为所有EXCEL都有HEADER


                if (fileData.FileType==(int)FileTypeEnum.XLSX){// 2007
                    workbook = new XSSFWorkbook(fileData.UploadFile.InputStream);
                }

                 if (fileData.FileType==(int)FileTypeEnum.XLS){// 2003
                    workbook = new HSSFWorkbook(fileData.UploadFile.InputStream);
                }

                 if (fileData.FileType == (int)FileTypeEnum.CSV) {// csv
                     // csv 处理
                 } 

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);
                }

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    int rowCount = sheet.LastRowNum;//最后一列的标号
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        Object entity = System.Activator.CreateInstance(entityType);
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　
                        foreach (MapRuleItem ruleItem in ruleItemList)
                        {
                            int j = ruleItem.Sorting;       // 获得这个ruleItem的顺序，它对应你EXCEL的位置
                            var column = ruleItem.Column;   // 获得这个列的对象化名字（对象属性名称）

                            if (row.GetCell(j) == null){ // 如果mapping有定义一个列，但EXCEL并没有这个列，要跳过
                                continue;
                            }
                            var value = ConvertCellValue(row.GetCell(j).ToString());    // 获得EXCEL中，这个第J列的值,这里要转换值类型，比如string的要转int
                            string error = LangHelper.Get("ExcelHelper_Result_Error_ImportRowError");
                            error = string.Format(error,i);
                            if (String.IsNullOrEmpty(value.ToString())) { // 一个异常捕捉的例子
                                throw new Exception(error);
                            }

                            CommonHelper.SetObjectValue(entity, column, value);         // 赋值
                        }// end foreach
                        processCount = i;      // 告知外部目前处理到第几条
                        dataList.Add(entity);  // 把它增加进数组。
                    }//end for
                }//end if
                
                return dataList;
            }
            catch (Exception e)
            {
                throw new Exception("Import Fail:" + e.Message,e.InnerException);

            }
        }
        



        /// <summary>
        /// 导出方法，这里负责导出EXCEL
        /// </summary>
        /// <param name="dataList">需要导出的数据集合</param>
        /// <param name="ruleItemList">导出的规则</param>
        /// <returns>返回含有Excel的Memory Stream</returns>
        public static MemoryStream GetExportMemoryStream(Type entityType, List<Object> dataList, FileData fileData, out int processCount)
        {
            ICollection<MapRuleItem> ruleItemList = fileData.MapRule.RuleItemList;
            processCount = 0;
            try
            {
                MemoryStream ms = new MemoryStream();

                IWorkbook workbook = null;//Create an excel Workbook
                if (fileData.FileType == (int)FileTypeEnum.XLSX)
                {// 2007
                    workbook = new XSSFWorkbook();
                }

                if (fileData.FileType == (int)FileTypeEnum.XLS)
                {// 2003
                    workbook = new HSSFWorkbook();
                }

                if (fileData.FileType == (int)FileTypeEnum.CSV)
                {// csv
                    // csv 处理
                } 
                ISheet sheet = workbook.CreateSheet();//Create a work table in the table
                IRow headerRow = sheet.CreateRow(0); //To add a row in the table

              

                // create the header
                foreach (MapRuleItem ruleItem in ruleItemList)
                {
                    String headerName = ruleItem.Name;
                    int sorting = ruleItem.Sorting;
                    headerRow.CreateCell(sorting).SetCellValue(headerName);
                    sheet.SetColumnWidth(sorting, 20 * 256);
                }

                // create the rows
                int rowIndex = 1;

                for (int i = 0; i < dataList.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (MapRuleItem ruleItem in ruleItemList)
                    {
                        String columnName = ruleItem.Column;
                        String columnValue = CommonHelper.GetObjectValue(dataList[i], columnName);
                        if (columnValue == null)
                        {
                            String errMsg =LangHelper.Get("ExcelHelper_Result_Error_ImportColumnError"); 
                            errMsg = string.Format(errMsg, columnName, rowIndex);
                            throw new Exception("Export Fail:" + errMsg);
                        }
                        int sorting = ruleItem.Sorting;
                        dataRow.CreateCell(sorting).SetCellValue(columnValue);
                    }
                    rowIndex++;
                    processCount = i + 1;      // 告知外部目前处理到第几条
                }
                workbook.SetSheetName(0, entityType.Name);
                workbook.Write(ms);
                //ms.Flush();
                //ms.Position = 0;

                return ms;

            }
            catch (Exception e)
            {
                throw new Exception("Export Fail:" + e.Message, e.InnerException);

            }

        }


        /// <summary>
        /// Convert the cell value(string)  to correct data type return
        /// Ex: Convert number cell to int and return.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ConvertCellValue(String value)
        {

            value = value.ToString().Trim();

            int intResult;
            bool isInt = int.TryParse(value, out intResult);
            if (isInt)
            {
                return Int32.Parse(value);
            }

            double doubleResult;
            bool isDouble = double.TryParse(value, out doubleResult);
            if (isDouble)
            {
                return double.Parse(value);
            }
            return value;
        }

    }




}

