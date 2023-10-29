using System;
using TheWarriors;
using UnityEngine;

public class Initialize : MonoBehaviour
{
	[SerializeField]
	public GameObject _characterParent;

	[SerializeField]
	public GameObject _objectParent;

	[SerializeField]
	public GameObject _levelParent;

	[SerializeField]
	public GameObject _sectorsParent;

	void Awake()
	{
		// The Warriors, Playstation 2 - Released Sep 25 2005, 20:25:16

		if (!ConfigJsonManager.ReadConfigJson())
		{
			throw new Exception("*** Error: Failed to read config Json file.");
		}

		if (!LevelJsonManager.ReadLevelJson())
		{
			throw new Exception("*** Error: Failed to read level Json file.");
		}

		if (!RockstarArchiveManager.OpenArchives())
		{
			throw new Exception("*** Error: Failed to open file archives.");
		}

		if (!RockstarMetadataManager.CreateMetadata())
		{
			RockstarArchiveManager.CloseArchiveFiles();

			throw new Exception("*** Error: Failed to create file metadata.");
		}

		if (!UnitySceneManager.LoadLevel("level80", _levelParent, _sectorsParent))
        {
			Debug.Log("*** Error: LoadLevel failed.");
        }

		UnitySceneManager.LoadCharacter("warr_aj", _characterParent);
		UnitySceneManager.SetCharacterPosition("warr_aj", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, -90f));

		//UnitySceneManager.LoadObject("boltcutters", _objectParent);
		//UnitySceneManager.SetObjectPosition("boltcutters", new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 270f));
	}
}
