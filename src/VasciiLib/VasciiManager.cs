using OpenCvSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using VasciiLib.Video;

namespace VasciiLib {
	public class VasciiManager {
		private Thread _renderThread;

		public VasciiManager(int width, int height, double scale = 1) {
			Width = width;
			Height = height;
			Scale = scale;
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
		public double Scale { get; private set; }

		public string Characters { get; set; } = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ";

		public event EventHandler<EventArgs> Ready;

		///<summary>
		///<para>Generates a playable Vascii video</para>
		///</summary>
		public VasciiVideo GenerateVasciiVideo(string videoFile) {
			if (!File.Exists(videoFile)) {
				throw new VasciiLibException("VasciiManager-GenerateVasciiVideo(): file does not exist");
			}

			VasciiVideo video = new(Width, Height, videoFile);
			VideoCapture capture = new(videoFile);

			video.Fps = capture.Fps;

			_renderThread = new(_ => {
				Mat image = new();

				while (capture.Read(image)) {
					if (image.Empty()) {
						break;
					}

					Mat imageResized = image.Resize(new Size(Width * Scale, Height * Scale));
					video.Frames.Add(GenerateAscii(imageResized));
				}

				Ready?.Invoke(this, new EventArgs());
			});

			_renderThread.Start();
			return video;
		}

		///<summary>
		///<para>Generates an ASCII image</para>
		///</summary>
		public string GenerateVasciiImage(string imageFile) {
			if (!File.Exists(imageFile)) {
				throw new VasciiLibException("VasciiManager-GenerateVasciiImage(): file does not exist");
			}

			Mat image = new(imageFile);
			image = image.Resize(new Size(image.Width * Scale, image.Height * Scale));
			return GenerateAscii(image);
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
			return Characters[(int)(avg / 256 * 100 * Characters.Length / 100)];
		}

		///<summary>
		///<para>Inverts the colors for a white on black background</para>
		///</summary>
		public void InvertColors() {
			char[] array = Characters.ToCharArray();
			Array.Reverse(array);
			Characters = new string(array);
		}
	}
}