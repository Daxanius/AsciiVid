using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace VasciiLib.Camera {
	public class VasciiCamera {
		private readonly VasciiManager _vasciiManager;
		private Thread _cameraThread;

		public VideoCapture Capture { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public VasciiCamera(VasciiManager vasciiManager, int width, int height) {
			_vasciiManager = vasciiManager;

			Width = width;
			Height = height;
		}

		///<summary>
		///<para>Starts the camera capture</para>
		///</summary>
		public void StartCapture(int camera, Action<string> draw) {
			Capture = new(camera);
			Capture.Open(camera);

			Mat frame = new();

			_cameraThread = new(_ => {
				while (Capture.IsOpened()) {
					Capture.Read(frame);

					Mat frameResized = frame.Resize(new Size(Width, Height));
					string asciiFrame = _vasciiManager.GenerateAscii(frameResized);

					Task.Run(() => draw(asciiFrame));
				}
			});

			_cameraThread.Start();
		}

		///<summary>
		///<para>Stops the camera capture</para>
		///</summary>
		public void StopCapture() {
			if (!IsCapturing()) {
				return;
			}

			Capture.Dispose();
		}

		///<summary>
		///<para>Checks if the camera is capturing</para>
		///</summary>
		public bool IsCapturing() {
			return Capture.IsOpened();
		}
	}
}