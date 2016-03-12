// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.util {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A DynamicByteBuffer maintains a ByteBuffer and provides methods to ensure a
	/// minimum capacity, resizing if necessary.
	/// </summary>
	///
	public class DynamicByteBuffer {
		/// <summary>
		/// Create a new DynamicByteBuffer with an initial capacity.
		/// </summary>
		///
		/// <param name="initialCapacity">The initial capacity of buffer().</param>
		public DynamicByteBuffer(int initialCapacity) {
			buffer_ = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(initialCapacity);
		}
	
		/// <summary>
		/// Ensure that buffer().capacity() is greater than or equal to capacity.  If
		/// it is, just set the limit to the capacity.
		/// Otherwise, allocate a new buffer and copy everything from 0 to the position
		/// to the new buffer, set the same position and set the limit to the new
		/// capacity.
		/// Note that this does not copy the mark to the new buffer.
		/// </summary>
		///
		/// <param name="capacity">The minimum needed capacity.</param>
		public void ensureCapacity(int capacity) {
			if (buffer_.capacity() >= capacity) {
				// Make sure the limit stays at the capacity while we are writing.
				buffer_.limit(buffer_.capacity());
				return;
			}
	
			// See if double is enough.
			int newCapacity = buffer_.capacity() * 2;
			if (capacity > newCapacity)
				// The needed capacity is much greater, so use it.
				newCapacity = capacity;
	
			ByteBuffer newBuffer = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(newCapacity);
			// Save the position so we can reset before calling put.
			int savePosition = buffer_.position();
			buffer_.flip();
			newBuffer.put(buffer_);
	
			// Preserve the position and limit.
			newBuffer.position(savePosition);
			newBuffer.limit(newBuffer.capacity());
	
			buffer_ = newBuffer;
		}
	
		/// <summary>
		/// Use ensureCapacity to ensure there are remainingCapacity bytes after
		/// position().
		/// </summary>
		///
		/// <param name="remainingCapacity">The desired minimum capacity after position().</param>
		public void ensureRemainingCapacity(int remainingCapacity) {
			ensureCapacity(buffer_.position() + remainingCapacity);
		}
	
		/// <summary>
		/// Call ensureCapacity to ensure there is capacity for 1 more byte and call
		/// buffer().put(b).
		/// This increments the position by 1.
		/// </summary>
		///
		/// <param name="b">The byte to put.</param>
		public void ensuredPut(byte b) {
			ensureCapacity(buffer_.position() + 1);
			buffer_.put(b);
		}
	
		/// <summary>
		/// Call ensureCapacity to ensure there is capacity for buffer.remaining() more
		/// bytes and use buffer().put to copy.
		/// This increments the position by buffer.remaining().
		/// This does update buffer's position to its limit.
		/// </summary>
		///
		/// <param name="buffer"></param>
		public void ensuredPut(ByteBuffer buffer) {
			ensureRemainingCapacity(buffer.remaining());
			int savePosition = buffer.position();
			buffer_.put(buffer);
			buffer.position(savePosition);
		}
	
		/// <summary>
		/// Call ensureCapacity to ensure there is capacity for (limit - position) more
		/// bytes and use buffer().put to copy.
		/// This increments the position by (limit - position).
		/// </summary>
		///
		/// <param name="buffer"></param>
		/// <param name="position">The position in buffer to copy from.</param>
		/// <param name="limit">The limit in buffer to copy from.</param>
		public void ensuredPut(ByteBuffer buffer, int position, int limit) {
			ensureRemainingCapacity(limit - position);
			int savePosition = buffer.position();
			int saveLimit = buffer.limit();
			try {
				buffer.position(position);
				buffer.limit(limit);
				buffer_.put(buffer);
			} finally {
				// put updates buffer's position and limit, so restore.
				buffer.position(savePosition);
				buffer.limit(saveLimit);
			}
		}
	
		/// <summary>
		/// Ensure that buffer().capacity() is greater than or equal to capacity.  If
		/// it is, just set the limit to the capacity.
		/// Otherwise, allocate a new buffer and copy everything from the position to
		/// the limit to the back of the new buffer, set the limit to the new capacity
		/// and set the position to keep the same number of remaining bytes.
		/// Note that this does not copy the mark to the new buffer.
		/// </summary>
		///
		/// <param name="capacity">The minimum needed capacity.</param>
		public void ensureCapacityFromBack(int capacity) {
			if (buffer_.capacity() >= capacity) {
				// Make sure the limit stays at the capacity while we are writing.
				buffer_.limit(buffer_.capacity());
				return;
			}
	
			// See if double is enough.
			int newCapacity = buffer_.capacity() * 2;
			if (capacity > newCapacity)
				// The needed capacity is much greater, so use it.
				newCapacity = capacity;
	
			ByteBuffer newBuffer = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(newCapacity);
			// Save the remaining so we can restore the position later.
			int saveRemaining = buffer_.remaining();
			newBuffer.position(newBuffer.capacity() - saveRemaining);
			newBuffer.put(buffer_);
	
			// The limit is still at capacity().  Set the position.
			newBuffer.position(newBuffer.capacity() - saveRemaining);
	
			buffer_ = newBuffer;
		}
	
		/// <summary>
		/// Change the position so that there are remaining bytes in the buffer. If
		/// position would be negative, use ensureCapacityFromBack to expand the
		/// buffer.
		/// </summary>
		///
		/// <param name="remaining"></param>
		/// <returns>The new position.</returns>
		public int setRemainingFromBack(int remaining) {
			ensureCapacityFromBack(remaining);
			buffer_.position(buffer_.limit() - remaining);
			return buffer_.position();
		}
	
		/// <summary>
		/// Call setRemainingFromBack to ensure there are remaining bytes for 1 more
		/// byte and put b at the new position.
		/// </summary>
		///
		/// <param name="b">The byte to put.</param>
		public void ensuredPutFromBack(byte b) {
			buffer_.put(setRemainingFromBack(buffer_.remaining() + 1), b);
		}
	
		/// <summary>
		/// Return the ByteBuffer.  Note that ensureCapacity can change the returned
		/// ByteBuffer.
		/// </summary>
		///
		/// <returns>The ByteBuffer.</returns>
		public ByteBuffer buffer() {
			return buffer_;
		}
	
		/// <summary>
		/// Return a new ByteBuffer which is the flipped version of buffer().  The
		/// returned buffer's position is 0 and its
		/// limit is position().
		/// </summary>
		///
		/// <returns>A new ByteBuffer</returns>
		public ByteBuffer flippedBuffer() {
			ByteBuffer result = buffer_.duplicate();
			result.flip();
			return result;
		}
	
		/// <summary>
		/// Return buffer_.position().
		/// </summary>
		///
		/// <returns>The position.</returns>
		public int position() {
			return buffer_.position();
		}
	
		/// <summary>
		/// Call buffer_.position(newPosition).
		/// </summary>
		///
		/// <param name="newPosition">The new position.</param>
		public void position(int newPosition) {
			buffer_.position(newPosition);
		}
	
		/// <summary>
		/// Return buffer_.limit().
		/// </summary>
		///
		/// <returns>The limit.</returns>
		public int limit() {
			return buffer_.limit();
		}
	
		/// <summary>
		/// Return buffer_.remaining().
		/// </summary>
		///
		/// <returns>The number of remaining bytes.</returns>
		public int remaining() {
			return buffer_.remaining();
		}
	
		private ByteBuffer buffer_;
	}
}
