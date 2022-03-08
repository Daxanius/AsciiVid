namespace AsciiProcessor {
	public class AsciiVideoAsync : AsciiVideo {
		private int _frameIndex = 0;

		public AsciiVideoAsync(int frameCount, double fps, int width, int height) : base(new string[frameCount], fps, width, height) { }

		public void AddFrame(string frame) {
			Frames[_frameIndex] = frame;

			_frameIndex++;
		}
	}
}