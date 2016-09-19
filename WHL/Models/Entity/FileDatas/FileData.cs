using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using WHL.Helpers;
using WHL.Models.Virtual;
using WHL.Models.Entity.Bases;
using WHL.Models.Entity.MapRules;

namespace WHL.Models.Entity.FileDatas
{   
    /// <summary>
    /// Author: Pango
    /// FileData：the import and export file info.
    /// </summary>
    public class FileData : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [LangDisplayName("File Name", ResourceName = "FileData_Entity_Name")]
        public string FileName { get; set; }

        // xls,xlsx,csv
        public int FileType { get; set; }

        // export or import
        public int AssType { get; set; }

        // inventory or delivery
        public int DataType { get; set; }

        // total records from file
        public int TotalCount { get; set; }

        // success records of handing
        public int SuccessCount { get; set; }

        // using map rule ID
        public int MapRuleID { get; set; }
      
        // the time cost of handing
        public int Duration { get; set; }

        // 0 - not process, 1 success, 2 fail
        public int Status { get; set; }

        // Import Process Type  0 - ALL, 1 - New Only , 2 - Update Only
        public int ProcessType { get; set; }


        [NotMapped]
        // lazy load, not map it. post from UI
        public MapRule MapRule { get; set; }

        [NotMapped]
        // lazy load, not map it. post from UI
        public HttpPostedFileBase UploadFile { get; set; }


    


        [LangDisplayName("File Type", ResourceName = "FileData_Entity_FileType")]
        public virtual string FileTypeLayout
        {
            get
            {
                return GetFileTypeLayout(this.FileType);
            }
        }

        public static string GetFileTypeLayout(int fileType)
        {
            switch (fileType)
            {
                case (int)FileTypeEnum.XLS:
                    return LangHelper.Get("FileData_Entity_FileType_Enum_XLS");     // .xls excel 2003 format
                case (int)FileTypeEnum.XLSX:
                    return LangHelper.Get("FileData_Entity_FileType_Enum_XLSX");    // .xlsx excel 2007 above format
                case (int)FileTypeEnum.CSV:
                    return LangHelper.Get("FileData_Entity_FileType_Enum_CSV");     // .csv with delimited format
                default:
                    return "undefined";
            }
        }

        public virtual string FileDownloadPath {
            get {
                return "/" + CommonHelper.USER_FILE_FOLDER + "/" + this.FileName;
            }
        }



        public static string GetFullExportFileName(int fileType,string fileName)
        {
            switch (fileType)
            {
                case (int)FileTypeEnum.XLS:
                    return fileName + ".xls";     // .xls excel 2003 format
                case (int)FileTypeEnum.XLSX:
                    return fileName + ".xlsx";    // .xlsx excel 2007 above format
                case (int)FileTypeEnum.CSV:
                    return fileName + ".csv";     // .csv with delimited format
                default:
                    return "undefined";
            }
        }

        [LangDisplayName("Process Type", ResourceName = "FileData_Entity_ProcessType")]
        public virtual string ProcessTypeLayout
        {
            get
            {
                return GetProcessTypeLayout(this.ProcessType);
            }
        }

        public static string GetProcessTypeLayout(int processType)
        {
            switch (processType)
            {
                case (int)FileProcessTypeEnum.All:
                    return LangHelper.Get("FileData_Entity_ProcessType_Enum_All");
                case (int)FileProcessTypeEnum.NewOnly:
                    return LangHelper.Get("FileData_Entity_ProcessType_Enum_NewOnly");
                case (int)FileProcessTypeEnum.UpdateOnly:
                    return LangHelper.Get("FileData_Entity_ProcessType_Enum_UpdateOnly");     
                default:
                    return "undefined";
            }
        }


    }


    /// <summary>
    /// Author: Pango
    /// File Type
    /// </summary>
    public enum FileTypeEnum
    {
        XLS = 1,
        XLSX = 2,
        CSV = 3,
    }

    /// <summary>
    /// Author: Pango
    /// File Type
    /// </summary>
    public enum FileProcessTypeEnum
    {
        All = 1,
        NewOnly = 2,
        UpdateOnly = 3
    }

    /// <summary>
    /// Author: Pango
    /// File Type
    /// </summary>
    public enum FileStatusEnum
    {
        NotProcess = 0,
        Success = 1,
        Fail = 2
    }

}