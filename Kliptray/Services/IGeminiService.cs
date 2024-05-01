using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Kliptray.Services;

public interface IGeminiService
{
    Task<string> PromptText(string text);
    Task<string> PromptImage(string text, IRandomAccessStreamWithContentType StreamReference);
}
