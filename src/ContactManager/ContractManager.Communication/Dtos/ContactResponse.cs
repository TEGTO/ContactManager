namespace ContractManager.Communication.Dtos
{
    public class ContactResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool Married { get; set; }
        public string? Phone { get; set; }
        public decimal Salary { get; set; }
    }
}
