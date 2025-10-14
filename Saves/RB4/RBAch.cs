namespace davesave.Saves.RB4
{
    public class RBAchSongListPersistentData
    {
#pragma warning disable CS8618
        public int[] mRB4FanSongs;
        public int[] mRB4MasterSongs;
        public int[] mRB4ImmortalSongs;
        public int[] mRB4GoldStarSongs;
        public int[] mRB4AuthoredSoloSongs;
        public int[] mAwesomeVocalsSongs;
#pragma warning restore CS8618

        public static RBAchSongListPersistentData ReadFromStream(Stream stream)
        {
            RBAchSongListPersistentData ach = new();
            RevisionStream rev = new RevisionStream(stream, 0x1, 0x1);

            ach.mRB4FanSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mRB4FanSongs.Length; i++)
                ach.mRB4FanSongs[i] = rev.ReadInt32LE();

            ach.mRB4MasterSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mRB4MasterSongs.Length; i++)
                ach.mRB4MasterSongs[i] = rev.ReadInt32LE();

            ach.mRB4ImmortalSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mRB4ImmortalSongs.Length; i++)
                ach.mRB4ImmortalSongs[i] = rev.ReadInt32LE();

            ach.mRB4GoldStarSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mRB4GoldStarSongs.Length; i++)
                ach.mRB4GoldStarSongs[i] = rev.ReadInt32LE();

            ach.mRB4AuthoredSoloSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mRB4AuthoredSoloSongs.Length; i++)
                ach.mRB4AuthoredSoloSongs[i] = rev.ReadInt32LE();

            ach.mAwesomeVocalsSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mAwesomeVocalsSongs.Length; i++)
                ach.mAwesomeVocalsSongs[i] = rev.ReadInt32LE();

            rev.FinishReading();
            return ach;
        }
    }
    public class RBAchCampaignPersistentData
    {
#pragma warning disable CS8618
        public string[] mAttemptedGigs;
        public string[] mBonusSetsPlayed;
        public int mCashEarned;
        public int mFansEarned;
#pragma warning restore CS8618

        public static RBAchCampaignPersistentData ReadFromStream(Stream stream)
        {
            RBAchCampaignPersistentData ach = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            ach.mAttemptedGigs = new string[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mAttemptedGigs.Length; i++)
                ach.mAttemptedGigs[i] = rev.ReadLengthUTF8();

            ach.mBonusSetsPlayed = new string[rev.ReadInt32LE()];
            for (int i = 0; i < ach.mBonusSetsPlayed.Length; i++)
                ach.mBonusSetsPlayed[i] = rev.ReadLengthUTF8();

            ach.mCashEarned = rev.ReadInt32LE();
            ach.mFansEarned = rev.ReadInt32LE();

            rev.FinishReading();
            return ach;
        }
    }

    public class RBAchPersistentData
    {
#pragma warning disable CS8618
        public RBAchSongListPersistentData mSongList;
        public RBAchCampaignPersistentData mCampaign;
        public int mRejectionStreak;

        public bool IsPlayStation;
        // TODO: figure out what these are
        public uint mUnknown1; // possibly a song ID?
        public uint mUnknown2;
        public uint mUnknown3;
#pragma warning restore CS8618

        public static RBAchPersistentData ReadFromStream(Stream stream, bool isPs = false)
        {
            RBAchPersistentData ach = new();
            ach.IsPlayStation = isPs;
            int version = isPs ? 0x6 : 0x3;
            RevisionStream rev = new RevisionStream(stream, version, version);

            ach.mSongList = RBAchSongListPersistentData.ReadFromStream(rev);
            ach.mCampaign = RBAchCampaignPersistentData.ReadFromStream(rev);
            if (ach.IsPlayStation)
            {
                ach.mUnknown1 = rev.ReadUInt32LE();
                ach.mUnknown2 = rev.ReadUInt32LE();
                ach.mUnknown3 = rev.ReadUInt32LE();
            }
            ach.mRejectionStreak = rev.ReadInt32LE();

            rev.FinishReading();
            return ach;
        }
    }
}
