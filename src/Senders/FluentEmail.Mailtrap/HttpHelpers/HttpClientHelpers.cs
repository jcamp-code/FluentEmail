using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using FluentEmail.Core;
using System.Net.Http.Headers;

namespace FluentEmail.Mailtrap.HttpHelpers
{
    public class HttpClientHelpers
    {
   
        public static HttpContent GetJsonBody(object value)
        {
            return new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        }

    }

    public static class HttpClientExtensions
    { 

        public static async Task<ApiResponse<T>> Post<T>(this HttpClient client, string url, HttpContent httpContent)
        {
            var response = await client.PostAsync(url, httpContent);
            var qr = await QuickResponse<T>.FromMessage(response);
            return qr.ToApiResponse();
        }
      
    }

    public class QuickResponse
    {
        public HttpResponseMessage Message { get; set; }

        public string ResponseBody { get; set; }

        public IList<ApiError> Errors { get; set; }

        public QuickResponse()
        {
            Errors = new List<ApiError>();
        }

        public ApiResponse ToApiResponse()
        {
            return new ApiResponse
            {
                Errors = Errors
            };
        }

        public static async Task<QuickResponse> FromMessage(HttpResponseMessage message)
        {
            var response = new QuickResponse();
            response.Message = message;
            response.ResponseBody = await message.Content.ReadAsStringAsync();

            if (!message.IsSuccessStatusCode)
            {
                response.HandleFailedCall();
            }

            return response;
        }

        protected void HandleFailedCall()
        {
            try
            {
                Errors = JsonSerializer.Deserialize<List<ApiError>>(ResponseBody) ?? new List<ApiError>();

                if (!Errors.Any())
                {
                    Errors.Add(new ApiError
                    {
                        ErrorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message.StatusCode.ToString()
                    });
                }
            }
            catch (Exception)
            {
                Errors.Add(new ApiError
                {
                    ErrorMessage = !string.IsNullOrEmpty(ResponseBody) ? ResponseBody : Message.StatusCode.ToString()
                });
            }
        }
    }

    public class QuickResponse<T> : QuickResponse
    {
        public T Data { get; set; }

        public new ApiResponse<T> ToApiResponse()
        {
            return new ApiResponse<T>
            {
                Errors = Errors,
                Data = Data
            };
        }

        public new static async Task<QuickResponse<T>> FromMessage(HttpResponseMessage message)
        {
            var response = new QuickResponse<T>();
            response.Message = message;
            response.ResponseBody = await message.Content.ReadAsStringAsync();

            if (message.IsSuccessStatusCode)
            {
                try
                {
                    response.Data = JsonSerializer.Deserialize<T>(response.ResponseBody);
                }
                catch (Exception)
                {
                    response.HandleFailedCall();
                }
            }
            else
            {
                response.HandleFailedCall();
            }

            return response;
        }
    }

    public class QuickFile : QuickResponse<Stream>
    {
        public new static async Task<QuickFile> FromMessage(HttpResponseMessage message)
        {
            var response = new QuickFile();
            response.Message = message;
            response.ResponseBody = await message.Content.ReadAsStringAsync();

            if (message.IsSuccessStatusCode)
            {
                response.Data = await message.Content.ReadAsStreamAsync();
            }
            else
            {
                response.HandleFailedCall();
            }

            return response;
        }
    }
}
