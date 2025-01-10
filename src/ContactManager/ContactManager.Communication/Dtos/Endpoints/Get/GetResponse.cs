namespace ContractManager.Communication.Dtos.Endpoints.Get
{
    public class GetResponse
    {
        public IEnumerable<ContactResponse> Data { get; set; } = Enumerable.Empty<ContactResponse>();
        public int TotalCount { get; set; }
    }
}
