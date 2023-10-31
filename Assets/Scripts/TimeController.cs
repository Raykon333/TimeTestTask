using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class TimeController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ShowTime(string str);

    string timeString = "";

    private void Start()
    {
    }

    public void GetMoscowTime()
    {
        StartCoroutine(SyncTime());
    }

    IEnumerator SyncTime()
    {
        using (var request = UnityWebRequest.Get("http://worldtimeapi.org/api/timezone/Europe/Moscow.txt"))
        {
            yield return request.SendWebRequest();

            while (request.result == UnityWebRequest.Result.InProgress)
            {
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            var text = request.downloadHandler.text;
            var lines = text.Split('\n');
            var line = lines.FirstOrDefault(l => l.StartsWith("datetime: "));
            var timePos = line.IndexOf('T');
            timeString = line.Substring(timePos + 1, 8);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
                ShowTime(timeString);
            Debug.Log(timeString);
        }
    }
}
