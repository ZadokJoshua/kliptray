using CommunityToolkit.Mvvm.ComponentModel;
using Kliptray.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using Windows.System;

namespace Kliptray.ViewModels;

public class MainViewModel : ObservableObject
{
    public ObservableCollection<ClipboardItem> ClipboardItems = new();

    public MainViewModel()
    {
        PopulateClipboardItems();

        Clipboard.HistoryEnabledChanged += (S, e) =>
        {
            PopulateClipboardItems();
        };
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
                if (view.Contains(StandardDataFormats.Bitmap))
                {
                    var bitmapReference = await view.GetBitmapAsync();

                    if (bitmapReference != null)
                    {
                        var bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(await bitmapReference.OpenReadAsync());
                        clipboardItems.Add(new ClipboardItem { Image = bitmap, TimeStamp = item.Timestamp});
                    }
                }
                else if (view.Contains(StandardDataFormats.Text))
                {
                    var text = await view.GetTextAsync();

                    if (text != null)
                    {
                        clipboardItems.Add(new ClipboardItem { Text = text, TimeStamp = item.Timestamp });
                    }
                }
            }
        }

        return clipboardItems;
    }

    private async void PopulateClipboardItems()
    {
        ClipboardItems.Clear();

        var items = await GetClipboardHistoryItemsAsync();
        if (items.Count > 0 && items != null)
        {
            foreach( var item in items)
            {
                ClipboardItems.Add(item);
            }
        }
    }

    private async void ClipboardHistoryEnabled()
    {
        if(Clipboard.IsHistoryEnabled())
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:clipboard"));
        }
    }
}
