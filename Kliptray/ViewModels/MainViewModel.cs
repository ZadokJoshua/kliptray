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
using System.Linq;

namespace Kliptray.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ClipboardItem? _selectedItem;

    [ObservableProperty]
    private bool _isItemSelected;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isClipboardEnabled = Clipboard.IsHistoryEnabled();

    [ObservableProperty]
    private string? _message;

    private readonly IGeminiService _geminiService;

    public ObservableCollection<ClipboardItem> ClipboardItems { get; set; } = new();
    public ObservableCollection<Message> Chat { get; set; } = new();

    public MainViewModel()
    {
        _geminiService = new GeminiService();
        PopulateClipboardItems();

        Clipboard.HistoryEnabledChanged += ClipboardHistoryEnabledChanged_EventHandler;
        Clipboard.ContentChanged += ClipboardContentChanged_EventHandler;
    }

    private async void ClipboardContentChanged_EventHandler(object? sender, object e)
    {
        await Task.Delay(3000);
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
            if (items.Count > 0 && items is not null)
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
        if (string.IsNullOrEmpty(Message) && !IsItemSelected) return;

        Chat.Add(new Message
        {
            IsUser = true,
            Text = Message
        });

        var promptText = Message;
        Message = string.Empty;

        IsBusy = true;

        try
        {
            var response = string.Empty;

            if (SelectedItem.IsImage && SelectedItem.StreamReference != null)
            {
                response = await _geminiService.PromptImage($"{promptText}", SelectedItem.StreamReference);
            }
            else
            {
                response = await _geminiService.PromptText($"{promptText}: \"{SelectedItem.Text}\"");
            }

            Chat.Add(new Message
            {
                IsUser = false,
                Text = response
            });
        }
        catch (Exception e)
        {
            Chat.Add(new Message
            {
                IsUser = false,
                Text = $"Exception: {e.Message}"
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowTextOnly() =>
        ClipboardItems = (ObservableCollection<ClipboardItem>)ClipboardItems.Where(c => !(c.IsImage));

    [RelayCommand]
    private void ShowImagesOnly() =>
        ClipboardItems = (ObservableCollection<ClipboardItem>)ClipboardItems.Where(c => c.IsImage);

    [RelayCommand]
    private async Task RecommendedPrompt(int index)
    {
        if (SelectedItem is null) return;

        Message = SelectedItem.SuggestedPrompts[index];
        await SendPrompt();
    }
}
