namespace KKHCleanBus.MicroServices.Models
{
    public class CorsConfig
    {
        public string[]? Origins { get; set; }
        public string[]? Headers { get; set; }
        public string[]? Methods { get; set; }
    }
}
