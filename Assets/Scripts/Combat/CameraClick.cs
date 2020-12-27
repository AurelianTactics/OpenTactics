//using UnityEngine;
//using System.Collections;
//using UnityEngine.EventSystems;
////using EckTechGames.FloatingCombatText;

//public class CameraClick : MonoBehaviour {
//    //float smoothing; //= 5f;
//    //Vector3 offset; //= new Vector3(30, 30, 30);
//    //public int playerIndex;

//    void Start()
//    {
//        //smoothing = 5f;
//        //offset = new Vector3(1, 15, -27);
//        //playerIndex = 0;
//    }

//    void Update()
//    {
//        if(EventSystem.current.IsPointerOverGameObject())
//        {
//            return;
//        }

//        //testing something by clicking on a unit
//        //if (Input.GetMouseButtonDown(0))
//        //{
//        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        //    RaycastHit hit;
//        //    if (Physics.Raycast(ray, out hit))
//        //    {
//        //        GameObject hitObject = hit.transform.gameObject;
//        //        PlayerUnitObject target = hitObject.GetComponent<PlayerUnitObject>();
//        //        if (target != null)
//        //        {
//        //            int z1 = Random.Range(0, 4);
//        //            target.RotatePlayerInstant(0, z1, true);
//        //        }
//        //    }   
//        //}

//        if (SceneCreate.active_unit != 1919 && Input.GetMouseButtonDown(0)  ) //is active turn and there has been a click
//        {
//            //testing SoundManager.Instance.PlaySoundClip();
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit; 
//            if ( SceneCreate.phaseMenu == 1) //moving
//            {
//                if (Physics.Raycast(ray, out hit))
//                {
//                    GameObject hitObject = hit.transform.gameObject;
//                    //check for map tile hit
//                    MapTileObject target = hitObject.GetComponent<MapTileObject>();
//                    if (target != null)
//                    {
                        
//                        int zIndex = target.GetIndex();
//                        if (MapTileManager.Instance.CheckMovableTile(zIndex))
//                        {
//                            MapTileManager.Instance.SetAsSelectedTile(target.GetIndex());
//                            //target.ReactToHit();
//                            //Vector3 movement = target.transform.position;
                            
//                            GameObject p = PlayerManager.Instance.GetPlayerUnitObject(SceneCreate.active_unit);
//                            if (p != null)
//                            {
//                                //PlayerManager.Instance.MovePlayer(p, movement,zIndex);
//                                PlayerManager.Instance.MovePlayer(p, zIndex);
//                            }
//                        }
//                    }
//                    //else {
//                    //    StartCoroutine(SphereIndicator(hit.point));
//                    //}
//                }
//            }
//            else if( SceneCreate.phaseMenu == 2 || SceneCreate.phaseMenu == 20) //2 allows for range finder for attack/abilities, 20 lets act/abilities be confirmed
//            {
//                if (Physics.Raycast(ray, out hit))
//                {
//                    GameObject hitObject = hit.transform.gameObject;
//                    MapTileObject target = hitObject.GetComponent<MapTileObject>();
//                    PlayerUnitObject puTarget = hitObject.GetComponent<PlayerUnitObject>();
//                    //check for map tile hit
//                    if (target != null)
//                    {
//                        int zIndex = target.GetIndex();
//                        if (MapTileManager.Instance.CheckMovableTile(zIndex)) //works for targetable tiles too
//                        {
//                            CalculationAT.ConfirmActionPhase(zIndex);
//                            //moves to action preview phase
//                            //SceneCreate.phaseMenu = 20; //preview phase
//                            //Debug.Log("Need to Popup Up Confirm Button and status about what the attack does");
//                        }

//                    }
//                    else if( puTarget != null)
//                    {
//                        int zIndex = PlayerManager.Instance.GetUnitMapTileIndex(puTarget.GetUnitId());
//                        if (MapTileManager.Instance.CheckMovableTile(zIndex)) //works for targetable tiles too
//                        {
//                            CalculationAT.ConfirmActionPhase(zIndex);
//                        }
//                    }
//                    //else {
//                    //    StartCoroutine(SphereIndicator(hit.point));
//                    //}
//                }
//            }
//            else if( SceneCreate.phaseMenu == 0 ) //top menu
//            { 
//                if (Physics.Raycast(ray, out hit))
//                {
//                    GameObject hitObject = hit.transform.gameObject;
//                    MapTileManager.Instance.UnhighlightAllTiles();
//                    MapTileObject target = hitObject.GetComponent<MapTileObject>();
//                    if (target != null)
//                    {
//                        //Debug.Log("tile hit");
//                        int z1 = target.GetIndex();
//                        SceneCreate.targetPanel.SetTile(MapTileManager.Instance.GetMapTileByIndex(z1));
//                        MapTileManager.Instance.SetAsSelectedTile(z1);
//                        //MapTileManager.Instance.HighlightMapTileObject(z1, 1, false);
//                        //target.HighlightTile(0,true);
//                        return;
//                    }

//                    PlayerUnitObject playerTarget = hitObject.GetComponent<PlayerUnitObject>();
//                    if (playerTarget != null)
//                    {
//                        //Debug.Log("player hithit");
//                        int z1 = playerTarget.GetUnitId();
//                        PlayerUnit pu = PlayerManager.Instance.GetPlayerUnit(z1);
//                        SceneCreate.targetPanel.SetActor(true,pu);
//                        MapTileManager.Instance.SetAsSelectedTile(pu.GetMap_tile_index());
//                        //MapTileManager.Instance.HighlightMapTileObject(pu.GetMap_tile_index(), 1, false);
//                        //GameObject tempObject = MapTileManager.Instance.GetMapTileObjectByIndex(pu.GetMap_tile_index());
//                        //target = tempObject.GetComponent<MapTileObject>();
//                        //target.HighlightTile(0, true);
//                        return;
//                    }

//                    //Debug.Log("nothing hit");
//                    //else {
//                    //    StartCoroutine(SphereIndicator(hit.point));
//                    //}
//                }
//            }
//        }   
//    }


//    //void FixedUpdate()
//    //{
//    //    //FollowActivePlayer();
//    //}

//    //private IEnumerator SphereIndicator(Vector3 pos)
//    //{
//    //    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//    //    sphere.transform.position = pos;

//    //    yield return new WaitForSeconds(1);

//    //    Destroy(sphere);
//    //}

//    //void FollowActivePlayer()
//    //{
//    //    playerIndex = SceneCreate.active_unit;
//    //    //Debug.Log("playerIndex is " + playerIndex);
//    //    if( playerIndex != 1919)
//    //    {
//    //        Transform target = PlayerManager.Instance.GetPlayerUnitObject(playerIndex).transform;
//    //        //Debug.Log("target is " + target.position.x + "," + target.position.y + "," + target.position.z);
//    //        //Debug.Log("asdf");
//    //        Vector3 targetCamPos = target.position + offset;
//    //        //Debug.Log("targetCamPos is " + targetCamPos.x + "," + targetCamPos.y + "," + targetCamPos.z);
//    //        //Debug.Log("offset is " + offset.x + "," + offset.y + "," + offset.z);
//    //        // Smoothly interpolate between the camera's current position and it's target position.
//    //        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, targetCamPos, smoothing * Time.deltaTime);
//    //        //Debug.Log("camera is at " + this.gameObject.transform.position.x + "," + this.gameObject.transform.position.y + "," + this.gameObject.transform.position.z);
//    //        //this.gameObject.transform.position = targetCamPos;
//    //    }
//    //}
//}
