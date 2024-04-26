using Kliptray.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace Kliptray.Helpers;

public static class ClipboardHelper
{
    private static async Task<ClipboardItem?> AccessItemContent(DataPackageView view)
    {
        if (view.Contains(StandardDataFormats.Bitmap))
        {
            var bitmapReference = await view.GetBitmapAsync();

            if (bitmapReference != null)
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(await bitmapReference.OpenReadAsync());
                return new ClipboardItem
                {
                    Image = bitmap,
                    IsImage = true
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
                    Text = text
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
                var view = item.Content;
                var clipboardItem = await AccessItemContent(view);
                if (clipboardItem != null)
                {
                    clipboardItems.Add(clipboardItem);
                }
            }
        }

        return clipboardItems;
    }

    public static async Task<ClipboardItem?> GetCurrentClipboardItemAsync()
    {
        var currentItem = await AccessItemContent(Clipboard.GetContent());
        return currentItem;
    }

}
