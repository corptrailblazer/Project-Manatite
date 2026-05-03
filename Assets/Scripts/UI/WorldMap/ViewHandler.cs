using ProjectManatite.Core;
using UnityEngine;

/// <summary>
/// Hard-cuts the main camera between two world positions: a "main" view and an
/// arbitrary target view. Method names kept lower-case for backwards compat
/// with existing UnityEvent serialised bindings.
///
/// Refactor notes: <see cref="CameraCache.Main"/> replaces <c>Camera.main</c>
/// per call; null guards added for missing references.
/// </summary>
public class ViewHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainView;

    private static readonly Vector3 CameraZOffset = new(0f, 0f, -10f);

    public void returnToMainView(GameObject currentView)
    {
        if (mainView == null) return;
        var cam = CameraCache.Main;
        if (cam == null) return;
        cam.transform.position = mainView.transform.position + CameraZOffset;
    }

    public void goToView(GameObject targetView)
    {
        if (targetView == null) return;
        var cam = CameraCache.Main;
        if (cam == null) return;
        cam.transform.position = targetView.transform.position + CameraZOffset;
    }
}
