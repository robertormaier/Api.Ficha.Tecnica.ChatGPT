namespace Api.Ficha.Tecnica.ChatGPT.Models
{
    public class ChatGptImput
    {
        public ChatGptImput(string prompt)
        {
            this.prompt = $"Ficha técnica do produto: {prompt}";
            this.temperature = 0;
            this.max_tokens = 300;
            this.model = "text-davinci-003";
        }

        public string model { get; set; }
        public string prompt { get; set; }
        public int temperature { get; set; }
        public int max_tokens { get; set; }
    }
}
