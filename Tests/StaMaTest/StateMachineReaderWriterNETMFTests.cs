#region BinaryReaderWriterTests.cs file
//
// Tests for StaMa state machine controller library
//
// Copyright (c) 2005-2015, Roland Schneider. All rights reserved.
//
#endregion

using System;
using StaMa;
using System.IO;
using System.Collections;

#if !MF_FRAMEWORK
using NUnit.Framework;
#else
using MFUnitTest.Framework;
using Microsoft.SPOT;
#endif


namespace StaMaTest
{
    [TestFixture]
    public class StateMachineReaderWriterNETMFTests
    {
        [Test]
        public void BinaryReaderConstructor_ValidArguments_IsProperlyInitialized()
        {
            // Act
            StaMa.BinaryReader reader = new StaMa.BinaryReader(new MemoryStream(new Byte[] { }), System.Text.Encoding.UTF8);

            // Assert
            Assert.That(new TestDelegate(delegate() { reader.ReadByte(); }), Throws.TypeOf(typeof(IOException)));
            Assert.That(new TestDelegate(delegate() { reader.Dispose(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { reader.ReadByte(); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { reader.ReadInt16(); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { reader.ReadString(); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { reader.ToString(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { reader.Dispose(); }), Throws.Nothing); // 2nd Dispose doesn't access any cleared members.
            Assert.That(new TestDelegate(delegate() { reader.Close(); }), Throws.Nothing);

            // Act
            StaMa.BinaryReader reader2 = new StaMa.BinaryReader(new MemoryStream(new Byte[] { }), System.Text.Encoding.UTF8);

            // Assert
            Assert.That(new TestDelegate(delegate() { reader2.Close(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { reader2.ReadByte(); }), Throws.TypeOf(typeof(ObjectDisposedException)));
        }


        [Test]
        public void BinaryReaderConstructor_InvalidArguments_Throws()
        {
            Assert.That(new TestDelegate(delegate() { new StaMa.BinaryReader(null, System.Text.Encoding.UTF8); }), Throws.TypeOf(typeof(ArgumentNullException)));
            Assert.That(new TestDelegate(delegate() { new StaMa.BinaryReader(new MemoryStream(new Byte[] { }), null); }), Throws.TypeOf(typeof(ArgumentNullException)));
        }


#if !MF_FRAMEWORK
        [TestCase(new Byte[] { 52 }, TestName = "BinaryReaderReadByte_ValidStream1_ReturnsExpectedValues")]
        [TestCase(new Byte[] { 52, 63 }, TestName = "BinaryReaderReadByte_ValidStream2_ReturnsExpectedValues")]
        [TestCase(new Byte[] { 52, 63, 107 }, TestName = "BinaryReaderReadByte_ValidStream3_ReturnsExpectedValues")]
        public void BinaryReaderReadByte_ValidStream_ReturnsExpectedValues(Byte[] expectedBytes)
#else
        public void BinaryReaderReadByte_ValidStream1_ReturnsExpectedValues() { BinaryReaderReadByte_ValidStream_ReturnsExpectedValues(new Byte[] { 52 }); }
        public void BinaryReaderReadByte_ValidStream2_ReturnsExpectedValues() { BinaryReaderReadByte_ValidStream_ReturnsExpectedValues(new Byte[] { 52, 63 }); }
        public void BinaryReaderReadByte_ValidStream3_ReturnsExpectedValues() { BinaryReaderReadByte_ValidStream_ReturnsExpectedValues(new Byte[] { 52, 63, 107 }); }
        private void BinaryReaderReadByte_ValidStream_ReturnsExpectedValues(Byte[] expectedBytes)
#endif
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            stream.Write(expectedBytes, 0, expectedBytes.Length);
            stream.Position = 0;
            StaMa.BinaryReader reader = new StaMa.BinaryReader(stream, System.Text.Encoding.UTF8);

            for (int i = 0; i < expectedBytes.Length; i++)
            {
                // Act
                Byte result = reader.ReadByte();

                // Assert
                Assert.That(result, Is.EqualTo(expectedBytes[i]));
            }

            // Assert
            Assert.That(new TestDelegate(delegate() { reader.ReadByte(); }), Throws.TypeOf(typeof(IOException)));
        }


#if !MF_FRAMEWORK
        [TestCase(new Int16[] { 0x6952 }, TestName = "BinaryReaderReadInt16_ValidStream1_ReturnsExpectedValues")]
        [TestCase(new Int16[] { 0x6952, 0x6317 }, TestName = "BinaryReaderReadInt16_ValidStream2_ReturnsExpectedValues")]
        [TestCase(new Int16[] { 0x6952, 0x6317, 0x7107 }, TestName = "BinaryReaderReadInt16_ValidStream3_ReturnsExpectedValues")]
        public void BinaryReaderReadInt16_ValidStream_ReturnsExpectedValues(Int16[] expectedInt16s)
#else
        public void BinaryReaderReadInt16_ValidStream1_ReturnsExpectedValues() { BinaryReaderReadInt16_ValidStream_ReturnsExpectedValues(new Int16[] { 0x6952 }); }
        public void BinaryReaderReadInt16_ValidStream2_ReturnsExpectedValues() { BinaryReaderReadInt16_ValidStream_ReturnsExpectedValues(new Int16[] { 0x6952, 0x6317 }); }
        public void BinaryReaderReadInt16_ValidStream3_ReturnsExpectedValues() { BinaryReaderReadInt16_ValidStream_ReturnsExpectedValues(new Int16[] { 0x6952, 0x6317, 0x7107 }); }
        private void BinaryReaderReadInt16_ValidStream_ReturnsExpectedValues(Int16[] expectedInt16s)
#endif
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            for (int i = 0; i < expectedInt16s.Length; i++)
            {
                stream.WriteByte((Byte)expectedInt16s[i]);
                stream.WriteByte((Byte)(expectedInt16s[i] >> 8));
            }
            stream.Position = 0;
            StaMa.BinaryReader reader = new StaMa.BinaryReader(stream, System.Text.Encoding.UTF8);

            for (int i = 0; i < expectedInt16s.Length; i++)
            {
                // Act
                Int16 result = reader.ReadInt16();

                // Assert
                Assert.That(result, Is.EqualTo(expectedInt16s[i]));
            }

            // Assert
            Assert.That(new TestDelegate(delegate() { reader.ReadInt16(); }), Throws.TypeOf(typeof(IOException)));
        }


#if !MF_FRAMEWORK
        [TestCase(new Object[] { "Hello" }, TestName = "BinaryReaderReadString_ValidStream1_ReturnsExpectedValues")]
        [TestCase(new Object[] { "Lorem", "ipsum" }, TestName = "BinaryReaderReadString_ValidStream2_ReturnsExpectedValues")]
        [TestCase(new Object[] { "feugiat", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris sit amet dolor id arcu tincidunt scelerisque. Donec et urna dignissim", "imperdiet" }, TestName = "BinaryReaderReadString_ValidStream3_ReturnsExpectedValues")]
        public void BinaryReaderReadString_ValidStream_ReturnsExpectedValues(Object[] expectedStrings)
#else
        public void BinaryReaderReadString_ValidStream1_ReturnsExpectedValues() { BinaryReaderReadString_ValidStream_ReturnsExpectedValues(new Object[] { "Hello" }); }
        public void BinaryReaderReadString_ValidStream2_ReturnsExpectedValues() { BinaryReaderReadString_ValidStream_ReturnsExpectedValues(new Object[] { "Lorem", "ipsum" }); }
        public void BinaryReaderReadString_ValidStream3_ReturnsExpectedValues() { BinaryReaderReadString_ValidStream_ReturnsExpectedValues(new Object[] { "feugiat", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris sit amet dolor id arcu tincidunt scelerisque. Donec et urna dignissim", "imperdiet" }); }
        private void BinaryReaderReadString_ValidStream_ReturnsExpectedValues(Object[] expectedStrings)
#endif
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            foreach (String expectedString in expectedStrings)
            {
                Byte[] expectedStringBytes = System.Text.Encoding.UTF8.GetBytes(expectedString);

                UInt32 val = (UInt32)expectedStringBytes.Length;
                while (val >= 0x80)
                {
                    stream.WriteByte((Byte)(val | 0x80));
                    val = val >> 7;
                }
                stream.WriteByte((Byte)val);
                stream.Write(expectedStringBytes, 0, expectedStringBytes.Length);
            }
            stream.Position = 0;
            StaMa.BinaryReader reader = new StaMa.BinaryReader(stream, System.Text.Encoding.UTF8);

            foreach (String expectedString in expectedStrings)
            {
                // Act
                String result = reader.ReadString();

                // Assert
                Assert.That(result, Is.EqualTo(expectedString));
            }

            // Assert
            Assert.That(new TestDelegate(delegate() { reader.ReadString(); }), Throws.TypeOf(typeof(IOException)));
        }


        [Test]
        public void BinaryWriterConstructor_ValidArguments_IsProperlyInitialized()
        {
            // Act
            StaMa.BinaryWriter writer = new StaMa.BinaryWriter(new MemoryStream(), System.Text.Encoding.UTF8);

            // Assert
            Assert.That(new TestDelegate(delegate() { writer.Flush(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { writer.Dispose(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { writer.Write((Byte)0); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { writer.Write((Int16)0); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { writer.Write(String.Empty); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { writer.Flush(); }), Throws.TypeOf(typeof(ObjectDisposedException)));
            Assert.That(new TestDelegate(delegate() { writer.ToString(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { writer.Dispose(); }), Throws.Nothing); // 2nd Dispose doesn't access any cleared members.
            Assert.That(new TestDelegate(delegate() { writer.Close(); }), Throws.Nothing);

            // Act
            StaMa.BinaryWriter writer2 = new StaMa.BinaryWriter(new MemoryStream(new Byte[] { }), System.Text.Encoding.UTF8);

            // Assert
            Assert.That(new TestDelegate(delegate() { writer2.Close(); }), Throws.Nothing);
            Assert.That(new TestDelegate(delegate() { writer2.Write((Byte)0); }), Throws.TypeOf(typeof(ObjectDisposedException)));
        }


        [Test]
        public void BinaryWriterConstructor_InvalidArguments_Throws()
        {
            Assert.That(new TestDelegate(delegate() { new StaMa.BinaryWriter(null, System.Text.Encoding.UTF8); }), Throws.TypeOf(typeof(ArgumentNullException)));
            Assert.That(new TestDelegate(delegate() { new StaMa.BinaryWriter(new MemoryStream(), null); }), Throws.TypeOf(typeof(ArgumentNullException)));
        }



#if !MF_FRAMEWORK
        [TestCase(new Byte[] { 52 }, TestName = "BinaryWriterWriteByte_ValidStream1_ReturnsExpectedValues")]
        [TestCase(new Byte[] { 52, 63 }, TestName = "BinaryWriterWriteByte_ValidStream2_ReturnsExpectedValues")]
        [TestCase(new Byte[] { 52, 63, 107 }, TestName = "BinaryWriterWriteByte_ValidStream3_ReturnsExpectedValues")]
        public void BinaryWriterWriteByte_ValidStream_ReturnsExpectedValues(Byte[] expectedBytes)
#else
        public void BinaryWriterWriteByte_ValidStream1_ReturnsExpectedValues() { BinaryWriterWriteByte_ValidStream_ReturnsExpectedValues(new Byte[] { 52 }); }
        public void BinaryWriterWriteByte_ValidStream2_ReturnsExpectedValues() { BinaryWriterWriteByte_ValidStream_ReturnsExpectedValues(new Byte[] { 52, 63 }); }
        public void BinaryWriterWriteByte_ValidStream3_ReturnsExpectedValues() { BinaryWriterWriteByte_ValidStream_ReturnsExpectedValues(new Byte[] { 52, 63, 107 }); }
        private void BinaryWriterWriteByte_ValidStream_ReturnsExpectedValues(Byte[] expectedBytes)
#endif
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            //stream.Write(expectedBytes, 0, expectedBytes.Length);
            //stream.Position = 0;
            StaMa.BinaryWriter writer = new StaMa.BinaryWriter(stream, System.Text.Encoding.UTF8);

            // Act
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                writer.Write(expectedBytes[i]);
            }
            writer.Flush();

            // Assert
            Assert.That(stream.ToArray(), Is.EqualTo(expectedBytes));
        }


#if !MF_FRAMEWORK
        [TestCase(new Int16[] { 0x6952 }, TestName = "BinaryWriterWriteInt16_ValidStream1_ReturnsExpectedValues")]
        [TestCase(new Int16[] { 0x6952, 0x6317 }, TestName = "BinaryWriterWriteInt16_ValidStream2_ReturnsExpectedValues")]
        [TestCase(new Int16[] { 0x6952, 0x6317, 0x7107 }, TestName = "BinaryWriterWriteInt16_ValidStream3_ReturnsExpectedValues")]
        public void BinaryWriterWriteInt16_ValidStream_ReturnsExpectedValues(Int16[] expectedInt16s)
#else
        public void BinaryWriterWriteInt16_ValidStream1_ReturnsExpectedValues() { BinaryWriterWriteInt16_ValidStream_ReturnsExpectedValues(new Int16[] { 0x6952 }); }
        public void BinaryWriterWriteInt16_ValidStream2_ReturnsExpectedValues() { BinaryWriterWriteInt16_ValidStream_ReturnsExpectedValues(new Int16[] { 0x6952, 0x6317 }); }
        public void BinaryWriterWriteInt16_ValidStream3_ReturnsExpectedValues() { BinaryWriterWriteInt16_ValidStream_ReturnsExpectedValues(new Int16[] { 0x6952, 0x6317, 0x7107 }); }
        private void BinaryWriterWriteInt16_ValidStream_ReturnsExpectedValues(Int16[] expectedInt16s)
#endif
        {
            // Arrange
            Byte[] expectedBytes = new Byte[2 * expectedInt16s.Length];
            int pos = 0;
            for (int i = 0; i < expectedInt16s.Length; i++)
            {
                expectedBytes[pos] = (Byte)expectedInt16s[i];
                pos += 1;
                expectedBytes[pos] = (Byte)(expectedInt16s[i] >> 8);
                pos += 1;
            }

            MemoryStream stream = new MemoryStream();
            //stream.Position = 0;
            StaMa.BinaryWriter writer = new StaMa.BinaryWriter(stream, System.Text.Encoding.UTF8);

            // Act
            for (int i = 0; i < expectedInt16s.Length; i++)
            {
                writer.Write(expectedInt16s[i]);
            }
            writer.Flush();

            // Assert
            Assert.That(stream.ToArray(), Is.EqualTo(expectedBytes));
        }


#if !MF_FRAMEWORK
        [TestCase(new Object[] { "Hello" }, TestName = "BinaryWriterWriteString_ValidStream1_ReturnsExpectedValues")]
        [TestCase(new Object[] { "Lorem", "ipsum" }, TestName = "BinaryWriterWriteString_ValidStream2_ReturnsExpectedValues")]
        [TestCase(new Object[] { "feugiat", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris sit amet dolor id arcu tincidunt scelerisque. Donec et urna dignissim", "imperdiet" }, TestName = "BinaryWriterWriteString_ValidStream3_ReturnsExpectedValues")]
        public void BinaryWriterWriteString_ValidStream_ReturnsExpectedValues(Object[] expectedStrings)
#else
        public void BinaryWriterWriteString_ValidStream1_ReturnsExpectedValues() { BinaryWriterWriteString_ValidStream_ReturnsExpectedValues(new Object[] { "Hello" }); }
        public void BinaryWriterWriteString_ValidStream2_ReturnsExpectedValues() { BinaryWriterWriteString_ValidStream_ReturnsExpectedValues(new Object[] { "Lorem", "ipsum" }); }
        public void BinaryWriterWriteString_ValidStream3_ReturnsExpectedValues() { BinaryWriterWriteString_ValidStream_ReturnsExpectedValues(new Object[] { "feugiat", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris sit amet dolor id arcu tincidunt scelerisque. Donec et urna dignissim", "imperdiet" }); }
        private void BinaryWriterWriteString_ValidStream_ReturnsExpectedValues(Object[] expectedStrings)
#endif
        {
            // Arrange
            MemoryStream expectedData = new MemoryStream();
            foreach (String expectedString in expectedStrings)
            {
                Byte[] expectedStringBytes = System.Text.Encoding.UTF8.GetBytes(expectedString);

                UInt32 val = (UInt32)expectedStringBytes.Length;
                while (val >= 0x80)
                {
                    expectedData.WriteByte((Byte)(val | 0x80));
                    val = val >> 7;
                }
                expectedData.WriteByte((Byte)val);
                expectedData.Write(expectedStringBytes, 0, expectedStringBytes.Length);
            }
            expectedData.Flush();
            Byte[] expectedBytes = expectedData.ToArray();

            MemoryStream stream = new MemoryStream();
            StaMa.BinaryWriter writer = new StaMa.BinaryWriter(stream, System.Text.Encoding.UTF8);

            // Act
            foreach (String expectedString in expectedStrings)
            {
                writer.Write(expectedString);
            }
            writer.Flush();

            // Assert
            Assert.That(stream.ToArray(), Is.EqualTo(expectedBytes));
        }


        [Test]
        public void BinaryWriterWriteBinaryReaderRead_SomeData_ReaderCanReadWhatWriterWrites()
        {
            // Arrange
            ArrayList data = new ArrayList();
            data.Add((Byte)255);
            data.Add("asdf\0ka sdfk jla ssd");
            data.Add("\u2302");
            data.Add(new String('y', 0x3FFF + 1)); // One more than 2 * 7 bit
            data.Add((Byte)45);
            data.Add((Int16)9876);

            MemoryStream stream = new MemoryStream();
            StaMa.BinaryWriter writer = new StaMa.BinaryWriter(stream, System.Text.Encoding.UTF8);

            // Act
            foreach (Object item in data)
            {
                if (item.GetType() == typeof(String))
                {
                    writer.Write((String)item);
                }
                else if (item.GetType() == typeof(Int16))
                {
                    writer.Write((Int16)item);
                }
                else if (item.GetType() == typeof(Byte))
                {
                    writer.Write((Byte)item);
                }
                else
                {
                    throw new NotSupportedException(item.GetType() + " data not supported by BinaryWriter and BinaryReader");
                }
            }
            writer.Flush();
            stream.Position = 0;

            StaMa.BinaryReader reader = new StaMa.BinaryReader(stream, System.Text.Encoding.UTF8);
            foreach (Object item in data)
            {
                Object result;
                if (item.GetType() == typeof(String))
                {
                    result = reader.ReadString();
                }
                else if (item.GetType() == typeof(Int16))
                {
                    result = reader.ReadInt16();
                }
                else if (item.GetType() == typeof(Byte))
                {
                    result = reader.ReadByte();
                }
                else
                {
                    throw new InvalidOperationException("Unexpected data encountered.");
                }

                // Assert
                Assert.That(result, Is.EqualTo(item));
            }
        }
    }
}
