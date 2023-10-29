using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{
    public static class UnityTextureManager
    {
        private static Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D>();

        public static bool bGetTextureNameOnly = false;

        /// <summary>
        /// Stores Texture2D objects from a RenderWareStream (TextureDictionary) using the RenderWareTexture class
        /// </summary>
        public static void LoadTexturesFromRenderWareSections(RenderWareSection[] renderWareSections)
        {
            foreach (RenderWareSection renderWareSection in renderWareSections)
            {
                if (renderWareSection is TextureDictionary textureDictionary)
                {
                    foreach (TextureNative textureNative in textureDictionary.textureNativeList)
                    {
                        if (textures.ContainsKey(textureNative.textureNativeStructure.sTextureName) == false)
                        {
                            textures.Add(textureNative.textureNativeStructure.sTextureName, RenderWareTexture.LoadTextureFromRenderWareTextureNative(textureNative));
                        }
                        else
                        {
                            //Debug.Log("Warning: Duplicate texture found. Skipped loading \"" + textureNative.textureNativeStructure.sTextureName + "\".");
                        }
                    }
                }
            }
        }

        public static String GetTextureNameFromFileNameHash(UInt32 uiFileHash_)
        {
            bGetTextureNameOnly = true;

            RenderWareStream renderWareStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(uiFileHash_)));

            bGetTextureNameOnly = false;

            foreach (RenderWareSection renderWareSection in renderWareStream.RenderWareStreamSections)
            {
                if (renderWareSection is TextureDictionary textureDictionary)
                {
                    if (textureDictionary.textureNativeList.Count > 1)
                    {
                        Debug.Log("Warning: More than 1 texture while using UnityTextureManager.GetTextureNameFromTXD(). Returning first texture name.");
                    }

                    return textureDictionary.textureNativeList[0].textureNativeStructure.sTextureName;
                }
            }

            return "";
        }

        public static Texture2D GetTextureFromDictionary(String textureName)
        {
            if (textures.ContainsKey(textureName) == true)
            {
                return textures[textureName];
            }

            // TODO: Return a default texture to identify failure...?

            return null;
        }

        public static void DisposeTextures()
        {
            foreach (Texture2D texture in textures.Values)
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
            }
        }
    }
}