using UnityEngine;
using System.Collections;

public class ViewHandler : MonoBehaviour
{

    [SerializeField] GameObject mainView;

    public void returnToMainView(GameObject currentView)
    {
        Camera.main.transform.position = mainView.transform.position + new Vector3(0f,0f, -10f);
        

    }

    public void goToView(GameObject targetView)
    {
        Camera.main.transform.position = targetView.transform.position + new Vector3(0f,0f, -10f);
    }

}
