using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class DataController : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        public StoryData storyData;
        private void Awake()
        {
            storyData = JsonUtility.FromJson<StoryData>(jsonFile.text);
            /*foreach (var videoSnippet in storyData.videoSnippets)
            {
                Debug.Log("videoSnippet: "+JsonUtility.ToJson(videoSnippet));
                Debug.Log("device decoded = "+videoSnippet.device.ToString());
            }*/
        }
    }
}
