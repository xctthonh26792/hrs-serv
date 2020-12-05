using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Tenjin.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tenjin.Sys.Helpers
{
    public abstract class SpreadsheetReader<T>
    {
        protected readonly SpreadsheetDocument _document;
        public class SpreadsheetModel
        {
            public int Row { get; set; }
            public T Model { get; set; }
        }
        public SpreadsheetReader(Stream stream)
        {
            _document = SpreadsheetDocument.Open(stream, false);
        }

        protected abstract T Convert(Row row);

        public List<SpreadsheetModel> Parse(int skip = 1)
        {
            var part = _document.WorkbookPart.WorksheetParts.FirstOrDefault();
            var sheet = part.Worksheet.GetFirstChild<SheetData>();
            var models = new List<SpreadsheetModel>();
            int index = skip;
            foreach (Row row in sheet.Descendants<Row>().Skip(skip))
            {
                index++;
                var model = Convert(row);
                if (model != null)
                {
                    models.Add(new SpreadsheetModel
                    {
                        Row = index,
                        Model = Convert(row)
                    });
                }
            }
            return models;
        }

        protected double GetDouble(Cell cell, double def = 0)
        {
            if (cell == null)
            {
                return def;
            }
            return TenjinConverts.GetDouble(GetString(cell));
        }


        protected int GetInt32(Cell cell, int def = 0)
        {
            if (cell == null)
            {
                return def;
            }
            return TenjinConverts.GetInt32(GetString(cell));
        }

        protected bool GetBoolean(Cell cell, bool def = false)
        {
            if (cell == null)
            {
                return def;
            }
            return GetString(cell, "0") != "0";
        }

        protected string GetString(Cell cell, string def = "")
        {
            if (cell == null)
            {
                return def;
            }
            var shared = _document.WorkbookPart.SharedStringTablePart;
            var value = cell.CellValue?.InnerXml ?? string.Empty;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return shared.SharedStringTable.ChildElements[TenjinConverts.GetInt32(value)].InnerText;
            }
            return value;
        }

        protected DateTime GetDate(Cell cell)
        {
            return DateTime.FromOADate(TenjinConverts.GetDouble(GetString(cell))).Date;
        }

        protected DateTime GetDate(Cell cell, string format)
        {
            return TenjinConverts.GetDateTimeExact(GetString(cell), format);
        }
    }

    public static class SpreadsheetReaderExtensiosn
    {
        private static string Column(int column)
        {
            var response = string.Empty;
            while (column > 0)
            {
                var modulo = (column - 1) % 26;
                response = Convert.ToChar(65 + modulo).ToString() + response;
                column = (column - modulo) / 26;
            }
            return response;
        }
        public static Cell Cell(this Row row, int index)
        {
            if (row == null) return null;
            var reference = $"{Column(index)}{row.RowIndex}";
            return row.Elements<Cell>().FirstOrDefault(x => x.CellReference.InnerText == reference);
        }

        public static Dictionary<int, Cell> ToColumnDictionary(this Row row)
        {
            if (row == null) return null;
            return row.Elements<Cell>().ToDictionary((cell) => column(cell.CellReference.InnerText));

            int column(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                var reference = Regex.Replace(value.ToUpper(), @"[\d]", string.Empty);
                int col = -1;
                int multiplier = 1;
                foreach (char c in reference.ToCharArray().Reverse())
                {
                    col += multiplier * ((int)c - 64);

                    multiplier = multiplier * 26;
                }
                return col + 1;
            }
        }

        public static Cell Cell(this Dictionary<int, Cell> maps, int index)
        {
            if (maps == null) return null;
            return maps.TryGetValue(index, out var response) ? response : null;
        }
    }
}
