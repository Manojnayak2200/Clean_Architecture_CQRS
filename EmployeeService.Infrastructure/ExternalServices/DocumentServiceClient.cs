using EmployeeService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static EmployeeService.Application.Interfaces.IEmployeeDbContext;

namespace EmployeeService.Infrastructure.ExternalServices
{
    public class DocumentServiceClient : IDocumentServiceClient
    {
        private readonly HttpClient _httpClient;
        string BaseURL = "//localhost:7000/";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DocumentServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<DocumentSummaryDto>> GetDocumentsByUserIdAsync()
        {
            try
            {
                // 1. 🟢 FORWARD THE JWT FROM THE INCOMING REQUEST
                var authHeader = _httpContextAccessor.HttpContext?
                    .Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(authHeader))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        AuthenticationHeaderValue.Parse(authHeader);
                }


                var response = await _httpClient.GetAsync($"{BaseURL}Documents/MyDocuments");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Token is missing, invalid, or expired.
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    // Authenticated, but access is not permitted.
                }

                if (!response.IsSuccessStatusCode)
                    return new List<DocumentSummaryDto>();

                var docs = await response.Content
                    .ReadFromJsonAsync<List<DocumentSummaryDto>>();

                return docs ?? new List<DocumentSummaryDto>();
            }
            catch (HttpRequestException)
            {
                // Graceful fallback if Document Service is down
                return new List<DocumentSummaryDto>();
            }
        }
    }
}
