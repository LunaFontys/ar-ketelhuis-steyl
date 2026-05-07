using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MarkerSpawnManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject interactionRootPrefab;
    [SerializeField] private Vector3 localPositionOffset;
    [SerializeField] private Vector3 localRotationOffset;
    [SerializeField] private Vector3 localScale = Vector3.one;

    private GameObject spawnedInteractionRoot;

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            SpawnOrUpdateObject(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in args.updated)
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                SpawnOrUpdateObject(trackedImage);
            }
        }
    }

    private void SpawnOrUpdateObject(ARTrackedImage trackedImage)
    {
        if (spawnedInteractionRoot == null)
        {
            spawnedInteractionRoot = Instantiate(interactionRootPrefab);
        }

        spawnedInteractionRoot.transform.SetParent(trackedImage.transform);
        spawnedInteractionRoot.transform.localPosition = localPositionOffset;
        spawnedInteractionRoot.transform.localRotation = Quaternion.Euler(localRotationOffset);
        spawnedInteractionRoot.transform.localScale = localScale;
        spawnedInteractionRoot.SetActive(true);
    }
}