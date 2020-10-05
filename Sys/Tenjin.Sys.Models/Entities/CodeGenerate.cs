using Tenjin.Models.Entities;

namespace Tenjin.Sys.Models.Entities
{
    public class CodeGenerate : BaseEntity
    {
        public string Code { get; set; }
        public int Count { get; set; }
    }
}
