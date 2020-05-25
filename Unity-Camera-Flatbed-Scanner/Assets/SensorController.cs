using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SensorController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI x_Value;
    [SerializeField] TextMeshProUGUI y_Value;
    [SerializeField] TextMeshProUGUI z_Value;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Vector3 sense = Input.acceleration;
        x_Value.text = System.Math.Round(sense.x, 2).ToString();
        y_Value.text = System.Math.Round(sense.y, 2).ToString();
        z_Value.text = System.Math.Round(sense.z, 2).ToString();


    }
    void GyroModifyCamera()
    {
        transform.rotation = GyroToUnity(Input.gyro.attitude);
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
