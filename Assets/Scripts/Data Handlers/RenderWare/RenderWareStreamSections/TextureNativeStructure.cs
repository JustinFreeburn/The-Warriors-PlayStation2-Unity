using System;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public class TextureNativeStructure : RenderWareSection
    {
        public UInt32 uiPlatform;

        public TextureFilterMode FilterMode;

        public TextureAddressMode AddressModeU;

        public TextureAddressMode AddressModeV;

        public UInt16 uiUseMipLevels;

        public String sTextureName;

        public String sAlphaName;

        public Int32 iWidth;

        public Int32 iHeight;

        public Int32 iSwizzleWidth;

        public Int32 iSwizzleHeight;

        public Int32 iDepth;
        
        public TextureRasterFormat RasterFormat;
        
        public Int32 iDataSize;
        
        public Int32 iPaletteSize;
        
        public Int32 iCombinedGPUDataSize;
        
        public Int32 iSkyMipMapValue;

        public byte[] bData = null;
        
        public byte[] bPaletteData = null;

        private long lStartSectionPosition;
        
        private long lDataSectionSize;
        
        private long lDataSectionStart;

        public TextureNativeStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            lStartSectionPosition = reader.Position();

            UInt32 uiPlatform = reader.ReadUInt32();

            if ((Platform)uiPlatform == Platform.Playstation2TextureNative)
            {
                ReadPS2TextureNativeStructure(reader);
                FormatPS2TextureNativeData();
            }
            else
            {
                throw new InvalidDataException("*** Error: TextureNativeStructure.Read() - unsupported platform " + uiPlatform + ".");
            }

            // TODO: using lStartSectionPosition, verify EOF has been reached.

            return this;
        }

        public void ReadPS2TextureNativeStructure(ArchiveFileBinaryReader reader)
        {
            Int32 iStringLength;

            // NOTE: texture filter mode
            {
                FilterMode = (TextureFilterMode)reader.ReadByte();

                byte bAddressMode = reader.ReadByte();

                AddressModeU = (TextureAddressMode)((bAddressMode & 0xF0) >> 4);
                AddressModeV = (TextureAddressMode)(bAddressMode & 0x0F);

                // TODO: investigate if these two bits are the mip map levels?
                // NOTE: This is the uiUseMipLevels variable
                //reader.SeekCurrent(2);
                uiUseMipLevels = reader.ReadUInt16();
            }

            // TODO: Handle this differently? Should I have a String().Read(reader) class???
            // NOTE: texture name
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.String)
                {
                    throw new Exception("*** Error: Expected String at position " + (reader.Position() - 4));
                }

                iStringLength = reader.ReadInt32();
                reader.SeekCurrent(4);

                sTextureName = new String(reader.ReadChars(iStringLength));
                sTextureName = sTextureName.Replace("\0", "");
            }

            // TODO: Handle this differently? Should I have a String().Read(reader) class???
            // NOTE: texture alpha/mask name
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.String)
                {
                    throw new Exception("*** Error: Expected String at position " + (reader.Position() - 4));
                }

                iStringLength = reader.ReadInt32();
                reader.SeekCurrent(4);

                sAlphaName = new String(reader.ReadChars(iStringLength));
                sAlphaName = sTextureName.Replace("\0", "");
            }

            // NOTE: struct. this contains the actual data section size. the next struct follows immediately...
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected Struct at position " + (reader.Position() - 4));
                }

                // NOTE: stop processing this file if we are only after the texture name...
                if (UnityTextureManager.bGetTextureNameOnly == true)
                {
                    reader.SeekCurrent(reader.ReadInt32() + 4);

                    return;
                }
                else
                {
                    // NOTE: first struct is the data section size...
                    lDataSectionSize = reader.ReadInt32();
                    reader.SeekCurrent(4);
                    lDataSectionStart = reader.Position();
                }
            }

            // NOTE: struct. must be 64 bytes in size. this contains ps2 texture meta data.
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected Struct at position " + (reader.Position() - 4));
                }

                // NOTE: assert that this Structure is 64 bytes long...
                if (reader.ReadUInt32() != 64)
                {
                    throw new Exception("*** Error: Expected Struct section size to be 64 bytes at " + (reader.Position() - 4));
                }

                reader.SeekCurrent(4);

                // @128
                iSwizzleWidth = iWidth = reader.ReadInt32();
                // @132
                iSwizzleHeight = iHeight = reader.ReadInt32();
                // @136
                // NOTE: if iDepth == 16, has alpha?
                iDepth = reader.ReadInt32();
                // @140
                RasterFormat = (TextureRasterFormat)reader.ReadInt32();

                // NOTE: believe this is ps2 gs register data. unrequired...?
                // @144
                reader.SeekCurrent(32);

                // @176
                iDataSize = reader.ReadInt32();
                // @180
                iPaletteSize = reader.ReadInt32();
                // @184
                iCombinedGPUDataSize = reader.ReadInt32();
                // @188
                iSkyMipMapValue = reader.ReadInt32();

                // NOTE: data validation. currently unaware of other raster formats...
                if (((RasterFormat & TextureRasterFormat.RASTER_PAL4) == 0) && ((RasterFormat & TextureRasterFormat.RASTER_PAL8) == 0))
                {
                    //throw new InvalidDataException("*** Error: Unsupported texture raster format.");
                    Debug.Log("*** Error: \"" + sTextureName + "\" - unsupported texture raster format.");
                }
            }

            // NOTE: struct... this contains all the data.
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected Struct at position " + (reader.Position() - 4));
                }

                reader.SeekCurrent(8);

                // NOTE: 0x2000 (RASTER_PAL8) = swizzled, has header
                // NOTE: we are reading only parts of the ps2 register data here...
                // NOTE: iDataSize changes here if the texture is swizzled.
                {
                    if ((RasterFormat & TextureRasterFormat.RASTER_PAL8) != 0)
                    {
                        // NOTE: skip ps2 header data (registers). unecessary to read...
                        reader.SeekCurrent(32);

                        iSwizzleWidth = reader.ReadInt32();
                        iSwizzleHeight = reader.ReadInt32();

                        // NOTE: skip ps2 header data (registers). unecessary to read...
                        reader.SeekCurrent(24);

                        // NOTE: 4 bytes per r, g, b, a - (16) * iDataSize
                        iDataSize = 16 * reader.ReadInt32();

                        // NOTE: skip ps2 header data (registers). unecessary to read...
                        reader.SeekCurrent(12);
                    }
                }

                // NOTE: read in the texture data...
                {
                    bData = new byte[iDataSize];
                    bData = reader.ReadBytes(iDataSize);
                }

                // NOTE: finally, read in the palette data...
                // NOTE: RASTER_PAL8=1024, RASTER_PAL4: 8bit depth=128, 4bit depth=96
                {
                    if (((RasterFormat & TextureRasterFormat.RASTER_PAL8) != 0) || (((RasterFormat & TextureRasterFormat.RASTER_PAL4) != 0)) && iDepth == 4) {
                        // NOTE: deduct header size...
                        iPaletteSize -= 80;

                        // NOTE: skip ps2 header data (registers). unecessary to read...
                        reader.SeekCurrent(80);
                    }

                    iPaletteSize /= 4;

                    if ((RasterFormat & TextureRasterFormat.RASTER_PAL8) != 0)
                    {
                        if (iPaletteSize != 256)
                        {
                            //throw new InvalidDataException("*** Error: Texture iPaletteSize=" + iPaletteSize + ", not 256 (1024 / 4).");
                            Debug.Log("*** Error: Texture iPaletteSize=" + iPaletteSize + ", not 256 (2^8 = 256).");
                        }
                    }
                    else if ((RasterFormat & TextureRasterFormat.RASTER_PAL4) != 0)
                    {
                        if (iDepth == 4)
                        {
                            if (iPaletteSize != 24)
                            {
                                //throw new InvalidDataException("*** Error: Texture iPaletteSize=" + iPaletteSize + ", not 24 (2^4 = 24).");
                                Debug.Log("Warning: Texture iPaletteSize=" + iPaletteSize + ", not 24 (2^4 = 24).");
                            }
                        }
                        else
                        {
                            if (iPaletteSize != 16)
                            {
                                //throw new InvalidDataException("*** Error: Texture iPaletteSize=" + iPaletteSize + ", not 16 (2^4 = 16).");
                                Debug.Log("Warning: Texture iPaletteSize=" + iPaletteSize + ", not 16 (2^4 = 16).");
                            }
                            else
                            {
                                //throw new InvalidDataException("*** Error: Texture iPaletteSize=" + iPaletteSize + ", unknown!.");
                                Debug.Log("Warning: Texture iPaletteSize=" + iPaletteSize + ", unknown! Texture iDepth=" + iDepth + ".");
                            }
                        }
                    }

                    // NOTE: 4 bytes per r, g, b, a - iPaletteSize * 4
                    bPaletteData = new byte[iPaletteSize * 4];
                    bPaletteData = reader.ReadBytes(iPaletteSize * 4);
                }
            }

            // NOTE: New! Skip unread data... World files such as 0x408982B4 (9665) have 544+ (upto 1648 observed) bytes of data in some of their TextureNativeStructures!
            long lDataSectionSkip = lDataSectionSize - (reader.Position() - lDataSectionStart);

            if (lDataSectionSkip > 0)
            {
                //Debug.Log("Warning: Skipping " + lDataSectionSkip + " bytes at end of TextureNativeStructure!");

                reader.SeekCurrent(lDataSectionSkip);
            }
        }

        public void FormatPS2TextureNativeData()
        {
            if (iDepth != 4)
            {
                // TODO: confirm RASTER_PAL8 is the only texture format that needs to be unclut?
                UnClutPS2TextureNativeData();
            }

            // TODO: check for header...
            if (((RasterFormat & TextureRasterFormat.RASTER_PAL8) != 0) || ((RasterFormat & TextureRasterFormat.RASTER_PAL4) != 0))
            {
                UnswizzlePS2TextureNativeData();
            }

            // NOTE: convert the alpha.
            for (Int32 iIterator = 0; iIterator < iPaletteSize; iIterator++)
            {
                Int32 iNewAlpha = (bPaletteData[iIterator * 4 + 3] * 255) / 128;
                bPaletteData[iIterator * 4 + 3] = (byte)iNewAlpha;
            }

            // NOTE: convert the data and palette to RGBA for Unity.
            if (((RasterFormat & TextureRasterFormat.RASTER_PAL8) != 0) || ((RasterFormat & TextureRasterFormat.RASTER_PAL4) != 0))
            {
                byte[] bConvertedData = null;

                bConvertedData = new byte[iWidth * iHeight * 4];

                for (Int32 iIterator = 0; iIterator < (iWidth * iHeight); iIterator++)
                {
                    bConvertedData[iIterator * 4 + 0] = bPaletteData[bData[iIterator] * 4 + 0];
                    bConvertedData[iIterator * 4 + 1] = bPaletteData[bData[iIterator] * 4 + 1];
                    bConvertedData[iIterator * 4 + 2] = bPaletteData[bData[iIterator] * 4 + 2];
                    bConvertedData[iIterator * 4 + 3] = bPaletteData[bData[iIterator] * 4 + 3];
                }

                bData = bConvertedData;
            }
        }

        public void UnClutPS2TextureNativeData()
        {
            byte[] bUnclutMap = bUnclutMap = new byte[] { 0, 16, 8, 24 };

            for (Int32 iIterator = 0; iIterator < (iWidth * iHeight); iIterator++)
            {
                bData[iIterator] = (byte)((bData[iIterator] & ~0x18) | bUnclutMap[(bData[iIterator] & 0x18) >> 3]);
            }
        }

        // NOTE: Big thanks and shout out to Dageron and Fireboyd78 for sharing their 4 bit unswizzle code (Python & C#)!
        // NOTE: I modified their 4 bit unswizzle code to convert 8 bit also. UnswizzlePS2TextureNativeData is a neat little function now...
        public void UnswizzlePS2TextureNativeData()
        {
            Int32 iUnswizzledTextureDataSize = iSwizzleHeight * iSwizzleWidth * 4;
            Int32 iUnswizzledHeight = iSwizzleHeight * 2;
            Int32 iUnswizzledWidth = iSwizzleWidth * 2;

            if (iDepth == 4)
            {
                iUnswizzledTextureDataSize = iSwizzleHeight * iSwizzleWidth;
                iUnswizzledHeight = iSwizzleHeight;
                iUnswizzledWidth = iSwizzleWidth;

                // NOTE: depending on the depth, the palette index can be 4 or 8 bits wide.
                // NOTE: the ps2 architecture expects a 4 bits wide palette index for RASTER_PAL4 while the xbox/pc expect a 8 bits wide palette index.
                // NOTE: the ps2 does not support 8bit RASTER_PAL4 rasters, only 4bit.

                // NOTE: convert 4 bit to 8 bit, palette stays 4 bit.
                byte[] bNewData = new byte[iDataSize * 2];

                for (Int32 iIterator = 0; iIterator < iDataSize; iIterator++)
                {
                    bNewData[iIterator * 2 + 0] = (byte)(bData[iIterator] & 0xF);
                    bNewData[iIterator * 2 + 1] = (byte)(bData[iIterator] >> 4);
                }

                bData = bNewData;

                Array.Reverse(bData);
            }

            byte[] bUnswizzledTextureData = new byte[iUnswizzledTextureDataSize];
            byte[] bInterlaceMatrix = { 0x00, 0x10, 0x02, 0x12, 0x11, 0x01, 0x13, 0x03 };

            int[] iMatrix = { 0, 1, -1, 0 };
            int[] iTileMatrix = { 4, -4 };

            // NOTE: unswizzle!
            for (Int32 iIteratorY = 0; iIteratorY < iUnswizzledHeight; iIteratorY++)
            {
                for (Int32 iIteratorX = 0; iIteratorX < iUnswizzledWidth; iIteratorX++)
                {
                    Int32 iSum1 = (byte)((iIteratorY / 4) & 1);
                    Int32 iSum2 = (byte)((iIteratorX / 4) & 1);
                    Int32 iSum3 = (iIteratorY % 4);
                    Int32 iSum4 = ((iIteratorX / 4) % 4);
                    Int32 iSum5 = ((iIteratorX * 4) % 16);
                    Int32 iSum6 = ((iIteratorX / 16) * 32);
                    Int32 iSum7 = 0;
                    Int32 iSumX = iIteratorX + iSum1 * iTileMatrix[iSum2];
                    Int32 iSumY = iIteratorY + iMatrix[iSum3];

                    if ((iIteratorY & 1) != 0)
                    {
                        iSum4 += 4;
                        iSum7 = (iIteratorY - 1) * iSwizzleWidth;
                    }
                    else
                    {
                        iSum7 = iIteratorY * iSwizzleWidth;
                    }

                    if (iDepth == 4)
                    {
                        if ((iSumY * iUnswizzledWidth + iSumX) < bUnswizzledTextureData.Length &&
                            (bInterlaceMatrix[iSum4] + iSum5 + iSum6 + iSum7) < bInterlaceMatrix.Length)
                        {
                            bUnswizzledTextureData[iSumY * iUnswizzledWidth + iSumX] = bData[bInterlaceMatrix[iSum4] + iSum5 + iSum6 + iSum7];
                        }
                        else
                        {
                            // TODO: There are some WLD files that break this. 0x609FEFE1 (8629) is one.
                        }
                    }
                    else
                    {
                        bUnswizzledTextureData[iSumY * iUnswizzledWidth + iSumX] = bData[bInterlaceMatrix[iSum4] + iSum5 + iSum6 + iSum7 * 2];
                    }
                }
            }

            bData = bUnswizzledTextureData;

            if (iDepth == 4)
            {
                Array.Reverse(bData);
                iDepth = 8;
            }
        }
    }
}