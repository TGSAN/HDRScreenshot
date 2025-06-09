using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Microsoft.Graphics.Canvas;
using System.IO;

namespace HDRScreenshot
{
    internal class Screenshot
    {
        public static void Capture()
        {
            var _captureItem = GraphicsCapture.CreateItemForMonitor(IntPtr.Zero);
            if (_captureItem != null)
            {
                var _canvasDevice = new CanvasDevice();
                var _framePool = Direct3D11CaptureFramePool.Create(
                    _canvasDevice, // D3D device
                    DirectXPixelFormat.R16G16B16A16Float, // Pixel format
                    1, // Number of frames
                    _captureItem.Size); // Size of the buffers
                Debug.WriteLine("DXGI Create");
                var _session = _framePool.CreateCaptureSession(_captureItem);
                // _session.IsCursorCaptureEnabled = false;
                // _session.IsBorderRequired = false;
                _framePool.FrameArrived += async (s, a) =>
                {
                    Debug.WriteLine("FrameArrived");
                    using (var frame = _framePool.TryGetNextFrame())
                    {
                        Debug.WriteLine("Get Frame");
                        CanvasBitmap canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(_canvasDevice, frame.Surface);
                        var screenshotsDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshots");
                        if (!Directory.Exists(screenshotsDirPath))
                        {
                            Directory.CreateDirectory(screenshotsDirPath);
                        }
                        string screenshotsName = FolderNameHelper.GetLocalizedFolderName(screenshotsDirPath);
                        var now = DateTime.Now;
                        string date = now.ToString("yyyy-MM-dd");
                        TimeSpan sinceMidnight = now - now.Date;
                        int csUnits = (int)(sinceMidnight.TotalMilliseconds / 100);
                        string time = csUnits.ToString("D6");
                        string filename = $"{screenshotsName} {date} {time}.jxr";
                        await canvasBitmap.SaveAsync(Path.Combine(screenshotsDirPath, filename), CanvasBitmapFileFormat.JpegXR);
                        canvasBitmap.Dispose();
                        _session.Dispose();
                        _framePool.Dispose();
                        _canvasDevice.Dispose();
                    }
                };
                _session.StartCapture();
            }
        }
    }
}
