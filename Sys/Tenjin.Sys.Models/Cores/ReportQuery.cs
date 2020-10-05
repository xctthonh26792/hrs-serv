using MongoDB.Bson;
using System.Collections.Generic;

namespace Tenjin.Sys.Models.Cores
{
    public class ReportQuery
    {
        public string FacutlyCode { get; set; }

        public IEnumerable<ObjectId> CenterCodes { get; set; }

        public IEnumerable<ObjectId> EmployeeCodes { get; set; }

        public IEnumerable<ObjectId> FacutlyCodes { get; set; }

        public string Start { get; set; }

        public string End { get; set; }
    }
}
