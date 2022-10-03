﻿using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AdvancedClipboard.Wpf.Services
{
  public class ClipboardWatcher
  {
    private class NativeMethods
    {
      [DllImport("user32.dll")]
      public static extern IntPtr GetForegroundWindow();

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public static extern int GetWindowThreadProcessId(IntPtr windowHandle, out int processId);

      [DllImport("user32.dll")]
      public static extern int GetWindowText(int hWnd, StringBuilder text, int count);
    }

    private readonly ClipboardChangedEvent clipboardChanged;
    private string lastText = string.Empty;
    private string lastImageMd5;
    private const string ImageFolder = "ClipboardImages";

    public static Uri GetUriForImage(string imageFileName)
    {
      return new Uri(Path.Combine(Environment.CurrentDirectory, ImageFolder, imageFileName));
    }

    public ClipboardWatcher(IEventAggregator eventAggregator)
    {
      this.clipboardChanged = eventAggregator.GetEvent<ClipboardChangedEvent>();
      eventAggregator.GetEvent<ClipboardUpdateEvent>().Subscribe(OnClipboardUpdate, ThreadOption.UIThread);
    }

    private void OnClipboardUpdate(ClipboardUpdateData obj)
    {
      BitmapSource imageContent;
      if (Clipboard.ContainsText())
      {
        string text = Clipboard.GetText(TextDataFormat.Text);
        if (text == lastText)
        {
          return;
        }

        lastText = text;

        string processName = this.GetForegroundProcessName();
        ClipData data = new ClipData() { Content = text, ProcessName = processName, Datum = DateTime.Now };
        this.clipboardChanged.Publish(new ClipboardChangedData() { Data = data });

        this.lastImageMd5 = string.Empty;
      }
      else if ((imageContent = Clipboard.GetImage()) != null)
      {
        using (var memoryStream = new MemoryStream())
        {
          BitmapEncoder encoder = new PngBitmapEncoder();
          encoder.Frames.Add(BitmapFrame.Create(imageContent));
          encoder.Save(memoryStream);
          memoryStream.Seek(0, SeekOrigin.Begin);

          var md5 = MD5.Create();
          var currentImageMd5 = md5.ComputeHash(memoryStream).ToStringHex();
          memoryStream.Seek(0, SeekOrigin.Begin);

          if (this.lastImageMd5 != currentImageMd5)
          {
            this.lastImageMd5 = currentImageMd5;
            Directory.CreateDirectory(ImageFolder);
            string fileName = currentImageMd5 + ".png";
            string filePath = Path.Combine(ImageFolder, fileName);

            try
            {
              using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
              {
                memoryStream.WriteTo(file);
                file.Flush();
                file.Close();
              }
            }
            catch(IOException)
            {
              // File already exists
            }

            this.lastImageMd5 = currentImageMd5;

            string processName = this.GetForegroundProcessName();
            ClipData data = new ClipData()
            {
              ImageFileName = fileName,
              ProcessName = processName,
              Datum = DateTime.Now
            };

            this.clipboardChanged.Publish(new ClipboardChangedData() { Data = data });
          }
        }

        this.lastText = string.Empty;
      }
    }

    private string GetForegroundProcessName()
    {
      IntPtr activeWindowId = NativeMethods.GetForegroundWindow();
      if (activeWindowId.Equals(0))
      {
        return string.Empty;
      }

      NativeMethods.GetWindowThreadProcessId(activeWindowId, out int processId);
      if (processId == 0)
      {
        return string.Empty;
      }

      Process process = Process.GetProcessById(processId);
      return process.ProcessName;
    }

    internal void PutOntoClipboard(ClipData clipData)
    {
      if(!string.IsNullOrEmpty(clipData.Content))
      {
        if(lastText != clipData.Content)
        {
          lastText = clipData.Content;
          Clipboard.SetText(clipData.Content);
        }
      }
      else if(!string.IsNullOrEmpty(clipData.ImageFileName))
      {
        string md5 = Path.GetFileNameWithoutExtension(clipData.ImageFileName);
        if (lastImageMd5 != md5)
        {
          lastImageMd5 = md5;
          Clipboard.SetImage(new BitmapImage(GetUriForImage(clipData.ImageFileName)));
        }
      }
    }
  }
}
