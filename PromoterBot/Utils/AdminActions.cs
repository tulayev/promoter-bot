using ClosedXML.Excel;

namespace PromoterBot.Utils
{
    public static class AdminActions<T> where T : class
    {
        public static void ExportDataToExcel(List<T> result)
        {
            bool showHeader = true;
            var properties = result.First().GetType().GetProperties();
            var headerNames = properties.Select(prop => prop.Name).ToList();
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");

            if (showHeader)
            {
                for (int i = 0; i < headerNames.Count; i++)
                    ws.Cell(1, i + 1).Value = headerNames[i];

                ws.Cell(2, 1).InsertData(result);
            }
            else
            {
                ws.Cell(1, 1).InsertData(result);
            }

            wb.SaveAs("data.xlsx");
        }
    }
}
