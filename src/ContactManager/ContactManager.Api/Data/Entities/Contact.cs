using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Api.Data.Entities
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = default!;

        [Required, MaxLength(512)]
        public string Name { get; set; } = default!;

        public DateTime DateOfBirth { get; set; }

        public bool Married { get; set; }

        [MaxLength(13)]
        public string? Phone { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Salary { get; set; }

        public void Copy(Contact other)
        {
            this.Name = other.Name;
            this.DateOfBirth = other.DateOfBirth;
            this.Married = other.Married;
            this.Phone = other.Phone;
            this.Salary = other.Salary;
        }
    }
}
