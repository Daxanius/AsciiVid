using System;
using System.Diagnostics;
using System.Threading;

namespace AsciiProcessor {
	public class AsciiVideo {
		public string[] Frames { get; private set; }
		public double Fps { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int PositionLeft { get; set; }
		public int PositionTop { get; set; }

		public bool clearScreen = true;

		public AsciiVideo(string[] frames, double fps, int width, int height) {
			Frames = frames;
			Fps = fps;

			Width = width;
			Height = height;

			PositionLeft = Console.WindowLeft;
			PositionTop = Console.WindowTop;
		}

		public void Play() {
			Stopwatch timer = new();
			int frameTime = (int)(1000 / Fps);

			ClearScreen();

			foreach (string frame in Frames) {
				timer.Restart();
				Console.SetCursorPosition(PositionLeft, PositionTop);
				Console.WriteLine(frame);

				Thread.Sleep((int)(frameTime - timer.ElapsedMilliseconds));
			}

			ClearScreen();
		}

		protected void ClearScreen() {
			if (clearScreen) {
				Console.Clear();
				Console.CursorVisible = true;
			}
		}
	}
}