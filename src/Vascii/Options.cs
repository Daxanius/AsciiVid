using CommandLine;

namespace Vascii {
	public class Options {
		[Option('i', Required = false, HelpText = "Invert the color for white text on a black background.")]
		public bool Invert { get; set; }

		[Option('m', Required = false, HelpText = "Disable audio.")]
		public bool Audio { get; set; }

		[Option('y', Required = false, HelpText = "Don't prompt to start.")]
		public bool SkipPrompt { get; set; }

		[Option("volume", Default = 100, Required = false, HelpText = "Set the audio volume.")]
		public int Volume { get; set; }

		[Option("width", Required = false, HelpText = "Set the width to render to.")]
		public int Width { get; set; }

		[Option("height", Required = false, HelpText = "Set the height to render to.")]
		public int Height { get; set; }

		[Option("characters", Required = false, HelpText = "Set the characters to use from dense -> light.")]
		public string Characters { get; set; }

		[Option("camera", Required = false, HelpText = "Set what camera to use.")]
		public int Camera { get; set; }

		[Option("fps", Required = false, HelpText = "Override the FPS.")]
		public int Fps { get; set; }

		[Option("file", Required = false, HelpText = "The file to use.")]
		public string file { get; set; }

		[Option("mode", Default = VasciiMode.Video, Required = false, HelpText = "The mode to run the program in.")]
		public VasciiMode Mode { get; set; }
	}
}