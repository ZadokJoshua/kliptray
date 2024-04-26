using Microsoft.UI.Xaml.Media;
using System;

namespace Kliptray.Models;

public class ClipboardItem
{
    public DateTimeOffset TimeStamp { get; set; }

    public ImageSource? Image { get; set; }

    public string? Text { get; set; }

    public bool IsImage { get; set; }
}
