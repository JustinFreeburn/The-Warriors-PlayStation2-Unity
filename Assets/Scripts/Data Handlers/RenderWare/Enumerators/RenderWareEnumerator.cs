using System;

namespace TheWarriors
{
    public enum DMATag : UInt32
    {
        Vertex3Float32 = 0x68008000,

        Vertex4Int16 = 0x6D008000
    }

    public enum Platform : UInt32
    {
        Playstation2ClumpNative = 0x00000004,

        Playstation2TextureNative = 0x00325350
    }

    public enum RenderWareStreamFileID : Int32
    {
        Texture = 0x2A,

        Mesh = 0x47,

        // NOTE: This looks like extra bone data... only present if there are two files in a DFF (0x47)
        SomethingAtTheEndOfADff = 0x28,

        World = 0x15,

        LevelCollisionMesh = 0x07
    }

    public enum RenderWareSectionID : Int32
    {
        None = 0x0,

        Struct = 0x1,

        String = 0x2,

        Extension = 0x3,

        Texture = 0x6,

        Material = 0x7,

        MaterialList = 0x8,

        AtomicSection = 0x9,

        PlaneSection = 0xA,

        World = 0xB,

        FrameList = 0xE,

        Geometry = 0xF,

        Clump = 0x10,

        Atomic = 0x14,

        TextureNative = 0x15,

        TextureDictionary = 0x16,

        GeometryList = 0x1A,

        ChunkGroupStart = 0x29,

        ChunkGroupEnd = 0x2A,

        ColTree = 0x2C,

        MorphPLG = 0x105,

        SkyMipmapVal = 0x110,

        SkinPLG = 0x116,

        CollisionPLG = 0x11D,

        UserDataPLG = 0x11F,

        MaterialEffectsPLG = 0x120,

        BinMeshPLG = 0x50E,

        NativeDataPLG = 0x510,

        HAnimPLG = 0x011E,

        RightToRender = 0x1F,

        RockstarGamesCustom1 = 0x3F0,

        RockstarGamesCustom2 = 0x3F1,

        GeometricPVSPlg = 0x12A,

        BFBB_CollisionData_Section1 = 0x00BEEF01,

        BFBB_CollisionData_Section2 = 0x00BEEF02,

        BFBB_CollisionData_Section3 = 0x00BEEF03,
    }

    public enum PipelineDeliverySystem : UInt32
    {
        Unknown = 0xFFFFFFFF,

        // NOTE: So far, only used with skinned characters...
        PDS_PS2_CustomSkinPed_MatPipeID = 0x00030081,

        // NOTE: Shared between objects and sectors... and it also appears level meshes now???
        PDS_PS2_CustomBuildingEnvMap1_MatPipeID = 0x00030084,

        PDS_PS2_CustomBuildingEnvMap2_MatPipeID = 0x00030088,

        PDS_PS2_CustomBuildingEnvMap3_MatPipeID = 0x00030086,

        PDS_PS2_CustomWorld_MatPipeID = 0x00000131
    }

    public enum WorldFlag : Int32
    {
        UseTriangleStrips = 0x00000001,

        HasVertexPositions = 0x00000002,

        HasOneSetOfTextCoords = 0x00000004,

        HasVertexColors = 0x00000008,

        HasNormals = 0x00000010,

        UseLighting = 0x00000020,

        ModulateMaterialColors = 0x00000040,

        HasMultipleSetsOfTextCoords = 0x00000080,

        IsNativeGeometry = 0x01000000,

        IsNativeInstance = 0x02000000,

        FlagsMask = 0x000000FF,

        NativeFlagsMask = 0x0F000000,

        WorldSectorsOverlap = 0x40000000
    }

    public enum TextureAddressMode : byte
    {
        // No tiling
        TEXTUREADDRESSNATEXTUREADDRESS = 0,

        // Tile in U or V direction
        TEXTUREADDRESSWRAP = 1,

        // Mirror in U or V direction
        TEXTUREADDRESSMIRROR = 2,

        TEXTUREADDRESSCLAMP = 3,

        TEXTUREADDRESSBORDER = 4

        /*
            WRAP_NONE    0x00
            WRAP_WRAP    0x01
            WRAP_MIRROR  0x02
            WRAP_CLAMP   0x03
        */
    }

    public enum TextureFilterMode : byte
    {
        FILTERNAFILTERMODE = 0,     // Filtering is disabled

        FILTERNEAREST = 1,          // Point sampled

        FILTERLINEAR = 2,           // Bilinear

        FILTERMIPNEAREST = 3,       // Point sampled per pixel mip map

        FILTERMIPLINEAR = 4,        // Bilinear per pixel mipmap

        FILTERLINEARMIPNEAREST = 5, // MipMap interp point sampled

        FILTERLINEARMIPLINEAR = 6   // Trilinear

        /*
            FILTER_NONE                0x00
            FILTER_NEAREST             0x01
            FILTER_LINEAR              0x02
            FILTER_MIP_NEAREST         0x03
            FILTER_MIP_LINEAR          0x04
            FILTER_LINEAR_MIP_NEAREST  0x05
            FILTER_LINEAR_MIP_LINEAR   0x06
        */
    }

    public enum TextureRasterFormat : Int32
    {
        RASTER_DEFAULT = 0x0000,

        RASTER_C1555 = 0x0100,

        RASTER_C565 = 0x0200,

        RASTER_C4444 = 0x0300,

        RASTER_LUM8 = 0x0400,

        RASTER_C8888 = 0x0500,

        RASTER_C888 = 0x0600,

        RASTER_D16 = 0x0700,

        RASTER_D24 = 0x0800,

        RASTER_D32 = 0x0900,

        RASTER_C555 = 0x0A00,

        RASTER_NOEXT = 0x0FFF,

        RASTER_AUTOMIPMAP = 0x1000,

        RASTER_PAL8 = 0x2000,

        RASTER_PAL4 = 0x4000,

        RASTER_MIPMAP = 0x8000

        /*
        FORMAT_DEFAULT          0x0000
        FORMAT_1555             0x0100 (1 bit alpha, RGB 5 bits each; also used for DXT1 with alpha)
        FORMAT_565              0x0200 (5 bits red, 6 bits green, 5 bits blue; also used for DXT1 without alpha)
        FORMAT_4444             0x0300 (RGBA 4 bits each; also used for DXT3)
        FORMAT_LUM8             0x0400 (gray scale, D3DFMT_L8)
        FORMAT_8888             0x0500 (RGBA 8 bits each)
        FORMAT_888              0x0600 (RGB 8 bits each, D3DFMT_X8R8G8B8)
        FORMAT_555              0x0A00 (RGB 5 bits each - rare, use 565 instead, D3DFMT_X1R5G5B5)

        FORMAT_EXT_AUTO_MIPMAP  0x1000 (RW generates mipmaps, see special section below)
        FORMAT_EXT_PAL8         0x2000 (2^8 = 256 palette colors)
        FORMAT_EXT_PAL4         0x4000 (2^4 = 16 palette colors)
        FORMAT_EXT_MIPMAP       0x8000 (mipmaps included) 
        */
    }
}