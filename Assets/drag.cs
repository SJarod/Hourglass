using UnityEngine;
using UnityEngine.InputSystem.Editor;
using UnityEngine.Rendering;

// TODO : compilation with ci/cd automation
// TODO : deployment to webgl

// TODO : physics
// TODO : mesh
// TODO : time prediction
// TODO : vfx sand
// TODO : counter
// TODO :   ajouter un counter
// TODO :       temps total passé
// TODO :       nombre de lancé

public class drag : MonoBehaviour
{
    private Vector3[] corners = new Vector3[4];

    private Vector3 localSpaceTarget = Vector3.zero;
    private Vector3 worldSpaceTarget = Vector3.zero;

    [SerializeField]
    private SpringJoint handObject = null;
    [SerializeField]
    private Vector3 grabbingPointOffset = Vector3.zero;

    [HideInInspector]
    public Rigidbody targetObject = null;

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

        Vector3 p = worldSpace;
        Vector3 dir = (p - Camera.main.transform.position).normalized;
        Debug.DrawLine(Camera.main.transform.position, p);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit, Camera.main.farClipPlane) &&
                hit.rigidbody)
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red, 2f);

                localSpaceTarget = hit.transform.InverseTransformPoint(hit.point);
                worldSpaceTarget = hit.transform.position + Vector3.Scale(localSpaceTarget, hit.transform.localScale);

                handObject.connectedBody = hit.rigidbody;
                handObject.anchor = localSpaceTarget;

                targetObject = hit.rigidbody;

                handObject.gameObject.transform.position = hit.transform.position + Vector3.up * 5f;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (targetObject)
            {
                targetObject = null;
                handObject.connectedBody = null;
                handObject.anchor = Vector3.zero;

                Debug.Log("releasing object");
            }
        }

        if (targetObject)
        {
            Vector3 vecToTarget = worldSpaceTarget - Camera.main.transform.position;
            Vector3 vecToFarClip = p - Camera.main.transform.position;

            Debug.Log(vecToTarget.magnitude + ", " + vecToFarClip.magnitude);

            float dot = Vector3.Dot(vecToFarClip, vecToTarget) / vecToFarClip.magnitude;

            Debug.Log(dot);

            Vector3 midPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dot));
            midPoint += grabbingPointOffset;

            // TODO : take hand rotation into account
            // TODO : constraints for hand rotation

            handObject.gameObject.transform.position = midPoint;
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

        Gizmos.DrawSphere(worldSpaceTarget, 0.5f);
    }
}
