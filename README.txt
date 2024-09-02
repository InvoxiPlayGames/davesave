usage: davesave [decrypt/encrypt] [platform] [input] [output]
where platform is ps4, ps3, 360, xb1 or pc

does not parse RevisionStream header yet - skip the first 5 bytes on the save file
before decrypting and add 5 bytes after encrypting (7A 00 00 00 00)

uses code from LibForge (RIP maxton): https://github.com/mtolly/LibForge
