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
        private const string OpenAiApiUrl = "https://api.openai.com/v1/completions";
        private readonly HttpClient _httpClient;

        public FichaTecnicaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string texto, [FromServices] IConfiguration configuration)
        {
            try
            {
                var result = await SendRequestToOpenAiApiAsync(texto, configuration);

                return Ok(result.choices.First().text.Replace("\n", " ").Replace("\t", " "));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<ChatGptResponse> SendRequestToOpenAiApiAsync(string texto, IConfiguration configuration)
        {
            using var content = CreateRequestContent(texto, configuration);
            using var response = await _httpClient.PostAsync(OpenAiApiUrl, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatGptResponse>();
        }

        private StringContent CreateRequestContent(string texto, IConfiguration configuration)
        {
            var toke = configuration.GetValue<string>("ChatGPTSecretKey");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", toke);

            var model = new ChatGptImput(texto);

            var requestBody = JsonSerializer.Serialize(model);

            return new StringContent(requestBody, Encoding.UTF8, "application/json");
        }
    }

}
