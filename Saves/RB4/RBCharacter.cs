using DtxCS.DataTypes;

namespace davesave.Saves.RB4
{
    public class RBCharacterPersistentData
    {
#pragma warning disable CS8618
        public int mNumCharacters;
        public string mSavedCharName;
        public bool mSavedCharIsPrefab;
        public DataArray[] mCharacters;
#pragma warning restore CS8618

        public static RBCharacterPersistentData ReadFromStream(Stream stream)
        {
            RBCharacterPersistentData charData = new();
            RevisionStream rev = new RevisionStream(stream, 0x4, 0x4);

            charData.mNumCharacters = rev.ReadInt32LE();
            charData.mSavedCharName = rev.ReadLengthUTF8();
            charData.mSavedCharIsPrefab = rev.ReadUInt8() != 0x00;
            charData.mCharacters = new DataArray[charData.mNumCharacters];
            for (int i = 0; i < charData.mNumCharacters; i++)
            {
                charData.mCharacters[i] = DtxCS.DTX.FromDtb(rev);
            }

            rev.FinishReading();
            return charData;
        }
    }
}
