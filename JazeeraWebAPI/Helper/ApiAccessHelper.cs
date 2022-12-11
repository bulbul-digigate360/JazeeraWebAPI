using JazeeraWebAPI.Models;
using Newtonsoft.Json;
using TripLover.AirCommonModels;

namespace JazeeraWebAPI.Helper;

public abstract class ApiAccessHelper
{
    
    public static async Task<string> ApiAuthRequest(AuthRequest model, string baseUrl, string methodUrl)
    {
        try
        {
            Dictionary<string, string> contents = new Dictionary<string, string>();
            contents.Add("grant_type", model.grant_type);
            contents.Add("scope", model.scope);
            contents.Add("client_id", model.client_id);
            contents.Add("client_secret", model.client_secret);
            using (var httpClient = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Post, baseUrl + methodUrl) { Content = new FormUrlEncodedContent(contents) };
                using (var response = await httpClient.SendAsync(req))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return apiResponse;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
     public static async Task<string> GetAccessToken(ACMApiCredential apiCredential, string UniqueTransID)
            {
                var authRequest = new AuthRequest() { client_id = apiCredential.UserName, client_secret = apiCredential.Password.Trim(), grant_type = "client_credentials", scope = "FlightEngine" };
                FileHelper.ToWriteJson($"Auth-{UniqueTransID}-Req", "Auth", JsonConvert.SerializeObject(authRequest));
                var authResponseJson = await ApiAuthRequest(authRequest, apiCredential.OtherUrl ?? "https://api.gfa-hub.com/", "connect/token");
                var authResObj = JsonConvert.DeserializeObject<AuthResponse>(authResponseJson);
                if (authResObj != null && string.IsNullOrEmpty(authResObj.error))
                {
                    FileHelper.ToWriteJson($"Auth-{UniqueTransID}-Rsp", "Auth", authResponseJson);
                    return authResObj.access_token;
                }
                else
                {
                    FileHelper.ToWriteJson($"Auth-{UniqueTransID}-Rsp", "Auth", authResponseJson);
                    return null;
                }
            }
    
}