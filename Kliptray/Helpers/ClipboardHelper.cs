using Kliptray.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using WinRT;

namespace Kliptray.Helpers;

public static class ClipboardHelper
{
    //private static DateTimeOffset _previousImageDateTime;
    private static async Task<ClipboardItem?> AccessItemContent(ClipboardHistoryItem item)
    {
        var view = item.Content;
        if (view.Contains(StandardDataFormats.Bitmap))
        {
            var bitmapReference = await view.GetBitmapAsync();

            if (bitmapReference != null)
            {
                var bitmap = new BitmapImage();
                var randomAccessStreamWithContentType = await bitmapReference.OpenReadAsync();
                await bitmap.SetSourceAsync(randomAccessStreamWithContentType);

                //if(IsDuplicateScreenshot(_previousImageDateTime, item.Timestamp)) 
                //{
                //    return null;
                //}

                //_previousImageDateTime = item.Timestamp;

                return new ClipboardItem
                {
                    Id = item.Id,
                    Image = bitmap,
                    IsImage = true,
                    TimeStamp = item.Timestamp,
                    StreamReference = randomAccessStreamWithContentType
                };
            }
        }
        else if (view.Contains(StandardDataFormats.Text))
        {
            var text = await view.GetTextAsync();

            if (text != null)
            {
                return new ClipboardItem
                {
                    Text = text,
                    TimeStamp = item.Timestamp
                };
            }
        }
        return null;
    }


    public static async Task<List<ClipboardItem>> GetClipboardHistoryItemsAsync()
    {
        List<ClipboardItem> clipboardItems = new();

        var historyItems = await Clipboard.GetHistoryItemsAsync();
        if (historyItems.Status == ClipboardHistoryItemsResultStatus.Success)
        {
            foreach (var item in historyItems.Items)
            {
                var clipboardItem = await AccessItemContent(item);
                if (clipboardItem != null)
                {
                    clipboardItems.Add(clipboardItem);
                }
            }
        }

        return clipboardItems;
    }

    //private static bool IsDuplicateScreenshot(DateTimeOffset previousImageDT, DateTimeOffset currentImageDT)
    //{
    //    _previousImageDateTime = currentImageDT;
    //    return (previousImageDT - currentImageDT) < TimeSpan.FromSeconds(1);
    //}
}
