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

        host.text = HTTPLogServer.Instance.Host;
    }
}
