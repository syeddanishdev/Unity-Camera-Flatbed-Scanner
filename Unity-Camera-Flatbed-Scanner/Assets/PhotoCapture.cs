using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class PhotoCapture : MonoBehaviour
{
    WebCamTexture webCamTexture;
    [SerializeField] RawImage image;
    // Image rotation
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);



    void Start()
    {
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height, 60);
        image.texture = webCamTexture;
        image.material.mainTexture = webCamTexture;
        webCamTexture.filterMode = FilterMode.Trilinear;
        webCamTexture.Play();
    }
    private void Update()
    {
        // Skip making adjustment for incorrect camera data
        if (webCamTexture.width < 100)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        rotationVector.z = -webCamTexture.videoRotationAngle;
        //image.rectTransform.localEulerAngles = rotationVector;
    }
    /*
    void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), webCamTexture);
        }
    }
    */

}
