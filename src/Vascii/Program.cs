using CommandLine;
using CommandLine.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VasciiLib;
using VasciiLib.Camera;

namespace Vascii {
	class Program {
		static void Main(string[] args) {
			var parser = new Parser(with => { with.HelpWriter = null; with.CaseInsensitiveEnumValues = true; });
			var parserResult = parser.ParseArguments<Options>(args);
			parserResult
			  .WithParsed(options => Run(options))
			  .WithNotParsed(errs => DisplayHelp(parserResult));
		}

		static void DisplayHelp<T>(ParserResult<T> result) {
			var helpText = HelpText.AutoBuild(result, h => {
				h.AdditionalNewLineAfterOption = false;
				h.Heading = "Vascii";
				h.Copyright = String.Empty;
				h.AddPreOptionsLine("usage: vascii [options]");
				h.AddPreOptionsLine("\nMain options:");
				return h;
			});
			Console.WriteLine(helpText);
		}

		static void Run(Options options) {
			switch (options.Mode) {
				case VasciiMode.Video:
					PlayVideo(options);
					break;
				case VasciiMode.Camera:
					StartCamera(options);
					break;
				case VasciiMode.Image:
					RenderImage(options);
					break;
			}
		}

		public static void Prompt(Options options, string action, int millisecondsTimeout = 500) {
			if (!options.SkipPrompt) {
				Console.Write($"Press enter to {action} > ");
				Console.Read();
			} else {
				Thread.Sleep(millisecondsTimeout);
			}
		}

		static VasciiManager GetVasciiManager(Options options, int defaultWidth, int defaultHeight, double defaultScale = 1) {
			if (options.Width != 0) defaultWidth = options.Width;
			if (options.Height != 0) defaultHeight = options.Height;
			if (options.Scale != 0) defaultScale = options.Scale;

			VasciiManager vasciiManager = new(defaultWidth, defaultHeight, defaultScale);

			if (options.Invert) vasciiManager.InvertColors();
			if (!String.IsNullOrWhiteSpace(options.Characters)) vasciiManager.Characters = options.Characters;

			return vasciiManager;
		}

		static void RenderImage(Options options) {
			if (String.IsNullOrWhiteSpace(options.File) || !File.Exists(options.File)) {
				Console.WriteLine("Please provide a valid image file, specify a file with --file=<file>");
				return;
			}

			(int width, int height) = VasciiManager.GetImageSize(options.File);
			var vasciiManager = GetVasciiManager(options, width, height);
			string image = vasciiManager.GenerateVasciiImage(options.File);

			Prompt(options, "display image", 0);
			Console.WriteLine(image);
		}

		static void StartCamera(Options options) {
			VasciiCamera camera = new(GetVasciiManager(options, Console.WindowWidth -2, Console.WindowHeight -2));
			camera.TargetFps = options.Fps;

			Prompt(options, "start camera", 0);

			Console.Clear();
			camera.StartCapture(options.Camera, draw: frame => {
				Console.SetCursorPosition(Console.WindowTop, Console.WindowLeft);
				Console.WriteLine(frame);
			});
		}

		static void PlayVideo(Options options) {
			if (String.IsNullOrWhiteSpace(options.File) || !File.Exists(options.File)) {
				Console.WriteLine("Please provide a valid video file, specify a file with --file=<file>");
				return;
			}

			Stopwatch stopwatch = new();

			var manager = GetVasciiManager(options, Console.WindowWidth - 2, Console.WindowHeight - 2);
			var video = manager.GenerateVasciiVideo(options.File);

			video.Started += (_, _) => {
				stopwatch.Restart();
			};

			video.Finished += (_, _) => {
				stopwatch.Stop();
				Console.Clear();
				Console.SetCursorPosition(Console.WindowTop, Console.WindowLeft);
				Console.WriteLine($"Finished playing, took {stopwatch.Elapsed}");
			};

			video.PlayAudio = !options.Audio;
			video.Volume = options.Volume;

			if (options.Fps != 0) {
				video.Fps = options.Fps;
			}

			Prompt(options, "play video");

			Console.Clear();
			video.Play(cancellationToken: CancellationToken.None, draw: frame => {
				Console.SetCursorPosition(Console.WindowTop, Console.WindowLeft);
				Console.WriteLine(frame);
			});
		}
	}
}