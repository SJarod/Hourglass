using UnityEngine;
using UnityEngine.Rendering;

public class drag : MonoBehaviour
{
    Vector3[] corners = new Vector3[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rect extent = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height);
        extent = new Rect(0f, 0f, 1f, 1f);
        Camera.main.CalculateFrustumCorners(extent, Camera.main.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);

        // https://codersdesiderata.com/2016/09/10/screen-view-to-world-coordinates/
        // https://antongerdelan.net/opengl/raycasting.html

        Vector3 mousePos = Input.mousePosition;
        mousePos.x = 2f * (mousePos.x / Screen.currentResolution.width) - 1f;
        mousePos.y = 2f * (mousePos.y / Screen.currentResolution.height) - 1f;
        Matrix4x4 pv = Camera.main.previousViewProjectionMatrix;
        Vector3 transformed = pv.inverse * mousePos;

        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + transformed);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawLine(Camera.main.transform.position, hit.transform.position, Color.red, 2f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Color c = Gizmos.color;
        Gizmos.color = new Color(c.r, c.g, 0f, 1f);
        for (int i = 0; i < 4; ++i)
        {
            Gizmos.color = new Color(c.r, c.g, i / 4f, 1f);
            Gizmos.DrawSphere(Camera.main.transform.rotation * corners[i] + Camera.main.transform.position, 1f);
        }
    }
}
