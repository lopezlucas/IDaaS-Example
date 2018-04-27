using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net;

namespace WebAppOkta.Pages
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        private static HttpClient client = new HttpClient();
        private static string baseURL = "http://localhost:59569";

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(baseURL + "/api/token"),
                    Method = HttpMethod.Get
                };
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);

                var result = client.SendAsync(request).Result;

                Message = result.StatusCode == HttpStatusCode.OK ? result.Content.ReadAsStringAsync().Result : "Error HTTP Code: " + result.StatusCode;
            }
        }
    }
}
