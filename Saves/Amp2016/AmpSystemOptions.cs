namespace davesave.Saves.Amp2016
{
    public class AmpSystemOptionsPersistentData
    {
#pragma warning disable CS8618
        public string mControlScheme;
        // TODO: figure out what gets stored in these arrays
        public int[] mFlags1; // always 26 entries
        public int[] mFlags2; // always 2 entries
#pragma warning restore CS8618

        public void WriteToStream(Stream stream, bool isBE = false)
        {
            stream.WriteLengthUTF8(mControlScheme, isBE);
            if (isBE)
            {
                stream.WriteUInt32BE((uint)mFlags1.Length);
                for (int i = 0; i < mFlags1.Length; i++)
                    stream.WriteUInt32BE((uint)mFlags1[i]);
                stream.WriteUInt32BE((uint)mFlags2.Length);
                for (int i = 0; i < mFlags2.Length; i++)
                    stream.WriteUInt32BE((uint)mFlags2[i]);
            }
            else
            {
                stream.WriteInt32LE(mFlags1.Length);
                for (int i = 0; i < mFlags1.Length; i++)
                    stream.WriteInt32LE(mFlags1[i]);
                stream.WriteInt32LE(mFlags2.Length);
                for (int i = 0; i < mFlags2.Length; i++)
                    stream.WriteInt32LE(mFlags2[i]);
            }
        }

        public static AmpSystemOptionsPersistentData ReadFromStream(Stream stream, bool isBE = false)
        {
            AmpSystemOptionsPersistentData options = new();

            options.mControlScheme = stream.ReadLengthUTF8(isBE);
            if (isBE)
            {
                options.mFlags1 = new int[stream.ReadInt32BE()];
                for (int i = 0; i < options.mFlags1.Length; i++)
                    options.mFlags1[i] = stream.ReadInt32BE();
                options.mFlags2 = new int[stream.ReadInt32BE()];
                for (int i = 0; i < options.mFlags2.Length; i++)
                    options.mFlags2[i] = stream.ReadInt32BE();
            } else
            {
                options.mFlags1 = new int[stream.ReadInt32LE()];
                for (int i = 0; i < options.mFlags1.Length; i++)
                    options.mFlags1[i] = stream.ReadInt32LE();
                options.mFlags2 = new int[stream.ReadInt32LE()];
                for (int i = 0; i < options.mFlags2.Length; i++)
                    options.mFlags2[i] = stream.ReadInt32LE();
            }

            return options;
        }
    }
}
