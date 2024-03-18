using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MapLoader : MonoBehaviour
{
    public BuildingGenerator buildingGenerator; // Reference to BuildingGenerator script

    private string overpassApiUrl = "http://overpass-api.de/api/interpreter";
    private float latitude = 37.7749f;
    private float longitude = -122.4194f;
    private float radius = 50f;

    void Start()
    {
        StartCoroutine(GetOverpassData());
    }

    IEnumerator GetOverpassData()
    {
        string query = $"[out:json];(way[\"building\"](around:{radius},{latitude},{longitude}););out body;>;out skel qt;";
        string apiUrlWithQuery = overpassApiUrl + "?data=" + UnityWebRequest.EscapeURL(query);

        UnityWebRequest www = UnityWebRequest.Get(apiUrlWithQuery);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {www.error}");
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            // Pass the Overpass data to BuildingGenerator for further processing
            buildingGenerator.CreateBuildings(jsonData);
        }

        www.Dispose();
    }
}
