namespace davesave.Saves
{
    public class RevisionStream : Stream
    {
        private Stream _stream;
        public int Version;
        private int _curVersion;
        private bool _bigEndian;

        public RevisionStream(Stream original, int minVersion, int curVersion, bool bigEndian = false)
        {
            byte magic = original.ReadUInt8();
            if (magic != 0x7A)
                throw new Exception("Input is not a RevisionStream!");

            int version;
            if (bigEndian)
                version = original.ReadInt32BE();
            else
                version = original.ReadInt32LE();

            if (version < minVersion)
                throw new Exception($"RevisionStream version {version} is older than {minVersion}");

            if (curVersion < version)
                throw new Exception($"RevisionStream version {version} is newer than {curVersion}");

            Version = version;
            _stream = original;
            _curVersion = curVersion;
            _bigEndian = bigEndian;
        }

        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => _stream.CanSeek;

        public override bool CanWrite => _stream.CanWrite;

        public override long Length => _stream.Length;

        public override long Position { get => _stream.Position; set => _stream.Position = value; }

        public void FinishReading()
        {
            byte magic = _stream.ReadUInt8();
            if (magic != 0x7B)
                throw new Exception($"Unexpected {magic:X2} at end of RevisionStream!");
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}
