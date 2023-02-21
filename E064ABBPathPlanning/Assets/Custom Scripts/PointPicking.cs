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
    private bool isTesting = true; // Boolean indicating whether the system is being tested on the physical ABB GoFa robot (false) or purely with simulations and virtual controllers (true)

    // These represent the UI elements on the DataPointHolder slate:
    [SerializeField]
    public TextMeshProUGUI xCoor; // x-coordinate label
    public TextMeshProUGUI yCoor; // y-coordinate label
    public TextMeshProUGUI zCoor; // z-coordinate label
    public TextMeshProUGUI savePointStatusLabel; // Label representing whether or not any more points are able to be saved + other information
    public TextMeshProUGUI coordinateSystemStatusLabel; // Label representing whether or not the coordinate system has been set
    public TextMeshProUGUI pointPickingStatusLabel; // Label representing whether or not the point-picking process is active
    public TextMeshProUGUI ptpIndexIndicator; // Label representing the PTP Index
    public TextMeshProUGUI pathViabilityLabel; // Label representing the path status/path viability
    public TextMeshProUGUI qrTrackingLabel; // Label representing the status of the QR tracker

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
    public float addThresh = 0.5f; // Addition threshold - minimum amount of time that pinching must occur in order for a new point to be added

    // Deletion settings:
    Dictionary<int, float> delDict = new Dictionary<int, float>(); // Dictionary representing the indices of each point and the most recent time that the points were modified
    List<int> del_list = new List<int>(); // List representing the indices of the points to be deleted
    private float deleteThresh = 1f; // Deletion threshold - the maximum amount of time that must occur between consecutive taps of a point in order for it to be deleted

    // Modification list:
    List<int> mod_list = new List<int>(); // List representing the indices of the point that are currently being modified

    // Coordinate system parameters:
    public GameObject QRCodeObject; // Coordinate system game object created by scanning a QR Code
    private Matrix4x4 Tr_mat = Matrix4x4.identity; // Total transformation matrix converting Unity's left-handed coordinate system to RobotStudio's right-handed coordinate system
    private Matrix4x4 LHStoRHS_mat = Matrix4x4.identity; // Placeholder matrix representing the coordinate transformation from a left-hand coordinate system to a right-hand coordinate system
    private Matrix4x4 TRS_mat = Matrix4x4.identity; // Placeholder matrix representing the left-handed transformation matrix converting Unity's coordinate system to RobotStudio's coordinate system
    private bool isCoordSysSet = false; // Boolean to indicate if the coordinate system has been set

    // Data-storage variables:
    private int num_coors = 100; // Sets the maximum number of points that are able to be created
    public Vector3[] posArr; // Instantiating array containing Cartesian position data of selected points
    public Quaternion[] rotArr; // Instantiating array containing Cartesian rotation data of selected points
    public Quaternion zeroQuart = Quaternion.identity;
    public int speedInd; // Stores the index of the selected speed value
    public int zoneInd; // Stores the index of the selected zone value
    //public string[] curPosStr = new string[3];
    public int maxLength = 12; // Maximum length of the string to represent a given coordinate


    // Offset variables: 
    public GameObject offsetMenu;

    public CoordinateSlider x_slid;
    public CoordinateSlider y_slid;
    public CoordinateSlider z_slid;
    public float x_off; // x-value of offset
    public float y_off; // y-value of offset
    public float z_off; // z-value of offset
    public Vector3 offsetVec = new Vector3(0, 0, 0); // Placeholder variable for the offset vector
    public TextMeshProUGUI offsetLabel;

    // Settings variables:
    public GameObject settingsMenu;
    public TextMeshProUGUI pathTraversalModeLabelSettings;
    public bool settingsMenuActive = false; // Boolean to indicate if the settings menu is active
    public PinchingSlider pinchingSlider; // Slider representing pinching threshold
    public AddSlider addSlider; // Slider representing addition threshold
    public DeleteSlider deleteSlider; // Slider representing deletion threshold


    private void Awake()
    {
        // Initializes variables relevant to points and their corresponding data:
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
        // This method determines whether or not the tracked hand is being pinched in accordance with the pinching threshold

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

        string[] curPosStr = new string[3];

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
        // This method, which runs every frame, determines if the pinching gesture to add a point has been performed

        if (pointPickingActive)
        {
            handPinchedprev = handPinched;
            handPinched = IsPinching(trackedHand);

            if (handPinched)
            {
                lastPinchPos = GetIndexFingerPos(); // Obtains the position of the hand when pinched

                // If the pinching motion has initiated, start the timer:
                if (!handPinchedprev)
                {
                    timeStart_add = Time.time;
                }

            }

            if (handPinchedprev && !handPinched)
            {
                timeFinish_add = Time.time; // Ends the timer of the pinching motion

                // If the pinching motion has occurred for a sufficiently long duration, add a point:
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
        // Method, which runs every frame, to apply the necessary changes to any points currently being modified, which includes updating its position and rotation as well as their connecting lines

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
        // Method, which runs every frame, to update the UI representing the PTP index

        ptpIndexIndicator.text = "PTP Index: " + ptpIndex.ToString();
        ptpIndexIndicatorminiMenu.text = "PTP Index: " + ptpIndex.ToString();

    }

    public void updatePointMaterials()
    {
        // Method, which runs every frame, to update the colors of the points 

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
            else // For all other cases, simply make the point color the default color
            {
                pointObjects[i].gameObject.GetComponent<Renderer>().material = pointMaterial;
            }
        }

    }

    // Point-Picking helper methods:
    public void togglePointPicking()
    {
        // Method to toggle the point addition process

        pointPickingActive = !pointPickingActive;
        timeStart_add = float.NaN;
        timeFinish_add = float.NaN;

        // Updating UI:
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
        // Method to add points:

        if (pointCounter < num_coors)
        {
            pointObjects[pointCounter].position = lastPinchPos; // Sets the pinched position to the location of the new point
            pointObjects[pointCounter].gameObject.SetActive(true); // Sets the game object of the point active

            // Converting coordinate units from meters to mm and storing in point array:
            if (isCoordSysSet) // If the coordinate system is set
            {
                pointObjects[pointCounter].rotation = applyInverseRotationTransformation(new Quaternion(0, 1, 0, 0));
                posArr[pointCounter] = applyCoordinateTransformation(lastPinchPos);
                rotArr[pointCounter] = applyRotationTransformation(pointObjects[pointCounter].rotation);
            }
            else // If the coordinate system is not set
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
            tcpClient.SendDataMessage(); // Sends the updated data to the server as points are added
            savePointStatusLabel.text = "Point added!"; // Updates UI
        }
        else
        {
            savePointStatusLabel.text = "Points full!"; // Indicates that all points have been selected
        }
    }

    public void deletePoint(int pointNum)
    {
        // Method to delete points:

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

                pointObjects[i].gameObject.SetActive(false); // De-activates the point game object

            }
        }

        pointCounter--; // Decrements the point counter

        // If the deleted point is prior to the current point and the next point to be traversed to, then update the ptpIndex accordingly:
        if(pointNum < (ptpIndex - 2))
        {
            ptpIndex--;
        }

        del_list.Remove(pointNum); // Removes the index of the point to be deleted from the deletion list

        tcpClient.SendDataMessage(); // Sends the updated data to the server as points are deleted
        
    }


    public void modifyingPoint(int pointNum)
    {
        // Method that runs when a point is currently being modified

        //pointObjects[pointNum].gameObject.GetComponent<Renderer>().material = pointMaterialModify;
        mod_list.Add(pointNum); // Adds the index of the modified point to the modification list

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
        // Method that runs when the modification of a point has ended

        float timeNow_del = Time.time; // Represents the most updated time that a point was modified
        
        // If the difference in time between two consecutive taps is below the deletion threshold, then delete the point:
        if((timeNow_del - delDict[pointNum] < deleteThresh) && del_list.Count == 0) // Ensures that only one point is able to be deleted at a time (instead of using a boolean representing whether or not a point is in the deletion process, a deletion list was used to allow for the possibility of multiple points being simultaneously deleted)
        {
            del_list.Add(pointNum); // Adds the index of the deleted point to the deletion list 
            deletePoint(pointNum); // Deletes the selected point
        }
        else
        {
            tcpClient.SendDataMessage(); // Sends updated data to the server as points are being modified
            delDict[pointNum] = timeNow_del; // Adds the previous time the point was modified to the deletion dictionary
        }

        mod_list.Remove(pointNum); // Removes index of the modified point from the modification list

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
        // Method to run while modifying any object that is not a point

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
        // Method to run when the modification process for any non-point object has ended

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
        // Method to clear all data

        // Sets point-picking boolean to false if initially true:
        if (pointPickingActive)
        {
            togglePointPicking();
        }

        // Deletes all line objects:
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
        
        // Updating UI:
        savePointStatusLabel.text = "";
        pathViabilityLabel.text = "Path Viability: NOT CHECKED";
        pathViabilityLabelminiMenu.text = "Path Viability: NOT CHECKED";

        tcpClient.ClearAllPoints(); // Sending message to server to indicate to clear all points
    }



    // Coordinate System + Data Methods:
    public void setCoordSys()
    {
        // Method to set the coordinate system

        // Unity's local frame - The coordinate system of the scanned QR Code
        // Unity's world frame - The coordinate system of Unity
        // Intention of this function is to transform coordinates expressed in Unity's world frame to the RobotStudio's world frame

        // Determines whether the system is being run on the virtual ABB controllers or on the real, physical ABB robot:
        if(isTesting)
        {
            QRCodeObject = GameObject.Find("QRCodeTestObject"); // Locates the previously-implemented game object to represent the origin of the new coordinate system
        }
        else
        {
            QRCodeObject = GameObject.Find("QRCode_Detected"); // Locates the scanned QR code game object
        }

        Vector3 QRPos = QRCodeObject.transform.position; // Represents the position vector of the local frame in terms of Unity's world frame
        Quaternion QRRotation = QRCodeObject.transform.rotation; // Quaternion representing the rotation of the QR code frame
        TRS_mat = Matrix4x4.TRS(QRPos, QRRotation, Vector3.one); // Creating a transformation matrix using the position and rotation of the game object representing the origin of the new coordinate system
        TRS_mat = TRS_mat.inverse; // Transformation matrix that converts from Unity's world frame to the left-handed QR code frame
        
        // Converting from left-handed to right-handed coordinate system:
        LHStoRHS_mat.SetColumn(0, new Vector4(0, 1, 0, 0));
        LHStoRHS_mat.SetColumn(1, new Vector4(1, 0, 0, 0));
        LHStoRHS_mat.SetColumn(2, new Vector4(0, 0, 1, 0));
        LHStoRHS_mat.SetColumn(3, new Vector4(0, 0, 0, 1));
       
        Tr_mat = LHStoRHS_mat * TRS_mat; // Represents the overall transformation matrix converting points from Unity's coordinate system to RobotStudio's coordinate system

        //GoFa.transform.parent = QRCodeObject.transform;
        GoFa.transform.position = QRPos;

        // Ensure that the orientation of the virtual GoFa robot corresponds with that of the coordinate system game object
        Vector3 GoFaEuler = new Vector3(QRRotation.eulerAngles.x, QRRotation.eulerAngles.y, (QRRotation.eulerAngles.z + 45));
        GoFa.transform.eulerAngles = GoFaEuler;
        GoFa.SetActive(true); // Sets the virtual GoFa robot game object to active
        
        // Sets the location of the mini-menu to the origin of the virtual GoFa robot and activates the game object:
        miniDataMenu.transform.position = GoFa.transform.position;
        miniDataMenu.SetActive(true);

        offsetMenu.SetActive(true); // Sets the game object representing the offset menu to active
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
        // Method to finalize the coordinate system transformation between Unity's world frame and RobotStudio's world frame

        TRS_mat = Matrix4x4.TRS(GoFa.transform.position, QRCodeObject.transform.rotation, Vector3.one);
        Tr_mat = LHStoRHS_mat * TRS_mat.inverse;

        // Sets relevant game objects false:
        if (!isTesting)
        {
            GoFa.SetActive(false);
            QRCodeObject.SetActive(false);
        }

        offsetMenu.SetActive(false); // Sets the game object representing the offset menu to inactive

        // Applies coordinate transformation to previously-created points:
        for (int i = 0; i < pointCounter; i++)
        {
            posArr[i] = applyCoordinateTransformation(pointObjects[i].position);
            rotArr[i] = applyRotationTransformation(pointObjects[i].rotation);
        }

        tcpClient.SendDataMessage(); // Sends updated data to the server

        coordinateSystemStatusLabel.text = "Coordinate System Set!"; // Updates UI
    }

    public Vector3 applyCoordinateTransformation(Vector3 pointPos)
    {
        // Method to apply the coordinate transformation to each point
        
        // pointPos - Input represents the position of a produced point in terms of Unity's left-hand coordinate system
        // Returns the coordinates of the point expressed in RobotStudio's right-hand coordinate system


        if (isCoordSysSet)
        {
            Vector4 pointPos_trans = Tr_mat * new Vector4(pointPos.x, pointPos.y, pointPos.z, 1); // Spatial coordinates of the transformed point in a four-dimensional vector
            pointPos = new Vector3(pointPos_trans.x, pointPos_trans.y, pointPos_trans.z); // Spatial coordinates of the transformed point in a three-dimensional vector
        }

        return pointPos;
    }

    public Quaternion applyRotationTransformation(Quaternion pointQuart)
    {
        // pointQuart - Represents the quaternion of the TCP at each point w.r.t Unity's world frame
        // Output - Represents the quaternion of the TCP at each point w.r.t RobotStudio's world frame
        
        if (isCoordSysSet)
        {
            Matrix4x4 pointRotMat = Tr_mat * Matrix4x4.Rotate(pointQuart) * Tr_mat.inverse; // Rotation matrix of the transformed point
            pointQuart = pointRotMat.rotation; // Quaternion of the transformed point

        }

        return pointQuart;
    }


    public Vector3 applyInverseCoordinateTransformation(Vector3 pointPos)
    {
        // pointPos - Input represents the position of a produced point in terms of RobotStudio's right-hand coordinate system
        // Returns the coordinates of the point expressed in Unity's left-hand coordinate system

        if (isCoordSysSet)
        {
            Vector4 pointPos_trans = Tr_mat.inverse * new Vector4(pointPos.x, pointPos.y, pointPos.z, 1); // Spatial coordinates of the inversely-transformed point stored in a four-dimensional vector
            pointPos = new Vector3(pointPos_trans.x, pointPos_trans.y, pointPos_trans.z); // Spatial coordinates of the inversely-transformed point stored in a three-dimensional vector
        }

        return pointPos;
    }


    public Quaternion applyInverseRotationTransformation(Quaternion pointQuart)
    {
        // pointQuart - Represents the quaternion of the TCP at each point w.r.t RobotStudio's world frame
        // Output - Represents the quaternion of the TCP at each point w.r.t Unity's world frame

        if (isCoordSysSet)
        {
            Matrix4x4 pointRotMat = Tr_mat.inverse * Matrix4x4.Rotate(pointQuart) * Tr_mat; // Rotation matrix of the inversely-transformed point
            pointQuart = pointRotMat.rotation; // Quaternion of the inversely-transformed point

        }

        return pointQuart;
    }


    public bool isPointInBounds(int pointNum)
    {
        // Method to determine if the given point is in bounds:

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
        // Method to convert each given spatial coordinate vector to a string

        float coorPos; // Placeholder variable representing the current axial-coordinate
        string coorStr; // Placeholder variable representing the string representation of each axial-coordinate
        string[] posStr = new string[3]; // String array representation of current position

        for (int i = 0; i < 3; i++)
        {
            coorPos = pos[i];
            coorPos *= 1000; // Converting each positional coordinate from meters to mm
            coorStr = coorPos.ToString();

            // Adjusts length of the final strings:
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
        // Method to toggle the settings menu

        if(!settingsMenuActive)
        {
            //settingsMenu.transform.localPosition = new Vector3(0.145f, -0.185f, -0.396f);
            settingsMenu.SetActive(true); // Sets the settings menu to active
        }
        else
        {
            settingsMenu.SetActive(false); // Sets the settings menu to inactive
            //settingsMenu.transform.localPosition = new Vector3(-1000, -1000, -1000);
        }

        settingsMenuActive = !settingsMenuActive;
    }

    public void toggleDominantHand()
    {
        // Method to toggle the dominant hand of the user

        if(isRightHand)
        {
            // Toggles the dominant hand to the left hand:
            trackedHand = Handedness.Left;
            isRightHand = false;

            dominantHandLabel.text = "Dominant Hand: Left"; // Updates UI
        }
        else
        {
            // Toggles the dominant hand to the right hand:
            trackedHand = Handedness.Right;
            isRightHand = true;

            dominantHandLabel.text = "Dominant Hand: Right"; // Updates UI
        }
    }

    public void setSettings()
    {
        // Method to set and store the updated settings

        PinchThreshold = pinchingSlider.pinchSens;
        addThresh = addSlider.addThresh;
        deleteThresh = deleteSlider.deleteThresh;

        toggleSettingsMenu(); // Toggles settings menu off
    }

}
