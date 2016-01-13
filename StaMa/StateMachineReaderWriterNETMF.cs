#region StateMachineReaderWriterNETMF.cs file
//
// StaMa state machine controller library, http://stama.codeplex.com/
//
// Copyright (c) 2005-2015, Roland Schneider. All rights reserved.
//
#endregion

using System;
#if !MF_FRAMEWORK
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.IO;
#if MF_FRAMEWORK
using Microsoft.SPOT;
#endif


namespace StaMa
{
    internal class BinaryReader : IDisposable
    {
        private Stream m_stream;
        private System.Text.Encoding m_encoding;
        private byte[] m_buffer;


        public BinaryReader(Stream input, System.Text.Encoding encoding)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            if (!input.CanRead)
            {
                throw new ArgumentException("Stream was not readable", "input");
            }
            m_stream = input;
            m_encoding = encoding;
            m_buffer = new byte[0x10];
        }


        public short ReadInt16()
        {
            this.FillBuffer(2);
            return (short)(m_buffer[0] | (m_buffer[1] << 8));
        }


        public virtual string ReadString()
        {
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryReader).FullName);
            }
            int length = this.Read7BitEncodedInt();
            if (length < 0)
            {
                throw new IOException("BinaryReader encountered an invalid string length.");
            }
            if (length == 0)
            {
                return string.Empty;
            }

            byte[] buffer = new byte[length];
            m_stream.Read(buffer, 0, length);
            char[] chars = m_encoding.GetChars(buffer, 0, length);
            string ret = new String(chars);
            return ret;
        }


        public virtual byte ReadByte()
        {
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryReader).FullName);
            }
            int num = m_stream.ReadByte();
            if (num == -1)
            {
                throw new IOException("Unable to read beyond the end of the stream.");
            }
            return (byte)num;
        }

            
        protected int Read7BitEncodedInt()
        {
            // Read an Int32 splitted into bytes with 7 bit payload and high bit used as continuation marker.
            int value = 0;
            int shift = 0;
            byte b;
            do
            {
                if (shift == 5 * 7)  // Int32 has 32 bits, splitting into 7 bit groups may at most result in 5 bytes.
                {
                    throw new IOException("Too many bytes in what should have been a 7 bit encoded Int32.");
                }

                b = ReadByte();
                value |= (b & 0x7F) << shift;
                shift += 7;
            }
            while ((b & 0x80) != 0);
            return value;
        }

            
        protected virtual void FillBuffer(int numBytes)
        {
            if ((m_buffer != null) && ((numBytes < 0) || (numBytes > m_buffer.Length)))
            {
                throw new ArgumentOutOfRangeException("numBytes", "The number of bytes requested does not fit into BinaryReader's internal buffer.");
            }
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryReader).FullName);
            }

            int offset = 0;
            do
            {
                int bytesRead = m_stream.Read(m_buffer, offset, numBytes - offset);
                if (bytesRead == 0)
                {
                    throw new IOException("Unable to read beyond the end of the stream.");
                }
                offset += bytesRead;
            }
            while (offset < numBytes);
        }

            
        public virtual void Close()
        {
            this.Dispose(true);
        }


        public void Dispose()
        {
            this.Dispose(true);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_stream != null)
                {
                    m_stream.Close();
                }
                m_stream = null;
                m_buffer = null;
                m_encoding = null;
            }
        }

    }


    internal class BinaryWriter : IDisposable
    {
        private Stream m_stream;
        private byte[] m_buffer;
        private System.Text.Encoding m_encoding;


        public BinaryWriter(Stream output, System.Text.Encoding encoding)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            if (!output.CanWrite)
            {
                throw new ArgumentException("Stream was not writable", "output");
            }
            m_stream = output;
            m_buffer = new byte[0x10];
            this.m_encoding = encoding;
        }


        public void Write(System.Int16 value)
        {
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryWriter).FullName);
            }
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            m_stream.Write(m_buffer, 0, 2);
        }


        public void Write(string value)
        {
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryWriter).FullName);
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            byte[] bytes = m_encoding.GetBytes(value);
            this.Write7BitEncodedInt(bytes.Length);
            m_stream.Write(bytes, 0, bytes.Length);
        }


        public virtual void Write(byte value)
        {
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryWriter).FullName);
            }
            m_stream.WriteByte(value);
        }


        protected void Write7BitEncodedInt(Int32 value)
        {
            // Write an Int32 by splitting into bytes with 7 bit payload and high bit used as continuation marker.
            UInt32 val = (UInt32)value;
            while (val >= 0x80)
            {
                Write((byte)(val | 0x80));
                val = val >> 7;
            }
            Write((Byte)val);
        }


        public virtual void Flush()
        {
            if (m_stream == null)
            {
                throw new ObjectDisposedException(typeof(BinaryWriter).FullName);
            }
            m_stream.Flush();
        }

            
        public virtual void Close()
        {
            this.Dispose(true);
        }

            
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_stream != null)
                {
                    m_stream.Close();
                }
                m_stream = null;
                m_buffer = null;
                m_encoding = null;
            }
        }


        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
