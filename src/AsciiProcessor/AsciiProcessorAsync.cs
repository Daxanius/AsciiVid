using OpenCvSharp;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace AsciiProcessor {
	public class AsciiProcessorAsync : AsciiProcessor {
		AsciiVideoAsync _video;
		string _cameraFrame;
		bool _capturing;

		public void StartCameraCapture(int camera, int width, int height) {
			VideoCapture capture = new(camera);
			capture.Open(camera);
			_capturing = capture.IsOpened();

			Mat frame = new();

			Console.Clear();
			while (_capturing) {
				capture.Read(frame);
				Bitmap map = MatToBitmap(frame, width, height);
				_cameraFrame = GenerateAscii(map);

				Console.SetCursorPosition(Console.WindowLeft, Console.WindowTop);
				Console.WriteLine(_cameraFrame);
			}
		}

		public Task RenderAsciiVideoAsync(string videoFile, int width, int height) {
			using VideoCapture capture = new(videoFile);
			using Mat image = new(); _video = new(capture.FrameCount, capture.Fps, width, height);

			while (capture.Read(image)) {
				if (image.Empty()) {
					break;
				}

				Bitmap map = MatToBitmap(image, width, height);

				_video.AddFrame(GenerateAscii(map));
			}

			return Task.CompletedTask;
		}

		public void StopCameraCapture() {
			_capturing = false;
		}

		public bool IsCapturing() {
			return _capturing;
		}

		public string GetCameraFrame() {
			if (!_capturing) { throw new Exception("AsciiProcessorAsync-GetCameraFrame(): Camera is not running"); }

			return _cameraFrame;
		}

		public AsciiVideo GetVideo() {
			while (_video == null) { }
			return _video;
		}
	}
}