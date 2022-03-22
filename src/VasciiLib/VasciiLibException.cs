using System;
using System.Runtime.Serialization;

namespace VasciiLib {
	internal class VasciiLibException : Exception {
		public VasciiLibException() { }
		public VasciiLibException(string message) : base(message) { }
		public VasciiLibException(string message, Exception innerException) : base(message, innerException) { }
		protected VasciiLibException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}