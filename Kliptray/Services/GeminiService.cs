using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Config;
using DotnetGeminiSDK.Model;
using Kliptray.Helpers;
using Kliptray.Models;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Kliptray.Services;

public class GeminiService : IGeminiService
{
    private readonly IGeminiClient _geminiClient;
    public GeminiService()
    {
        GoogleGeminiConfig config = new()
        {
            ApiKey = AppSettings.API_KEY
        };

        _geminiClient = new GeminiClient(config);
    }

    public async Task<string> PromptImage(string text, IRandomAccessStreamWithContentType StreamReference)
    {
        try
        {
            var image = (StreamReference != null) ? File.ReadAllBytes(await ImageHelper.ConvertBytesToPngAsync(StreamReference)) : throw new Exception();
            var response = await _geminiClient.ImagePrompt(text, image, ImageMimeType.Png);
            return response.Candidates[0].Content.Parts[0].Text;
        }
        catch (Exception ex)
        {
            throw;
        }


    }

    public async Task<string> PromptText(string text)
    {
        try
        {
            var response = await _geminiClient.TextPrompt(text);
            return response.Candidates[0].Content.Parts[0].Text;
        }
        catch (Exception)
        {
            throw;
        }
    }
}