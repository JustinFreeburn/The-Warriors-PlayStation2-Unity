using UnityEngine;

namespace TheWarriors
{
    public static class RenderWareTexture
    {
        /// <summary>
        /// Returns a single Texture2D object from a TextureNative
        /// </summary>
        public static Texture2D LoadTextureFromRenderWareTextureNative(TextureNative textureNative)
        {
            /*
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
            */

            /*
            if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_DEFAULT) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_DEFAULT");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_C1555) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_C1555");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_C565) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_C565");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_C4444) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_C4444");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_LUM8) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_LUM8");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_C8888) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_C8888");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_C888) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_C888");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_D16) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_D16");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_D24) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_D24");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_D32) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_D32");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_C555) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_C555");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_NOEXT) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_NOEXT");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_AUTOMIPMAP) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_AUTOMIPMAP");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_PAL8) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_PAL8");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_PAL4) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_PAL4");
            else if ((textureNative.textureNativeStructure.RasterFormat & TextureRasterFormat.RASTER_MIPMAP) != 0)
                Debug.Log("RasterFormat=TextureRasterFormat.RASTER_MIPMAP");
            */

            Texture2D texture = new Texture2D(textureNative.textureNativeStructure.iWidth, textureNative.textureNativeStructure.iHeight, TextureFormat.RGBA32, false);

            texture.name = textureNative.textureNativeStructure.sTextureName;
            texture.LoadRawTextureData(textureNative.textureNativeStructure.bData);

            texture.filterMode = FilterMode.Trilinear;
            texture.wrapModeU = TextureWrapMode.Clamp;
            texture.wrapModeV = TextureWrapMode.Clamp;

            if (textureNative.textureNativeStructure.AddressModeU == TextureAddressMode.TEXTUREADDRESSWRAP)
            {
                texture.wrapModeU = TextureWrapMode.Repeat;
            }
            else if (textureNative.textureNativeStructure.AddressModeU == TextureAddressMode.TEXTUREADDRESSMIRROR)
            {
                texture.wrapModeU = TextureWrapMode.Mirror;
            }
            else if (textureNative.textureNativeStructure.AddressModeU == TextureAddressMode.TEXTUREADDRESSCLAMP)
            {
                texture.wrapModeU = TextureWrapMode.Clamp;
            }

            if (textureNative.textureNativeStructure.AddressModeV == TextureAddressMode.TEXTUREADDRESSWRAP)
            {
                texture.wrapModeV = TextureWrapMode.Repeat;
            }
            else if (textureNative.textureNativeStructure.AddressModeV == TextureAddressMode.TEXTUREADDRESSMIRROR)
            {
                texture.wrapModeV = TextureWrapMode.Mirror;
            }
            else if (textureNative.textureNativeStructure.AddressModeV == TextureAddressMode.TEXTUREADDRESSCLAMP)
            {
                texture.wrapModeV = TextureWrapMode.Clamp;
            }

            texture.Apply();

            return texture;
        }
    }
}