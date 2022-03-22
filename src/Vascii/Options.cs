using CommandLine;

namespace Vascii {
	public class Options {
		[Option('i', "invert", Required = false, HelpText = "Invert the color for white text on a black background.")]
		public bool Invert { get; set; }

		[Option("disable-audio", Required = false, HelpText = "Disable audio.")]
		public bool Audio { get; set; }

		[Option('v', "volume", Default = 100, Required = false, HelpText = "Set the audio volume.")]
		public int Volume { get; set; }

		[Option('w', "width", Required = false, HelpText = "Set the width to render to.")]
		public int Width { get; set; }

		[Option('h', "height", Required = false, HelpText = "Set the height to render to.")]
		public int Height { get; set; }

		[Option('d', "density-chars", Required = false, HelpText = "Set the characters to use from dense -> light.")]
		public string DensityChars { get; set; }

		[Option('c', "camera", Required = false, HelpText = "Set what camera to use.")]
		public int Camera { get; set; }

		[Option("fps", Required = false, HelpText = "Override the FPS.")]
		public int Fps { get; set; }

		[Option('f', "file", Required = false, HelpText = "The file to use.")]
		public string Video { get; set; }

		[Option('m', "mode", Default = VasciiMode.Video, Required = false, HelpText = "The mode to run the program in.")]
		public VasciiMode Mode { get; set; }
	}
}