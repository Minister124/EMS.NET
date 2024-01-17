using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public List<Employee>? Employees { get; set; } //One to Many Relationship
    }
}
