/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: ByteArray
 * Date    : 2018/03/08
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections.Generic;

namespace GEngine.Managers
{
    public class ByteArray
    {
        readonly List<byte> buffer = new List<byte>();
        readonly object locker = new object();

        public int Length { get {lock(locker) {return buffer.Count;}} }

        public void Push(byte[] bytes)
        {
            lock (locker)
            {
                buffer.AddRange(bytes);
            }
        }

        public void Push(byte[] bytes, int start, int offset)
        {
            lock (locker)
            {
                byte[] buf = new byte[offset];
                Array.Copy(bytes, start, buf, 0, offset);
                buffer.AddRange(buf);
            }
        }

        public byte[] Pop(int length)
        {
            if(length > Length)
                throw new Exception("Pop Error:: Get Byte Length More Than Have.");
            lock (locker)
            {
                var bytes = buffer.GetRange(0, length);
                buffer.RemoveRange(0, length);
                return bytes.ToArray();
            }
        }

        public byte[] Peek(int length)
        {
            lock (locker)
            {
                var bytes = buffer.GetRange(0, length);
                return bytes.ToArray();
            }
        }

        public void Clear()
        {
            lock (locker)
            {
                buffer.Clear();
            }
        }
    }
}