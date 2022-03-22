﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace VasciiLib.Video {
	public class VasciiVideo {
		private Thread _playThread;

		// Properties
		public List<string> Frames { get; set; } = new();
		public double Fps { get; set; }
		public int Volume { get; set; } = 100;
		public bool PlayAudio { get; set; } = true;
		public int Width { get; private set; }
		public int Height { get; private set; }
		public string Source { get; private set; }

		// Events
		public event EventHandler<VasciiVideoEventArgs> Started;
		public event EventHandler<VasciiVideoEventArgs> Finished;

		///<summary>
		///<para>General information about the video</para>
		///</summary>
		public VasciiVideo(int width, int height, string source = "") {
			Width = width;
			Height = height;
			Source = source;
		}

		///<summary>
		///<para>Plays the frames on another thread</para>
		///</summary>
		public void Play(Action<string> draw, CancellationToken cancellationToken) {
			Stopwatch timer = new();
			int frameTime = (int)(1000 / Fps);

			_playThread = new Thread(_ => {
				Started?.Invoke(this, new VasciiVideoEventArgs());
				if (PlayAudio) { StreamManager.PlayAudioStream(Source, Volume); }

				for (int i = 0; i < Frames.Count; i++) {
					timer.Restart();

					if (cancellationToken.IsCancellationRequested) {
						break;
					}

					// Run the draw action
					Task.Run(() => draw(Frames[i])).Wait();

					// To make sure we hit the desired FPS
					Thread.Sleep((int)((frameTime - timer.ElapsedMilliseconds) / 1.1));
				}

				Finished?.Invoke(this, new VasciiVideoEventArgs());
			});

			_playThread.Start();
		}

		///<summary>
		///<para>Pauses playing the frames</para>
		///</summary>
		public void Pause() {
			if (_playThread == null) {
				throw new VasciiLibException("VasciiVideo-Pause(): thread is empty");
			}

			_playThread?.Join();
		}

		///<summary>
		///<para>Continues after being paused</para>
		///</summary>
		public void Continue() {
			if (_playThread == null) {
				throw new VasciiLibException("VasciiVideo-Pause(): thread is empty");
			}

			_playThread?.Interrupt();
		}
	}
}