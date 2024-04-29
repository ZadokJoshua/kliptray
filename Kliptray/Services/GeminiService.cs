using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Config;
using Kliptray.Models;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Kliptray.Services;

public class GeminiService : IGeminiService
{
    private readonly IGeminiClient _geminiClient;
    public GeminiService()
    {
        GoogleGeminiConfig config = new GoogleGeminiConfig()
        {
            ApiKey = AppSettings.API_KEY
        };

        _geminiClient = new GeminiClient(config);
    }

    public async Task<string> PromptText(string text)
    {
        var response = await _geminiClient.TextPrompt(text);
        return response.Candidates[0].Content.Parts[0].Text;
    }
}