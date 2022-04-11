using System;
using System.Collections;
using System.Collections.Generic;
using identifier;
using Interactions;
using Salesforce;
using TMPro;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    [Header("Debug Shit")] public float steeringAngle;
    [SerializeField] private float steeringAnglePrev = 0f;

    [Header("Vehicle Colour Material Assignments")] 
    [SerializeField] private Material BlueDuskMetallic;
    [SerializeField] private Material GlacierWhiteMetallic;
    [SerializeField] private Material LimeYellowMetallic;
    [SerializeField] private Material KingsRedMetallic;
    [SerializeField] private Material MakenaTurquoise;
    [SerializeField] private Material MoonstoneGrey;
    [SerializeField] private Material StonewashedBlueMetallic;

    [Header("dashboardControl")] [SerializeField]
    private float hideVolumeTime = 2f;
    [Header("public variables")]
    public Boolean showHandMenu = true;

    public Boolean showGarage = false;
    
    public Boolean showWallet = false;

    public Boolean showConfigurator = false;

    public GameObject vehicle;

    public VehicleColour selectedUpperBodyColour = VehicleColour.GlacierWhiteMetallic;
    public VehicleColour selectedLowerBodyColour = VehicleColour.StonewashedBlueMetallic;
    
    [Header("Hands")]
    public Animator rightHandAnimator;
    public Animator leftHandAnimator;
    public XRController LeftHandXRC;
    public XRController RightHandXRC;

    public GameObject NFT;

    [SerializeField] private Renderer _handMenuAvatarRenderer;
    [SerializeField] private TextMeshPro _handMenuUserName;

    [Header("Salesforce")] 
    [SerializeField] private String username;
    [SerializeField] private String password;
    [Serializable]
    public enum VehicleColour
    {
        BlueDuskMetallic = 0,
        GlacierWhiteMetallic = 1,
        LimeYellowMetallic = 2,
        KingsRedMetallic = 3,
        MakenaTurquoise = 4,
        MoonstoneGrey = 5,
        StonewashedBlueMetallic = 6
    };
    
    [Serializable]
    public enum HandSide
    {
        left = 0,
        right = 1
    };
    public enum VehicleArea
    {
        upperBody = 0,
        lowerBody = 1
    };

    private Renderer vehicleBodyRenderer;
    private Boolean hideVolumeTimerRunning = false;
    private float hideVolumeTimer = 0f;
    public delegate void hideVolumeMenuAction();
    public hideVolumeMenuAction m_hideVolumeMenuAction;
    private int VolumeSetting;
    private Rigidbody _originalHandle;
    private Vector3 _originalAxis;
    private Quaternion _originalRotation;
    private Boolean steeringWheelModeOn = false;

    private Transform steeringWheelTransform;
    private Transform steeringWheelHandleTransform;
    private Transform handOnWheelTransform;

    private GameObject SteeringWheelAttachPointCopy;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(salesforceLogin());
    }

    // Update is called once per frame
    void Update()
    {
        if (hideVolumeTimerRunning)
        {
            hideVolumeTimer += Time.deltaTime;
            if (hideVolumeTimer > hideVolumeTime)
            {
                hideVolumeTimerRunning = false;
                if (m_hideVolumeMenuAction != null)
                {
                    m_hideVolumeMenuAction();
                }
                StartCoroutine(sendSalesforceMessage("vr-volume-change", VolumeSetting.ToString()));
                Debug.Log("Volume set to " + VolumeSetting);
                
            }
        }

        if (steeringWheelModeOn)
        {
            var sw_targetDir = Vector3.ProjectOnPlane(handOnWheelTransform.position, steeringWheelTransform.forward) + Vector3.Dot(steeringWheelTransform.position, steeringWheelTransform.forward) * steeringWheelTransform.forward;
            var sw_forward = Vector3.ProjectOnPlane(SteeringWheelAttachPointCopy.transform.TransformPoint(steeringWheelHandleTransform.localPosition), steeringWheelTransform.forward) + Vector3.Dot(steeringWheelTransform.position, steeringWheelTransform.forward) * steeringWheelTransform.forward;
            steeringAngle = Vector3.SignedAngle(sw_forward - steeringWheelTransform.position, sw_targetDir - steeringWheelTransform.position, steeringWheelTransform.forward);
            steeringWheelTransform.localEulerAngles = new Vector3(steeringWheelTransform.localEulerAngles.x,steeringWheelTransform.localEulerAngles.y, steeringAngle);
        }
    }

    public void HideHandMenu()
    {
        showHandMenu = false;
    }
    
    public void UnhideHandMenu()
    {
        showHandMenu = true;
    }

    public void ShowConfiguratorAction()
    {
        showConfigurator = true;
        GetComponent<configurator>().showConfigurator();
    }

    public void changeVehicleColour(VehicleColour newColour, VehicleArea area)
    {
        Material mat;
        
        switch (newColour)
        {
            case VehicleColour.MakenaTurquoise:
                mat = MakenaTurquoise;
                break;
            case VehicleColour.MoonstoneGrey:
                mat = MoonstoneGrey;
                break;
            case VehicleColour.BlueDuskMetallic:
                mat = BlueDuskMetallic;
                break;
            case VehicleColour.GlacierWhiteMetallic:
                mat = GlacierWhiteMetallic;
                break;
            case VehicleColour.KingsRedMetallic:
                mat = KingsRedMetallic;
                break;
            case VehicleColour.LimeYellowMetallic:
                mat = LimeYellowMetallic;
                break;
            case VehicleColour.StonewashedBlueMetallic:
                mat = StonewashedBlueMetallic;
                break;
            default:
                mat = null;
                break;
        }

        if (mat != null)
        {
            switch (area)
            {
                case VehicleArea.lowerBody:
                    vehicle.GetComponentInChildren<VehicleBodyLower>().gameObject.GetComponentInChildren<Renderer>().material = mat;
                    selectedLowerBodyColour = newColour;
                    break;
                case VehicleArea.upperBody:
                    vehicle.GetComponentInChildren<VehicleBodyUpper>().gameObject.GetComponentInChildren<Renderer>().material = mat;
                    selectedUpperBodyColour = newColour;
                    break;
                default:
                    break;
            }
            
        }

    }

    public void hideVolumeInfoPanel(hideVolumeMenuAction actionToPerform, int volumeSettingSelected)
    {
        hideVolumeTimer = 0f;
        hideVolumeTimerRunning = true;
        m_hideVolumeMenuAction = actionToPerform;
        VolumeSetting = volumeSettingSelected;
        
    }
    
    public void setAnimatorFlag(String flagname, Boolean state)
    {
        rightHandAnimator.SetBool(flagname, state);
    }

    public void LaunchSteeringWheelMode(HandSide hand, Rigidbody steeringWheel)
    {
        steeringWheelModeOn = true;
        steeringWheelHandleTransform = steeringWheel.transform;
        steeringWheelTransform = steeringWheelHandleTransform.parent;
        SteeringWheelAttachPointCopy = new GameObject();
        SteeringWheelAttachPointCopy.transform.parent = steeringWheelTransform.parent;
        SteeringWheelAttachPointCopy.transform.localPosition = steeringWheelTransform.localPosition;
        SteeringWheelAttachPointCopy.transform.localEulerAngles = steeringWheelTransform.localEulerAngles;
        
        if (hand == HandSide.left)
        {
            var j = leftHandAnimator.gameObject.GetComponentInChildren<ConfigurableJoint>();
            _originalHandle = j.connectedBody;
            _originalAxis = j.secondaryAxis;
            _originalRotation = _originalHandle.transform.rotation;
            
            handOnWheelTransform = _originalHandle.transform.parent;
            leftHandAnimator.SetBool("SteeringWheel", true);
            j.autoConfigureConnectedAnchor = false;
            j.connectedBody = steeringWheel;
            j.anchor = Vector3.zero;
            
        }
        else
        {
            handOnWheelTransform = rightHandAnimator.gameObject.GetComponentInChildren<ConfigurableJoint>().connectedBody.transform.parent;
            rightHandAnimator.SetBool("SteeringWheel", true);
            _originalHandle = rightHandAnimator.gameObject.GetComponentInChildren<ConfigurableJoint>().connectedBody;
            rightHandAnimator.gameObject.GetComponentInChildren<ConfigurableJoint>().connectedBody = steeringWheel;
        }
    }
    
    public void EndSteeringWheelMode(HandSide hand)
    {
        steeringWheelModeOn = false;
        if (hand == HandSide.left)
        {
            var j = leftHandAnimator.gameObject.GetComponentInChildren<ConfigurableJoint>();
            leftHandAnimator.SetBool("SteeringWheel", false);
            j.connectedBody = _originalHandle;
            j.anchor = Vector3.zero;
            //TODO put hand back into the same rotation as it was before detaching
            //SetTargetRotationInternal(j,_originalHandle.rotation, _originalRotation,Space.World);
            /*Debug.Log("rotation correction starting on " + j.gameObject.GetComponentsInChildren<Transform>()[1].name);
            Debug.Log("rotation was: " + j.gameObject.GetComponentsInChildren<Transform>()[1].rotation);
            Debug.Log("rotating by " + _originalRotation.eulerAngles);
            j.gameObject.GetComponentsInChildren<Transform>()[1].Rotate(_originalRotation.eulerAngles, Space.World);
            Debug.Log("rotation is: " + j.gameObject.GetComponentsInChildren<Transform>()[1].rotation);*/
        }
        else
        {
            rightHandAnimator.SetBool("SteeringWheel", false);
            rightHandAnimator.gameObject.GetComponentInChildren<ConfigurableJoint>().connectedBody = _originalHandle;
        }
    }
    
    static void SetTargetRotationInternal (ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
    {
        // Calculate the rotation expressed by the joint's axis and secondary axis
        var right = joint.axis;
        var forward = Vector3.Cross (joint.axis, joint.secondaryAxis).normalized;
        var up = Vector3.Cross (forward, right).normalized;
        Quaternion worldToJointSpace = Quaternion.LookRotation (forward, up);
         
        // Transform into world space
        Quaternion resultRotation = Quaternion.Inverse (worldToJointSpace);
         
        // Counter-rotate and apply the new local rotation.
        // Joint space is the inverse of world space, so we need to invert our value
        if (space == Space.World) {
            resultRotation *= startRotation * Quaternion.Inverse (targetRotation);
        } else {
            resultRotation *= Quaternion.Inverse (targetRotation) * startRotation;
        }
         
        // Transform back into joint space
        resultRotation *= worldToJointSpace;
         
        // Set target rotation to our newly calculated rotation
        joint.targetRotation = resultRotation;
    }
/*
    private void OnDrawGizmos()
    {
        
        if (steeringWheelModeOn)
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawLine(steeringWheelTransform.position, steeringWheelHandleTransform.position);
            Gizmos.DrawSphere(steeringWheelTransform.position, 0.01f);
            Gizmos.color = Color.cyan;
            var handleT = Vector3.ProjectOnPlane(SteeringWheelAttachPointCopy.transform.TransformPoint(steeringWheelHandleTransform.localPosition), steeringWheelTransform.forward) + Vector3.Dot(steeringWheelTransform.position, steeringWheelTransform.forward) * steeringWheelTransform.forward;
            
            Gizmos.DrawSphere(handleT, 0.01f);
            Gizmos.DrawLine(steeringWheelTransform.position, handleT);
            //Gizmos.DrawLine(steeringWheelTransform.position, steeringWheelTransform.position + sw_forward);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(handOnWheelTransform.position, 0.01f);
            Gizmos.DrawLine(steeringWheelTransform.position, steeringWheelTransform.position - steeringWheelTransform.forward);
            //Gizmos.DrawLine(steeringWheelTransform.position, handOnWheelTransform.position);
            
            //Gizmos.DrawLine(steeringWheelTransform.position,(steeringWheelTransform.position + steeringWheelTransform.forward));
            Gizmos.color = Color.magenta;
            Vector3 pointOnPlane =
            Vector3.ProjectOnPlane(handOnWheelTransform.position, steeringWheelTransform.forward);
            Gizmos.DrawSphere(pointOnPlane, 0.01f);
            Gizmos.DrawLine(handOnWheelTransform.position, pointOnPlane);
            Gizmos.color = Color.green;
            var targetPos = Vector3.ProjectOnPlane(handOnWheelTransform.position, steeringWheelTransform.forward) + Vector3.Dot(steeringWheelTransform.position, steeringWheelTransform.forward) * steeringWheelTransform.forward;
            Gizmos.DrawSphere(targetPos, 0.01f);
            Gizmos.DrawLine(steeringWheelTransform.position, targetPos);
        }
        
        
    }*/

    private float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis)
    {
        Vector3 right = Vector3.Cross( forward, axis);
        forward = Vector3.Cross(axis, right );
        return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * Mathf.Rad2Deg;
    }
    
    
    //Salesforce stuff
    private IEnumerator salesforceLogin()
    {
        // Get Salesforce client component
        SalesforceClient sfdcClient = GetComponent<SalesforceClient>();

        // Init client & log in
        Coroutine<bool> loginRoutine = this.StartCoroutine<bool>(
            sfdcClient.login(username, password)
        );
        yield return loginRoutine.coroutine;
        try {
            loginRoutine.getValue();
            Debug.Log("Salesforce login successful.");
        }
        catch (SalesforceConfigurationException e) {
            Debug.Log("Salesforce login failed due to invalid auth configuration");
            throw e;
        }
        catch (SalesforceAuthenticationException e) {
            Debug.Log("Salesforce login failed due to invalid credentials");
            throw e;
        }
        catch (SalesforceApiException e) {
            Debug.Log("Salesforce login failed");
            throw e;
        }

       // send login message
       Coroutine<String> routine = this.StartCoroutine<String>(
           sfdcClient.runApex("POST", "sendmessage", "{\"mid\": \"vr-login\",\"payload\": \"JustinBieber\"}", "")
       );
       yield return routine.coroutine;
       Debug.Log("send login message - result: " + routine.getValue());
       yield return getSalesforceContact();
    }

    public IEnumerator sendSalesforceMessage(String mid, String payload)
    {
        SalesforceClient sfdcClient = GetComponent<SalesforceClient>();
        String msgBody = $"{{\"mid\": \"{mid}\",\"payload\": \"{payload}\"}}";
        Coroutine<String> routine = this.StartCoroutine<String>(
            sfdcClient.runApex("POST", "sendmessage", msgBody, "")
        );
        yield return routine.coroutine;
        Debug.Log("send login message - result: " + routine.getValue());
    }

    private IEnumerator getSalesforceContact()
    {
        SalesforceClient sfdcClient = GetComponent<SalesforceClient>();
        string query = Contact.BASE_QUERY + " WHERE demo_key__c = 'Contact_02' LIMIT 1";
        Coroutine<List<Contact>> getContactCoroutine = this.StartCoroutine<List<Contact>>(
            sfdcClient.query<Contact>(query)
        );
        yield return getContactCoroutine.coroutine;
        List<Contact> c = getContactCoroutine.getValue();
        Debug.Log("contact query - name: " + c[0].Name + " - img: " + c[0].public_avatar_link__c);
        _handMenuUserName.text = c[0].Name;
        yield return DownloadImage(c[0].public_avatar_link__c);
    }
    
    private IEnumerator DownloadImage(string MediaUrl)
    {   
        //TODO: encapsulate in try and catch block
        SalesforceClient sfdcClient = GetComponent<SalesforceClient>();
        Coroutine<byte[]> rc = this.StartCoroutine<byte[]>(sfdcClient.retrieveContentVersionData(MediaUrl));
        yield return rc.coroutine;
        var b64_buffer = rc.getValue();
        var avatarTexture = new Texture2D(1, 1);
        //avatarTexture.LoadRawTextureData(b64_buffer);
        avatarTexture.LoadImage(b64_buffer);
        _handMenuAvatarRenderer.material.mainTexture = avatarTexture;
    } 
}
