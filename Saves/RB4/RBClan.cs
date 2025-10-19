namespace davesave.Saves.RB4
{
    public class SpotlightSongHighScoreInstrumentInfo
    {
#pragma warning disable CS8618
        public bool mHasInfo;
        public int mInstrument;
        public int mHighScore;
        public int mStars;
        public int mDifficulty;
        public bool mBrutal;
        public string mPlatformUID;
        public string mGamertag;
        public string mUnused;
        public string mOnlineID;
        public int mSpotlightPercent;
        public bool mIsMine;
#pragma warning restore CS8618

        public static SpotlightSongHighScoreInstrumentInfo ReadFromStream(Stream stream)
        {
            SpotlightSongHighScoreInstrumentInfo info = new();

            info.mHasInfo = stream.ReadUInt8() != 0x00;
            info.mInstrument = stream.ReadInt32LE();
            info.mHighScore = stream.ReadInt32LE();
            info.mStars = stream.ReadInt32LE();
            info.mDifficulty = stream.ReadInt32LE();
            info.mBrutal = stream.ReadUInt8() != 0x00;
            info.mPlatformUID = stream.ReadLengthUTF8();
            info.mGamertag = stream.ReadLengthUTF8();
            info.mUnused = stream.ReadLengthUTF8();
            info.mOnlineID = stream.ReadLengthUTF8();
            info.mSpotlightPercent = stream.ReadInt32LE();
            info.mIsMine = stream.ReadUInt8() != 0x00;

            return info;
        }
    }

    public class RBClanPersistentData
    {
#pragma warning disable CS8618
        public uint mUnknown1;
        public uint mUnknown2;
        public uint mUnknown3;
        public int mTier;
        public int mHighestTier;
        public uint mCachedSpotlightSongEventID;
        public Dictionary<int, SpotlightSongHighScoreInstrumentInfo[]> mSpotlightSongHighScores;
        public uint mUnknown4;
        public int mSpotlightPercent;
        public int mCrewXPPercent;
        public int mLPPercent;
        public int mTotalPoints;
#pragma warning restore CS8618

        public static RBClanPersistentData ReadFromStream(Stream stream)
        {
            RBClanPersistentData clan = new();
            RevisionStream rev = new RevisionStream(stream, 0x6, 0x6);

            clan.mUnknown1 = rev.ReadUInt32LE();
            clan.mUnknown2 = rev.ReadUInt32LE();
            clan.mUnknown3 = rev.ReadUInt32LE();
            clan.mTier = rev.ReadInt32LE();
            clan.mHighestTier = rev.ReadInt32LE();
            clan.mCachedSpotlightSongEventID = rev.ReadUInt32LE();

            int mSpotlightSongHighScores_length = rev.ReadInt32LE();
            clan.mSpotlightSongHighScores = new Dictionary<int, SpotlightSongHighScoreInstrumentInfo[]>(mSpotlightSongHighScores_length);
            for (int i = 0; i < mSpotlightSongHighScores_length; i++)
            {
                int songID = rev.ReadInt32LE();
                int mHighScores_length = rev.ReadInt32LE();
                clan.mSpotlightSongHighScores[songID] = new SpotlightSongHighScoreInstrumentInfo[mHighScores_length];
                for (int j = 0; j < mHighScores_length; j++)
                    clan.mSpotlightSongHighScores[songID][j] = SpotlightSongHighScoreInstrumentInfo.ReadFromStream(rev);
            }

            clan.mUnknown4 = rev.ReadUInt32LE();
            clan.mSpotlightPercent = rev.ReadInt32LE();
            clan.mCrewXPPercent = rev.ReadInt32LE();
            clan.mLPPercent = rev.ReadInt32LE();
            clan.mTotalPoints = rev.ReadInt32LE();

            rev.FinishReading();
            return clan;
        }
    }
}
