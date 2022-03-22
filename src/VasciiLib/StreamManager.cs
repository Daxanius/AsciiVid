using System.Diagnostics;
using System.IO;

namespace VasciiLib {
	///<summary>
	///<para>A class for audio streaming</para>
	///</summary>
	public static class StreamManager {
		///<summary>
		///<para>Creates an audio stream from a file</para>
		///</summary>
		public static Process PlayAudioStream(string file, int volume = 100) {
			if (!File.Exists("ffplay.exe")) {
				throw new VasciiLibException("StreamManager-CreateAudioStream(): ffplay.exe was not found");
			}

			if (!File.Exists(file)) {
				throw new VasciiLibException($"StreamManager-CreateAudioStream(): {file} was not found");
			}

			return Process.Start(new ProcessStartInfo {
				FileName = "ffplay.exe",
				Arguments = $"-hide_banner -loglevel panic -vn -nodisp -noborder -i \"{file}\" -volume {volume}",
				UseShellExecute = false,
				RedirectStandardOutput = true,
			});
		}
	}
}