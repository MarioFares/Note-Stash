using System.Threading.Tasks;

namespace NoteStash.Services
{
    public interface IAiService
    {
        public Task<string> ExpandOnText(string text);
        public Task<string> SummarizeText(string text);
        public Task<string> FixGrammar(string arg);
        public Task<string> ExplainText(string arg);
        public Task<string> GenerateContent(string prompt);
    }
}
