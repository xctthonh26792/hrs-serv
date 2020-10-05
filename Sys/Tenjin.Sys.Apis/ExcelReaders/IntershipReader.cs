using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using Tenjin.Sys.Apis.Models;
using Tenjin.Sys.Helpers;


namespace Tenjin.Sys.Apis.ExcelReaders
{
    public class IntershipReader : SpreadsheetReader<IntershipModel>
    {
        public IntershipReader(Stream stream) : base(stream)
        {
        }
        protected override IntershipModel Convert(Row row)
        {
            var maps = row.ToColumnDictionary();
            return new IntershipModel
            {
                Code = GetString(maps.Cell(1)),
                StudentCode = GetString(maps.Cell(2)),
                FacutlyCode  = GetString(maps.Cell(4)),
                Start = GetDate(maps.Cell(6), "dd/MM/yyyy").ToString("yyyy-MM-dd"),
                End = GetDate(maps.Cell(7), "dd/MM/yyyy").ToString("yyyy-MM-dd")
            };
        }
    }
}
