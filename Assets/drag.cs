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
        // https://www.reddit.com/r/gamedev/comments/10izurv/screen_to_world_coordinates/
        // https://learnopengl.com/Getting-started/Coordinate-Systems

        Vector3 screenSpace = Input.mousePosition;
        Vector3 clipSpace = Vector3.zero;
        // invert axis
        clipSpace.x = -(2f * (screenSpace.x / Screen.currentResolution.width) - 1f);
        clipSpace.y = -(2f * (screenSpace.y / Screen.currentResolution.height) - 1f);
        // revert perspective division
        clipSpace.z = -(1f * Camera.main.farClipPlane);
        Matrix4x4 projection = Matrix4x4.Perspective(Camera.main.fieldOfView, Camera.main.aspect, Camera.main.nearClipPlane, Camera.main.farClipPlane);
        Vector4 viewSpace = projection.inverse * new Vector4(clipSpace.x * clipSpace.z, clipSpace.y * clipSpace.z, clipSpace.z, clipSpace.z);
        Matrix4x4 view = Camera.main.transform.worldToLocalMatrix;
        // vector4.w = 1f
        Vector4 worldSpace = view.inverse * new Vector4(viewSpace.x, viewSpace.y, viewSpace.z, 1f);

        Vector3 p = new Vector3(worldSpace.x, worldSpace.y, worldSpace.z);
        Vector3 dir = (p - Camera.main.transform.position).normalized;
        Debug.DrawLine(Camera.main.transform.position, new Vector3(worldSpace.x, worldSpace.y, worldSpace.z));

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Camera.main.farClipPlane))
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 2f);
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
