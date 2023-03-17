using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private GameObject firstPrefab;

    [SerializeField]
    private GameObject secondPrefab;


    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool placementMode = false;

    private float touchTime = 0;
    [SerializeField]
    private float minTouchTime;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }
    public void TogglePlacement()
    {
        placementMode = !placementMode;
        aRPlaneManager.enabled = placementMode;
    }

    public void PlaceFirst()
    {
        prefab = firstPrefab;
    }

    public void PlaceSecond()
    {
        prefab = secondPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        if (!placementMode)
        {
            return;
        }
        // Vaan eka kosketus
        Touch touch = Input.touches[0];

        if (Input.touches.Length == 1)
        {
            // Jos ruutua koskettaa vain yksi sormi.
            // Näin vältetään objektien luonti kun yritetään zoomata

            if (touch.phase == TouchPhase.Began)
            {
                touchTime = Time.time;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (Time.time - touchTime <= minTouchTime)
                {
                    //do stuff as a tap​

                }
                else
                {
                    // this is a long press or drag​
                    if (aRRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                    {
                        foreach (ARRaycastHit hit in hits)
                        {
                            Pose pose = hit.pose;
                            GameObject obj = Instantiate(prefab, pose.position, pose.rotation);

                            if (aRPlaneManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                            {
                                Vector3 position = obj.transform.position;
                                Vector3 cameraPosition = Camera.main.transform.position;
                                Vector3 direction = cameraPosition - position;
                                Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
                                Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, obj.transform.up.normalized);
                                Quaternion targetRotation = Quaternion.Euler(scaledEuler);
                                obj.transform.rotation = obj.transform.rotation * targetRotation;
                            }

                        }
                    }
                }

                touchTime = 0;
            }
        }
    }
}
