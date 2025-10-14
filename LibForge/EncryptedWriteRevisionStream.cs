/*
 * EncryptedWriteRevisionStream.cs
 * 
 * Copyright (c) 2015,2016,2017 maxton. All rights reserved.
 * Copyright (c) 2025 Emma / InvoxiPlayGames
 * 
 * 14-Oct-2025(Emma): Adapted the EncryptedWriteStream class to be
 * more well-equipped for writing encrypted RevisionStreams.
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
    public class EncryptedWriteRevisionStream : Stream
    {
        private long position;
        private int key;
        private int curKey;
        private long keypos;
        private Stream file;
        public byte xor;


        internal EncryptedWriteRevisionStream(Stream file, int key, bool ps3 = false)
        {
            file.Position = 0;
            // write the RevisionStream header
            file.WriteUInt8(0x7A);
            file.WriteUInt32LE(0); // 0 = 0 regardless of endianness
            if (ps3)
            {
                file.WriteUInt32BE((uint)key);
            } else
            {
                file.WriteInt32LE(key);
            }
            position = 0;
            keypos = 0;
            // The initial key is found in the first 4 bytes.
            this.key = cryptRound(key);
            this.curKey = this.key;
            this.file = file;
            if (key < 0)
                this.xor = 0xFF;
            else
                this.xor = 0x00;
        }

        public override bool CanRead => false;
        public override bool CanSeek => true;
        public override bool CanWrite => file.CanWrite;
        public override long Length => file.Length - (4 + 5);

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
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            int adjust = origin == SeekOrigin.Current ? 0 : (4 + 5);
            this.position = file.Seek(offset + adjust, origin) - (4 + 5);
            updateKey();
            return position;
        }

        public void FinishWriting()
        {
            file.WriteUInt8(0x7B);
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
            if (offset + count > buffer.Length)
            {
                throw new IndexOutOfRangeException("Attempt to read buffer past its end");
            }
            updateKey();
            var copy = new byte[count];
            Buffer.BlockCopy(buffer, offset, copy, 0, count);
            for (uint i = 0; i < count; i++)
            {
                copy[i] ^= (byte)(this.curKey ^ xor);
                position++;
                updateKey();
            }
            // ensure file is at correct offset
            file.Seek(this.position + (4 + 5) - count, SeekOrigin.Begin);
            file.Write(copy, 0, count);
        }

        #endregion
    }
}
