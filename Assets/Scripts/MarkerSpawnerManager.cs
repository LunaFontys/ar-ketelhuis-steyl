using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerSpawnManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject interactionRootPrefab;
    [SerializeField] private Vector3 localPositionOffset;
    [SerializeField] private Vector3 localRotationOffset;
    [SerializeField] private Vector3 localScale = Vector3.one;

    private GameObject spawnedInteractionRoot;
    private ARTrackedImage activeTrackedImage;

    private void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            TrySetActiveTrackedImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in args.updated)
        {
            if (activeTrackedImage == trackedImage &&
                trackedImage.trackingState == TrackingState.Tracking)
            {
                EnsureObjectExists();
                UpdateObjectTransform(trackedImage);
            }
        }
    }

    private void TrySetActiveTrackedImage(ARTrackedImage trackedImage)
    {
        if (activeTrackedImage != null)
            return;

        activeTrackedImage = trackedImage;

        EnsureObjectExists();
        UpdateObjectTransform(trackedImage);
    }

    private void EnsureObjectExists()
    {
        if (spawnedInteractionRoot != null)
            return;

        if (interactionRootPrefab == null)
        {
            Debug.LogError("InteractionRoot prefab is not assigned in MarkerSpawnManager.");
            return;
        }

        spawnedInteractionRoot = Instantiate(interactionRootPrefab);
        Debug.Log("InteractionRoot spawned once.");
    }

    private void UpdateObjectTransform(ARTrackedImage trackedImage)
    {
        if (spawnedInteractionRoot == null)
        {
            Debug.LogWarning("Cannot update object transform because spawnedInteractionRoot is null.");
            return;
        }

        if (trackedImage == null)
        {
            Debug.LogWarning("Cannot update object transform because trackedImage is null.");
            return;
        }

        spawnedInteractionRoot.transform.SetParent(trackedImage.transform);
        spawnedInteractionRoot.transform.localPosition = localPositionOffset;
        spawnedInteractionRoot.transform.localRotation = Quaternion.Euler(localRotationOffset);
        spawnedInteractionRoot.transform.localScale = localScale;
        spawnedInteractionRoot.SetActive(true);
    }
}