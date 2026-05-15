using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using acepickle_chat_api.Repository;
using static acepickle_chat_api.Dto.DTORoomResponse;

namespace acepickle_chat_api.Helper
{
    public class NotificationService
    {
        private readonly string _projectId = "become-a-skiller-dev";
        public NotificationService()
        {

        }

        public async Task<int> SendNotification(List<string> deviceTokens, string title, string body, NotificationData notificationData)
        {
            if (deviceTokens == null || deviceTokens.Count == 0)
            {
                return 0;
            }

            int successCount = 0;

            try
            {
                GoogleCredential credential = GoogleCredential
                .FromFile("Fcm/service_account.json")
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                object dataPayload = notificationData.RoomType == 1
                ? new                                           // Private chat
                {
                    screen = "PrivateChatList",
                    requestedUserId = notificationData.SenderId.ToString()
                }
                : new                                           // Group chat
                {
                    screen = "GroupChatView",
                    roomId = notificationData.RoomId,
                    batchId = notificationData.BatchId.ToString()
                };

                foreach (var token in deviceTokens)
                {
                    try
                    {
                        var message = new
                        {
                            message = new
                            {
                                token = token,
                                notification = new
                                {
                                    title = title,
                                    body = body
                                },
                                data = dataPayload
                            }
                        };

                        var json = JsonConvert.SerializeObject(message);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";
                        var response = await httpClient.PostAsync(url, content);
                        var responseContent = await response.Content.ReadAsStringAsync();

                        Console.WriteLine($"Token: {token}, Status: {response.StatusCode}, Response: {responseContent}");

                        if (response.IsSuccessStatusCode)
                            successCount++;
                    }
                    catch (System.Exception ex)
                    {
                        throw new HubException(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }

            return successCount;
        }
    }
}