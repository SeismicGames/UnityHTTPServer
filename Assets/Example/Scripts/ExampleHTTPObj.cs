using UnityHTTP;

public class ExampleHTTPObj : HTTPGameObj
{
    private ExampleHTTPObj _instance;

    protected override void AwakeServer()
    {
        if (_instance != null)
        {
            Destroy(gameObject); // There can be only one
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected override void DestroyServer()
    {
        if (_instance != this)
        {
            return;
        }

        _instance = null;
    }
}
