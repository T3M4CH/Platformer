using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.Sounds.Scripts.Editor
{
    #if UNITY_EDITOR
    public class SoundCreator : MonoBehaviour
    {
        [MenuItem("Assets/Create/Sound Asset", priority = 1)]
        public static void ButtonCreateSoundAsset()
        {
            var selectedList = Selection.objects;

            if (selectedList is { Length: > 0 })
            {
                var list = new List<AudioClip>();
                for (var i = 0; i < selectedList.Length; i++)
                {
                    if (selectedList[i] is AudioClip clip)
                    {
                        list.Add(clip);
                    }
                }

                CreateAsset(list.ToArray());
            }
        }

        private static void CreateAsset(AudioClip[] clips)
        {
            var path = "Assets/Core/Sounds/Resources/SoundAssets";

            var asset = CreateSoundAsset(clips[0].name, path);
            for (var i = 0; i < clips.Length; i++)
            {
                asset.AddClip(clips[i]);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        private static SoundAsset CreateSoundAsset(string nameAssets, string path)
        {
            var asset = ScriptableObject.CreateInstance<SoundAsset>();
            ValidateDirectory(path);
            AssetDatabase.CreateAsset(asset, $"{path}/{nameAssets}.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }

        private static void ValidateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
    #endif 
}