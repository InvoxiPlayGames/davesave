namespace davesave.Saves.RB4
{
    public class RBSongPersistentData
    {
#pragma warning disable CS8618
        public int mSongID;
        public int mVersion;
        public HxDateTime mLastPlayed;
        public uint[,] mScore;
        public byte[,] mStars;
        public byte[,] mAccuracy;
        public byte[,] mFullCombo;
#pragma warning restore CS8618

        public static RBSongPersistentData ReadFromStream(Stream stream)
        {
            RBSongPersistentData songData = new();
            RevisionStream rev = new RevisionStream(stream, 0x8, 0x8);

            songData.mSongID = rev.ReadInt32LE();
            songData.mVersion = rev.ReadInt32LE();
            songData.mLastPlayed = HxDateTime.ReadFromStream(rev);

            songData.mScore = new uint[7,4];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                    songData.mScore[i, j] = rev.ReadUInt32LE();
            }

            songData.mStars = new byte[7,4];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                    songData.mStars[i,j] = rev.ReadUInt8();
            }

            songData.mAccuracy = new byte[7,4];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                    songData.mAccuracy[i,j] = rev.ReadUInt8();
            }

            songData.mFullCombo = new byte[7,4];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                    songData.mFullCombo[i,j] = rev.ReadUInt8();
            }

            rev.FinishReading();
            return songData;
        }
    }

    public class RBSongMidiChecksum
    {
#pragma warning disable CS8618
        public byte[] digest;
        public byte[] leaderboardRevs;
        public uint[] scoreChecksums;
#pragma warning restore CS8618

        public static RBSongMidiChecksum ReadFromStream(Stream stream)
        {
            RBSongMidiChecksum midiChecksum = new();
            RevisionStream rev = new RevisionStream(stream, 0x3, 0x3);

            midiChecksum.digest = new byte[0x14];
            rev.Read(midiChecksum.digest, 0, 0x14);

            midiChecksum.leaderboardRevs = new byte[7];
            for (int i = 0; i < 7; i++)
                midiChecksum.leaderboardRevs[i] = rev.ReadUInt8();

            midiChecksum.scoreChecksums = new uint[7];
            for (int i = 0; i < 7; i++)
                midiChecksum.scoreChecksums[i] = rev.ReadUInt32LE();

            rev.FinishReading();
            return midiChecksum;
        }
    }

    public class RBQuickplaySongPersistentData
    {
#pragma warning disable CS8618
        public RBSongPersistentData mSongPersistentData;
        public RBSongMidiChecksum mChecksum;
#pragma warning restore CS8618

        public static RBQuickplaySongPersistentData ReadFromStream(Stream stream)
        {
            RBQuickplaySongPersistentData songData = new();

            songData.mSongPersistentData = RBSongPersistentData.ReadFromStream(stream);
            songData.mChecksum = RBSongMidiChecksum.ReadFromStream(stream);

            return songData;
        }
    }
}
