using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System;

namespace Kliptray.Helpers;

public static class ImageHelper
{
    public static async Task<string> ConvertBytesToPngAsync(IRandomAccessStreamWithContentType randomAccessStreamWithContentType)
    {
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStreamWithContentType);
        var pixelData = await decoder.GetPixelDataAsync();
        byte[] bytes = pixelData.DetachPixelData();

        StorageFolder folder = KnownFolders.PicturesLibrary; // You can change this to the desired folder
        StorageFile file = await folder.CreateFileAsync("image.png", CreationCollisionOption.GenerateUniqueName);

        using (Stream fileStream = await file.OpenStreamForWriteAsync())
        {
            // Create encoder for PNG
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream.AsRandomAccessStream());

            // Get pixel data from decoder and set them for encoder
            encoder.SetPixelData(decoder.BitmapPixelFormat,
                                 BitmapAlphaMode.Ignore,
                                 decoder.OrientedPixelWidth,
                                 decoder.OrientedPixelHeight,
                                 decoder.DpiX, decoder.DpiY,
                                 bytes);

            await encoder.FlushAsync();

            return file.Path;
        }
    }

    public static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
