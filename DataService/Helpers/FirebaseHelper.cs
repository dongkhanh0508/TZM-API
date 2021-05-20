using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TradeMap.Service.Helpers
{
    public static class FirebaseHelper
    {
        public async static Task SendNotificationAsync(string FcmToken, string Message)
        {
            HttpRequestMessage httpRequest = null;
            HttpClient httpClient = null;
            var googleCredential = (ServiceAccountCredential)GoogleCredential.GetApplicationDefaultAsync().Result.CreateScoped("https://www.googleapis.com/auth/firebase.messaging").UnderlyingCredential;
            await googleCredential.GetAccessTokenForRequestAsync();
            try
            {
                var data = new
                {
                    message = new
                    {
                        token = FcmToken,
                        notification = new
                        {
                            title = "New Notification",
                            body = Message
                        },
                    }
                };
                var jsonBody = JsonConvert.SerializeObject(data);

                httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/v1/projects/loginkhanhnd/messages:send");
                httpRequest.Headers.TryAddWithoutValidation("Authorization", "Bearer " + googleCredential.Token.AccessToken);
                httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using (httpClient = new HttpClient())
                {
                    await httpClient.SendAsync(httpRequest);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                httpRequest.Dispose();
                httpClient.Dispose();
            }
        }
    }
}