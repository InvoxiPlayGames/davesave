namespace davesave.Saves
{
    public class HxGuid
    {
#pragma warning disable CS8618
        public int mVersion;
        public int[] mData;
#pragma warning restore CS8618

        public static HxGuid ReadFromStream(Stream stream)
        {
            HxGuid guid = new();
            guid.mVersion = stream.ReadInt32LE();
            if (guid.mVersion != 1)
                throw new Exception("Unexpected version for HxGuid");
            guid.mData = new int[4];
            for (int i = 0; i < 4; i++)
                guid.mData[i] = stream.ReadInt32LE();
            return guid;
        }
    }

    public class HxDateTime
    {
#pragma warning disable CS8618
        public byte mSec;
        public byte mMin;
        public byte mHour;
        public byte mData;
        public byte mMonth;
        public byte mYear;
#pragma warning restore CS8618

        public static HxDateTime ReadFromStream(Stream stream)
        {
            HxDateTime dt = new();
            dt.mSec = stream.ReadUInt8();
            dt.mMin = stream.ReadUInt8();
            dt.mHour = stream.ReadUInt8();
            dt.mData = stream.ReadUInt8();
            dt.mMonth = stream.ReadUInt8();
            dt.mYear = stream.ReadUInt8();
            return dt;
        }
    }
}
