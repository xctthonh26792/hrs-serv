using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using Tenjin.Sys.Apis.Models;
using Tenjin.Sys.Helpers;

namespace Tenjin.Sys.Apis.ExcelReaders
{
    public class EmployeeCourseReader : SpreadsheetReader<EmployeeCourseModel>
    {
        public EmployeeCourseReader(Stream stream) : base(stream)
        {
        }
        protected override EmployeeCourseModel Convert(Row row)
        {
            var maps = row.ToColumnDictionary();
            return new EmployeeCourseModel
            {
                Code = GetString(maps.Cell(1)),
                EmployeeCode = GetString(maps.Cell(2)),
                CourseCode = GetString(maps.Cell(4))
            };
        }
    }
}
