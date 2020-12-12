using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using Tenjin.Sys.Apis.Models;
using Tenjin.Sys.Helpers;

namespace Tenjin.Sys.Apis.ExcelReaders
{
    public class StudentReader : SpreadsheetReader<StudentModel>
    {
        public StudentReader(Stream stream) : base(stream)
        {
        }
        protected override StudentModel Convert(Row row)
        {
            var maps = row.ToColumnDictionary();
            return new StudentModel
            {
                Code = GetString(maps.Cell(1)),
                Name = GetString(maps.Cell(2)),
                DateOfBirth = GetDate(maps.Cell(3), "dd/MM/yyyy").ToString("yyyy-MM-dd"),
                Email = GetString(maps.Cell(4)),
                Phone = GetString(maps.Cell(5)),
                CenterCode = GetString(maps.Cell(6)),
                Course = GetString(maps.Cell(8)),
                MajorCode = GetString(maps.Cell(9)),
                ClassCode = GetString(maps.Cell(11))
            };
        }
    }
}
