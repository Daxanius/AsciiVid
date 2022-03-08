using AsciiProcessor;
using NetCoreAudio;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AsciiVideoApplication {
	class Program {
		static async Task Main(string[] args) {
			int width = Console.WindowWidth, height = Console.WindowHeight;

			if (args.Length == 0) {
				Console.WriteLine("Please provide arguments");
				return;
			}

			int camera;
			bool canParse = int.TryParse(args[0], out camera);
			if (canParse) {
				PlayCamera(width - 2, height - 2, camera);
				return;
			}

			await PlayVideoAsync(args[0], width -2, height -2);
		}

		static void PlayCamera(int width, int height, int camera) {
			AsciiProcessorAsync processor = new();
			Thread videoCapture = new(() => processor.StartCameraCapture(camera, width, height));
			videoCapture.Start();
		}

		static void PlayVideo(string videoFile, int width, int height) {
			Stopwatch stopwatch = new();

			Console.WriteLine("processing video...");
			AsciiProcessor.AsciiProcessor processor = new();

			stopwatch.Start();
			AsciiVideo video = processor.RenderAsciiVideo(videoFile, width, height);
			stopwatch.Stop();

			Console.Beep();
			Console.Write($"processing done, took {stopwatch.Elapsed}. press any key to start playing > ");
			Console.Read();

			string input;

			do {
				stopwatch.Restart();
				video.Play();
				stopwatch.Stop();

				Console.Write($"finished playing, took {stopwatch.Elapsed}. play again (y/n)? ");
				input = Console.ReadLine().Trim().ToLower();
			} while (!input.Contains('n'));
		}

		static Task PlayVideoAsync(string videoFile, int width, int height) {
			Stopwatch stopwatch = new();

			var player = new Player();
			AsciiProcessorAsync processor = new();

			// Because we're writing white text on a black background
			processor.InvertColors();
			_ = Task.Run(() => processor.RenderAsciiVideoAsync(videoFile, width, height));
			string audioFile = Mp4ToMp3(videoFile);

			Console.Write($"Press any key to start playing > ");
			Console.Read();

			stopwatch.Restart();
			_ = player.Play(audioFile);
			processor.GetVideo().Play();
			stopwatch.Stop();

			Console.Write($"Finished playing, took {stopwatch.Elapsed}");
			return Task.CompletedTask;
		}

		static string Mp4ToMp3(string inputFile) {
			string mp3File = String.Concat(inputFile.Remove(inputFile.IndexOf('.')), ".mp3");

			if (!File.Exists(mp3File)) {
				Process.Start("ffmpeg.exe", $"-i {inputFile} -vn -f mp3 -ab 192k {mp3File}");
			}

			return mp3File;
		}
	}
}