namespace davesave.Saves.RB4
{
    public class RBBTMSongResult
    {
#pragma warning disable CS8618
        public string mSongShortName;
        public int mScore;
        public float mStars;
        public float mStarsFraction;
        public int mWholeStars;
        public int mPlayedDifficulty;
        public int mAwardedPlayerCallout;
        public float mDrumContribution;
        public float mBassContribution;
        public float mGuitarContribution;
        public float mVocalsContribution;
#pragma warning restore CS8618

        public static RBBTMSongResult ReadFromStream(Stream stream)
        {
            RBBTMSongResult songResult = new();
            RevisionStream rev = new RevisionStream(stream, 0x3, 0x3);

            songResult.mSongShortName = rev.ReadLengthUTF8();
            songResult.mScore = rev.ReadInt32LE();
            songResult.mStars = rev.ReadFloat();
            songResult.mStarsFraction = rev.ReadFloat();
            songResult.mWholeStars = rev.ReadInt32LE();
            songResult.mPlayedDifficulty = rev.ReadInt32LE();
            songResult.mAwardedPlayerCallout = rev.ReadInt32LE();
            songResult.mDrumContribution = rev.ReadFloat();
            songResult.mBassContribution = rev.ReadFloat();
            songResult.mGuitarContribution = rev.ReadFloat();
            songResult.mVocalsContribution = rev.ReadFloat();

            rev.FinishReading();
            return songResult;
        }
    }

    public class RBBTMGigResult
    {
#pragma warning disable CS8618
        public int mStarsWagered;
        public bool mWagerRequiresGoldStars;
        public int mFameEarned;
        public int mBaseFamePerStar;
        public int mFameEarnedFromChallengeBonus;
        public int mFameEarnedFromWager;
        public RBBTMSongResult[] mResults;
        public string mShowName;
        public int mShowNumOfSongs;
        public bool mOfferedRaisedStakesWager;
        public bool mAcceptedRaisedStakesWager;
        public int mRaisedStakesRawStarsNeeded;
        public int mRaisedStakesRawStarsEarned;
        public int mRaiseStakesFame;
#pragma warning restore CS8618

        public static RBBTMGigResult ReadFromStream(Stream stream)
        {
            RBBTMGigResult gigResult = new();
            RevisionStream rev = new RevisionStream(stream, 0x9, 0x9);

            gigResult.mStarsWagered = rev.ReadInt32LE();
            gigResult.mWagerRequiresGoldStars = rev.ReadUInt8() != 0x00;
            gigResult.mFameEarned = rev.ReadInt32LE();
            gigResult.mBaseFamePerStar = rev.ReadInt32LE();
            gigResult.mFameEarnedFromChallengeBonus = rev.ReadInt32LE();
            gigResult.mFameEarnedFromWager = rev.ReadInt32LE();

            int len_mResults = rev.ReadInt32LE();
            gigResult.mResults = new RBBTMSongResult[len_mResults];
            for (int i = 0; i < len_mResults; i++)
            {
                gigResult.mResults[i] = RBBTMSongResult.ReadFromStream(stream);
            }

            gigResult.mShowName = rev.ReadLengthUTF8();
            gigResult.mShowNumOfSongs = rev.ReadInt32LE();
            gigResult.mOfferedRaisedStakesWager = rev.ReadUInt8() != 0x00;
            gigResult.mAcceptedRaisedStakesWager = rev.ReadUInt8() != 0x00;
            gigResult.mRaisedStakesRawStarsNeeded = rev.ReadInt32LE();
            gigResult.mRaisedStakesRawStarsEarned = rev.ReadInt32LE();
            gigResult.mRaiseStakesFame = rev.ReadInt32LE();

            rev.FinishReading();
            return gigResult;
        }
    }

    public class RBBTMCampaignProgress
    {
#pragma warning disable CS8618
        public int mTotalFame;
        public string mCurrentChapter;
        public string mLastViewedChapter;
        public int mLastViewedShowIndex;
        public int mNumCompletedShowsSeenThisChapter;
        public string[] mShowsUsed;
        public string[] mNarrativesUsed;
        public uint mNextSongRandomVal;
        public uint mNextShowRaiseStakesRandomVal;
        public Dictionary<string, RBBTMGigResult[]> mChapterGigResults;
#pragma warning restore CS8618

        public static RBBTMCampaignProgress ReadFromStream(Stream stream)
        {
            RBBTMCampaignProgress campaignProgress = new();
            RevisionStream rev = new RevisionStream(stream, 0x5, 0x5);

            campaignProgress.mTotalFame = rev.ReadInt32LE();
            campaignProgress.mCurrentChapter = rev.ReadLengthUTF8();
            campaignProgress.mLastViewedChapter = rev.ReadLengthUTF8();
            campaignProgress.mLastViewedShowIndex = rev.ReadInt32LE();
            campaignProgress.mNumCompletedShowsSeenThisChapter = rev.ReadInt32LE();
            
            int len_mShowsUsed = rev.ReadInt32LE();
            campaignProgress.mShowsUsed = new string[len_mShowsUsed];
            for (int i = 0; i < len_mShowsUsed; i++)
            {
                campaignProgress.mShowsUsed[i] = rev.ReadLengthUTF8();
            }

            int len_mNarrativesUsed = rev.ReadInt32LE();
            campaignProgress.mNarrativesUsed = new string[len_mNarrativesUsed];
            for (int i = 0; i < len_mNarrativesUsed; i++)
            {
                campaignProgress.mNarrativesUsed[i] = rev.ReadLengthUTF8();
            }

            campaignProgress.mNextSongRandomVal = rev.ReadUInt32LE();
            campaignProgress.mNextShowRaiseStakesRandomVal = rev.ReadUInt32LE();

            int len_mChapterGigResults = rev.ReadInt32LE();
            campaignProgress.mChapterGigResults = new Dictionary<string, RBBTMGigResult[]>(len_mChapterGigResults);
            for (int i = 0; i < len_mChapterGigResults; i++)
            {
                string gigName = rev.ReadLengthUTF8();
                int numResults = rev.ReadInt32LE();
                campaignProgress.mChapterGigResults[gigName] = new RBBTMGigResult[numResults];
                for (int j = 0; j < numResults; j++)
                    campaignProgress.mChapterGigResults[gigName][j] = RBBTMGigResult.ReadFromStream(rev);
            }

            rev.FinishReading();
            return campaignProgress;
        }
    }

    public class RBBTMScore
    {
#pragma warning disable CS8618
        public int mFame;
        public string mBandName;
#pragma warning restore CS8618

        public static RBBTMScore ReadFromStream(Stream stream)
        {
            RBBTMScore score = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            score.mFame = stream.ReadInt32LE();
            score.mBandName = stream.ReadLengthUTF8();

            rev.FinishReading();
            return score;
        }
    }

    public class RBBTMBandPersistentData
    {
#pragma warning disable CS8618
        public RBBTMScore mBestScore;
        public RBBTMCampaignProgress mCurrentBTMProgress;
#pragma warning restore CS8618

        public static RBBTMBandPersistentData ReadFromStream(Stream stream)
        {
            RBBTMBandPersistentData btmBand = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            btmBand.mBestScore = RBBTMScore.ReadFromStream(rev);
            btmBand.mCurrentBTMProgress = RBBTMCampaignProgress.ReadFromStream(rev);

            rev.FinishReading();
            return btmBand;
        }
    }

    public class RBBTMPersistentData
    {
#pragma warning disable CS8618
        public int mBTMCampaignsStarted;
        public int mBTMCampaignsFinished;
        public RBBTMScore mBestScore;
#pragma warning restore CS8618

        public static RBBTMPersistentData ReadFromStream(Stream stream)
        {
            RBBTMPersistentData btm = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            btm.mBTMCampaignsStarted = rev.ReadInt32LE();
            btm.mBTMCampaignsFinished = rev.ReadInt32LE();
            btm.mBestScore = RBBTMScore.ReadFromStream(stream);

            rev.FinishReading();
            return btm;
        }
    }
}
