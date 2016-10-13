using UnityEngine;

//This script needs to be on an empty GameObject parenting the Main Camera.
public class CameraController : MonoBehaviour {
    //TODO: Set values changeable by the player to public
    private float yRotInvert;
    private float xRotInvert;
    private float xRotSpeed = 1.5f;
    private float yRotSpeed = 1.5f;

    [SerializeField]
    private float maxRot = 80, minRot = 20;

    private float xTransInvert;
    private float yTransInvert;
    private float xTransSpeed = 0.5f;
    private float yTransSpeed = 0.5f;
    private float scrollSpeed = 10;
    private float maxScroll = 30; //The furthest the camera can go
    private float minScroll = 1; //The closest the camera can go
    private Transform camTransform;

    public bool isYRotInverted;
    public bool isXRotInverted;
    public bool isXTransInverted;
    public bool isYTransInverted;
    public bool isPanWithMouse;
    public bool isPanWithKB;

    void Start() {
        yRotInvert = isYRotInverted ? 1 : -1;
        xRotInvert = isXRotInverted ? -1 : 1;
        xTransInvert = isXTransInverted ? -1 : 1;
        yTransInvert = isYTransInverted ? -1 : 1;

        camTransform = Camera.main.transform;
    }

    void Update() {
        //Move camera with translational movement
        if (Input.GetMouseButton(0) && isPanWithMouse) {
            Pan("Mouse X", "Mouse Y", 1, 1);
        }

        //Rotate camera around a point in the center of the screen
        if (Input.GetMouseButton(1)) {
            Rotate();
        }

        //Zoom in or out
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) {
            Zoom();
        }

        //Move camera with translational movement using WASD or movement keys
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && isPanWithKB) {
            Pan("Horizontal", "Vertical", -1, -1);
        }
    }

    private void Pan(string x, string y, float invertX, float invertY) {
        //Pan around
        transform.Translate(new Vector3(-Input.GetAxisRaw(x) * xTransSpeed * xTransInvert * invertX,
            -Input.GetAxisRaw(y) * yTransSpeed * yTransInvert * invertY, 0));

        //Limits the camera to only be at the active dungeon layer; can't go above or below that layer
        transform.position = new Vector3(transform.position.x, 3, transform.position.z);
    }

    private void Rotate() {
        //Rotate around
        transform.Rotate(new Vector3(Input.GetAxisRaw("Mouse Y") * yRotInvert * yRotSpeed, Input.GetAxisRaw("Mouse X") * xRotInvert * xRotSpeed, 0));
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

        //Limits the rotation along the x axis (the up and down rotation)
        if (transform.localEulerAngles.x > maxRot) {
            Debug.LogError("Maxed " + transform.localEulerAngles.x);
            transform.localEulerAngles = new Vector3(maxRot, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        if (transform.localEulerAngles.x < minRot) {
            Debug.Log("Mined " + transform.localEulerAngles.x);
            transform.localEulerAngles = new Vector3(minRot, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

    private void Zoom() {
        //Zoom
        camTransform.localPosition += new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed);

        //Setting the limits of zoom
        //The inequality symbols are opposite because we're dealing with negative numbers
        if (camTransform.localPosition.z < -maxScroll) {
            camTransform.localPosition = new Vector3(0, 0, maxScroll);
        }

        if (camTransform.localPosition.z > -minScroll) {
            camTransform.localPosition = new Vector3(0, 0, minScroll);
        }
    }
}
