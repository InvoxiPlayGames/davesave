namespace davesave.Saves.RB4
{
    public class RBCampaignGigProgressPersistentData
    {
#pragma warning disable CS8618
        public string mVenueLocation;
        public int mStarsEarned;
        public bool mHaveAttempted;
        public int mHighestScore;
        public int mDisplayStars;
#pragma warning restore CS8618

        public static RBCampaignGigProgressPersistentData ReadFromStream(Stream stream)
        {
            RBCampaignGigProgressPersistentData gigProgress = new();
            RevisionStream rev = new RevisionStream(stream, 0x2, 0x2);

            gigProgress.mVenueLocation = rev.ReadLengthUTF8();
            gigProgress.mStarsEarned = rev.ReadInt32LE();
            gigProgress.mHaveAttempted = rev.ReadUInt8() != 0x00;
            gigProgress.mHighestScore = rev.ReadInt32LE();
            gigProgress.mDisplayStars = rev.ReadInt32LE();

            rev.FinishReading();
            return gigProgress;
        }
    }

    public class RBCampaignCityProgressPersistentData
    {
#pragma warning disable CS8618
        public int mFansEarned;
        public int mMoneyEarned;
        public Dictionary<string, RBCampaignGigProgressPersistentData> mGigProgress;
#pragma warning restore CS8618

        public static RBCampaignCityProgressPersistentData ReadFromStream(Stream stream)
        {
            RBCampaignCityProgressPersistentData cityProgress = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            cityProgress.mFansEarned = rev.ReadInt32LE();
            cityProgress.mMoneyEarned = rev.ReadInt32LE();

            int len_mGigProgress = rev.ReadInt32LE();
            cityProgress.mGigProgress = new Dictionary<string, RBCampaignGigProgressPersistentData>(len_mGigProgress);
            for (int i = 0; i < len_mGigProgress; i++)
            {
                string gigName = rev.ReadLengthUTF8();
                RBCampaignGigProgressPersistentData gigProgress = RBCampaignGigProgressPersistentData.ReadFromStream(rev);
                cityProgress.mGigProgress[gigName] = gigProgress;
            }

            rev.FinishReading();
            return cityProgress;
        }
    }

    public class RBCampaignSessionMusician
    {
#pragma warning disable CS8618
        public int mSlot;
        public string mName;
        public bool mIsPrefab;
#pragma warning restore CS8618

        public static RBCampaignSessionMusician ReadFromStream(Stream stream)
        {
            RBCampaignSessionMusician sessionMusician = new();

            sessionMusician.mSlot = stream.ReadInt32LE();
            sessionMusician.mName = stream.ReadLengthUTF8();
            sessionMusician.mIsPrefab = stream.ReadUInt8() != 0x00;

            return sessionMusician;
        }
    }

    public class RBCampaignBandPersistentData
    {
#pragma warning disable CS8618
        public string mBandName;
        public string mHomeCity;
        public string mCurrentTour;
        public Dictionary<string, RBCampaignCityProgressPersistentData>? mCityProgress;
        public string[] mCompletedTours;
        public string mLastCompletedTourGig;
        public string mLastCityVisited;
        public int mStarsAtTourStart;
        public bool mNarrativeEndingReached;
        public int mCashSpent;
        public RBCampaignSessionMusician[] mSessionMusicians;
        public int mPendingFanGigUnlocks;
        public bool mRecentlyPlayed;
        public Dictionary<string, int> mTourFansEarned;
        public Dictionary<string, int> mTourCashEarned;
        public string mCityForNextTourChoice;
        public Dictionary<int, RBSongPersistentData> mDeprecatedSongData;
        public int[] mSongsPlayed;
        public HxGuid mBandGuid;
        public string[] mPatchesToRun;
        public RBBTMBandPersistentData mBTMData;
#pragma warning restore CS8618

        public static RBCampaignBandPersistentData ReadFromStream(Stream stream)
        {
            RBCampaignBandPersistentData band = new();
            RevisionStream rev = new RevisionStream(stream, 0x13, 0x13);

            band.mBandName = rev.ReadLengthUTF8();
            band.mHomeCity = rev.ReadLengthUTF8();
            band.mCurrentTour = rev.ReadLengthUTF8();

            int len_mCityProgress = rev.ReadInt32LE();
            band.mCityProgress = new Dictionary<string, RBCampaignCityProgressPersistentData>(len_mCityProgress);
            for (int i = 0; i < len_mCityProgress; i++)
            {
                string cityName = rev.ReadLengthUTF8();
                RBCampaignCityProgressPersistentData cityProgress = RBCampaignCityProgressPersistentData.ReadFromStream(rev);
                band.mCityProgress[cityName] = cityProgress;
            }

            int len_mCompletedTours = rev.ReadInt32LE();
            band.mCompletedTours = new string[len_mCompletedTours];
            for (int i = 0; i < len_mCompletedTours; i++)
            {
                band.mCompletedTours[i] = rev.ReadLengthUTF8();
            }

            band.mLastCompletedTourGig = rev.ReadLengthUTF8();
            band.mLastCityVisited = rev.ReadLengthUTF8();
            band.mStarsAtTourStart = rev.ReadInt32LE();
            band.mNarrativeEndingReached = rev.ReadUInt8() != 0x00;
            band.mCashSpent = rev.ReadInt32LE();

            int len_mSessionMusicians = rev.ReadInt32LE();
            band.mSessionMusicians = new RBCampaignSessionMusician[len_mSessionMusicians];
            for (int i = 0; i < len_mSessionMusicians; i++)
            {
                band.mSessionMusicians[i] = RBCampaignSessionMusician.ReadFromStream(rev);
            }

            band.mPendingFanGigUnlocks = rev.ReadInt32LE();
            band.mRecentlyPlayed = rev.ReadUInt8() != 0x00;
            
            int len_mTourFansEarned = rev.ReadInt32LE();
            band.mTourFansEarned = new Dictionary<string, int>(len_mTourFansEarned);
            for (int i = 0; i < len_mTourFansEarned; i++)
            {
                string tourName = rev.ReadLengthUTF8();
                int fansEarned = rev.ReadInt32LE();
                band.mTourFansEarned[tourName] = fansEarned;
            }

            int len_mTourCashEarned = rev.ReadInt32LE();
            band.mTourCashEarned = new Dictionary<string, int>(len_mTourCashEarned);
            for (int i = 0; i < len_mTourCashEarned; i++)
            {
                string tourName = rev.ReadLengthUTF8();
                int cashEarned = rev.ReadInt32LE();
                band.mTourCashEarned[tourName] = cashEarned;
            }

            band.mCityForNextTourChoice = rev.ReadLengthUTF8();

            int len_mDeprecatedSongData = rev.ReadInt32LE();
            band.mDeprecatedSongData = new Dictionary<int, RBSongPersistentData>(len_mDeprecatedSongData);
            for (int i = 0; i < len_mDeprecatedSongData; i++)
            {
                int songID = rev.ReadInt32LE();
                band.mDeprecatedSongData[songID] = RBSongPersistentData.ReadFromStream(rev);
            }

            int len_mSongsPlayed = rev.ReadInt32LE();
            band.mSongsPlayed = new int[len_mSongsPlayed];
            for (int i = 0; i < len_mSongsPlayed; i++)
            {
                band.mSongsPlayed[i] = rev.ReadInt32LE();
            }

            band.mBandGuid = HxGuid.ReadFromStream(rev);

            int len_mPatchesToRun = rev.ReadInt32LE();
            band.mPatchesToRun = new string[len_mPatchesToRun];
            for (int i = 0; i < len_mPatchesToRun; i++)
            {
                band.mPatchesToRun[i] = rev.ReadLengthUTF8();
            }

            band.mBTMData = RBBTMBandPersistentData.ReadFromStream(rev);

            rev.FinishReading();
            return band;
        }
    }
}
