using OpenCvSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using VasciiLib.Video;

namespace VasciiLib {
	public class VasciiManager {
		private Thread _renderThread;

		public string DensityChars { get; set; } = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ";

		public event EventHandler<EventArgs> Ready;

		///<summary>
		///<para>Generates a playable Vascii video</para>
		///</summary>
		public VasciiVideo GenerateVasciiVideo(string videoFile, int width, int height) {
			if (!File.Exists(videoFile)) {
				throw new VasciiLibException("VasciiManager-GenerateVasciiVideo(): file does not exist");
			}

			VasciiVideo video = new(width, height, videoFile);

			VideoCapture capture = new(videoFile);
			Mat image = new();

			_renderThread = new(_ => {
				video.Fps = capture.Fps;

				while (capture.Read(image)) {
					if (image.Empty()) {
						break;
					}

					Mat imageResized = image.Resize(new Size(width, height));
					video.Frames.Add(GenerateAscii(imageResized));
				}

				Ready?.Invoke(this, new EventArgs());
			});

			_renderThread.Start();
			return video;
		}

		///<summary>
		///<para>Converts a Mat to a string</para>
		///</summary>
		public string GenerateAscii(Mat image) {
			string result = string.Empty;

			for (int y = 0; y < image.Rows; ++y) {
				for (int x = 0; x < image.Cols; ++x) {
					byte r = image.At<Vec3b>(y, x)[0];
					byte g = image.At<Vec3b>(y, x)[1];
					byte b = image.At<Vec3b>(y, x)[2];

					result += GenerateChar(r, g, b);
					//Console.WriteLine(result);
				}

				result += '\n';
			}

			return result;
		}

		///<summary>
		///<para>Converts an RGB value to a char</para>
		///</summary>
		public char GenerateChar(byte r, byte g, byte b) {
			double avg = Enumerable.Average(new int[] { r, g, b });
			return DensityChars[(int)(avg / 256 * 100 * DensityChars.Length / 100)];
		}

		///<summary>
		///<para>Inverts the colors for a white on black background</para>
		///</summary>
		public void InvertColors() {
			char[] array = DensityChars.ToCharArray();
			Array.Reverse(array);
			DensityChars = new string(array);
		}
	}
}