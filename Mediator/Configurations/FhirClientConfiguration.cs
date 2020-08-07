namespace Mediator.Configurations
{
    public class FhirClientConfiguration
    {
        public string BaseAddress { get; set; }
        public string Thumbprint { get; set; }
        public int TimeoutMilliseconds { get; set; }
    }
}