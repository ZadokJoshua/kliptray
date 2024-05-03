using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage.Streams;

namespace Kliptray.Models;

public record ClipboardItem
{
    public string? Id { get; set; }
    public string ItemId { get; set; } = Guid.NewGuid().ToString();

    public DateTimeOffset TimeStamp { get; set; }

    public BitmapImage? Image { get; set; }

    public string? Text { get; set; }

    public bool IsImage { get; set; }

    public IRandomAccessStreamWithContentType? StreamReference { get; set; }

    public string[] SuggestedPrompts => IsImage ? 
        new string[]
            {
                "What's in this image?",
                "Extract text"
            } :
        new string[]
            {
                "Summarize this text",
                "What does this mean"
            };
}
