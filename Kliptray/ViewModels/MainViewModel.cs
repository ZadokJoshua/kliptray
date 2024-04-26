using CommunityToolkit.Mvvm.ComponentModel;
using Kliptray.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using Windows.System;
using CommunityToolkit.Mvvm.Input;
using Kliptray.Helpers;

namespace Kliptray.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<ClipboardItem> ClipboardItems = new();

    [ObservableProperty]
    private ClipboardItem? _selectedItem;

    [ObservableProperty]
    private bool _isItemSelected;

    
    private bool _isClipboardEnabled;
    
    public bool IsClipboardEnabled
    {
        get => _isClipboardEnabled;
        set
        {
            _isClipboardEnabled = value;
            OnPropertyChanged(nameof(IsClipboardEnabled));

            if (IsClipboardEnabled )
            {
                PopulateClipboardItems();
            }
        }
    }

    public MainViewModel()
    {
        IsClipboardEnabled = Clipboard.IsHistoryEnabled();
        Clipboard.HistoryEnabledChanged += ClipboardHistoryEnabledChanged_EventHandler;
        Clipboard.ContentChanged += ClipboardContentChanged_EventHandler;
    }

    private void ClipboardContentChanged_EventHandler(object? sender, object e)
    {
        PopulateClipboardItems();
    }

    private void ClipboardHistoryEnabledChanged_EventHandler(object? sender, object e)
    {
        IsClipboardEnabled = Clipboard.IsHistoryEnabled();
    }

    

    private async void PopulateClipboardItems()
    {
        if (IsClipboardEnabled)
        {
            ClipboardItems.Clear();
            var items = await ClipboardHelper.GetClipboardHistoryItemsAsync();
            if (items.Count > 0 && items != null)
            {
                foreach (var item in items)
                {
                    ClipboardItems.Add(item);
                }
            }
        }
    }

    [RelayCommand]
    private async Task EnableClipboardHistory()
    {
        if(!IsClipboardEnabled)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:clipboard"));
        }
    }


    [RelayCommand]
    private void GetSelectedItem(ClipboardItem clipboardItem)
    {
        if (clipboardItem != null)
        {
            SelectedItem = clipboardItem;
            IsItemSelected = true;
        }
    }
}
