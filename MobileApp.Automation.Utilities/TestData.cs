using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace MobileApp.Automation.Utilities
{
    public class TestData
    {
        public static String fetch(String columnName)
        {
            String returnString = null;
            try
            {
                Dictionary<String, Dictionary<String, Dictionary<String, String>>> sheetCollection = CommonMethods.CommonMethods.sheetCollectionData;                
                String sheetName = CommonMethods.CommonMethods.currentClassName;
                String TestCaseRowKey = CommonMethods.CommonMethods.currentTestCaseName;
                foreach (String sheet in sheetCollection.Keys)
                {
                    if (sheet.Equals(sheetName))
                    {
                        Dictionary<String, Dictionary<String, String>> sheet1 = sheetCollection[sheet];
                        foreach (String RowKey in sheet1.Keys)
                        {
                            if (RowKey.Equals(TestCaseRowKey))
                            {
                                Dictionary<String, String> rowCollection = sheet1[RowKey];
                                foreach (String CellKey in rowCollection.Keys)
                                {
                                    if (CellKey.Equals(columnName))
                                    {
                                        returnString = rowCollection[CellKey];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
            }
            return returnString;
        }

        public static void TestDataInitialize()
        {
            string filePath = Config.testDataFilePath;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found!");
                return;
            }            
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var workbook = package.Workbook;
                Dictionary<String, Dictionary<String, Dictionary<String, String>>> sheetCollection = new Dictionary<String, Dictionary<String, Dictionary<String, String>>>();
                String sheetNameKey = "";
                Dictionary<String, Dictionary<String, String>> TableValueCollection;
                for (int index = 0; index < workbook.Worksheets.Count; index++)
                {
                    Dictionary<String, String> rowValueCollection;
                    String rowTestCaseKey = "";
                    sheetNameKey = workbook.Worksheets[index].Name;
                    var sheet = workbook.Worksheets[sheetNameKey];
                    TableValueCollection = new Dictionary<String, Dictionary<String, String>>();
                    int totalRows = sheet.Dimension.End.Row;
                    for (int rowindex = 2; rowindex <= totalRows; rowindex++)
                    {
                        rowTestCaseKey = (String)sheet.Cells[rowindex, 1].Value;
                        rowValueCollection = new Dictionary<String, String>();
                        int totalColumns = sheet.Dimension.End.Column;
                        for (int colindex = 2; colindex <= totalColumns; colindex++)
                        {
                            String colKey = (String)sheet.Cells[1, colindex].Value;
                            String colValue = (String)sheet.Cells[rowindex, colindex].Value;
                            rowValueCollection.Add(colKey, colValue);
                        }
                        TableValueCollection.Add(rowTestCaseKey, rowValueCollection);
                    }
                    sheetCollection.Add(sheetNameKey, TableValueCollection);
                }
                CommonMethods.CommonMethods.sheetCollectionData = sheetCollection;
            }
        }
    }
}
