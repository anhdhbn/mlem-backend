using System.Collections.Generic;

namespace Common
{
    public interface ICurrentContext : IServiceScoped
    {
        long AccountId { get; set; }
        string Email { get; set; }
        int TimeZone { get; set; }
        string Language { get; set; }
    }
    public class CurrentContext : ICurrentContext
    {
        public long AccountId { get; set; }
        public string Email { get; set; }
        public int TimeZone { get; set; }
        public string Language { get; set; } = "VN";
    }
}
