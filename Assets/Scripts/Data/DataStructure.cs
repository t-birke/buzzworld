using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct StoryData
    {
        public string storyTitle;
        public string storyDescription;
        public List<ChapterData> chapters;
    }

    [Serializable]
    public struct ChapterData
    {
        public int chapterId;
        public string chapterTitle;
        public string chapterDescription;
        public string chapterImageUrl;
        public bool chapterContainsAvatarMovement;
        public ChapterType type;
        public List<Cloud> clouds;
        public List<VideoSnippet> videoSnippets;
        public List<Button> buttons;
        public List<TimedMovement> avatarMovement;
        public bool hasWorldPositionAttached;
        public Vector3 worldPositionOffset;
        public Vector3 worldRotationOffset;
    }

    [Serializable]
    public struct VideoSnippet
    {
        public string id;
        public float startHookTimeInMS;
        public float endHookTimeInMS;
        public float durationInMS;
        public Cloud cloud;
        public bool mainStory;
        public Device device;
        public bool keepDeviceAfterVideoHasStopped;
        public List<TimedMovement> videoMovement;
        public bool containsVideoMovement;
        public Vector3 position;
        public Vector3 rotation;
        public bool containsPosition;
        public bool containsRotation;
    }

    [Serializable]
    public struct Button
    {
        public ButtonType buttonType;
        public string buttonText;
        public ButtonAction action;
        public int actionPayload;
        public Vector3 position;
    }

    [Serializable]
    public enum Cloud
    {
        None = 0,
        Sales = 1,
        Service = 2,
        Marketing = 3,
        Experience = 4
    };
    [Serializable]
    public enum Device
    {
        Avatar = 0,
        Mobile = 1,
        Desktop = 2,
        Tablet = 3,
        Storyline = 4,
        Persona = 5,
        DesktopTwo = 6
    };

    [Serializable]
    public enum ChapterType
    {
        Story = 0,
        Decision = 1,
        AvatarMovement = 2,
        WaitChapter = 3
    }

    [Serializable]
    public enum ButtonAction
    {
        GotoChapter = 0,
        StopThisStory = 1
    }
    
    [Serializable]
    public struct TimedMovement
    {
        public float targetTimeInMs;
        public float transitionDurationInMs;
        public Vector3 targetLocation;
    }

    [Serializable]
    public enum ButtonType
    {
        hoverPill = 0,
        hoverOrb = 1
    }
}