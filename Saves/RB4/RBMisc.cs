namespace davesave.Saves.RB4
{
    public class RBGameplayPersistentData
    {
        public bool mIsGuitarLefty;
        public bool mIsDrumLefty;
        public int mGuitarSoloMode;
        public int mDrumFillLogic;
        public bool mVocalImprovEnabled;
        public int mBreakneckSpeedModifierIndex;
        public int mMicVolumeIndex;
        public int mVocalStemVolumeIndex;
        public int mIGSVolumeIndex;
        public bool mMicODDeployEnabled;

        public static RBGameplayPersistentData ReadFromStream(Stream stream)
        {
            RBGameplayPersistentData gameplay = new();
            RevisionStream rev = new RevisionStream(stream, 0xB, 0xB);

            gameplay.mIsGuitarLefty = rev.ReadUInt8() != 0x00;
            gameplay.mIsDrumLefty = rev.ReadUInt8() != 0x00;
            gameplay.mGuitarSoloMode = rev.ReadInt32LE();
            gameplay.mDrumFillLogic = rev.ReadInt32LE();
            gameplay.mVocalImprovEnabled = rev.ReadUInt8() != 0x00;
            gameplay.mBreakneckSpeedModifierIndex = rev.ReadInt32LE();
            gameplay.mMicVolumeIndex = rev.ReadInt32LE();
            gameplay.mVocalStemVolumeIndex = rev.ReadInt32LE();
            gameplay.mIGSVolumeIndex = rev.ReadInt32LE();
            gameplay.mMicODDeployEnabled = rev.ReadUInt8() != 0x00;

            rev.FinishReading();
            return gameplay;
        }
    }

    public class RBUnknownPersistentData
    {
#pragma warning disable CS8618
        public int[] mData;
#pragma warning restore CS8618

        public static RBUnknownPersistentData ReadFromStream(Stream stream)
        {
            RBUnknownPersistentData unk = new();
            RevisionStream rev = new RevisionStream(stream, 0x1, 0x1);

            unk.mData = new int[10];
            for (int i = 0; i < 10; i++)
                unk.mData[i] = rev.ReadInt32LE();

            rev.FinishReading();
            return unk;
        }
    }

    public class RBRockShopPersistentData
    {
#pragma warning disable CS8618
        public string[] mUnlockedPieces;
        public string[] mOwnedPieces;
        public string[] mNotViewedPieces;
#pragma warning restore CS8618

        public static RBRockShopPersistentData ReadFromStream(Stream stream)
        {
            RBRockShopPersistentData rockShop = new();
            RevisionStream rev = new RevisionStream(stream, 0x3, 0x3);

            rockShop.mUnlockedPieces = new string[rev.ReadInt32LE()];
            for (int i = 0; i < rockShop.mUnlockedPieces.Length; i++)
                rockShop.mUnlockedPieces[i] = rev.ReadLengthUTF8();

            rockShop.mOwnedPieces = new string[rev.ReadInt32LE()];
            for (int i = 0; i < rockShop.mOwnedPieces.Length; i++)
                rockShop.mOwnedPieces[i] = rev.ReadLengthUTF8();

            rockShop.mNotViewedPieces = new string[rev.ReadInt32LE()];
            for (int i = 0; i < rockShop.mNotViewedPieces.Length; i++)
                rockShop.mNotViewedPieces[i] = rev.ReadLengthUTF8();

            rev.FinishReading();
            return rockShop;
        }
    }

    public class RBFTUEPersistentData
    {
#pragma warning disable CS8618
        public string[] mEventsSeen;
#pragma warning restore CS8618

        public static RBFTUEPersistentData ReadFromStream(Stream stream)
        {
            RBFTUEPersistentData ftue = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            ftue.mEventsSeen = new string[rev.ReadInt32LE()];
            for (int i = 0; i < ftue.mEventsSeen.Length; i++)
                ftue.mEventsSeen[i] = rev.ReadLengthUTF8();

            rev.FinishReading();
            return ftue;
        }
    }

    public class RBVenuePersistentData
    {
#pragma warning disable CS8618
        public string[] mUnlockedVenues;
#pragma warning restore CS8618

        public static RBVenuePersistentData ReadFromStream(Stream stream)
        {
            RBVenuePersistentData venue = new();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            venue.mUnlockedVenues = new string[rev.ReadInt32LE()];
            for (int i = 0; i < venue.mUnlockedVenues.Length; i++)
                venue.mUnlockedVenues[i] = rev.ReadLengthUTF8();

            rev.FinishReading();
            return venue;
        }
    }

    public class RBTrackSkinPersistentData
    {
#pragma warning disable CS8618
        public string mSelectedSkin;
        public int mSelectedColour;
        public bool mShowSkin;
        public byte mUnknown;
        public bool mRandomizeSkin;
        public bool mRandomizeColour;
#pragma warning restore CS8618

        public static RBTrackSkinPersistentData ReadFromStream(Stream stream)
        {
            RBTrackSkinPersistentData ts = new();
            RevisionStream rev = new RevisionStream(stream, 0x1, 0x1);

            ts.mSelectedSkin = rev.ReadLengthUTF8();
            ts.mSelectedColour = rev.ReadInt32LE();
            ts.mShowSkin = rev.ReadUInt8() != 0x00;
            ts.mUnknown = rev.ReadUInt8();
            ts.mRandomizeSkin = rev.ReadUInt8() != 0x00;
            ts.mRandomizeColour = rev.ReadUInt8() != 0x00;

            rev.FinishReading();
            return ts;
        }
    }

    public class RBClanPersistentData
    {
#pragma warning disable CS8618
        // TODO: figure out what this actually is
        // !! there is a chance that this could be a non-fixed size !!
        public uint[] mUnknown1;
        public int mTier;
        public int mHighestTier;
        public uint[] mUnknown2;
#pragma warning restore CS8618

        public static RBClanPersistentData ReadFromStream(Stream stream)
        {
            RBClanPersistentData clan = new();
            RevisionStream rev = new RevisionStream(stream, 0x6, 0x6);

            clan.mUnknown1 = new uint[3];
            for (int i = 0; i < clan.mUnknown1.Length; i++)
                clan.mUnknown1[i] = rev.ReadUInt32LE();

            clan.mTier = rev.ReadInt32LE();
            clan.mHighestTier = rev.ReadInt32LE();

            clan.mUnknown2 = new uint[7];
            for (int i = 0; i < clan.mUnknown2.Length; i++)
                clan.mUnknown2[i] = rev.ReadUInt32LE();

            rev.FinishReading();
            return clan;
        }
    }
}
