davesave is a save file information viewer, decryptor and encryptor for
Harmonix's "Forge" engine games (RB4, Amp2016, FME, RBVR, DCS)

available commands: info, decrypt, encrypt
supported games: Amplitude 2016 (1.0/1.1), Rock Band 4 (2.3.7)

- davesave info [filename]
    will display savegame info from the file provided

- davesave convert [input file] [output file]
    Amplitude 2016 ONLY!! will convert between PS4 and PS3 save file formats

- davesave decrypt [input file] [output file] (optional: ps3/360/ps4/xb1/pc)
    will decrypt the encrypted save from the input file into the output
    platform must be provided if working with an unsupported game

- davesave encrypt [input file] [output file] (optional: ps3/360/ps4/xb1/pc)
    will encrypt a decrypted save from the input file into the output
    platform must be provided if working with an unsupported game

uses code from LibForge: https://github.com/mtolly/LibForge
uses DtxCS: https://github.com/InvoxiPlayGames/DtxCS
thank you for everything, maxton
