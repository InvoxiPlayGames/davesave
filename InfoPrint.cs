using davesave.Saves;
using davesave.Saves.Amp2016;
using davesave.Saves.RB4;
using LibForge.Util;

namespace davesave
{
    public class InfoPrint
    {
        public static Dictionary<SaveDetection.SaveType, string> SaveDescriptions = new()
        {
            { SaveDetection.SaveType.NotASave, "Not a valid save file." },
            { SaveDetection.SaveType.Unsupported, "Unsupported game." },
            { SaveDetection.SaveType.Failed, "Failed to identify save file." },

            { SaveDetection.SaveType.PS4SaveContainer, "PS4 Save Container (please decrypt before use)" },
            { SaveDetection.SaveType.PS4SealedKey, "PS4 Sealed Key (please decrypt before use)" },

            { SaveDetection.SaveType.AmpPS3, "Amplitude 2016 (PS3)" },
            { SaveDetection.SaveType.AmpPS4, "Amplitude 2016 (PS4)" },
            { SaveDetection.SaveType.AmpPS3_Decrypted, "Amplitude 2016 (PS3, Decrypted)" },
            { SaveDetection.SaveType.AmpPS4_Decrypted, "Amplitude 2016 (PS4, Decrypted)" },

            { SaveDetection.SaveType.RB4PS4, "Rock Band 4 Save Data (PS4)" },
            { SaveDetection.SaveType.RB4PS4_Decrypted, "Rock Band 4 Save Data (PS4, Decrypted)" },
            { SaveDetection.SaveType.RB4Xbox, "Rock Band 4 Save Data (Xbox)" },
            { SaveDetection.SaveType.RB4Xbox_Decrypted, "Rock Band 4 Save Data (Xbox, Decrypted)" },
            { SaveDetection.SaveType.RB4XboxSystemOptions, "Rock Band 4 System Options (Xbox)" },
            { SaveDetection.SaveType.RB4XboxSystemOptions_Decrypted, "Rock Band 4 System Options (Xbox, Decrypted)" },
        };

        public static void RB4SystemInfo(RBSystemOptions system)
        {
            Console.WriteLine("Audio Offset: {0}ms", system.mSystemOptions.mAudioOffset);
            Console.WriteLine("Video Offset: {0}ms", system.mSystemOptions.mVideoOffset);
            Console.WriteLine("Awesomeness Detection: {0}", system.mAwesomenessDetection);
            Console.WriteLine("Song Cache:");
            Console.WriteLine("  Content Packages: {0}", system.mSongCache.mSongsInContent.Count);
            Console.WriteLine("  Discovered Songs: {0}", system.mSongCache.mMetadataMap.Count);
        }

        public static void RB4SaveInfo(RBProfile profile)
        {
            if (profile.mSystemOptions != null)
                RB4SystemInfo(profile.mSystemOptions);

            Console.WriteLine("Bands: {0}", profile.mCampaignBands.Length);
            for (int i = 0; i < profile.mCampaignBands.Length; i++)
            {
                Console.WriteLine(" - {0}", profile.mCampaignBands[i].mBandName);
            }
            Console.WriteLine("Quickplay Scores: {0}", profile.mQuickplaySongData.Count);
            Console.WriteLine("Brutal Scores: {0}", profile.mBrutalSongData.Count);
            Console.WriteLine("Song Ratings: {0}", profile.mSongRatings.Count);
            Console.WriteLine("Lefty Flip: {0}", profile.mGameplayOptions.mIsGuitarLefty);
            Console.WriteLine("Characters: {0}", profile.mCharacters.mNumCharacters);
            for (int i = 0; i < profile.mCharacters.mNumCharacters; i++)
            {
                Console.WriteLine(" - {0}", profile.mCharacters.mCharacters[i].Node(1).ToString(0));
            }
            Console.WriteLine("Rock Shop Unlocked Items: {0}", profile.mRockShopData.mUnlockedPieces.Length);
            Console.WriteLine("Rock Shop Owned Items: {0}", profile.mRockShopData.mOwnedPieces.Length);
            Console.WriteLine("First-Time Prompts Seen: {0}", profile.mFTUEData.mEventsSeen.Length);
            Console.WriteLine("5-Starred On-Disc Expert Songs: {0}", profile.mAchPersistentData.mSongList.mRB4ImmortalSongs.Length);
            Console.WriteLine("Total Song Plays: {0}", profile.mTotalSongPlays);
            Console.WriteLine("Setlists: {0}", profile.mSetlistData.mSetlists.Length);
            for (int i = 0; i < profile.mSetlistData.mSetlists.Length; i++)
            {
                Console.WriteLine(" - {0} ({1} songs)", profile.mSetlistData.mSetlists[i].mName, profile.mSetlistData.mSetlists[i].mSongList.Length);
            }
            Console.WriteLine("Best Rockudrama Score: {0} ({1})", profile.mBTMData.mBestScore.mFame, profile.mBTMData.mBestScore.mBandName);
            Console.WriteLine("Unlocked Venues: {0}", profile.mUnlockedVenueData.mUnlockedVenues.Length);
            Console.WriteLine("Highest Rivals Tier: {0}", profile.mClanPersistentData.mHighestTier);
            Console.WriteLine("Track Skin: {0}", profile.mTrackTextureData.mSelectedSkin);
            Console.WriteLine("Server-Granted Entitlements: {0}", profile.mEntitlements.Length);
        }

        public static void AmpSaveInfo(AmpProfile profile)
        {
            Console.WriteLine("Save Version: {0}", profile.mVersion);
            Console.WriteLine("Saved Scores: {0}", profile.mSongData.Length);
            Console.WriteLine("Control Scheme: {0}", profile.mOptions.mControlScheme);
            Console.WriteLine("Unlocks: {0}", profile.mUnlocks.Length);
            Console.WriteLine("Played Campaign Songs: {0}", profile.mCampaignData.mSongIndex);
            Console.WriteLine("Next Campaign Song: {0}", profile.mCampaignData.mNextSong);
        }

        public static async Task SaveInfo(string inputFile)
        {
            Stream inFile = File.OpenRead(inputFile);
            EncryptedReadRevisionStream? encStr = null;

            // detect the game
            SaveDetection.SaveType type = await SaveDetection.DetectSaveTypeAsync(inFile);
            Console.WriteLine("Save: {0}", SaveDescriptions[type]);

            // we can't do anything here
            if (type == SaveDetection.SaveType.NotASave || type == SaveDetection.SaveType.Unsupported
                || type == SaveDetection.SaveType.Failed)
            {
                return;
            }

            // handle decryption if necessary
            if (type == SaveDetection.SaveType.AmpPS4 || type == SaveDetection.SaveType.RB4Xbox ||
                type == SaveDetection.SaveType.RB4PS4 || type == SaveDetection.SaveType.RB4XboxSystemOptions)
            {
                // big endian save file
                encStr = new EncryptedReadRevisionStream(inFile, false);
            }
            else if (type == SaveDetection.SaveType.AmpPS3)
            {
                encStr = new EncryptedReadRevisionStream(inFile, true);
            }

            // handle save parsing
            if (type == SaveDetection.SaveType.AmpPS4 || type == SaveDetection.SaveType.AmpPS4_Decrypted)
            {
                AmpProfile profile = AmpProfile.ReadFromStream(encStr == null ? inFile : encStr, false);
                AmpSaveInfo(profile);
                if (encStr != null)
                    encStr.FinishReading();
            }
            else if (type == SaveDetection.SaveType.AmpPS3 || type == SaveDetection.SaveType.AmpPS3_Decrypted)
            {
                AmpProfile profile = AmpProfile.ReadFromStream(encStr == null ? inFile : encStr, true);
                AmpSaveInfo(profile);
                if (encStr != null)
                    encStr.FinishReading();
            }
            else if (type == SaveDetection.SaveType.RB4PS4 || type == SaveDetection.SaveType.RB4PS4_Decrypted)
            {
                RBProfile profile = RBProfile.ReadFromStream(encStr == null ? inFile : encStr, true);
                RB4SaveInfo(profile);
                if (encStr != null)
                    encStr.FinishReading();
            }
            else if (type == SaveDetection.SaveType.RB4Xbox || type == SaveDetection.SaveType.RB4Xbox_Decrypted)
            {
                RBProfile profile = RBProfile.ReadFromStream(encStr == null ? inFile : encStr, false);
                RB4SaveInfo(profile);
                if (encStr != null)
                    encStr.FinishReading();
            }
            else if (type == SaveDetection.SaveType.RB4XboxSystemOptions || type == SaveDetection.SaveType.RB4XboxSystemOptions_Decrypted)
            {
                RBSystemOptions options = RBSystemOptions.ReadFromStream(encStr == null ? inFile : encStr);
                RB4SystemInfo(options);
                if (encStr != null)
                    encStr.FinishReading();
            }
            else
            {
                Console.WriteLine("Can't get info for this file type.");
            }
            if (encStr != null)
                encStr.Close();
            inFile.Close();
        }
    }
}
