using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class CameraControls : MonoBehaviour
{

    //public float smoothTime = 0.3f;
    float smoothing; //= 5f;

    public Camera camera;
    public Button rotateLeftButton;
    public Button rotateRightButton;
    public Button panButtonUp;
    public Button panDownButton;
    public Button panLeftButton;
    public Button panRightButton;
    public Button zoomButton;
    public Button overheadButton;
    public Button returnButton; //goes to active player if is one, if not to center of map
    //public Button hideButton;
    int zoomInt;
    int overheadInt;
    int rotateInt;
    int cameraMove;
    int cameraRotate;
    int cameraSize;
    int cameraMinX;
    int cameraMaxX;
    int cameraMinZ;
    int cameraMaxZ;

    Vector3 cameraDefaultOffset;
    //Vector3 cameraDefaultOffset2;
    Vector3 cameraDefaultRotation;
    float cameraDefaultSize;
    Vector3 cameraMoveOffset;
    Transform cameraMoveRotation;
    float cameraMoveSize;
    //Camera.mainCamera.gameObject.transform.position

    //click stuff
    float timerForDoubleClick = 0.0f;
    float delay = 0.6f;
    bool isDoubleClick = false;
    int countedClicks = 0;

    float timeForHold = 0.0f;
    float holdDelay = 0.3f;
    bool isHold = false;

    int priorMapTileIndex = 1919;

    void Start()
    {
        cameraDefaultOffset = new Vector3(-6.0f, 15.0f, -15.0f);//new Vector3(1.0f, 15.0f, -24.0f);
        //cameraDefaultOffset2 = new Vector3(3f, 15.0f, -15.0f);
        cameraDefaultRotation = new Vector3(40, 25, 0);//new Vector3(30, 0, 0);
        //cameraDefaultRotation.eulerAngles = new Vector3(30, 0, 0);
        cameraMoveOffset = new Vector3(-6.0f, 15.0f, -15.0f);//new Vector3(1.0f, 15.0f, -24.0f);
        cameraMoveRotation = camera.gameObject.transform;
        cameraMoveRotation.eulerAngles = new Vector3(40, 25, 0);//new Vector3(30, 0, 
        cameraDefaultSize = 5.0f; //4.5f;
        cameraMoveSize = 5.0f; //4.5f;

        cameraMove = 0;
        cameraRotate = 0;
        cameraSize = 0;

        zoomInt = 0;
        overheadInt = 0;
        rotateInt = 0;

        cameraMinX = -50;
        cameraMinZ = -50;
        cameraMaxX = 120;
        cameraMaxZ = 120;
        smoothing = 5f;
    }

    void CheckForObjectAndMoveCamera()
    {
        //Debug.Log("adsf " + zoomInt);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;

            Tile tile = hitObject.GetComponent<Tile>();
            if( tile != null)
            {
                UserMoveToTile(hitObject.transform.position);
            }

            //MapTileObject target = hitObject.GetComponent<MapTileObject>();
            //PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
            ////check for map tile hit
            //if (target != null)
            //{
            //    int zIndex = target.GetIndex();
            //    UserMoveToMapTile(zIndex);
            //}
            //else if (puTarget != null)
            //{
            //    int zIndex = PlayerManager.Instance.GetUnitMapTileIndex(puTarget.GetUnitId());
            //    UserMoveToMapTile(zIndex);
            //}
        }
        //Debug.Log("adsf " + zoomInt);
    }

    void SetValidTarget(int zIndex)
    {
        //if (MapTileManager.Instance.CheckMovableTile(zIndex)) //is a valid move tile
        //{
        //    //zIndex is good, stays the same
        //}
        //else
        //{ //get nearest movable tile, have to click on board for this to work
        //    zIndex = MapTileManager.Instance.GetNearestMovableTile(zIndex);
        //}

        //if (zIndex != 1919)
        //{
        //    CalculationAT.ConfirmActionPhase(zIndex);
        //}
    }

    int GetMapTileIndex()
    {
        int zIndex = 1919;
        //if (Input.GetMouseButtonDown(0) || isHold)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        GameObject hitObject = hit.transform.gameObject;
        //        //check for map tile hit
        //        MapTileObject target = hitObject.GetComponent<MapTileObject>();
        //        if (target != null)
        //        {
        //            zIndex = target.GetIndex();
        //            SceneCreate.targetPanel.SetTile(MapTileManager.Instance.GetMapTileByIndex(zIndex));
        //        }
        //        else //check for player target
        //        {
        //            PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
        //            if (puTarget != null)
        //            {
        //                zIndex = PlayerManager.Instance.GetUnitMapTileIndex(puTarget.GetUnitId());
        //                SceneCreate.targetPanel.SetActor(PlayerManager.Instance.GetPlayerUnit(puTarget.GetUnitId()));
        //            }
        //        }
        //    }
        //}

        return zIndex;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                //SoundManager.Instance.PlaySoundClip(0);
            }
            return;
        }
        HandleDoubleClick();

       
    }

    void SetValidMapMove(int zIndex)
    {
        
    }



    void FixedUpdate()
    {

        //Debug.Log("ortho size is " + camera.orthographicSize);
        if (cameraMove == 1) //camera is moving, let it move, turns off when reaches position or button is pressed that moves camera
        {
            MoveCamera();
            TestCameraMove();
        }

        if (cameraRotate == 1)
        {
            //Debug.Log("Camera Rotate is " + cameraRotate);
            RotateCamera();
            TestCameraRotate();
            //Debug.Log("Camera Rotate is " + cameraRotate);
        }
        if (cameraSize == 1)
        {
            //Debug.Log("Camera Rotate is " + cameraRotate);
            ResizeCamera();
            TestCameraSize();
            //Debug.Log("Camera Rotate is " + cameraRotate);
        }
    }

    void TestCameraMove()
    {
        float precision = 0.1f;
        Vector3 pos1 = camera.gameObject.transform.position;
        if (Math.Abs(pos1.x - cameraMoveOffset.x) < precision && Math.Abs(pos1.y - cameraMoveOffset.y) < precision
            && Math.Abs(pos1.z - cameraMoveOffset.z) < precision)
        {
            cameraMove = 0;
        }
    }

    void TestCameraRotate()
    {
        //Vector3 rot1 = camera.gameObject.transform.rotation;
        float precision = 0.1f;
        Transform rot1 = camera.gameObject.transform;
        if (Math.Abs(rot1.eulerAngles.x - cameraMoveRotation.eulerAngles.x) < precision
            && Math.Abs(rot1.eulerAngles.y - cameraMoveRotation.eulerAngles.y) < precision && Math.Abs(rot1.eulerAngles.z - cameraMoveRotation.eulerAngles.z) < precision)
        {
            cameraRotate = 0;
        }
    }

    void TestCameraSize()
    {
        if (camera.orthographicSize == cameraMoveSize)
        {
            cameraSize = 1;
        }
    }

    void MoveCamera()
    {
        camera.gameObject.transform.position = Vector3.Lerp(camera.gameObject.transform.position, cameraMoveOffset, smoothing * Time.deltaTime);
    }

    void RotateCamera()
    {
        //Transform from = camera.gameObject.transform.rotate;
        camera.gameObject.transform.rotation = Quaternion.Lerp(camera.gameObject.transform.rotation, cameraMoveRotation.rotation, smoothing * Time.deltaTime); // from, to http://docs.unity3d.com/ScriptReference/Quaternion.Lerp.html
        //camera.gameObject.transform.rotate = Vector3.Lerp(camera.gameObject.transform.rotate, cameraMoveRotation, smoothing * Time.deltaTime);
    }

    void ResizeCamera()
    {
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, cameraMoveSize, smoothing * Time.deltaTime);
    }

    public void SetCameraMoveActive()
    {
        cameraMove = 1;
        cameraRotate = 1;
        cameraSize = 1;
    }

    //rotate and pan should have the same inner function with different arguments depending on the click and (in pan's case) the camera's current orientation
    public void RotateCameraLeft()
    {
        PanCamera(true);
    }

    public void RotateCameraRight()
    {
        PanCamera(false);
    }

    public void PanCamera(bool panningLeft)
    {
        if (overheadInt != 0)
        {
            return;
        }
        SetCameraMoveActive();

        //have to move the y angle by 90 and move the camera position
        //doing this based on the default view, not caring about overhead or not
        Vector3 zRotation = cameraDefaultRotation;
        //for position need to get the current target then move it around based on that
        Vector3 zPosition = new Vector3(0, 15, 0);
        Vector3 targetPosition;
        targetPosition = camera.gameObject.transform.position;

        //Debug.Log("panning camera rotate int is: " + rotateInt);
        //return camera to default facing, then rotate it off of that
        if (rotateInt == 1)
        {
            //camera 
            targetPosition.x -= cameraDefaultOffset.z;
            targetPosition.z += cameraDefaultOffset.x;
            //zPosition.x = targetPosition.x + cameraDefaultOffset.z;
            //zPosition.z = targetPosition.z + cameraDefaultOffset.x;
        }
        else if (rotateInt == 2)
        {
            targetPosition.x += cameraDefaultOffset.x;
            targetPosition.z += cameraDefaultOffset.z;
            //zPosition.x = targetPosition.x + cameraDefaultOffset.x;
            //zPosition.z = targetPosition.z - cameraDefaultOffset.z;
        }
        else if (rotateInt == 3)
        {
            //OLD WAY
            //targetPosition.x += cameraDefaultOffset.z;
            //targetPosition.z += cameraDefaultOffset.x;
            //zPosition.x = targetPosition.x - cameraDefaultOffset.z;
            //zPosition.z = targetPosition.z - cameraDefaultOffset.x;

            //NEW WAY
            targetPosition.x += cameraDefaultOffset.z;
            targetPosition.z -= cameraDefaultOffset.x;
        }
        else
        {
            targetPosition.x -= cameraDefaultOffset.x;
            targetPosition.z -= cameraDefaultOffset.z;
        }


        if (panningLeft)
        {
            rotateInt += 1;
        }
        else
        {
            rotateInt -= 1;
        }


        if (rotateInt < 0)
        {
            rotateInt = 3;
        }
        else if (rotateInt > 3)
        {
            rotateInt = 0;
        }

        //Debug.Log("panning camera rotate int is: " + rotateInt);
        if (rotateInt == 1)
        {
            //OLD WAY X IS POSITIVE
            //zPosition.x = targetPosition.x + cameraDefaultOffset.z;
            //zPosition.z = targetPosition.z + cameraDefaultOffset.x;
            //NEW WAY X IS NEGATIVE
            zRotation.y = 90 + cameraDefaultRotation.y;
            zPosition.x = targetPosition.x + cameraDefaultOffset.z;
            zPosition.z = targetPosition.z + -cameraDefaultOffset.x;
        }
        else if (rotateInt == 2)
        {
            zRotation.y = 180 + cameraDefaultRotation.y;
            //OLD WAY X IS POSITIVE x is fine but z is on the opposite side and cameraDefautltOffset.z far away
            //zPosition.x = targetPosition.x + cameraDefaultOffset.x;
            //zPosition.z = targetPosition.z - cameraDefaultOffset.z;


            //NEW WAY X IS NEGATIVE
            zPosition.x = targetPosition.x + -cameraDefaultOffset.x;
            zPosition.z = targetPosition.z - cameraDefaultOffset.z;
        }
        else if (rotateInt == 3)
        {
            zRotation.y = 270 + cameraDefaultRotation.y;
            //old way
            //zPosition.x = targetPosition.x - cameraDefaultOffset.z;
            //zPosition.z = targetPosition.z - cameraDefaultOffset.x;
            //new way, x is negative
            zPosition.x = targetPosition.x - cameraDefaultOffset.z;
            zPosition.z = targetPosition.z + cameraDefaultOffset.x;
        }
        else //back to the default angle
        {
            //zPosition.x = cameraDefaultOffset2.x;
            //zPosition.z = cameraDefaultOffset2.z;
            //ReturnCamera();
            //return;

            //NEW WAY X IS NEGATIVE
            zPosition.x = cameraDefaultOffset.x;
            zPosition.z = cameraDefaultOffset.z;
        }

        //if zPosition and zRotation not changed defaults to defaults
        cameraMoveOffset = zPosition;
        cameraMoveRotation.eulerAngles = zRotation;

    }

    void SetCameraMoveOffset(Vector3 vec)
    {
        cameraMoveOffset = vec;
        if (cameraMoveOffset.x < cameraMinX)
        {
            cameraMoveOffset.x = cameraMinX;
        }
        if (cameraMoveOffset.x > cameraMaxX)
        {
            cameraMoveOffset.x = cameraMaxX;
        }
        if (cameraMoveOffset.z < cameraMinZ)
        {
            cameraMoveOffset.z = cameraMinZ;
        }
        if (cameraMoveOffset.z > cameraMaxZ)
        {
            cameraMoveOffset.z = cameraMaxZ;
        }
    }

    void SetPanAmount(int direction)
    {
        SetCameraMoveActive();
        float zFloat = 5.0f;
        if (zoomInt == 1)
        {
            zFloat = 3.0f;
        }
        else if (zoomInt == 2)
        {
            zFloat = 10.0f;
        }

        //direction 0 for up, 1 for right, 2 for down, 3 for left

        Vector3 currentPos = camera.gameObject.transform.position;
        if (rotateInt == 0)
        {
            if (direction == 0)
            {
                currentPos.z += zFloat;
            }
            else if (direction == 1)
            {
                currentPos.x += zFloat;
            }
            else if (direction == 2)
            {
                currentPos.z -= zFloat;
            }
            else if (direction == 3)
            {
                currentPos.x -= zFloat;
            }
        }
        else if (rotateInt == 1)
        {
            if (direction == 0)
            {
                currentPos.x += zFloat;
            }
            else if (direction == 1)
            {
                currentPos.z -= zFloat;
            }
            else if (direction == 2)
            {
                currentPos.x -= zFloat;
            }
            else if (direction == 3)
            {
                currentPos.z += zFloat;
            }
        }
        else if (rotateInt == 2)
        {
            if (direction == 0)
            {
                currentPos.z -= zFloat;
            }
            else if (direction == 1)
            {
                currentPos.x -= zFloat;
            }
            else if (direction == 2)
            {
                currentPos.z += zFloat;
            }
            else if (direction == 3)
            {
                currentPos.x += zFloat;
            }
        }
        else if (rotateInt == 3)
        {
            if (direction == 0)
            {
                currentPos.x -= zFloat;
            }
            else if (direction == 1)
            {
                currentPos.z += zFloat;
            }
            else if (direction == 2)
            {
                currentPos.x += zFloat;
            }
            else if (direction == 3)
            {
                currentPos.z -= zFloat;
            }
        }

        SetCameraMoveOffset(currentPos);
    }


    public void PanUp()
    {
        SetPanAmount(0);
    }

    public void PanRight()
    {
        SetPanAmount(1);
    }

    public void PanDown()
    {
        SetPanAmount(2);
    }

    public void PanLeft()
    {
        SetPanAmount(3);
    }


    public void Zoom()
    {
        SetCameraMoveActive();
        zoomInt += 1;
        if (zoomInt > 2)
        {
            zoomInt = 0;
        }

        if (zoomInt == 1)
        {
            cameraMoveSize = 2.0f;
            //PanUp();
        }
        else if (zoomInt == 2)
        {
            cameraMoveSize = 8.5f;
        }
        else
        {
            cameraMoveSize = 5.0f; //4.5f
        }
        //camera.orthographicSize = cameraDefaultSize;
    }

    public void ToggleOverhead()
    {
        SetCameraMoveActive();
        overheadInt += 1;
        overheadInt = overheadInt % 2;
        if (overheadInt == 1)
        {
            cameraMoveOffset = new Vector3(0, 15, 0);
            cameraMoveRotation.eulerAngles = new Vector3(90, 0, 0);
        }
        else
        {
            ReturnCamera(); //meh probably want to un overhead it...
        }
    }

    public void ReturnCamera()
    {
        SetCameraMoveActive();
        cameraMoveOffset = cameraDefaultOffset;
       

        cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
        overheadInt = 0;
        zoomInt = 0;
        rotateInt = 0;
        cameraMoveSize = cameraDefaultSize;
        //camera.gameObject.transform.rotation = Quaternion.Euler(cameraDefaultRotation.x, cameraDefaultRotation.y, cameraDefaultRotation.z);
    }

    public void FollowUnit(int unitId = 1919, float delaySeconds = 2.0f, bool delay = false)
    {
        if (isHold || rotateInt != 0 || overheadInt != 0)
        {
            return;
        }
        if (delay)
        {
            StartCoroutine(DelayedFollowUnit(delaySeconds, unitId));
        }
        else
        {
            
        }

    }

    void SetSelectedMapTile(int mapTileIndex)
    {
        //MapTileManager.Instance.SetAsSelectedTile(mapTileIndex);
    }

    private IEnumerator DelayedFollowUnit(float seconds, int unitId = 1919)
    {
        yield return new WaitForSeconds(seconds);
        //Debug.Log("in delay");
        FollowUnit(unitId);
    }

    public void MoveToMapTile(int mapTileIndex, float delaySeconds = 2.0f, bool delay = false)
    {
        //if (isHold || rotateInt != 0 || overheadInt != 0)
        //{
        //    return;
        //}
        //if (delay)
        //{
        //    StartCoroutine(DelayedMapTile(delaySeconds, mapTileIndex));
        //}
        //else
        //{
        //    if (mapTileIndex != 1919)
        //    {
        //        SetSelectedMapTile(mapTileIndex);
        //        SetCameraMoveActive();
        //        Vector3 mapTilePosition = MapTileManager.Instance.GetMapTileObjectPosition(mapTileIndex);//returns the transform.position
        //        //Transform target = PlayerManager.Instance.GetPlayerUnitObject(unitId).transform;
        //        cameraMoveOffset = mapTilePosition + cameraDefaultOffset;
        //        cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
        //        overheadInt = 0;
        //        zoomInt = 0;
        //        rotateInt = 0;
        //        cameraMoveSize = cameraDefaultSize;
        //    }
        //}
    }

    void UserMoveToTile(Vector3 targetPosition)
    {
        if (isHold || rotateInt != 0)
        {
            return;
        }

        SetCameraMoveActive();
        Vector3 tilePosition = targetPosition;
        if (overheadInt == 1)
        {
            cameraMoveOffset = tilePosition + new Vector3(0, 15, 0);
        }
        else
        {
            cameraMoveOffset = tilePosition + cameraDefaultOffset;// GetUserMoveCameraPosition();//
        }
        
    }

    public void UserMoveToMapTile(int mapTileIndex)
    {
        //need to change the cameraDefaultOffset to the on based on the rotation if you want to comment this out
        //if (isHold || rotateInt != 0)
        //{
        //    return;
        //}
        //if (mapTileIndex != 1919)
        //{
        //    SetCameraMoveActive();
        //    Vector3 mapTilePosition = MapTileManager.Instance.GetMapTileObjectPosition(mapTileIndex);//returns the transform.position //Transform target = PlayerManager.Instance.GetPlayerUnitObject(unitId).transform;

        //    if (overheadInt == 1)
        //    {
        //        cameraMoveOffset = mapTilePosition + new Vector3(0, 15, 0);
        //    }
        //    else
        //    {
        //        cameraMoveOffset = mapTilePosition + cameraDefaultOffset;// GetUserMoveCameraPosition();//
        //    }

        //    //cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
        //    //overheadInt = 0;
        //    //zoomInt = 0;
        //    rotateInt = 0;
        //    //cameraMoveSize = cameraDefaultSize;
        //}
    }

    private IEnumerator DelayedMapTile(float seconds, int mapTileIndex = 1919)
    {
        yield return new WaitForSeconds(seconds);
        //Debug.Log("in delay");
        MoveToMapTile(mapTileIndex);
    }

    void HandleDoubleClick()
    {
        timerForDoubleClick += Time.deltaTime;
        if (timerForDoubleClick >= delay)
        {
            ClearDoubleClick();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                countedClicks += 1; //Debug.Log("number of counted clicks: " + countedClicks);
                if (countedClicks > 1)
                {
                    isDoubleClick = true;
                }
            }
        }

        if (isDoubleClick)
        {
            //Debug.Log("double click is true: " + countedClicks);
            CheckForObjectAndMoveCamera();
            ClearDoubleClick();
        }

    }

    void ClearDoubleClick()
    {
        timerForDoubleClick = 0.0f;
        isDoubleClick = false;
        countedClicks = 0;
    }

    
}

