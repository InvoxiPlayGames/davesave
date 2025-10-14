/*
 * EncryptedReadRevisionStream.cs
 * 
 * Copyright (c) 2015,2016,2017 maxton. All rights reserved.
 * Copyright (c) 2025 Emma / InvoxiPlayGames
 * 
 * 14-Oct-2025(Emma): Adapted the EncryptedReadStream class to be
 * more well-equipped for reading encrypted RevisionStreams.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; If not, see
 * <http://www.gnu.org/licenses/>.
 */
using System;
using System.IO;

namespace LibForge.Util
{
    class EncryptedReadRevisionStream : Stream
    {
        private long position;
        private int key;
        private int curKey;
        private long keypos;
        private Stream file;
        public byte xor;

        internal EncryptedReadRevisionStream(Stream file, bool ps3 = false)
        {
            file.Position = 0;
            // Make sure we are actually a RevisionStream
            if (file.ReadUInt8() != 0x7A)
                throw new Exception("Input isn't a RevisionStream");
            if (file.ReadUInt32LE() != 0)
                throw new Exception("Unexpected version for encrypted RevisionStream");
            // The initial key is found in the first 4 bytes.
            int keyNum;
            if (ps3)
            {
                keyNum = file.ReadInt32BE();
            } else
            {
                keyNum = file.ReadInt32LE();
            }
            this.key = cryptRound(keyNum);
            this.position = 0;
            this.keypos = 0;
            this.curKey = this.key;
            this.file = file;
            this.Length = file.Length - (4 + 5) - 1;
            if (keyNum < 0)
                this.xor = 0xFF;
            else
                this.xor = 0x00;
        }

        public override bool CanRead => position < Length && position >= 0;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length { get; }

        public override long Position
        {
            get
            {
                return position;
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        private void updateKey()
        {
            if (keypos == position)
                return;
            if (keypos > position) // reset key
            {
                keypos = 0;
                curKey = key;
            }
            while (keypos < position) // don't think there's a faster way to do this
            {
                curKey = cryptRound(curKey);
                keypos++;
            }
        }

        private int cryptRound(int key)
        {
            int ret = (key - ((key / 0x1F31D) * 0x1F31D)) * 0x41A7 - (key / 0x1F31D) * 0xB14;
            if (ret <= 0)
                ret += 0x7FFFFFFF;
            return ret;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // ensure file is at correct offset
            file.Seek(this.position + (4 + 5), SeekOrigin.Begin);
            if (offset + count > buffer.Length)
            {
                throw new IndexOutOfRangeException("Attempt to fill buffer past its end");
            }
            if (this.Position == this.Length || this.Position + count > this.Length)
            {
                count = (int)(this.Length - this.Position);
                //throw new System.IO.EndOfStreamException("Cannot read past end of file.");
            }

            int bytesRead = file.Read(buffer, offset, count);

            for (uint i = 0; i < bytesRead; i++)
            {
                buffer[offset + i] ^= (byte)(this.curKey ^ xor);
                this.position++;
                updateKey();
            }
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            int adjust = origin == SeekOrigin.Current ? 0 : (4 + 5);
            this.position = file.Seek(offset + adjust, origin) - (4 + 5);
            updateKey();
            return position;
        }

        public void FinishReading()
        {
            byte magic = file.ReadUInt8();
            if (magic != 0x7B)
                throw new Exception($"Unexpected {magic:X2} at end of EncryptedReadRevisionStream!");
        }

        #region Not Used

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
