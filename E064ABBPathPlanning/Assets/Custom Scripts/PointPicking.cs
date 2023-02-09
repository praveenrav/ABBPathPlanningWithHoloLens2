using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

public class PointPicking : MonoBehaviour
{
    private bool isTesting = true; // Boolean indicating whether the system is being tested on the physical ABB GoFa robot (false) or purely with simulations (true)

    // These represent the UI elements on the DataPointHolder slate:
    [SerializeField]
    public TextMeshProUGUI xCoor;
    public TextMeshProUGUI yCoor;
    public TextMeshProUGUI zCoor; // z coordinate
    public TextMeshProUGUI savePointStatusLabel; // Label representing whether or not any more points are able to be saved + other information
    public TextMeshProUGUI coordinateSystemStatusLabel;
    public TextMeshProUGUI pointPickingStatusLabel; // Label representing whether or not the point-picking process is active
    public TextMeshProUGUI ptpIndexIndicator;
    public TextMeshProUGUI pathViabilityLabel;
    public TextMeshProUGUI qrTrackingLabel;

    // These represent the GameObjects and UI elements on the mini-menu:
    public GameObject miniDataMenu;
    public TextMeshPro pointPickingStatusLabelminiMenu;
    public TextMeshPro ptpIndexIndicatorminiMenu;
    public TextMeshPro pathViabilityLabelminiMenu;

    [SerializeField]
    public GameObject PointContainerObject;
    private Transform[] pointObjects;
    private GameObject[] lineObjects = new GameObject[100]; // GameObject array storing all lines connecting consecutive points
    public Material pointMaterial; // Material assigned to a general point in the path
    public Material pointMaterialModify; // Material assigned to a point when it is currently being modified
    public Material pointMaterialOOB; // Material assigned to a point when out of bounds (OOB)
    public Material pointMaterialPrev; // Material assigned to the previous point in the path
    public Material pointMaterialCur; // Material assigned to the current point in the path
    public Material pointMaterialNext; // Material assigned to the next point in the path
    public Material lineMaterial; // Material assigned to the lines connecting consecutive points

    public GameObject pointPrefab; // Prefab representing a point object
    public GameObject qrCodePrefab; // Prefab representing the QR Code Object
    public GameObject GoFa; // Prefab representing an accurately-scaled virtual version of the ABB GoFa robot

    ////////// TCP Client Settings /////////////
    public TCPClient tcpClient;

    ////////// Point-Picking Settings //////////

    ///// Hand settings:
    ///
    // Determines which hand(s) are tracked for pinching
    [SerializeField]
    private Handedness trackedHand = Handedness.Right;
    private bool isRightHand = true;
    public TextMeshProUGUI dominantHandLabel;

    // Tracked hand joint:
    [SerializeField]
    private TrackedHandJoint trackedHandJoint = TrackedHandJoint.IndexTip;

    // Pinch threshold - feel free to change threshold value to alter pinch sensitivity
    [SerializeField]
    public float PinchThreshold = 0.7f;

    MixedRealityPose pose;
    public Vector3 curIndexPos; // Vector indicating the current position of the right hand as it's pinching
    public Vector3 lastPinchPos; // Vector indicating the most recent position of the right hand when it was pinching
    public Vector3 nullPinchPos = Vector3.zero; // Vector that is returned when the tracked hand is not able to be detected within the HoloLen's frame
    private bool handPinched = false; // Boolean to indicate whether or not the hand is currently being pinched
    private bool handPinchedprev = false; // Boolean to indicate the previous hand-pinching state

    ///// Point settings:
    public int ptpIndex = 1; // Represents RAPID index of the next point to be traversed to in point-to-point mode
    public int pointCounter = 0; // Counter to indicate which point object must be activated next
    private bool pointPickingActive = false; // Boolean to indicate if the point-picking process is active
    private bool pointPickingActivePrev = false; // Boolean to indicate if the point-picking process was previously active PRIOR to modifying a certain point

    // Add settings:
    private float timeStart_add = float.NaN; // Dictates the time when pinching first starts
    private float timeFinish_add = float.NaN; // Dictates the time when pinching ends
    public float addThresh = 0.5f;

    // Deletion settings:
    Dictionary<int, float> delDict = new Dictionary<int, float>();
    List<int> del_list = new List<int>();
    private float deleteThresh = 1f;

    // Modification list:
    List<int> mod_list = new List<int>();

    // Coordinate system parameters:
    public GameObject QRCodeObject; // Coordinate system game object created by scanning a QR Code
    private Matrix4x4 Tr_mat = Matrix4x4.identity; // Total transformation matrix
    private Matrix4x4 LHStoRHS_mat = Matrix4x4.identity;
    private Matrix4x4 TRS_mat = Matrix4x4.identity; // 
    private bool isCoordSysSet = false; // Boolean to indicate if the coordinate system has been set

    // Data-storage variables:
    private int num_coors = 100;
    public Vector3[] posArr; // Instantiating array containing Cartesian position data of selected points
    public Quaternion[] rotArr; // Instantiating array containing Cartesian rotation data of selected points
    public Quaternion zeroQuart = Quaternion.identity;
    public int speedInd;
    public int zoneInd;
    public string[] curPosStr = new string[3];
    public int maxLength = 12;


    // Offset variables: 
    public GameObject offsetMenu;

    public CoordinateSlider x_slid;
    public CoordinateSlider y_slid;
    public CoordinateSlider z_slid;
    public float x_off;
    public float y_off;
    public float z_off;
    public Vector3 offsetVec = new Vector3(0, 0, 0);
    public TextMeshProUGUI offsetLabel;

    // Settings variables:
    public GameObject settingsMenu;
    public TextMeshProUGUI pathTraversalModeLabelSettings;
    public bool settingsMenuActive = false; // Boolean to indicate if the settings menu is active
    public PinchingSlider pinchingSlider;
    public AddSlider addSlider;
    public DeleteSlider deleteSlider;


    private void Awake()
    {
        posArr = new Vector3[num_coors];
        rotArr = new Quaternion[num_coors];

        pointObjects = new Transform[num_coors];

        for (int i = 0; i < num_coors; i++)
        {
            delDict.Add(i, 0);
            pointObjects[i] = PointContainerObject.transform.GetChild(i); // Adding each point to pointObjects array
        }
        

    }

    // Start is called before the first frame update
    void Start()
    {
        offsetMenu.SetActive(false);
        miniDataMenu.SetActive(false);
        settingsMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        updateCurLoc();
        updatePTPIndexUI();
        updateAddNewPoint();
        updateModifiedPoint();
        updatePointMaterials();
    }

    // Hand-Gesture Methods:
    public bool IsPinching(Handedness trackedHand)
    {
        if (HandJointUtils.TryGetJointPose(trackedHandJoint, trackedHand, out pose))
        {
            return HandPoseUtils.CalculateIndexPinch(trackedHand) > PinchThreshold;
        }
        else
        {
            return false;
        }
    }

    private Vector3 GetIndexFingerPos()
    {
        // This method obtains the position of the hand's index finger when pinching is active

        if (HandJointUtils.TryGetJointPose(trackedHandJoint, trackedHand, out pose))
        {
            return pose.Position;
        }
        else
        {
            return nullPinchPos;
        }
    }

    // Update Methods (any methods beginning with "update" are methods that are run in Unity's Update() method):
    public void updateCurLoc()
    {
        // This method updates the current location of the index finger of the right hand

        curIndexPos = GetIndexFingerPos();

        if (!isCoordSysSet)
        {
            curPosStr = VectorToString(curIndexPos, maxLength);
        }
        else
        {
            curPosStr = VectorToString(applyCoordinateTransformation(curIndexPos), maxLength);
        }

        xCoor.text = "X: " + curPosStr[0];
        yCoor.text = "Y: " + curPosStr[1];
        zCoor.text = "Z: " + curPosStr[2];

    }

    public void updateAddNewPoint()
    {
        if (pointPickingActive)
        {
            handPinchedprev = handPinched;
            handPinched = IsPinching(trackedHand);

            if (handPinched)
            {
                lastPinchPos = GetIndexFingerPos(); // Obtains the position of the hand when pinched

                if (!handPinchedprev)
                {
                    timeStart_add = Time.time;
                }

            }

            if (handPinchedprev && !handPinched)
            {
                timeFinish_add = Time.time;

                if ((timeFinish_add - timeStart_add) > addThresh)
                {
                    if (lastPinchPos != nullPinchPos) // Ensures that a point is not added when hand leaves the screen
                    {
                        addNewPoint();
                    }
                }

            }
        }
    }

    public void updateModifiedPoint()
    {
        if(mod_list.Count > 0)
        {
            foreach (var i in mod_list)
            {

                // Updating the new position + associated UI:
                if (!isCoordSysSet)
                {
                    posArr[i] = pointObjects[i].position;
                    rotArr[i] = pointObjects[i].rotation;
                }
                else
                {
                    posArr[i] = applyCoordinateTransformation(pointObjects[i].position);
                    rotArr[i] = applyRotationTransformation(pointObjects[i].rotation);
                }


                // Updating the line in back:
                if (i != 0)
                {
                    lineObjects[i - 1].GetComponent<LineRenderer>().SetPosition(1, pointObjects[i].position);
                }

                // Updating the line in front:
                if (i != (pointCounter - 1))
                {
                    lineObjects[i].GetComponent<LineRenderer>().SetPosition(0, pointObjects[i].position);
                }

            }
        }
    }

    public void updatePTPIndexUI()
    {
        ptpIndexIndicator.text = "PTP Index: " + ptpIndex.ToString();
        ptpIndexIndicatorminiMenu.text = "PTP Index: " + ptpIndex.ToString();

    }

    public void updatePointMaterials()
    {
        // pointCounter - represents the index of the next point to be added
        // ptpIndex - represents the index of the next point to be added PLUS 1

        for(int i = 0; i < pointCounter; i++)
        {
            if(mod_list.Contains(i)) // If point material is being modified
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterialModify;
            }
            else if (!isPointInBounds(i)) // If point is out of bounds
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterialOOB;
            }
            else if (i == ptpIndex - 3 && (ptpIndex - 3) >= 0) // Previous point in the path
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterialPrev;
            }
            else if (i == ptpIndex - 2 && (ptpIndex - 2) >= 0) // Current point in the path
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterialCur;
            }
            else if (i == ptpIndex - 1 && (ptpIndex - 1) >= 0) // Next point in the path
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterialNext;
            }
            else
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterial;
            }
        }

    }

    // Point-Picking helper methods:
    public void togglePointPicking()
    {
        pointPickingActive = !pointPickingActive;
        timeStart_add = float.NaN;
        timeFinish_add = float.NaN;

        if (pointPickingActive)
        {
            pointPickingStatusLabel.text = "Point Selection: Active";
            pointPickingStatusLabelminiMenu.text = "Point Selection: Active";
        }
        else
        {
            pointPickingStatusLabel.text = "Point Selection: Inactive";
            pointPickingStatusLabelminiMenu.text = "Point Selection: Inactive";
        }
    }

    public void addNewPoint()
    {
        if (pointCounter < num_coors)
        {
            pointObjects[pointCounter].position = lastPinchPos;
            pointObjects[pointCounter].gameObject.SetActive(true);

            // Converting coordinate units from meters to mm and storing in point array:
            if (isCoordSysSet)
            {
                pointObjects[pointCounter].rotation = applyInverseRotationTransformation(new Quaternion(0, 1, 0, 0));
                posArr[pointCounter] = applyCoordinateTransformation(lastPinchPos);
                rotArr[pointCounter] = applyRotationTransformation(pointObjects[pointCounter].rotation);
            }
            else
            {
                //pointObjects[pointCounter].eulerAngles = new Vector3(180f, 0f, 0f);
                posArr[pointCounter] = lastPinchPos;
                rotArr[pointCounter] = pointObjects[pointCounter].rotation;
            }

            // Determines whether or not the point is in bounds and applies the color accordingly:
            if(isPointInBounds(pointCounter))
            {
                pointObjects[pointCounter].gameObject.GetComponent<Renderer>().material = pointMaterial;
            }
            else
            {
                pointObjects[pointCounter].gameObject.GetComponent<Renderer>().material = pointMaterialOOB;
            }

            // Draws lines connecting consecutive points
            if (pointCounter != 0)
            {
                GameObject connectingLine = new GameObject("Line " + pointCounter.ToString(), typeof(LineRenderer));
                connectingLine.GetComponent<LineRenderer>().startWidth = (float)0.01;
                connectingLine.GetComponent<LineRenderer>().endWidth = (float)0.01;
                connectingLine.GetComponent<LineRenderer>().SetPosition(0, pointObjects[pointCounter - 1].position);
                connectingLine.GetComponent<LineRenderer>().SetPosition(1, pointObjects[pointCounter].position);
                connectingLine.GetComponent<Renderer>().material = lineMaterial;

                lineObjects[pointCounter - 1] = connectingLine;
            }

            pointCounter++; // Incrementing the point counter
            tcpClient.SendDataMessage();
            savePointStatusLabel.text = "Point added!";
        }
        else
        {
            savePointStatusLabel.text = "Points full!"; // Indicates that all points have been selected
        }
    }

    public void deletePoint(int pointNum)
    {
        // (pointCounter - 1) represents the index of the LAST point that has been added
        // This implies that at any given time, pointCounter represents the index of the NEXT point to be added

        // Prevents the point that the robot is at from being deleted:
        if(pointNum == (ptpIndex - 2))
        {
            del_list.Remove(pointNum);
            return;
        }

        for (int i = pointNum; i < (pointCounter); i++)
        {
            if (i != (pointCounter - 1)) // Conditional statement that tests if the selected point is the most recently-added point
            {
                // Updating line objects (line objects are updated by updating the line in BACK):
                if (i == pointNum)
                {
                    if (pointNum != 0)
                    {
                        lineObjects[i - 1].GetComponent<LineRenderer>().SetPosition(1, pointObjects[i+1].position);
                    }
                }
                else
                {
                    lineObjects[i - 1].GetComponent<LineRenderer>().SetPosition(0, pointObjects[i].position);
                    lineObjects[i - 1].GetComponent<LineRenderer>().SetPosition(1, pointObjects[i+1].position);
                }

                // Updating positions and rotations of point objects and the data in their respective arrays:
                pointObjects[i].position = pointObjects[i+1].position;
                pointObjects[i].rotation = pointObjects[i+1].rotation;                
                posArr[i] = posArr[i + 1];
                rotArr[i] = rotArr[i + 1];

                // Updating delete dictionary:
                delDict[i] = delDict[i + 1];
            }
            else // Operations applied to the last added point:
            {
                if (pointCounter != 1) // Determines whether or not only one point is remaining
                {
                    Destroy(lineObjects[i - 1].gameObject);
                    lineObjects[i - 1] = null;
                }

                pointObjects[i].gameObject.SetActive(false);

            }
        }

        pointCounter--; // Decrements the point counter

        // If the deleted point is prior to the current point and the next point to be traversed to, then update the ptpIndex accordingly:
        if(pointNum < (ptpIndex - 2))
        {
            ptpIndex--;
        }

        del_list.Remove(pointNum);

        tcpClient.SendDataMessage();
        
    }


    public void modifyingPoint(int pointNum)
    {
        pointObjects[pointNum].gameObject.GetComponent<Renderer>().material = pointMaterialModify;
        mod_list.Add(pointNum);

        // Temporarily disables point-picking:
        if (pointPickingActive)
        {
            pointPickingActive = false;
            pointPickingActivePrev = true;
            timeStart_add = float.NaN;
            timeFinish_add = float.NaN;
        }

    }

    public void endModifyingPoint(int pointNum)
    {
        float timeNow_del = Time.time; // Represents the most updated time that a point was modified
        
        if((timeNow_del - delDict[pointNum] < deleteThresh) && del_list.Count == 0) // Ensures that only one point is able to be deleted at a time
        {
            
            del_list.Add(pointNum);
            deletePoint(pointNum);
        }
        else
        {
            tcpClient.SendDataMessage();
            delDict[pointNum] = timeNow_del;
        }

        mod_list.Remove(pointNum);

        // Re-enables point-picking if it was previously disabled AND if no other points are currently being modified:
        if (pointPickingActivePrev && mod_list.Count == 0)
        {
            pointPickingActive = true;
            pointPickingActivePrev = false;
            timeStart_add = float.NaN;
            timeFinish_add = float.NaN;
        }

    }

    // The following two methods are included in order to ensure that no points are unintentionally registered when manipulating any GameObjects:
    public void modifyingObject()
    {
        // Temporarily disables point-picking:
        if (pointPickingActive)
        {
            pointPickingActive = false;
            pointPickingActivePrev = true;
            timeStart_add = float.NaN;
            timeFinish_add = float.NaN;
        }
    }

    public void endModifyingObject()
    {

        // Re-enables point-picking if it was previously disabled:
        if (pointPickingActivePrev)
        {
            pointPickingActive = true;
            pointPickingActivePrev = false;
            timeStart_add = float.NaN;
            timeFinish_add = float.NaN;
        }

    }


    public void ClearPoints()
    {

        // Sets point-picking boolean to false if initially true:
        if (pointPickingActive)
        {
            togglePointPicking();
        }

        for (int i = 0; i < (pointCounter - 1); i++)
        {
            Destroy(lineObjects[i].gameObject);
            lineObjects[i] = null;
        }

        ptpIndex = 1; // Resetting ptpIndex
        pointCounter = 0; // Resetting point counter

        for (int i = 0; i < pointObjects.Length; i++)
        {
            posArr[i] = Vector3.zero; // Resetting position coordinates
            rotArr[i] = Quaternion.identity; // Resetting rotation coordinates

            // Resetting position of spheres and setting them inactive:
            pointObjects[i].position = Vector3.zero;
            pointObjects[i].gameObject.SetActive(false);


        }

        savePointStatusLabel.text = "";
        pathViabilityLabel.text = "Path Viability: NOT CHECKED";
        pathViabilityLabelminiMenu.text = "Path Viability: NOT CHECKED";

        tcpClient.ClearAllPoints(); // Sending message to server to indicate to clear all points
    }



    // Coordinate System + Data Methods:
    public void setCoordSys()
    {
        // Unity's local frame - The coordinate system of the scanned QR Code
        // Unity's world frame - The coordinate system of Unity
        // Intention of this function is to transform coordinates expressed in Unity's world frame to the RobotStudio's world frame

    
        if(isTesting)
        {
            QRCodeObject = GameObject.Find("QRCodeTestObject");
        }
        else
        {
            QRCodeObject = GameObject.Find("QRCode_Detected");
        }

        Vector3 QRPos = QRCodeObject.transform.position; // Represents the position vector of the local frame in terms of Unity's world frame
        Quaternion QRRotation = QRCodeObject.transform.rotation; // Quaternion representing the rotation of the QR code frame
        TRS_mat = Matrix4x4.TRS(QRPos, QRRotation, Vector3.one);
        TRS_mat = TRS_mat.inverse; // Transformation matrix that converts from Unity's world frame to the left-handed QR code frame
        
        // Converting from left-handed to right-handed coordinate system:
        LHStoRHS_mat.SetColumn(0, new Vector4(0, 1, 0, 0));
        LHStoRHS_mat.SetColumn(1, new Vector4(1, 0, 0, 0));
        LHStoRHS_mat.SetColumn(2, new Vector4(0, 0, 1, 0));
        LHStoRHS_mat.SetColumn(3, new Vector4(0, 0, 0, 1));
       
        Tr_mat = LHStoRHS_mat * TRS_mat;

        //GoFa.transform.parent = QRCodeObject.transform;
        GoFa.transform.position = QRPos;

        Vector3 GoFaEuler = new Vector3(QRRotation.eulerAngles.x, QRRotation.eulerAngles.y, (QRRotation.eulerAngles.z + 45));
        GoFa.transform.eulerAngles = GoFaEuler;
        GoFa.SetActive(true);
        miniDataMenu.transform.position = GoFa.transform.position;
        miniDataMenu.SetActive(true);

        offsetMenu.SetActive(true);
        isCoordSysSet = true; // Sets the coordinate system transformation boolean to true
    }


    public void updateOffset(int coor_change)
    {
        // If coor_change equals 0, the x-coordinate is being updated
        // If coor_change equals 1, the y-coordinate is being updated
        // If coor_change equals 2, the z-coordinate is being updated

        QRCodeObject.SetActive(true);

        if (coor_change == 0)
        {
            x_off = x_slid.coorVal;
        }
        else if (coor_change == 1)
        {
            y_off = y_slid.coorVal;
        }
        else if (coor_change == 2)
        {
            z_off = z_slid.coorVal;
        }

        // Updating offset vector:
        offsetVec.x = x_off;
        offsetVec.y = y_off;
        offsetVec.z = z_off;

        // Updating offset label:
        string[] offsetVec_str = VectorToString(offsetVec, maxLength);
        offsetLabel.text = "Offset: (" + offsetVec_str[0] + ", " + offsetVec_str[1] + ", " + offsetVec_str[2] + ")";

        // Updating GoFa robot position:
        GoFa.transform.position = applyInverseCoordinateTransformation(offsetVec);
    }

    public void finalizeCoorSys()
    {
        TRS_mat = Matrix4x4.TRS(GoFa.transform.position, QRCodeObject.transform.rotation, Vector3.one);
        Tr_mat = LHStoRHS_mat * TRS_mat.inverse;

        // Sets relevant game objects false:
        if (!isTesting)
        {
            GoFa.SetActive(false);
            QRCodeObject.SetActive(false);
        }

        offsetMenu.SetActive(false);
        
        // Applies coordinate transformation to previously-created points:
        for(int i = 0; i < pointCounter; i++)
        {
            posArr[i] = applyCoordinateTransformation(pointObjects[i].position);
            rotArr[i] = applyRotationTransformation(pointObjects[i].rotation);
        }

        tcpClient.SendDataMessage();

        coordinateSystemStatusLabel.text = "Coordinate System Set!";
    }

    public Vector3 applyCoordinateTransformation(Vector3 pointPos)
    {
        // pointPos - Input represents the position of a produced point in terms of Unity's left-hand coordinate system
        // Returns the coordinates of the point expressed in RobotStudio's right-hand coordinate system


        if (isCoordSysSet)
        {
            Vector4 pointPos_trans = Tr_mat * new Vector4(pointPos.x, pointPos.y, pointPos.z, 1); // In Unity coordinates
            pointPos = new Vector3(pointPos_trans.x, pointPos_trans.y, pointPos_trans.z);
        }

        return pointPos;
    }

    public Quaternion applyRotationTransformation(Quaternion pointQuart)
    {
        // pointQuart - Represents the quaternion of the TCP at each point w.r.t Unity's world frame
        // Output - Represents the quaternion of the TCP at each point w.r.t RobotStudio's world frame
        
        if (isCoordSysSet)
        {
            Matrix4x4 pointRotMat = Tr_mat * Matrix4x4.Rotate(pointQuart) * Tr_mat.inverse;
            pointQuart = pointRotMat.rotation;

        }

        return pointQuart;
    }


    public Vector3 applyInverseCoordinateTransformation(Vector3 pointPos)
    {
        // pointPos - Input represents the position of a produced point in terms of RobotStudio's right-hand coordinate system
        // Returns the coordinates of the point expressed in Unity's left-hand coordinate system

        if (isCoordSysSet)
        {
            Vector4 pointPos_trans = Tr_mat.inverse * new Vector4(pointPos.x, pointPos.y, pointPos.z, 1); // Converting vector to Unity coordinates before applying inverse transform
            pointPos = new Vector3(pointPos_trans.x, pointPos_trans.y, pointPos_trans.z);
        }

        return pointPos;
    }


    public Quaternion applyInverseRotationTransformation(Quaternion pointQuart)
    {
        // pointQuart - Represents the quaternion of the TCP at each point w.r.t RobotStudio's world frame
        // Output - Represents the quaternion of the TCP at each point w.r.t Unity's world frame

        if (isCoordSysSet)
        {
            Matrix4x4 pointRotMat = Tr_mat.inverse * Matrix4x4.Rotate(pointQuart) * Tr_mat;
            pointQuart = pointRotMat.rotation;

        }

        return pointQuart;
    }


    public bool isPointInBounds(int pointNum)
    {
        // Bounds variables (in meters):
        float xb_low = -0.5f;
        float xb_high = 1f;
        float yb_low = -1f;
        float yb_high = 1f;
        float zb_low = -0.5f;
        float zb_high = 3f;

        float x = posArr[pointNum].x;
        float y = posArr[pointNum].y;
        float z = posArr[pointNum].z;

        return (x > xb_low && x < xb_high) && (y > yb_low && y < yb_high) && (z > zb_low && z < zb_high);
    }

    public string[] VectorToString(Vector3 pos, int maxLen)
    {
        float coorPos;
        string coorStr;
        string[] posStr = new string[3]; // String array representation of current position

        for (int i = 0; i < 3; i++)
        {
            coorPos = pos[i];
            coorPos *= 1000; // Converting each positional coordinate from meters to mm
            coorStr = coorPos.ToString();

            if (coorStr.Length <= maxLen)
            {
                posStr[i] = coorStr;
            }

            else if (coorPos < 0)
            {
                posStr[i] = coorStr.Substring(0, (maxLen + 1));
            }
            else
            {
                posStr[i] = coorStr.Substring(0, maxLen);
            }
        }

        return posStr;
    }

    public void toggleSettingsMenu()
    {
        if(!settingsMenuActive)
        {
            //settingsMenu.transform.localPosition = new Vector3(0.145f, -0.185f, -0.396f);
            settingsMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
            //settingsMenu.transform.localPosition = new Vector3(-1000, -1000, -1000);
        }

        settingsMenuActive = !settingsMenuActive;
    }

    public void toggleDominantHand()
    {
        if(isRightHand)
        {
            trackedHand = Handedness.Left;
            isRightHand = false;

            dominantHandLabel.text = "Dominant Hand: Left";
        }
        else
        {
            trackedHand = Handedness.Right;
            isRightHand = true;

            dominantHandLabel.text = "Dominant Hand: Right";
        }
    }

    public void setSettings()
    {
        PinchThreshold = pinchingSlider.pinchSens;
        addThresh = addSlider.addThresh;
        deleteThresh = deleteSlider.deleteThresh;

        toggleSettingsMenu();
    }

}
