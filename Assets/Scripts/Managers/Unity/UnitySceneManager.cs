using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{
    public static class UnitySceneManager
    {
        // NOTE: Data comes from SEC/BSP file
        public static List<GameObject> sectorModelList = new List<GameObject>();

        // NOTE: Data comes from WLD file
        public static Dictionary<UInt32, Vector3> sectorModelPositionList = new Dictionary<UInt32, Vector3>();

        // NOTE: Data comes from LEV file
        public static GameObject skybox;

        public static GameObject sky;

        public static GameObject terrain;

        public static GameObject collisionMesh;

        // NOTE: Character GameObject pool
        public static Dictionary<String, GameObject> characterList = new Dictionary<String, GameObject>();

        // NOTE: Object GameObject pool
        public static Dictionary<String, GameObject> objectList = new Dictionary<String, GameObject>();

        public static GameObject camera;

        public static bool LoadLevel(string levelName, GameObject levelObject, GameObject sectorObject)
        {
            LevelJsonManager.Level level = LevelJsonManager.FindLevelByName(levelName);

            if (level != null)
            {
                if (!string.IsNullOrEmpty(level.levelHash))
                {
                    if (levelObject != null)
                    {
                        LoadLevel(Utility.ConvertToHex(level.levelHash), levelObject);
                    }
                    else
                    {
                        Debug.Log("*** Error: UnitySceneManager.LoadLevel failed. levelObject empty.");

                        return false;
                    }
                }
                if (level.sectorCount == 1)
                {
                    List<UInt32> sector1Hashes = new List<UInt32>();

                    foreach (var test in level.sector1Hashes)
                    {
                        sector1Hashes.Add(Utility.ConvertToHex(test));
                    }

                    if (sectorObject != null)
                    {
                        LoadWorldAndSectors(Utility.ConvertToHex(level.sector1Hash), sectorObject, sector1Hashes, false);
                    }
                    else
                    {
                        Debug.Log("*** Error: UnitySceneManager.LoadLevel failed. sectorObject empty.");

                        return false;
                    }
                }

                // NOTE: The RenderWare engine must load the WLD files before the SEC files. I have found some SEC files reference textures in both WLD files.
                if (level.sectorCount == 2)
                {
                    List<UInt32> sector1Hashes = new List<UInt32>();
                    List<UInt32> sector2Hashes = new List<UInt32>();

                    foreach (var test in level.sector1Hashes)
                    {
                        sector1Hashes.Add(Utility.ConvertToHex(test));
                    }

                    foreach (var test in level.sector2Hashes)
                    {
                        sector2Hashes.Add(Utility.ConvertToHex(test));
                    }

                    if (sectorObject != null)
                    {
                        //LoadWorldAndSectors(Utility.ConvertToHex(level.sector1Hash), sectorObject, sector1Hashes, false);
                        //LoadWorldAndSectors(Utility.ConvertToHex(level.sector2Hash), sectorObject, sector2Hashes, false);

                        RenderWareWorld test1 = LoadWorldOnly(Utility.ConvertToHex(level.sector1Hash));
                        RenderWareWorld test2 = LoadWorldOnly(Utility.ConvertToHex(level.sector2Hash));

                        if (test1 != null)
                        {
                            LoadSectorsOnly(Utility.ConvertToHex(level.sector1Hash), test1, sectorObject, sector1Hashes, false);
                        }

                        if (test2 != null)
                        {
                            LoadSectorsOnly(Utility.ConvertToHex(level.sector2Hash), test2, sectorObject, sector2Hashes, false);
                        }
                    }
                    else
                    {
                        Debug.Log("*** Error: UnitySceneManager.LoadLevel failed. sectorObject empty.");

                        return false;
                    }

                }

                return true;
            }

            return false;
        }

        public static void LoadLevel(UInt32 uiFileHash_, GameObject gameObject)
        {
            RenderWareLevel renderWareLevel = new RenderWareLevel(uiFileHash_);

            if (renderWareLevel.renderWareStream == null)
            {
                Debug.Log("*** Error: UnitySceneManager.LoadLevel(" + String.Format("{0:X8}", uiFileHash_) + ") failed.");

                return;
            }

            UnityEngine.Object.Destroy(skybox);
            UnityEngine.Object.Destroy(sky);
            UnityEngine.Object.Destroy(terrain);
            UnityEngine.Object.Destroy(collisionMesh);

            collisionMesh = renderWareLevel.CreateCollisionObject();
            terrain = renderWareLevel.CreateGameObjectFromIndex("terrain", 0);
            skybox = renderWareLevel.CreateGameObjectFromIndex("level_skybox", 1);
            sky = renderWareLevel.CreateGameObjectFromIndex("level_sky", 2);

            if (gameObject != null)
            {
                collisionMesh.transform.parent = gameObject.transform;
                terrain.transform.parent = gameObject.transform;
                skybox.transform.parent = gameObject.transform;
                sky.transform.parent = gameObject.transform;
            }

            terrain.SetActive(false);
            skybox.SetActive(false);
            sky.SetActive(false);
        }

        public static void LoadWorldAndSectors(UInt32 uiFileHash, GameObject gameObject, List<UInt32> sectors, bool bTransparent)
        {
            RenderWareWorld renderWareWorld = new RenderWareWorld(uiFileHash);

            if (renderWareWorld.renderWareWorldFile == null)
            {
                Debug.Log("*** Error: UnitySceneManager.LoadWorld(" + String.Format("{0:X8}", uiFileHash) + ") failed.");

                return;
            }

            for (Int32 iIterator = 0; iIterator < sectors.Count; iIterator++)
            {
                GameObject renderWareSectorGameObject = new RenderWareSector(sectors[iIterator]).CreateSectorObject(renderWareWorld.atomicPositions, bTransparent);

                if (renderWareSectorGameObject != null)
                {
                    sectorModelList.Add(renderWareSectorGameObject);

                    if (gameObject != null)
                    {
                        renderWareSectorGameObject.transform.parent = gameObject.transform;
                    }
                }
            }
        }

        public static RenderWareWorld LoadWorldOnly(UInt32 uiFileHash)
        {
            RenderWareWorld renderWareWorld = new RenderWareWorld(uiFileHash);

            if (renderWareWorld.renderWareWorldFile == null)
            {
                Debug.Log("*** Error: UnitySceneManager.LoadWorldOnly(" + String.Format("{0:X8}", uiFileHash) + ") failed.");
            }

            return renderWareWorld;
        }

        public static void LoadSectorsOnly(UInt32 uiFileHash, RenderWareWorld renderWareWorld, GameObject gameObject, List<UInt32> sectors, bool bTransparent)
        {
            if (renderWareWorld.renderWareWorldFile == null)
            {
                Debug.Log("*** Error: UnitySceneManager.LoadSectorsOnly(" + String.Format("{0:X8}", uiFileHash) + ") failed.");

                return;
            }

            for (Int32 iIterator = 0; iIterator < sectors.Count; iIterator++)
            {
                RenderWareSector test = new RenderWareSector(sectors[iIterator]);

                GameObject renderWareSectorGameObject = test.CreateSectorObject(renderWareWorld.atomicPositions, bTransparent);

                if (renderWareSectorGameObject != null)
                {
                    sectorModelList.Add(renderWareSectorGameObject);

                    if (gameObject != null)
                    {
                        renderWareSectorGameObject.transform.parent = gameObject.transform;
                    }
                }
            }
        }

        public static void LoadWorld(UInt32 uiFileHash)
        {
            // NOTE: Load textures for the sector models
            // NOTE: Store sector model positions
            RenderWareWorld renderWareWorld = new RenderWareWorld(uiFileHash);

            if (renderWareWorld.renderWareWorldFile == null)
            {
                Debug.Log("*** Error: UnitySceneManager.LoadWorld(" + String.Format("{0:X8}", uiFileHash) + ") failed.");

                return;
            }

            DisposeSectorModelPositionList();

            // TODO: Store this dictionary in a dictionary.
            sectorModelPositionList = new Dictionary<UInt32, Vector3>(renderWareWorld.atomicPositions);
        }

        public static void LoadSectors(List<UInt32> sectors)
        {
            DisposeSectorModelList();

            for (Int32 iIterator = 0; iIterator < sectors.Count; iIterator++)
            {
                GameObject renderWareSectorGameObject = new RenderWareSector(sectors[iIterator]).CreateSectorObject(sectorModelPositionList, false);

                if (renderWareSectorGameObject != null)
                {
                    sectorModelList.Add(renderWareSectorGameObject);
                }
            }
        }

        public static void LoadCharacter(String sCharacterName_, GameObject gameObject)
        {
            if (characterList.ContainsKey(sCharacterName_) == false)
            {
                GameObject characterGameObject = new Character(sCharacterName_).CreateCharacterObject();

                if (characterGameObject != null)
                {
                    characterList.Add(sCharacterName_, characterGameObject);

                    if (gameObject != null)
                    {
                        characterGameObject.gameObject.transform.parent = gameObject.transform;
                    }
                }
            }
        }

        public static void LoadCharacterFromHash(UInt32 uiHash)
        {
            GameObject characterGameObject = new Character("").CreateCharacterObjectFromHash(uiHash);
        }

        public static void LoadObject(String sObjectName_, GameObject gameObject)
        {
            if (objectList.ContainsKey(sObjectName_) == false)
            {
                GameObject objectGameObject = new Object(sObjectName_).CreateObjectObject();

                if (objectGameObject != null)
                {
                    objectList.Add(sObjectName_, objectGameObject);

                    if (gameObject != null)
                    {
                        objectGameObject.gameObject.transform.parent = gameObject.transform;
                    }
                }
            }
        }

        public static void LoadObjectFromHash(UInt32 uiHash)
        {
            GameObject objectGameObject = new Object("").CreateObjectObjectFromHash(uiHash);
        }

        public static void SetCharacterPosition(String sCharacterName_, Vector3 position, Vector3 rotation)
        {
            if (characterList.ContainsKey(sCharacterName_) == true)
            {
                characterList[sCharacterName_].transform.position = position;
                characterList[sCharacterName_].transform.eulerAngles = rotation;
            }
        }

        public static void SetObjectPosition(String sObjectName_, Vector3 position, Vector3 rotation)
        {
            if (objectList.ContainsKey(sObjectName_) == true)
            {
                objectList[sObjectName_].transform.position = position;
                objectList[sObjectName_].transform.eulerAngles = rotation;
            }
        }

        public static void DisposeSectorModelList()
        {
            foreach (GameObject model in sectorModelList)
            {
                if (model != null)
                {
                    UnityEngine.Object.Destroy(model);
                }
            }

            sectorModelList.Clear();
        }

        public static void DisposeSectorModelPositionList()
        {
            sectorModelPositionList.Clear();
        }
    }
}