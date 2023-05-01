using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace droneService.Controllers
{
    [Route("login")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> logger;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IHttpClientFactory clientFactory;

        public IndexController(ILogger<IndexController> logger, IHttpContextAccessor contextAccessor, IHttpClientFactory clientFactory)
        {
            this.logger = logger;
            this.contextAccessor = contextAccessor;
            this.clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //?code=f5f9967b17b7f040e807&state=d5104dc76695721d
            var requet = contextAccessor!.HttpContext.Request;      
            var code = requet.Query["code"].ToString();
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var dictionary = await GetRequestBodyAsync(code);
            var requestMessage = GetRequestMessage(dictionary);
            requestMessage.RequestUri = new Uri("https://github.com/login/oauth/access_token");
            var response = await client.SendAsync(requestMessage);
            var responseMessage = await response.Content.ReadAsStringAsync();
            logger.LogInformation(responseMessage);
            return Redirect("http://192.168.30.128");
        }

        private async Task<Dictionary<string, object>> GetRequestBodyAsync(string code)
        {
            var jsonDic = new Dictionary<string, object>
            {
                { "client_id", "695f5f5a39e8e280102b" },
                { "client_secret", "08797088d803ba7038b862ee08e68a3632e64ce2" },
                { "code", code },
                //{ "redirect_uri", "http://fi9vru.natappfree.cc/index" }
            };
            return jsonDic;
        }
        private HttpRequestMessage GetRequestMessage(Dictionary<string, object> dics)
        {
            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            var json = JsonSerializer.Serialize(dics);
            message.Content = new StringContent(json, Encoding.UTF8);
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return message;
        }

    }
}
