using CommunityToolkit.Mvvm.ComponentModel;
using Kliptray.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using Windows.System;
using CommunityToolkit.Mvvm.Input;
using Kliptray.Helpers;
using Kliptray.Services;

namespace Kliptray.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<ClipboardItem> ClipboardItems = new();
    public ObservableCollection<Message> Chat = new();

    [ObservableProperty]
    private ClipboardItem? _selectedItem;

    [ObservableProperty]
    private bool _isItemSelected;

    [ObservableProperty]
    private bool _isClipboardEnabled = Clipboard.IsHistoryEnabled();

    [ObservableProperty]
    private string? _message;

    private readonly IGeminiService _geminiService;

    public MainViewModel()
    {
        _geminiService = new GeminiService();

        PopulateClipboardItems();

        Clipboard.HistoryEnabledChanged += ClipboardHistoryEnabledChanged_EventHandler;
        Clipboard.ContentChanged += ClipboardContentChanged_EventHandler;
    }

    private async void ClipboardContentChanged_EventHandler(object? sender, object e)
    {
        await Task.Delay(5000);
        await PopulateClipboardItems();
    }

    private void ClipboardHistoryEnabledChanged_EventHandler(object? sender, object e)
    {
        IsClipboardEnabled = Clipboard.IsHistoryEnabled();
    }

    private async Task PopulateClipboardItems()
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

    [RelayCommand]
    private async Task SendPrompt()
    {
        // Send prompt with image
        // Suggestted prompts

        Chat.Add(new Message
        {   
            IsUser = true,
            Text = Message
        });

        try
        {
            var response = await _geminiService.PromptText($"{Message}: \"{SelectedItem.Text}\"");

            Chat.Add(new Message
            {
                IsUser = false,
                Text = response
            });
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
            throw;
        }
    }
}
