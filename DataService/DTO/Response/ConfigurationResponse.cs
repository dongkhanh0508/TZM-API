namespace TradeMap.Service.DTO.Response
{
    public class ConfigurationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public int? Version { get; set; }
        public bool? Active { get; set; }
        public string Description { get; set; }
    }
}
