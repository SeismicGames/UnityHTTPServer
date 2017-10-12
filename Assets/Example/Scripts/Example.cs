using UnityEngine;
using UnityEngine.UI;
using UnityHTTP;

public class Example : MonoBehaviour
{
    public Text host;

    private bool _hasUpdateRun = false;

	void Start ()
	{
        if (host == null)
        {
            Debug.LogError("Host Text was not initialized");
            return;
        }

        host.text = HTTPServer.Instance.Host;
    }

    void Update()
    {
        if (!_hasUpdateRun)
        {
            Debug.LogError("Test log error");
            Debug.LogWarning("Test log warning");
            Debug.Log("Test log debug");

            _hasUpdateRun = true;
        }
    }
}
