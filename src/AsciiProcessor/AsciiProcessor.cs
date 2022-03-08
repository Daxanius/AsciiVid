using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AsciiProcessor {
	public class AsciiProcessor {
		public string DensityChars { get; private set; }

		public AsciiProcessor(string densityChars = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ") => DensityChars = densityChars;

		public AsciiVideo RenderAsciiVideo(string videoFile, int width, int height) {
			string[] frames;
			double fps;

			using (VideoCapture capture = new(videoFile))
			using (Mat image = new()) {
				frames = new string[capture.FrameCount];
				fps = capture.Fps;

				while (capture.Read(image)) {
					if (image.Empty()) {
						break;
					}

					Bitmap map = MatToBitmap(image, width, height);

					frames[capture.PosFrames - 1] = GenerateAscii(map);
				}
			}

			return new(frames, fps, width, height);
		}

		public unsafe string GenerateAscii(Bitmap image) {
			string result = string.Empty;

			BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int bytesPerPixel = 3;

			byte* scan0 = (byte*)imageData.Scan0.ToPointer();
			int stride = imageData.Stride;

			for (int y = 0; y < imageData.Height; y++) {
				byte* row = scan0 + (y * stride);

				for (int x = 0; x < imageData.Width; x++) {
					int bIndex = x * bytesPerPixel;
					int gIndex = bIndex + 1;
					int rIndex = bIndex + 2;

					byte pixelR = row[rIndex];
					byte pixelG = row[gIndex];
					byte pixelB = row[bIndex];

					result += GenerateChar(pixelR, pixelG, pixelB);
				}
				result += ('\n');
			}

			image.UnlockBits(imageData);
			return result;
		}

		public char GenerateChar(byte r, byte g, byte b) {
			double avg = Enumerable.Average(new int[] { r, g, b });
			return DensityChars[(int)(avg / 256 * 100 * DensityChars.Length / 100)];
		}

		public void InvertColors() {
			char[] array = DensityChars.ToCharArray();
			Array.Reverse(array);
			DensityChars = new string(array);
		}

		protected static Bitmap MatToBitmap(Mat image, int width, int height) {
			return new Bitmap(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image), width, height);
		}
	}
}