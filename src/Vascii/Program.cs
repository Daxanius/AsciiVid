using System;
using System.Diagnostics;
using System.Threading;
using VasciiLib;
using VasciiLib.Camera;

namespace Vascii {
	class Program {
		static void Main(string[] args) {
			int width = Console.WindowWidth, height = Console.WindowHeight;

			if (args.Length == 0) {
				Console.WriteLine("Please provide arguments");
				return;
			}

			bool canParse = int.TryParse(args[0], out int camera);
			if (canParse) {
				PlayCamera(width - 2, height - 2, camera);
				return;
			}

			PlayVideo(args[0], width - 2, height - 2);
		}

		static void PlayCamera(int width, int height, int camera) {
			VasciiManager vasciiManager = new VasciiManager();
			vasciiManager.InvertColors();

			VasciiCamera capture = new(vasciiManager, width, height);

			Console.Clear();
			capture.StartCapture(camera: camera, draw: frame => {
				Console.SetCursorPosition(Console.WindowTop, Console.WindowLeft);
				Console.WriteLine(frame);
			});
		}

		static void PlayVideo(string videoFile, int width, int height) {
			Stopwatch stopwatch = new();
			VasciiManager manager = new();

			int positionTop = Console.WindowTop;
			int positionLeft = Console.WindowLeft;

			// Because we're writing white text on a black background
			manager.InvertColors();
			var video = manager.GenerateVasciiVideo(videoFile, width, height);

			video.Started += (_, _) => {
				stopwatch.Restart();
			};

			video.Finished += (_, _) => {
				stopwatch.Stop();
				Console.Clear();
				Console.SetCursorPosition(positionTop, positionLeft);
				Console.WriteLine($"Finished playing, took {stopwatch.Elapsed}");
			};

			Console.Write($"Press any key to start playing > ");
			Console.Read();

			Console.Clear();
			video.Play(cancellationToken: CancellationToken.None, draw: frame => {
				Console.SetCursorPosition(positionTop, positionLeft);
				Console.WriteLine(frame);
			});
		}
	}
}