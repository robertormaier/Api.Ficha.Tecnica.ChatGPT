using Api.Ficha.Tecnica.ChatGPT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Api.Ficha.Tecnica.ChatGPT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FichaTecnicaController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public FichaTecnicaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string texto, [FromServices] IConfiguration configuration)
        {
            var result = await RealizaRequestParaApiChatGPTAsync(texto, configuration);

            return Ok(result.choices.First().text.Replace("\n", " ").Replace("\t", " "));
        }

        private async Task<ChatGptResponse> RealizaRequestParaApiChatGPTAsync(string texto, IConfiguration configuration)
        {
            StringContent content = MontarRequest(texto, configuration);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/completions", content);

            return await response.Content.ReadFromJsonAsync<ChatGptResponse>();
        }

        private StringContent MontarRequest(string texto, IConfiguration configuration)
        {
            var toke = configuration.GetValue<string>("ChatGPTSecretKey");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", toke);

            var model = new ChatGptImput(texto);

            var requestBody = JsonSerializer.Serialize(model);

            var content = new StringContent(requestBody, Encoding.UTF8, "´8application/json");
            return content;
        }
    }
}
