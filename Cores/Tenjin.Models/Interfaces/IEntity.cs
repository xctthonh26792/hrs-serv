using System;

namespace Tenjin.Models.Interfaces
{
    public interface IEntity
    {
        string DefCode { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime LastModified { get; set; }
        bool IsPublished { get; set; }
        string ValueToSearch { get; }
    }
}
