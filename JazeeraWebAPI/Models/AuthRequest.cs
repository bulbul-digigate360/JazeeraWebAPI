namespace JazeeraWebAPI.Models;

public class AuthRequest
{
    public string client_id { get; internal set; }
    public string client_secret { get; internal set; }
    public string grant_type { get; internal set; }
    public string scope { get; internal set; }
}