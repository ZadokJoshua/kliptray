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
using System.Collections.Generic;
using Microsoft.UI.Xaml.Data;

namespace Kliptray.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IGeminiService _geminiService;

    [ObservableProperty]
    private ClipboardItem? _selectedItem;

    [ObservableProperty]
    private bool _shouldRefreshListView;

    [ObservableProperty]
    private bool _isItemSelected;

    [ObservableProperty]
    private bool _isClipboardEnabled = Clipboard.IsHistoryEnabled();

    [ObservableProperty]
    private string? _message;

    public ObservableCollection<ClipboardItem> ClipboardItems { get; set; } = new();
    
    public ObservableCollection<Message> Chat { get; set; } = new();

    public MainViewModel()
    {
        _geminiService = new GeminiService();
        PopulateClipboardItems();

        Clipboard.HistoryEnabledChanged += ClipboardHistoryEnabledChanged_EventHandler;
        Clipboard.ContentChanged += ClipboardContentChanged_EventHandler;
    }

    private void ClipboardContentChanged_EventHandler(object? sender, object e)
    {
        if (!ShouldRefreshListView) ShouldRefreshListView = true;
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

            LoadChat(clipboardItem.ItemId);
        }
    }

    private void LoadChat(string itemId)
    {
        Chat.Clear();
        if (ChatData.Chats.ContainsKey(itemId))
        {
            var exitingMessages = ChatData.Chats[itemId];


            foreach (var message in exitingMessages)
            {
                Chat.Add(message);
            }
        }
    }

    [RelayCommand]
    private async Task SendPrompt()
    {
        if (string.IsNullOrEmpty(Message) && !IsItemSelected) return;

        var itemId = SelectedItem.ItemId;
        List<Message> existingMessages;
        if (!ChatData.Chats.TryGetValue(itemId, out existingMessages))
        { 
            existingMessages = new();
            ChatData.Chats[itemId] = existingMessages;
        }

        var userMessage = new Message
            {
                IsUser = true,
                Text = Message
            };

        Chat.Add(userMessage);
        existingMessages.Add(userMessage);

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

            var geminiResponse = new Message
                {
                    IsUser = false,
                    Text = response
                };

            Chat.Add(geminiResponse);
            existingMessages.Add(geminiResponse);
        }
        catch (Exception e)
        {
            var exceptionMessage = new Message
                {
                    IsUser = false,
                    Text = $"Exception: {e.Message}"
                };

            Chat.Add(exceptionMessage);
            existingMessages.Add(exceptionMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void RefreshItems()
    {
        ShouldRefreshListView = false;
        IsItemSelected = false;
        SelectedItem = null;
        PopulateClipboardItems();
    }


    [RelayCommand]
    private async Task RecommendedPrompt(int index)
    {
        if (SelectedItem is null) return;

        Message = SelectedItem.SuggestedPrompts[index];
        await SendPrompt();
    }

    //[RelayCommand]
    //private void ShowTextOnly() => ClipboardItems = (ObservableCollection<ClipboardItem>)ClipboardItems.Where(c => !(c.IsImage));

    //[RelayCommand]
    //private void ShowImagesOnly()
    //{
    //    IList<ClipboardItem> items = new List<ClipboardItem>();
    //    var clipboardItems = ClipboardItems.Where(c => c.IsImage);
    //    ClipboardItems.Clear();
    //    foreach (var clipboardItem in clipboardItems)
    //    {
    //        ClipboardItems.Add(clipboardItem);
    //    }
    //}

}
