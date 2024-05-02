using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System;
using System.Collections.Generic;

namespace Kliptray.Helpers;

public static class ImageHelper
{
    private static StorageFolder _parentfolder = KnownFolders.DocumentsLibrary;
    public static async Task<string> ConvertBytesToPngAsync(IRandomAccessStreamWithContentType randomAccessStreamWithContentType)
    {
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStreamWithContentType);
        var pixelData = await decoder.GetPixelDataAsync();
        byte[] bytes = pixelData.DetachPixelData();

        
        StorageFolder folder = await _parentfolder.CreateFolderAsync("Kliptray", CreationCollisionOption.OpenIfExists);

        StorageFile file = await folder.CreateFileAsync("promptimage.png", CreationCollisionOption.GenerateUniqueName);

        using Stream fileStream = await file.OpenStreamForWriteAsync();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream.AsRandomAccessStream());

        encoder.SetPixelData(decoder.BitmapPixelFormat,
                             BitmapAlphaMode.Ignore,
                             decoder.OrientedPixelWidth,
                             decoder.OrientedPixelHeight,
                             decoder.DpiX, decoder.DpiY,
                             bytes);

        await encoder.FlushAsync();
        return file.Path;
    }

    public static async Task DeleteImageFiles()
    {
        StorageFolder folder = await _parentfolder.GetFolderAsync(Path.Combine(_parentfolder.Path, "Kliptray"));
        IReadOnlyList<StorageFile>  imageFiles = await folder.GetFilesAsync();

        for (int i = 0; i < imageFiles.Count; i++)
        {
            await imageFiles[i].DeleteAsync(StorageDeleteOption.Default);
        }
    }
}
