using UnityEngine;
using UnityEngine.UI;
using UnityHTTP;

public class Example : MonoBehaviour
{
    public Text host;

	void Start ()
	{
        if (host == null)
        {
            Debug.LogError("Host Text was not initialized");
            return;
        }

        host.text = HTTPServer.Instance.Host;

	    Debug.LogError("Test log error");
	    Debug.LogWarning("Test log warning");
	    Debug.Log("Test log debug");
    }
}
