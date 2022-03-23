using OpenCvSharp;
using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VasciiLib.Camera {
	public class VasciiCamera {
		private readonly VasciiManager _vasciiManager;
		private Thread _cameraThread;

		public VideoCapture Capture { get; private set; } = new();
		public double TargetFps { get; set; }

		public VasciiCamera(VasciiManager vasciiManager) {
			_vasciiManager = vasciiManager;
		}

		///<summary>
		///<para>Starts the camera capture</para>
		///</summary>
		public void StartCapture(int camera, Action<string> draw) {
			Stopwatch timer = new();
			Capture.Open(camera);

			if (TargetFps == 0) { TargetFps = Capture.Fps; }
			int frameTime = (int)Math.Floor(1000 / TargetFps);
			Mat frame = new();

			_cameraThread = new(_ => {
				while (Capture.IsOpened()) {
					timer.Restart();
					Capture.Read(frame);

					Mat frameResized = frame.Resize(new Size(_vasciiManager.Width, _vasciiManager.Height));
					string asciiFrame = _vasciiManager.GenerateAscii(frameResized);

					// Run the draw action
					Task.Run(() => draw(asciiFrame));

					// To make sure we hit the desired FPS
					Thread.Sleep(Math.Clamp((int)(frameTime - timer.ElapsedMilliseconds), 0, int.MaxValue));
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