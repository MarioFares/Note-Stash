using Google.GenAI;
using NoteStash.Models;
using System.Threading.Tasks;

namespace NoteStash.Services
{
    public class AiService : IAiService
    {
        private readonly Settings _settings;
        private readonly Client _client;

        public AiService(Settings settings)
        {
            _settings = settings;
            if (!string.IsNullOrEmpty(_settings.AiApiKey))
            {
                _client = new Client(apiKey: _settings.AiApiKey);
            }
        }

        private async Task<string> GenerateContent(string prompt)
        {
            if (_client == null)
            {
                return "ERROR: AI client not initialized. Please set your API key in the settings.";
            }
            var response = await _client.Models.GenerateContentAsync(model: _settings.AiModelName, prompt);
            return response?.Candidates?[0]?.Content?.Parts?[0].Text ?? "";
        }

        public async Task<string> ExpandOnText(string text) => await GenerateContent("Expand on the following: " + text);

        public async Task<string> SummarizeText(string text) => await GenerateContent("Summarize the following: " + text);

        public async Task<string> FixGrammar(string arg) => await GenerateContent("Fix the grammar and spelling of the following text: " + arg);

        public Task<string> ExplainText(string arg) => GenerateContent("Explain the following text in simple terms: " + arg);
    }
}
