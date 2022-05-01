using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Data
{
    public class AppController : MonoBehaviour
    {
        [SerializeField] private DataController _dataController;
        [SerializeField] private TextMeshProUGUI _timeTargetText;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject orbPrefab;
        [SerializeField] private Transform _playerHeadPosition;
        [SerializeField] private float _buttonSpawnOffset;
        
        [SerializeField] private Vector3 _buttonRotationOffset;
        [SerializeField] private int _buttonAngle;
        [Header("Main Story Screen Elements")]
        [SerializeField] private GameObject _avatar;
        [SerializeField] private GameObject _avatarBase;
        [SerializeField] private GameObject _mobile;
        [SerializeField] private GameObject _storyboard;
        [SerializeField] private GameObject _desktop;
        [SerializeField] private GameObject _desktopTwo;
        [SerializeField] private GameObject _personaImage;
        [SerializeField] private GameObject _pauseIndicator;

        [Header("Text Fields")]
        [SerializeField] private GameObject _storytitle;
        [SerializeField] private GameObject _chaptertitle;
        
        public bool choiceButtonActive = false;
        public bool isPaused = false;
        private float timer = 0.0f;
        private bool startDemo = false;
        private bool demoStarted = false;
        private bool newChapter = false;

        private int currentChapter = 0;
        private List<ChapterData> availableChapters = new List<ChapterData>();
        
        public List<VideoPlayer> mainStoryVideoPlayers = new List<VideoPlayer>();
        public List<GameObject> DevicesWithStoppedVideosRemainingInScene = new List<GameObject>();

        private Sequence AvatarMovementSequence;
        // Start is called before the first frame update
        void Start()
        {
            
            _storytitle.GetComponentInChildren<TextMeshProUGUI>().text = _dataController.storyData.storyTitle;
            availableChapters = _dataController.storyData.chapters;
            currentChapter = 0;
            playChapter();

            OVRManager.cpuLevel = 4;
            OVRManager.gpuLevel = 4;
        }

        private void playChapter()
        {
            ChapterData chapter = availableChapters[currentChapter];
            _chaptertitle.GetComponentInChildren<TextMeshProUGUI>().text = chapter.chapterTitle;
            //enqueue videos
            Debug.Log("new Chapter started, chapter title: " + chapter.chapterTitle);
                //if it is a Decision Chapter, start the decision process
                if (chapter.type == ChapterType.Decision)
                {
                    float angle = (_buttonAngle / 360f) * Mathf.PI*2f;
                    float totalAngle = _buttonAngle * (chapter.buttons.Count - 1) / 360f * Mathf.PI*2f;
                    int i = 0;
                    GameObject buttonContainer = Instantiate(new GameObject(), _playerHeadPosition.position,
                        Quaternion.Euler(0,_playerHeadPosition.rotation.eulerAngles.y,0));
                    foreach (var choice in chapter.buttons)
                    {
                        float ButtonAngle = (totalAngle/2) + (0.25f * Mathf.PI * 2f) - i*angle;
                        GameObject button = Instantiate(buttonPrefab,buttonContainer.transform);
                        button.transform.localPosition = new Vector3(Mathf.Cos(ButtonAngle)*_buttonSpawnOffset, 0, Mathf.Sin(ButtonAngle)*_buttonSpawnOffset);
                        button.transform.LookAt(_playerHeadPosition.position);
                        button.transform.Rotate(_buttonRotationOffset,Space.Self);
                        button.GetComponentInChildren<TMP_Text>().text = choice.buttonText;
                        button.GetComponent<ButtonHover>()._appController = this;
                        
                        //Determine Button Action and assigning it
                        switch (choice.action)
                        {
                            case ButtonAction.GotoChapter :
                                button.GetComponent<ButtonHover>().m_buttonAction = () =>
                                {
                                    Debug.Log("Go To Chapter " + choice.actionPayload);
                                    GoToChapter(choice.actionPayload);
                                };
                                break;
                            case ButtonAction.StopThisStory :
                                button.GetComponent<ButtonHover>().m_buttonAction = () =>
                                {
                                    Debug.Log("Stop Story");
                                };
                                break;
                            default:
                                Debug.Log("no button action defined");
                                break;
                            
                        }
                        i++;
                    }
                }
                else if(chapter.type == ChapterType.Story)
                {
                    //else it is probably a video chapter, so handle the vids
                    foreach (var videoSnippet in chapter.videoSnippets)
                    {
                        Debug.Log("videoSnippet = " + JsonUtility.ToJson(videoSnippet));
                        StartCoroutine(EnqueueVideo( videoSnippet.startHookTimeInMS / 1000, videoSnippet.device,
                            videoSnippet.durationInMS / 1000, videoSnippet));
                        if (videoSnippet.containsVideoMovement)
                        {
                            foreach (var vid in videoSnippet.videoMovement)
                            {
                                Vector3 adjustmentPosition = Vector3.zero;
                                if (chapter.hasWorldPositionAttached) adjustmentPosition = chapter.worldPositionOffset;

                                StartCoroutine(EnqueueVideoMovement(vid, adjustmentPosition, _desktop));
                            }

                            
                        }

                        if (videoSnippet.containsPosition)
                        {
                            Debug.Log("videoSnippet.position = " + videoSnippet.position.ToString());
                            //go to target Position
                            GameObject currentVideoDevice = null;
                            switch (videoSnippet.device)
                            {
                                case Device.Mobile:
                                    currentVideoDevice = _mobile;
                                    break;
                                case Device.Desktop:
                                    currentVideoDevice = _desktop;
                                    break;
                                case Device.DesktopTwo:
                                    currentVideoDevice = _desktopTwo;
                                    break;
                                case Device.Persona:
                                    currentVideoDevice = _personaImage;
                                    break;
                                default:
                                    Debug.Log("position given but no device reference. Device.ID: " + videoSnippet.device);
                                    break;
                            }
                            Vector3 PositionAdjustment = Vector3.zero;
                            if (chapter.hasWorldPositionAttached) PositionAdjustment = chapter.worldPositionOffset;

                           currentVideoDevice.transform.position = videoSnippet.position + PositionAdjustment;
                            
                        }
                        if (videoSnippet.containsRotation)
                        {
                            //go to target Rotation
                            GameObject currentVideoDevice = null;
                            switch (videoSnippet.device)
                            {
                                case Device.Mobile:
                                    currentVideoDevice = _mobile;
                                    break;
                                case Device.Desktop:
                                    currentVideoDevice = _desktop;
                                    break;
                                case Device.DesktopTwo:
                                    currentVideoDevice = _desktopTwo;
                                    break;
                                default:
                                    Debug.Log("rotation given but no device reference. Device.ID: " + videoSnippet.device);
                                    break;
                            }
                            Vector3 RotationAdjustment = Vector3.zero;
                            if (chapter.hasWorldPositionAttached) RotationAdjustment = chapter.worldRotationOffset;
                            Quaternion baseRotation = Quaternion.identity;
                            baseRotation.eulerAngles = RotationAdjustment;
                            Quaternion targetRotation = Quaternion.Euler(videoSnippet.rotation);
                            currentVideoDevice.transform.localRotation = targetRotation;
                        }
                    }    
                } else if (chapter.type == ChapterType.WaitChapter)
                {
                    if (chapter.buttons[0].buttonType == ButtonType.hoverOrb)
                    {
                        GameObject button = Instantiate(orbPrefab);
                        Vector3 positionAdjustment = Vector3.zero;
                        if (chapter.hasWorldPositionAttached) positionAdjustment = chapter.worldPositionOffset;
                        button.transform.position = chapter.buttons[0].position + positionAdjustment;
                        //Determine Button Action and assigning it
                        button.GetComponentInChildren<OrbHover>().m_buttonAction = () =>
                                {
                                    Debug.Log("Go To Chapter " + chapter.buttons[0].actionPayload);
                                    GoToChapter(chapter.buttons[0].actionPayload);
                                };
                    }
                      
                }
                //check if there are avatar movement coordinates
                if (chapter.chapterContainsAvatarMovement)
                {
                    foreach (var am in chapter.avatarMovement)
                    {
                        StartCoroutine(EnqueueVideoMovement(am, chapter.worldPositionOffset, _avatar));
                    }
                }
        }

        IEnumerator EnqueueVideoMovement(TimedMovement movement, Vector3 adjustment, GameObject objectToMove)
        {
            var timeCount = 0f;
            
            Debug.Log("in movement enqueer targetTimeInMs = " + movement.targetTimeInMs);
            Debug.Log("targetLocation = " + movement.targetLocation + adjustment);
            while(timeCount < movement.targetTimeInMs / 1000)
            {
                if (!isPaused)
                    timeCount += Time.deltaTime;
                yield return null;
            }
            //only have desktop for now
            Debug.Log("movement executed duration: " + movement.transitionDurationInMs);
            objectToMove.transform.DOMove(movement.targetLocation + adjustment, movement.transitionDurationInMs / 1000, false).SetEase(Ease.InOutSine);
        }

        private void Update()
        {
            if (startDemo && !demoStarted)
            {
                startDemo = false;
                demoStarted = true;
                isPaused = false;
                _storytitle.GetComponentInChildren<TextMeshPro>().text = _dataController.storyData.storyTitle;
                availableChapters = _dataController.storyData.chapters;
                newChapter = true;
            }

            if (newChapter)
            {
                
                newChapter = false;
                if (availableChapters[currentChapter].type == ChapterType.Story && DevicesWithStoppedVideosRemainingInScene.Count != 0)
                {
                    foreach (var container in DevicesWithStoppedVideosRemainingInScene)
                    {
                        container.SetActive(false);
                    }
                }
                timer = 0f;
                playChapter();
                
            }
            
            if (!isPaused)
            {
                timer += Time.deltaTime;
                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer - minutes * 60);
                _timeTargetText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }
        }

        public void setIsPaused(bool paused)
        {
            isPaused = paused;
            if (isPaused)
            {
                foreach (var videoPlayer in mainStoryVideoPlayers)
                {
                    videoPlayer.Pause();
                    _pauseIndicator.SetActive(true);
                }
            }
            else
            {
                foreach (var videoPlayer in mainStoryVideoPlayers)
                {
                    if(videoPlayer.enabled) videoPlayer.Play();
                    _pauseIndicator.SetActive(false);
                }
            }
        }

            
        IEnumerator EnqueueVideo(float startTime, Device device, float stopTime, VideoSnippet storyBlock)
        {
            Debug.Log("video "+ device.ToString() +" Enqueued, startTime = "+startTime.ToString()+", stopTime = "+stopTime.ToString());
            //yield return new WaitForSeconds(startTime);
            var timeCount = 0f;
            while(timeCount < startTime)
            {
                if (!isPaused)
                    timeCount += Time.deltaTime;
                yield return null;
            }
            switch (device)
            {
                case Device.Avatar:
                    yield return PlayVideo(_avatar, stopTime, storyBlock.id, storyBlock.keepDeviceAfterVideoHasStopped);
                    break;
                case Device.Mobile:
                    yield return PlayVideo(_mobile, stopTime, storyBlock.id, storyBlock.keepDeviceAfterVideoHasStopped);
                    break;
                case Device.Desktop:
                    if(storyBlock.mainStory){
                        
                    yield return PlayVideo(_desktop, stopTime, storyBlock.id, storyBlock.keepDeviceAfterVideoHasStopped);
                    }
                    else
                    {
                        yield return PrimeVideo(storyBlock);
                    }
                    break;
                case Device.DesktopTwo:
                    yield return PlayVideo(_desktopTwo, stopTime, storyBlock.id, storyBlock.keepDeviceAfterVideoHasStopped);
                    break;
                case Device.Storyline:
                    yield return ShowImage(_storyboard, stopTime, storyBlock.id, storyBlock.keepDeviceAfterVideoHasStopped);
                    break;
                case Device.Persona:
                    yield return ShowImage(_personaImage, stopTime, storyBlock.id, storyBlock.keepDeviceAfterVideoHasStopped);
                    break;
                default:
                    Debug.Log("Incorrect Device ID, will not play video");
                    break;
            }
        }

        private IEnumerator PrimeVideo(VideoSnippet storyBlock)
        {
            GameObject container = null;
            /*
            switch (storyBlock.cloud)
            
            {
                case Cloud.Sales:
                    container = _ScreenOne;
                    break;
                case Cloud.Service:
                    container = _ScreenTwo;
                    break;
                default:
                    Debug.Log("no Cloud specified");
                    break;
            }

            if (container)
            {   //indicate video is playable
                foreach (var child in container.GetComponentsInChildren<Transform>(true))
                {
                    if (child.CompareTag("IndicatorLight"))
                    {
                        child.gameObject.SetActive(true);
                        StartCoroutine(RemoveVideoIndicator(storyBlock.durationInMS/1000,child.gameObject));
                    }
                }
                */
                yield return null;
                /* TODO: Implement dynamic loading of videos / does not work atm
                //load correct video
                VideoPlayer vp = container.GetComponentInChildren<VideoPlayer>();
                CustomRenderTexture targetTexture = new CustomRenderTexture(1920, 1080, RenderTextureFormat.Default);
                var videoMat = container.GetComponentInChildren<ImageContainer>().transform.gameObject.GetComponent<Renderer>().material;
                videoMat.mainTexture = targetTexture;
                vp.targetTexture = targetTexture;
                vp.clip = (VideoClip)Resources.Load("videos/"+storyBlock.id);
               
            }
             */
        }

        private IEnumerator PlayVideo(GameObject container, float stopTime, string videoId, bool keep)
        {
//Debug.Log("player started for "+container.name);
            container.SetActive(true);

            var videoPlayer = container.GetComponentInChildren<VideoPlayer>();
            //load correct video
    //TODO: Implement dynamic loading of videos / does not work atm
    //load correct video
    videoPlayer.clip = (VideoClip)Resources.Load("videos/"+videoId);


//register videoPlayer
mainStoryVideoPlayers.Add(videoPlayer);
videoPlayer.Play();

yield return StartCoroutine(StopVideo(stopTime, container, keep));
}


private IEnumerator StopVideo(float waitTime, GameObject container, bool keep)
{
//Debug.Log("player stop method called for "+container.name);
var timeCount = 0f;
while (timeCount < waitTime)
{
    if (!isPaused)
        timeCount += Time.deltaTime;
    yield return null;
}

//Debug.Log("player stopped for "+container.name);
var videoPlayer = container.GetComponentInChildren<VideoPlayer>();
videoPlayer.Stop();
mainStoryVideoPlayers.Remove(videoPlayer);
if (mainStoryVideoPlayers.Count == 0)
{
    //go to next chapter if there is one left
    if (currentChapter < availableChapters.Count)
    {
        currentChapter++;
        newChapter = true;
    }
}

if (keep)
{
    DevicesWithStoppedVideosRemainingInScene.Add(container);
}
else
{
    container.SetActive(false);
}
    
}

private IEnumerator ShowImage(GameObject container, float stopTime, string textureName, bool keep)
{
container.SetActive(true);
foreach (var ic in container.GetComponentsInChildren<ImageContainer>())
{
    Material mat = ic.transform.gameObject.GetComponent<Renderer>().material;
//Debug.Log("mat = " + mat.name);
    if (mat)
    {
        var tex = (Texture2D) Resources.Load("img/" + textureName);
        mat.mainTexture = tex;
    }
}


if (!keep)
{
    yield return StartCoroutine(HideImage(stopTime, container));
}
}

private IEnumerator HideImage(float stopTime, GameObject container)
{
var timeCount = 0f;
while (timeCount < stopTime)
{
    if (!isPaused)
        timeCount += Time.deltaTime;
    yield return null;
}
//container.SetActive(false);
}

private void GoToChapter(int chapterId)
{
if (!availableChapters[chapterId].Equals(null))
{
    Debug.Log("switch to next chapter #" + chapterId);
    newChapter = true;
    currentChapter = chapterId;
    return;
}
else
{
    Debug.Log("chapter with id '" + chapterId + "' not found.");
}

}
}
}
