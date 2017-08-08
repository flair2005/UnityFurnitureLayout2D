using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// GameビューにてSceneビューのようなカメラの動きをマウス操作によって実現する
/// </summary>
[RequireComponent(typeof(Camera))]
public class SceneViewCamera : MonoBehaviour
{
    public UIScript uiscript;
    public Slider angleSlider;
    public Slider tateAngleSlider;
    bool ahead;
    bool right;
    bool left;
    bool back;
    Vector3 frontWay0, rightWay0, frontWay, rightWay;

    [SerializeField, Range(0.1f, 10f)]
    private float wheelSpeed = 1f;

    [SerializeField, Range(0.1f, 10f)]
    private float moveSpeed = 0.3f;

    [SerializeField, Range(0.1f, 10f)]
    private float rotateSpeed = 0.3f;

    private Vector3 preMousePos;

    private void Start()
    {
        ahead = false;
        right = false;
        left = false;
        back = false;
        angleSlider.value = 0.5f;
        tateAngleSlider.value = 0.5f;
        frontWay = new Vector3(0.0f, 0.0f, 1.0f);
        rightWay = new Vector3(1.0f, 0.0f, 0.0f);
        frontWay0 = frontWay;
        rightWay0 = rightWay;
    }

    private void Update()
    {
        if (uiscript.view3Dmode)
        {
            getAngleSliderToCameraRotation();
            //MouseUpdate();
        }

        if (ahead)
        {
            transform.position += frontWay * 0.1f;
        }

        if (right)
        {
            transform.position += rightWay * 0.1f;
        }

        if (left)
        {
            transform.position += rightWay * (-0.1f);
        }

        if (back)
        {
            transform.position += frontWay * (-0.1f);
        }

        return;
    }

    private void MouseUpdate()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0.0f)
            MouseWheel(scrollWheel);

        if (Input.GetMouseButtonDown(0) ||
           Input.GetMouseButtonDown(1) ||
           Input.GetMouseButtonDown(2))
            preMousePos = Input.mousePosition;

        MouseDrag(Input.mousePosition);
    }

    private void MouseWheel(float delta)
    {
        transform.position += transform.forward * delta * wheelSpeed;
        return;
    }

    private void MouseDrag(Vector3 mousePos)
    {
        Vector3 diff = mousePos - preMousePos;

        if (diff.magnitude < Vector3.kEpsilon)
            return;

        if (Input.GetMouseButton(2))
            transform.Translate(-diff * Time.deltaTime * moveSpeed);
        else if (Input.GetMouseButton(0))
            CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);

        preMousePos = mousePos;
    }

    public void CameraRotate(Vector2 angle)
    {
        transform.RotateAround(transform.position, transform.right, angle.x);
        transform.RotateAround(transform.position, Vector3.up, angle.y);
    }

    public void getAngleSliderToCameraRotation()
    {
        float angle = -180.0f + angleSlider.value * 360.0f;
        float tateAngle = 90.0f - tateAngleSlider.value * 180.0f;
        transform.eulerAngles = new Vector3(tateAngle, angle, 0.0f);
        Quaternion rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        Matrix4x4 m = Matrix4x4.identity;
        m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
        frontWay = m.MultiplyPoint3x4(frontWay0);
        rightWay = m.MultiplyPoint3x4(rightWay0);
    }

    public void GoAheadDownButton()
    {
        ahead = true;
    }

    public void GoAheadUPButton()
    {
        ahead = false;
    }

    public void GoRightDownButton()
    {
        right = true;
    }

    public void GoRightUPButton()
    {
        right = false;
    }

    public void GoLeftDownButton()
    {
        left = true;
    }

    public void GoLeftUpButton()
    {
        left = false;
    }

    public void GoBackDownButton()
    {
        back = true;
    }

    public void GoBackUPButton()
    {
        back = false;
    }
}
