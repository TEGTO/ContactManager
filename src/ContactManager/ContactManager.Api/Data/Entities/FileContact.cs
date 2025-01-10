using CsvHelper.Configuration.Attributes;

namespace ContactManager.Api.Data.Entities
{
    public class FileContact
    {
        [Index(0)]
        public string Name { get; set; } = default!;

        [Index(1)]
        public DateTime DateOfBirth { get; set; }

        [Index(2)]
        public bool Married { get; set; }

        [Index(3)]
        public string? Phone { get; set; }

        [Index(4)]
        public decimal Salary { get; set; }
    }
}
