using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Buffers.Text;
using Windows.Storage.Streams;

namespace Kliptray.Models;

public record ClipboardItem
{
    public DateTimeOffset TimeStamp { get; set; }

    public BitmapImage? Image { get; set; }

    public string? Text { get; set; }

    public bool IsImage { get; set; }

    public IRandomAccessStreamWithContentType? StreamReference { get; set; }
}
