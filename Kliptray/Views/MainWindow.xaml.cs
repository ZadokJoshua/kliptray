using Kliptray.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using WinRT.Interop;

namespace Kliptray.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        Title = "Kliptray";

        this.Activated += MainWindow_Activated;
        // this.Closed += MainWindow_Closed;
    }

    //private async void MainWindow_Closed(object sender, WindowEventArgs args)
    //{
        //await ImageHelper.DeleteImageFiles();
    //}

    private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.SetIcon(@"Assets\icon-kliptray-cb.ico");

        await ImageHelper.DeleteImageFiles();
    }
}
