using System.ComponentModel.DataAnnotations;

namespace ContractManager.Communication.Dtos.Endpoints.Update
{
    public class UpdateRequest
    {
        [Required]
        public string Id { get; set; } = default!;

        [Required, MaxLength(512)]
        public string Name { get; set; } = default!;

        public DateTime DateOfBirth { get; set; }

        public bool Married { get; set; }

        [MaxLength(13)]
        public string? Phone { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }
    }
}
