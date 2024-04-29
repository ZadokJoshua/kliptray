using System.Threading.Tasks;

namespace Kliptray.Services;

public interface IGeminiService
{
    Task<string> PromptText(string text);
}
