using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using Tenjin.Sys.Apis.Models;
using Tenjin.Sys.Helpers;

namespace Tenjin.Sys.Apis.ExcelReaders
{
    public class EmployeeReader : SpreadsheetReader<EmployeeModel>
    {
        public EmployeeReader(Stream stream) : base(stream)
        {
        }
        protected override EmployeeModel Convert(Row row)
        {
            var maps = row.ToColumnDictionary();
            var code = GetString(maps.Cell(1));
            if (Tenjin.Helpers.TenjinUtils.IsStringEmpty(code))
            {
                return null;
            }
            return new EmployeeModel
            {
                Code = code,
                Name = GetString(maps.Cell(2)),
                DateOfBirth = GetDate(maps.Cell(3), "dd/MM/yyyy").ToString("yyyy-MM-dd"),
                Email = GetString(maps.Cell(4)),
                Phone = GetString(maps.Cell(5)),
                FacutlyCode = GetString(maps.Cell(6)),
                MajorCode = GetString(maps.Cell(8)),
                LevelCode = GetString(maps.Cell(10))
            };
        }
    }
}
