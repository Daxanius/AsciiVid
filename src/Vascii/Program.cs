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
					PlayCamera(options);
					break;
				case VasciiMode.Image:
					throw new NotImplementedException();
			}
		}

		static VasciiManager GetVasciiManager(Options options) {
			int width = Console.WindowWidth - 2, height = Console.WindowHeight - 2;

			if (options.Width != 0) width = options.Width;
			if (options.Height != 0) height = options.Height;

			VasciiManager vasciiManager = new(width, height);

			if (options.Invert) vasciiManager.InvertColors();
			if (!String.IsNullOrWhiteSpace(options.DensityChars)) vasciiManager.DensityChars = options.DensityChars;

			return vasciiManager;
		}

		static void PlayCamera(Options options) {
			VasciiCamera capture = new(GetVasciiManager(options));
			capture.TargetFps = options.Fps;

			Console.Clear();
			capture.StartCapture(options.Camera, draw: frame => {
				Console.SetCursorPosition(Console.WindowTop, Console.WindowLeft);
				Console.WriteLine(frame);
			});
		}

		static void PlayVideo(Options options) {
			if (String.IsNullOrWhiteSpace(options.Video) || !File.Exists(options.Video)) {
				Console.WriteLine("Please provide a valid video file, specify a file with -f <file>");
				return;
			}

			Stopwatch stopwatch = new();

			int positionTop = Console.WindowTop;
			int positionLeft = Console.WindowLeft;

			// Because we're writing white text on a black background
			var manager = GetVasciiManager(options);
			var video = manager.GenerateVasciiVideo(options.Video);

			video.Started += (_, _) => {
				stopwatch.Restart();
			};

			video.Finished += (_, _) => {
				stopwatch.Stop();
				Console.Clear();
				Console.SetCursorPosition(positionTop, positionLeft);
				Console.WriteLine($"Finished playing, took {stopwatch.Elapsed}");
			};

			video.PlayAudio = !options.Audio;
			video.Volume = options.Volume;

			if (options.Fps != 0) {
				video.Fps = options.Fps;
			}

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