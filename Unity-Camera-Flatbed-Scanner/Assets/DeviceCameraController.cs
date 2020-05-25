using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class DeviceCameraController : MonoBehaviour
{
    public RawImage image;
    public RectTransform imageParent;
    public AspectRatioFitter imageFitter;
    [SerializeField] Button captureButton;

    // Device cameras
    WebCamDevice frontCameraDevice;
    WebCamDevice backCameraDevice;
    WebCamDevice activeCameraDevice;

    WebCamTexture frontCameraTexture;
    WebCamTexture backCameraTexture;
    WebCamTexture activeCameraTexture;

    // Image rotation
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
    Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
    Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    Vector3 fixedScale = new Vector3(-1f, 1f, 1f);


    void Start()
    {
       // captureButton.onClick.AddListener(delegate { TakeNativeCamPicture(1024); });

        captureButton.onClick.AddListener(takeLivePicture);

        // Check for device cameras
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("No devices cameras found");
            return;
        }

        // Get the device's cameras and create WebCamTextures with them
        frontCameraDevice = WebCamTexture.devices.Last();
        backCameraDevice = WebCamTexture.devices.First();

        frontCameraTexture = new WebCamTexture(frontCameraDevice.name , 2160/2,4320/2,120);
        backCameraTexture = new WebCamTexture(backCameraDevice.name, 2160/2, 4320/2, 120);

        // Set camera filter modes for a smoother looking image
        frontCameraTexture.filterMode = FilterMode.Trilinear;
        backCameraTexture.filterMode = FilterMode.Trilinear;

        // Set the camera to use by default
        //SetActiveCamera(frontCameraTexture);
        SetActiveCamera(backCameraTexture);
    }

    // Set the device camera to use and start it
    public void SetActiveCamera(WebCamTexture cameraToUse)
    {
        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();
        }

        activeCameraTexture = cameraToUse;
        activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
            device.name == cameraToUse.deviceName);

        image.texture = activeCameraTexture;
        image.material.mainTexture = activeCameraTexture;

        activeCameraTexture.Play();
    }

    // Switch between the device's front and back camera
    public void SwitchCamera()
    {
        SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ?
            backCameraTexture : frontCameraTexture);
    }

    // Make adjustments to image every frame to be safe, since Unity isn't 
    // guaranteed to report correct data as soon as device camera is started
    void Update()
    {
        // Skip making adjustment for incorrect camera data
        if (activeCameraTexture.width < 100)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        rotationVector.z = -activeCameraTexture.videoRotationAngle;
        image.rectTransform.localEulerAngles = rotationVector;

        // Set AspectRatioFitter's ratio
         float videoRatio =
            (float)activeCameraTexture.width / (float)activeCameraTexture.height;
        imageFitter.aspectRatio = videoRatio;

        // Unflip if vertically flipped
        image.uvRect =
            activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        imageParent.localScale =
            activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
    }
    private void TakeNativeCamPicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                NativeGallery.SaveImageToGallery(texture, "MyGallery", "Image.png");
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }
    private void takeLivePicture()
    {
        // Create a Texture2D from the captured image
       // Texture2D texture = new Texture2D(Screen.width, Screen.height);
       // texture.SetPixels(activeCameraTexture.GetPixels());
       // texture.Apply();

        Texture2D texture = new Texture2D(image.texture.width, image.texture.height, TextureFormat.ARGB32, false);

        //Save the image to the Texture2D
        texture.SetPixels(activeCameraTexture.GetPixels());

        texture.Apply();

        NativeGallery.SaveImageToGallery(texture, "MyGallery", "Image.png");
            
    }
}