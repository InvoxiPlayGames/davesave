namespace davesave.Saves.RB4
{
    public class RBProfile
    {
        public RBSystemOptions? mSystemOptions;

#pragma warning disable CS8618
        public RBCampaignBandPersistentData[] mCampaignBands;
        public Dictionary<int, RBQuickplaySongPersistentData> mQuickplaySongData;
        public Dictionary<int, RBQuickplaySongPersistentData> mBrutalSongData;
        public Dictionary<int, byte> mSongRatings;
        public RBGameplayPersistentData mGameplayOptions;
        public RBUnknownPersistentData mUnknownData;
        public RBCharacterPersistentData mCharacters;
        public RBRockShopPersistentData mRockShopData;
        public RBFTUEPersistentData mFTUEData;
        public RBAchPersistentData mAchPersistentData;
        public int mTotalSongPlays;
        public RBSetlistCollection mSetlistData;
        public RBBTMPersistentData mBTMData;
        public RBVenuePersistentData mUnlockedVenueData;
        public RBClanPersistentData mClanPersistentData;
        public RBTrackSkinPersistentData mTrackTextureData;
        public int[] mEntitlements;
#pragma warning restore CS8618

        public static RBProfile ReadFromStream(Stream stream, bool isPlayStation = false)
        {
            RBProfile profile = new();
            RevisionStream rev = new RevisionStream(stream, 0x19, 0x19, false);

            if (isPlayStation)
            {
                profile.mSystemOptions = RBSystemOptions.ReadFromStream(rev);
            }

            int len_mCampaignBands = rev.ReadInt32LE();
            profile.mCampaignBands = new RBCampaignBandPersistentData[len_mCampaignBands];
            for (int i = 0; i < len_mCampaignBands; i++)
                profile.mCampaignBands[i] = RBCampaignBandPersistentData.ReadFromStream(rev);

            int len_mQuickplaySongData = rev.ReadInt32LE();
            profile.mQuickplaySongData = new Dictionary<int, RBQuickplaySongPersistentData>();
            for (int i = 0; i < len_mQuickplaySongData; i++)
            {
                int songId = rev.ReadInt32LE();
                profile.mQuickplaySongData[songId] = RBQuickplaySongPersistentData.ReadFromStream(rev);
            }

            int len_mBrutalSongData = rev.ReadInt32LE();
            profile.mBrutalSongData = new Dictionary<int, RBQuickplaySongPersistentData>();
            for (int i = 0; i < len_mBrutalSongData; i++)
            {
                int songId = rev.ReadInt32LE();
                profile.mBrutalSongData[songId] = RBQuickplaySongPersistentData.ReadFromStream(rev);
            }

            int len_mSongRatings = rev.ReadInt32LE();
            profile.mSongRatings = new Dictionary<int, byte>(len_mSongRatings);
            for (int i = 0; i < len_mSongRatings; i++)
            {
                int songID = rev.ReadInt32LE();
                byte rating = rev.ReadUInt8();
                profile.mSongRatings[songID] = rating;
            }

            profile.mGameplayOptions = RBGameplayPersistentData.ReadFromStream(rev);
            profile.mUnknownData = RBUnknownPersistentData.ReadFromStream(rev);
            profile.mCharacters = RBCharacterPersistentData.ReadFromStream(rev);
            profile.mRockShopData = RBRockShopPersistentData.ReadFromStream(rev);
            profile.mFTUEData = RBFTUEPersistentData.ReadFromStream(rev);
            profile.mAchPersistentData = RBAchPersistentData.ReadFromStream(rev, isPlayStation);
            profile.mTotalSongPlays = rev.ReadInt32LE();
            profile.mSetlistData = RBSetlistCollection.ReadFromStream(rev);
            profile.mBTMData = RBBTMPersistentData.ReadFromStream(rev);
            profile.mUnlockedVenueData = RBVenuePersistentData.ReadFromStream(rev);
            profile.mClanPersistentData = RBClanPersistentData.ReadFromStream(rev);
            profile.mTrackTextureData = RBTrackSkinPersistentData.ReadFromStream(rev);
            profile.mEntitlements = new int[rev.ReadInt32LE()];
            for (int i = 0; i < profile.mEntitlements.Length; i++)
                profile.mEntitlements[i] = rev.ReadInt32LE();

            rev.FinishReading();
            return profile;
        }
    }
}
