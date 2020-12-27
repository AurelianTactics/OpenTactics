using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
//using UnityEngine.EventSystems;

//current way: 
//activated during certain states, lets user click around on buttons to pan the camera
//input on map click only done in certain states, handled elsewhere

public class UICameraMenu : MonoBehaviour {

    //public float smoothTime = 0.3f;
    float smoothing; //= 5f;

    public Camera scriptCamera;
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

    Vector3 targetPosition; //used for returncamera default position

    void Awake() {
        //Debug.Log("starting UICameraMenu");
        //Debug.Log("asdf " + scriptCamera.transform.position);

        cameraDefaultOffset = new Vector3(-6.0f, 15.0f, -15.0f);//new Vector3(1.0f, 15.0f, -24.0f);
        //cameraDefaultOffset2 = new Vector3(3f, 15.0f, -15.0f);
        cameraDefaultRotation = new Vector3(40, 25, 0);//new Vector3(30, 0, 0);
        //cameraDefaultRotation.eulerAngles = new Vector3(30, 0, 0);
        cameraMoveOffset = new Vector3(-6.0f, 15.0f, -15.0f);//new Vector3(1.0f, 15.0f, -24.0f);
        //GameObject tempGo = new GameObject();
        cameraMoveRotation = scriptCamera.gameObject.transform; // tempGo.transform;// scriptCamera.gameObject.transform;
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

        targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void Open(Tile t = null)
    {
        gameObject.SetActive(true);
        if( t != null)
        {
            targetPosition = t.transform.position;
        }
        else
        {
            targetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    public void Close()
    {
        ReturnCamera(); //can make this more cinematic in future
        gameObject.SetActive(false);
    }

    #region moving camera
    void FixedUpdate()
    {
        //Debug.Log("ortho size is " + scriptCamera.orthographicSize);
        if ( cameraMove == 1) //camera is moving, let it move, turns off when reaches position or button is pressed that moves camera
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
        Vector3 pos1 = scriptCamera.gameObject.transform.position;
        if ( Math.Abs(pos1.x - cameraMoveOffset.x) < precision && Math.Abs(pos1.y - cameraMoveOffset.y) < precision
            && Math.Abs(pos1.z - cameraMoveOffset.z) < precision)
        {
            cameraMove = 0;
        }
    }

    void TestCameraRotate()
    {
        //Vector3 rot1 = scriptCamera.gameObject.transform.rotation;
        float precision = 0.1f;
        Transform rot1 = scriptCamera.gameObject.transform;
        if ( Math.Abs(rot1.eulerAngles.x - cameraMoveRotation.eulerAngles.x) < precision
            && Math.Abs(rot1.eulerAngles.y - cameraMoveRotation.eulerAngles.y) < precision && Math.Abs(rot1.eulerAngles.z - cameraMoveRotation.eulerAngles.z) < precision)
        {
            cameraRotate = 0;
        }
    }

    void TestCameraSize()
    {
        if(scriptCamera.orthographicSize == cameraMoveSize)
        {
            cameraSize = 1;
        }
    }

    void MoveCamera()
    {
        scriptCamera.gameObject.transform.position = Vector3.Lerp(scriptCamera.gameObject.transform.position, cameraMoveOffset, smoothing * Time.deltaTime);
    }

    void RotateCamera()
    {
        //Transform from = scriptCamera.gameObject.transform.rotate;
        scriptCamera.gameObject.transform.rotation = Quaternion.Lerp(scriptCamera.gameObject.transform.rotation, cameraMoveRotation.rotation, smoothing * Time.deltaTime); // from, to http://docs.unity3d.com/ScriptReference/Quaternion.Lerp.html
        //scriptCamera.gameObject.transform.rotate = Vector3.Lerp(scriptCamera.gameObject.transform.rotate, cameraMoveRotation, smoothing * Time.deltaTime);
    }

    void ResizeCamera()
    {
        scriptCamera.orthographicSize = Mathf.Lerp(scriptCamera.orthographicSize, cameraMoveSize, smoothing * Time.deltaTime);
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
        if(overheadInt != 0)
        {
            return;
        }
        SetCameraMoveActive();
              
        //have to move the y angle by 90 and move the camera position
        //doing this based on the default view, not caring about overhead or not
        Vector3 zRotation = cameraDefaultRotation;
        //for position need to get the current target then move it around based on that
        Vector3 zPosition = new Vector3(0,15,0);
        Vector3 targetPosition;
        targetPosition = scriptCamera.gameObject.transform.position;
        
        //Debug.Log("panning camera rotate int is: " + rotateInt);
        //return camera to default facing, then rotate it off of that
        if ( rotateInt == 1)
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

        if ( panningLeft)
        {
            rotateInt += 1;   
        }
        else
        {
            rotateInt -= 1;
        }

        
        if( rotateInt < 0)
        {
            rotateInt = 3;
        }
        else if( rotateInt > 3)
        {
            rotateInt = 0;
        }

        //Debug.Log("panning camera rotate int is: " + rotateInt);
        if ( rotateInt == 1 )
        {
            //OLD WAY X IS POSITIVE
            //zPosition.x = targetPosition.x + cameraDefaultOffset.z;
            //zPosition.z = targetPosition.z + cameraDefaultOffset.x;
            //NEW WAY X IS NEGATIVE
            zRotation.y = 90 + cameraDefaultRotation.y;
            zPosition.x = targetPosition.x + cameraDefaultOffset.z;
            zPosition.z = targetPosition.z + -cameraDefaultOffset.x;
        } 
        else if( rotateInt == 2)
        {
            zRotation.y = 180 + cameraDefaultRotation.y;
            //OLD WAY X IS POSITIVE x is fine but z is on the opposite side and cameraDefautltOffset.z far away
            //zPosition.x = targetPosition.x + cameraDefaultOffset.x;
            //zPosition.z = targetPosition.z - cameraDefaultOffset.z;


            //NEW WAY X IS NEGATIVE
            zPosition.x = targetPosition.x + - cameraDefaultOffset.x;
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
        if( cameraMoveOffset.x < cameraMinX)
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

    void SetPanAmount( int direction)
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

        Vector3 currentPos = scriptCamera.gameObject.transform.position;
        if( rotateInt == 0)
        {
            if( direction == 0)
            {
                currentPos.z += zFloat;
            }
            else if( direction == 1)
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
        else if( rotateInt == 1)
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
        if( zoomInt > 2)
        {
            zoomInt = 0;
        }

        if( zoomInt == 1)
        {
            cameraMoveSize = 2.0f;
            //PanUp();
        }
        else if( zoomInt == 2)
        {
            cameraMoveSize = 8.5f;
        } else
        {
            cameraMoveSize = 5.0f; //4.5f
        }
        //scriptCamera.orthographicSize = cameraDefaultSize;
    }

    public void ToggleOverhead()
    {
        SetCameraMoveActive();
        overheadInt += 1;
        overheadInt = overheadInt % 2;
        if( overheadInt == 1)
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

        cameraMoveOffset = targetPosition + cameraDefaultOffset; //targetPosition.position set in Open or defaults to 0,0,0
        //Debug.Log("asdf" + cameraDefaultRotation);
        cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
        overheadInt = 0;
        zoomInt = 0;
        rotateInt = 0;
        cameraMoveSize = cameraDefaultSize;
        //scriptCamera.gameObject.transform.rotation = Quaternion.Euler(cameraDefaultRotation.x, cameraDefaultRotation.y, cameraDefaultRotation.z);
    }

    #endregion

    public void FollowUnit( int unitId = 1919, float delaySeconds = 2.0f, bool delay = false)
    {
        if (isHold || rotateInt != 0 || overheadInt != 0)
        {
            return;
        }
        if ( delay)
        {
            StartCoroutine(DelayedFollowUnit(delaySeconds, unitId));
        }
        else
        {
            //SetCameraMoveActive();
            if( unitId != 1919)
            {
                SetCameraMoveActive();
                Transform target = PlayerManager.Instance.GetPlayerUnitObject(unitId).transform;
                cameraMoveOffset = target.position + cameraDefaultOffset;
                cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
                overheadInt = 0;
                zoomInt = 0;
                rotateInt = 0;
                cameraMoveSize = cameraDefaultSize;
                //Debug.Log("UICameraMenu used to highlight a tile here, highlight it from the state now");
            }
            //else
            //{
            //    cameraMoveOffset = cameraDefaultOffset;
            //}

            //cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
            //overheadInt = 0;
            //zoomInt = 0;
            //rotateInt = 0;
            //cameraMoveSize = cameraDefaultSize;

        }

    }

    private IEnumerator DelayedFollowUnit(float seconds, int unitId = 1919)
    {
        yield return new WaitForSeconds(seconds);
        //Debug.Log("in delay");
        FollowUnit(unitId);
    }

    public void MoveToMapTile(Tile targetTile, float delaySeconds = 2.0f, bool delay = false)
    {
        if (isHold || rotateInt != 0 || overheadInt != 0)
        {
            return;
        }
        if (delay)
        {
            StartCoroutine(DelayedMapTile(delaySeconds, targetTile));
        }
        else
        {
            //Debug.Log("UiCameraMenu used to highlight a tile, do that in state now");
            SetCameraMoveActive();
            Vector3 mapTilePosition = targetTile.transform.position;
            cameraMoveOffset = mapTilePosition + cameraDefaultOffset;
            cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
            overheadInt = 0;
            zoomInt = 0;
            rotateInt = 0;
            cameraMoveSize = cameraDefaultSize;
        }
    }

    

    public void UserMoveToMapTile(Tile target)
    {
        //need to change the cameraDefaultOffset to the on based on the rotation if you want to comment this out
        if (isHold  || rotateInt != 0 )
        {
            return;
        }

        SetCameraMoveActive();
        Vector3 mapTilePosition = target.transform.position;

        if (overheadInt == 1)
        {
            cameraMoveOffset = mapTilePosition + new Vector3(0, 15, 0);
        }
        else
        {
            cameraMoveOffset = mapTilePosition + cameraDefaultOffset;// GetUserMoveCameraPosition();//
        }
        rotateInt = 0;
        
    }

    private IEnumerator DelayedMapTile(float seconds, Tile targetTile)
    {
        yield return new WaitForSeconds(seconds);
        //Debug.Log("in delay");
        MoveToMapTile(targetTile);
    }



    #region old code
    //void CheckForObjectAndMoveCamera()
    //{
    //    //Debug.Log("adsf " + zoomInt);
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        GameObject hitObject = hit.transform.gameObject;
    //        Tile target = hitObject.GetComponent<Tile>();
    //        PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
    //        //check for map tile hit
    //        if (target != null)
    //        {
    //            UserMoveToMapTile(target);
    //        }
    //        else if (puTarget != null)
    //        {
    //            PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(puTarget.GetUnitId());
    //            target = board.GetTile(pu);
    //            UserMoveToMapTile(target);
    //        }

    //    }
    //    //Debug.Log("adsf " + zoomInt);
    //}
    //public void MoveToMapTile(int mapTileIndex, float delaySeconds = 2.0f, bool delay = false)
    //{
    //    if (isHold || rotateInt != 0 || overheadInt != 0)
    //    {
    //        return;
    //    }
    //    if (delay)
    //    {
    //        StartCoroutine(DelayedMapTile(delaySeconds, mapTileIndex));
    //    }
    //    else
    //    {
    //        if (mapTileIndex != 1919)
    //        {
    //            //SetSelectedMapTile(mapTileIndex);
    //            Debug.Log("UiCameraMenu used to highlight a tile, do that in state now");
    //            SetCameraMoveActive();
    //            Vector3 mapTilePosition = MapTileManager.Instance.GetMapTileObjectPosition(mapTileIndex);//returns the transform.position
    //            //Transform target = PlayerManager.Instance.GetPlayerUnitObject(unitId).transform;
    //            cameraMoveOffset = mapTilePosition + cameraDefaultOffset;
    //            cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
    //            overheadInt = 0;
    //            zoomInt = 0;
    //            rotateInt = 0;
    //            cameraMoveSize = cameraDefaultSize;
    //        }
    //    }
    //}

    //void HandleDoubleClick()
    //{
    //    timerForDoubleClick += Time.deltaTime;
    //    if (timerForDoubleClick >= delay)
    //    {
    //        ClearDoubleClick();
    //    }
    //    else
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            countedClicks += 1; //Debug.Log("number of counted clicks: " + countedClicks);
    //            if (countedClicks > 1)
    //            {
    //                isDoubleClick = true;
    //            }
    //        }
    //    }

    //    if (isDoubleClick)
    //    {
    //        //Debug.Log("double click is true: " + countedClicks);
    //        CheckForObjectAndMoveCamera();
    //        ClearDoubleClick();
    //    }

    //}

    //void ClearDoubleClick()
    //{
    //    timerForDoubleClick = 0.0f;
    //    isDoubleClick = false;
    //    countedClicks = 0;
    //}   

    //void SetSelectedMapTile(int mapTileIndex)
    //{
    //    MapTileManager.Instance.SetAsSelectedTile(mapTileIndex);
    //}

    //void Update()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        if (Input.GetMouseButtonDown(0)) {
    //            SoundManager.Instance.PlaySoundClip(0);
    //        }
    //        return;
    //    }
    //    HandleDoubleClick();

    //    if (Input.GetMouseButton(0))
    //    {
    //        timeForHold += Time.deltaTime;
    //        if (timeForHold >= holdDelay)
    //        {
    //            isHold = true; //Debug.Log("button being held");
    //        }
    //    }
    //    else
    //    {
    //        timeForHold = 0.0f;
    //        isHold = false; //Debug.Log("NOT BEING HELD");
    //    }

    //    int zIndex = GetMapTileIndex(); //defaults to 1919

    //    if (zIndex != 1919 && !(isHold && zIndex == priorMapTileIndex) )
    //    {
    //        if (SceneCreate.active_unit != 1919 ) //is active turn and there has been a click
    //        {
    //            if (SceneCreate.phaseMenu == 1) //moving
    //            {
    //                SetValidMapMove(zIndex);
    //            }
    //            else if (SceneCreate.phaseMenu == 2 || SceneCreate.phaseMenu == 20) //2 allows for range finder for attack/abilities, 20 lets act/abilities be confirmed
    //            {
    //                //SetValidTarget(zIndex);
    //            }
    //            else if (SceneCreate.phaseMenu == 0) //top menu
    //            {
    //                MapTileManager.Instance.SetAsSelectedTile(zIndex);
    //            }
    //        }
    //        else
    //        {
    //            MapTileManager.Instance.SetAsSelectedTile(zIndex);
    //        }
    //    }

    //    priorMapTileIndex = zIndex;
    //}

    //void SetValidMapMove(int zIndex)
    //{
    //    if (MapTileManager.Instance.CheckMovableTile(zIndex)) //is a valid move tile
    //    {
    //        //zIndex is good, stays the same
    //    }
    //    else
    //    { //get nearest movable tile, have to click on board for this to work
    //        zIndex = MapTileManager.Instance.GetNearestMovableTile(zIndex);
    //    }

    //    if (zIndex != 1919)
    //    {
    //        MapTileManager.Instance.SetAsSelectedTile(zIndex);
    //        SceneCreate.cameraMenu.MoveToMapTile(zIndex);
    //        SceneCreate.turnMenu.ShowMovePreviewPhase(true);
    //        SceneCreate.targetMapIndex = zIndex;
    //        Debug.Log("Replace this with new SetTile");
    //        //SceneCreate.targetPanel.SetTile(MapTileManager.Instance.GetMapTileByIndex(zIndex));
    //        //SceneCreate.phaseMenu = 21;
    //        //MEH JUST DO IT HERE CalculationAT.ConfirmMovePhase(zIndex); //changes the game loop phase, which changes the menu (in game loop) and allows clicking on confirm to do the appropriate things
    //    }
    //    else {
    //        SceneCreate.turnMenu.ShowMovePreviewPhase(false);
    //    }
    //}

    //void SetValidTarget(int zIndex)
    //{
    //    if (MapTileManager.Instance.CheckMovableTile(zIndex)) //is a valid move tile
    //    {
    //        //zIndex is good, stays the same
    //    }
    //    else
    //    { //get nearest movable tile, have to click on board for this to work
    //        zIndex = MapTileManager.Instance.GetNearestMovableTile(zIndex);
    //    }

    //    if (zIndex != 1919)
    //    {
    //        CalculationAT.ConfirmActionPhase(zIndex);
    //    }
    //}

    //int GetMapTileIndex()
    //{
    //    int zIndex = 1919;
    //    if( Input.GetMouseButtonDown(0) || isHold)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            GameObject hitObject = hit.transform.gameObject;
    //            //check for map tile hit
    //            MapTileObject target = hitObject.GetComponent<MapTileObject>();
    //            if (target != null)
    //            {
    //                zIndex = target.GetIndex();
    //                Debug.Log("Replace this with new SetTile");
    //                //SceneCreate.targetPanel.SetTile(MapTileManager.Instance.GetMapTileByIndex(zIndex));
    //            }
    //            else //check for player target
    //            {
    //                PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
    //                if (puTarget != null)
    //                {
    //                    zIndex = PlayerManager.Instance.GetUnitMapTileIndex(puTarget.GetUnitId());
    //                    SceneCreate.targetPanel.SetActor(PlayerManager.Instance.GetPlayerUnit(puTarget.GetUnitId()));
    //                }
    //            }
    //        }
    //    }

    //    return zIndex;
    //}

    //void CheckForObjectAndMoveCamera()
    //{
    //    //Debug.Log("adsf " + zoomInt);
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        GameObject hitObject = hit.transform.gameObject;
    //        MapTileObject target = hitObject.GetComponent<MapTileObject>();
    //        PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
    //        //check for map tile hit
    //        if (target != null)
    //        {
    //            int zIndex = target.GetIndex();
    //            UserMoveToMapTile(zIndex);
    //        }
    //        else if (puTarget != null)
    //        {
    //            int zIndex = PlayerManager.Instance.GetUnitMapTileIndex(puTarget.GetUnitId());
    //            UserMoveToMapTile(zIndex);
    //        }
    //    }
    //    //Debug.Log("adsf " + zoomInt);
    //}

    //public void UserMoveToMapTile(int mapTileIndex)
    //{
    //    //need to change the cameraDefaultOffset to the on based on the rotation if you want to comment this out
    //    if (isHold || rotateInt != 0)
    //    {
    //        return;
    //    }
    //    if (mapTileIndex != 1919)
    //    {
    //        SetCameraMoveActive();
    //        Vector3 mapTilePosition = MapTileManager.Instance.GetMapTileObjectPosition(mapTileIndex);//returns the transform.position //Transform target = PlayerManager.Instance.GetPlayerUnitObject(unitId).transform;

    //        if (overheadInt == 1)
    //        {
    //            cameraMoveOffset = mapTilePosition + new Vector3(0, 15, 0);
    //        }
    //        else
    //        {
    //            cameraMoveOffset = mapTilePosition + cameraDefaultOffset;// GetUserMoveCameraPosition();//
    //        }

    //        //cameraMoveRotation.eulerAngles = cameraDefaultRotation; //new Vector3(30, 0, 0);
    //        //overheadInt = 0;
    //        //zoomInt = 0;
    //        rotateInt = 0;
    //        //cameraMoveSize = cameraDefaultSize;
    //    }
    //}


    //Debug.Log("z is " + targetPosition.z);

    //if (SceneCreate.active_unit == 1919)
    //{
    //    //scriptCamera.gameObject.transform.position = cameraDefaultOffset;
    //    targetPosition = cameraDefaultOffset;
    //    //cameraMoveRotation.eulerAngles = cameraDefaultRotation.eulerAngles;
    //}
    //else
    //{
    //    Transform target = PlayerManager.Instance.GetPlayerUnitObject(SceneCreate.active_unit).transform;
    //    targetPosition = target.position + cameraDefaultOffset;
    //    //cameraMoveRotation.eulerAngles = cameraDefaultRotation.eulerAngles;
    //}

    //Vector3 GetUserMoveCameraPosition()
    //{
    //    Vector3 targetPosition = scriptCamera.gameObject.transform.position;

    //    //Debug.Log("panning camera rotate int is: " + rotateInt);
    //    //return camera to default facing, then rotate it off of that
    //    if (rotateInt == 1)
    //    {
    //        //camera 
    //        targetPosition.x -= cameraDefaultOffset.z;
    //        targetPosition.z += cameraDefaultOffset.x;
    //        //zPosition.x = targetPosition.x + cameraDefaultOffset.z;
    //        //zPosition.z = targetPosition.z + cameraDefaultOffset.x;
    //    }
    //    else if (rotateInt == 2)
    //    {
    //        targetPosition.x += cameraDefaultOffset.x;
    //        targetPosition.z += cameraDefaultOffset.z;
    //        //zPosition.x = targetPosition.x + cameraDefaultOffset.x;
    //        //zPosition.z = targetPosition.z - cameraDefaultOffset.z;
    //    }
    //    else if (rotateInt == 3)
    //    {
    //        //OLD WAY
    //        //targetPosition.x += cameraDefaultOffset.z;
    //        //targetPosition.z += cameraDefaultOffset.x;
    //        //zPosition.x = targetPosition.x - cameraDefaultOffset.z;
    //        //zPosition.z = targetPosition.z - cameraDefaultOffset.x;

    //        //NEW WAY
    //        targetPosition.x += cameraDefaultOffset.z;
    //        targetPosition.z -= cameraDefaultOffset.x;
    //    }
    //    else
    //    {
    //        targetPosition.x -= cameraDefaultOffset.x;
    //        targetPosition.z -= cameraDefaultOffset.z;
    //    }

    //    return targetPosition;
    //}

    //void FixedUpdate()
    //{
    //    //FollowActivePlayer();
    //}

    //void FollowActivePlayer()
    //{
    //    playerIndex = SceneCreate.active_unit;
    //    //Debug.Log("playerIndex is " + playerIndex);
    //    if (playerIndex != 1919)
    //    {
    //        Transform target = PlayerManager.Instance.GetPlayerUnitObject(playerIndex).transform;
    //        //Debug.Log("target is " + target.position.x + "," + target.position.y + "," + target.position.z);
    //        //Debug.Log("asdf");
    //        Vector3 targetCamPos = target.position + offset;
    //        //Debug.Log("targetCamPos is " + targetCamPos.x + "," + targetCamPos.y + "," + targetCamPos.z);
    //        //Debug.Log("offset is " + offset.x + "," + offset.y + "," + offset.z);
    //        // Smoothly interpolate between the camera's current position and it's target position.
    //        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, targetCamPos, smoothing * Time.deltaTime);
    //        //Debug.Log("camera is at " + this.gameObject.transform.position.x + "," + this.gameObject.transform.position.y + "," + this.gameObject.transform.position.z);
    //        //this.gameObject.transform.position = targetCamPos;
    //    }
    //}
    #endregion
}


