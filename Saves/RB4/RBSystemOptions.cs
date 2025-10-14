namespace davesave.Saves.RB4
{
    public class SystemOptions
    {
#pragma warning disable CS8618
        public float mOverscan;
        public float mAudioOffset;
        public float mVideoOffset;
        public float mDialogVolume;
        public float mGammaValue;
        public bool mHasCalibrated;
#pragma warning restore CS8618

        public static SystemOptions ReadFromStream(Stream stream)
        {
            SystemOptions opt = new();
            RevisionStream rev = new RevisionStream(stream, 0x5, 0x5);

            opt.mOverscan = rev.ReadFloat();
            opt.mAudioOffset = rev.ReadFloat();
            opt.mVideoOffset = rev.ReadFloat();
            opt.mDialogVolume = rev.ReadFloat();
            opt.mGammaValue = rev.ReadFloat();
            opt.mHasCalibrated = rev.ReadUInt8() != 0x00;

            rev.FinishReading();
            return opt;
        }
    }

    public class RBSystemOptions
    {
#pragma warning disable CS8618
        public SystemOptions mSystemOptions;
        public int mBackgroundVolume;
        public int mInstrumentsVolume;
        public int mSFXVolume;
        public int mCrowdVolume;
        public bool mDolby;
        public bool mBassBoost;
        public bool mOverscan;
        public int mProDrumCymbalLanes;
        public bool mIsDrumNavigationAllowed;
        public bool mNoFail;
        public bool mIndependentTrackSpeeds;
        public bool mImprovSolosScored;
        public bool mAwesomenessDetection;
        public bool mLeaderboardLadder;
        public bool mHide1RatedSongs;
        public bool mUseSubtitles;
        public int mSongLibSort;
        public int mLastPlayedPatchVersion;
        public bool mSeenRivalsUpsell;
        public bool mDrumAutoKickEnabled;
        // TODO: figure out what these two are
        public string mUnknown1;
        public string mUnknown2;
        public bool mHideUnavailableSOMPSongs;
        // TODO: figure out what these are
        public bool mUnknown3;
        public uint mUnknown4;
        public uint mUnknown5;
        public bool mUnknown6;
        public uint mUnknown7;
        public uint mUnknown8;
        public RBSongCache mSongCache;
#pragma warning restore CS8618

        public static RBSystemOptions ReadFromStream(Stream stream)
        {
            RBSystemOptions opt = new RBSystemOptions();
            RevisionStream rev = new RevisionStream(stream, 0x1C, 0x1C);

            opt.mSystemOptions = SystemOptions.ReadFromStream(rev);
            opt.mBackgroundVolume = rev.ReadInt32LE();
            opt.mInstrumentsVolume = rev.ReadInt32LE();
            opt.mSFXVolume = rev.ReadInt32LE();
            opt.mCrowdVolume = rev.ReadInt32LE();
            opt.mDolby = rev.ReadUInt8() != 0x00;
            opt.mBassBoost = rev.ReadUInt8() != 0x00;
            opt.mOverscan = rev.ReadUInt8() != 0x00;
            opt.mProDrumCymbalLanes = rev.ReadInt32LE();
            opt.mIsDrumNavigationAllowed = rev.ReadUInt8() != 0x00;
            opt.mNoFail = rev.ReadUInt8() != 0x00;
            opt.mIndependentTrackSpeeds = rev.ReadUInt8() != 0x00;
            opt.mImprovSolosScored = rev.ReadUInt8() != 0x00;
            opt.mAwesomenessDetection = rev.ReadUInt8() != 0x00;
            opt.mLeaderboardLadder = rev.ReadUInt8() != 0x00;
            opt.mHide1RatedSongs = rev.ReadUInt8() != 0x00;
            opt.mUseSubtitles = rev.ReadUInt8() != 0x00;
            opt.mSongLibSort = rev.ReadInt32LE();
            opt.mLastPlayedPatchVersion = rev.ReadInt32LE();
            opt.mSeenRivalsUpsell = rev.ReadUInt8() != 0x00;
            opt.mDrumAutoKickEnabled = rev.ReadUInt8() != 0x00;
            opt.mUnknown1 = rev.ReadLengthUTF8();
            opt.mUnknown2 = rev.ReadLengthUTF8();
            opt.mHideUnavailableSOMPSongs = rev.ReadUInt8() != 0x00;
            opt.mUnknown3 = rev.ReadUInt8() != 0x00;
            opt.mUnknown4 = rev.ReadUInt32LE();
            opt.mUnknown5 = rev.ReadUInt32LE();
            opt.mUnknown6 = rev.ReadUInt8() != 0x00;
            opt.mUnknown7 = rev.ReadUInt32LE();
            opt.mUnknown8 = rev.ReadUInt32LE();
            opt.mSongCache = RBSongCache.ReadFromStream(rev);

            rev.FinishReading();
            return opt;
        }
    }

    public class RBSongMetadata
    {
        // TODO: We can fill this in later...
        //       It's a fixed size! So we don't have to care.
#pragma warning disable CS8618
        public byte[] mData;
#pragma warning restore CS8618

        // Notes:
        //   Data format is almost identical to .songdta_* files
        //   However missing the "type" signifier (starts with mId instead)
        //   and at offset 0x4B0 there's two values, one of which is
        //   the number of refreshes it's been since the song was last seen
        //   and the other is the most recent changelist version.

        public static RBSongMetadata ReadFromStream(Stream stream)
        {
            RBSongMetadata meta = new();

            meta.mData = new byte[0x4B8];
            stream.Read(meta.mData, 0, 0x4B8);

            return meta;
        }
    }

    public class RBSongCache
    {
#pragma warning disable CS8618
        public int mCacheVersion;
        public Dictionary<string, int[]> mSongsInContent;
        public Dictionary<string, string> mShortNametoSongDir;
        public Dictionary<int, RBSongMetadata> mMetadataMap;
        public int[] mRecentlyAcquiredSongs;
#pragma warning restore CS8618

        public static RBSongCache ReadFromStream(Stream stream)
        {
            RBSongCache cache = new();
            RevisionStream rev = new RevisionStream(stream, 0x8, 0x8);

            cache.mCacheVersion = rev.ReadInt32LE();
            if (cache.mCacheVersion != 0xC)
                throw new Exception($"Unexpected song cache version {cache.mCacheVersion}");

            int len_mSongsInContent = rev.ReadInt32LE();
            cache.mSongsInContent = new Dictionary<string, int[]>(len_mSongsInContent);
            for (int i = 0; i < len_mSongsInContent; i++)
            {
                string contentName = rev.ReadLengthUTF8();
                int numSongs = rev.ReadInt32LE();
                cache.mSongsInContent[contentName] = new int[numSongs];
                for (int j = 0; j < numSongs; j++)
                    cache.mSongsInContent[contentName][j] = rev.ReadInt32LE();
            }

            int len_mShortNametoSongDir = rev.ReadInt32LE();
            cache.mShortNametoSongDir = new Dictionary<string, string>(len_mShortNametoSongDir);
            for (int i = 0; i < len_mShortNametoSongDir; i++)
            {
                string shortName = rev.ReadLengthUTF8();
                string songDir = rev.ReadLengthUTF8();
                cache.mShortNametoSongDir[shortName] = songDir;
            }

            int songMetadataSize = rev.ReadInt32LE();
            if (songMetadataSize != 0x4B8)
                throw new Exception("Unexpected song metadata size");

            int len_mMetadataMap = rev.ReadInt32LE();
            cache.mMetadataMap = new Dictionary<int, RBSongMetadata>(len_mMetadataMap);
            for (int i = 0; i < len_mMetadataMap; i++)
            {
                int songID = rev.ReadInt32LE();
                cache.mMetadataMap[songID] = RBSongMetadata.ReadFromStream(rev);
            }

            cache.mRecentlyAcquiredSongs = new int[rev.ReadInt32LE()];
            for (int i = 0; i < cache.mRecentlyAcquiredSongs.Length; i++)
            {
                cache.mRecentlyAcquiredSongs[i] = rev.ReadInt32LE();
            }

            rev.FinishReading();
            return cache;
        }
    }
}
