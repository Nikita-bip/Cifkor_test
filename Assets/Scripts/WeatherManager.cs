using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private string weatherApiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    [SerializeField] private Text temperatureText; // ��������� ���� ��� ������ �����������
    [SerializeField] private Text forecastText; // ��������� ���� ��� ��������
    [SerializeField] private Image weatherIcon; // ������ ������
    [SerializeField] private GameObject loader; // ��������� ��������
    [SerializeField] private TabManager tabManager; // ������ �� �������� �������

    private UnityWebRequestAsyncOperation currentRequest; // �������� �������� �������
    private Coroutine weatherCoroutine; // ���������� �������� ��� ������������� ��������

    private void OnEnable()
    {
        // ��������� ������������� ������� �� ��������� ������
        weatherCoroutine = StartCoroutine(WeatherRequestLoop());
    }

    private void OnDisable()
    {
        // ������������� ������� ��� ���������� �������
        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
            weatherCoroutine = null;
        }

        // �������� ������� ������, ���� �� �������
        if (currentRequest != null)
        {
            currentRequest.webRequest.Abort();
            Debug.Log("Weather request canceled.");
        }
    }

    private IEnumerator WeatherRequestLoop()
    {
        // ���� �������� ������ 5 ������
        while (true)
        {
            if (tabManager.isWeatherTabActive)
            {
                yield return StartCoroutine(GetWeather());
            }

            yield return new WaitForSeconds(5f); // ����� ����� ���������
        }
    }

    private IEnumerator GetWeather()
    {
        // ���� ���� ������� ������, �������� ���
        if (currentRequest != null)
        {
            currentRequest.webRequest.Abort();
            Debug.Log("Previous weather request aborted.");
        }

        // �������� ��������� ��������
        loader.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get(weatherApiUrl);
        currentRequest = request.SendWebRequest(); // ��������� ������� ������
        yield return currentRequest;

        // ������ ���������
        loader.SetActive(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Weather request error: " + request.error);
            yield break;
        }

        Debug.Log("Weather Response: " + request.downloadHandler.text);

        // �������������� ������
        try
        {
            WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);
            UpdateWeatherUI(weatherResponse);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing weather JSON: " + ex.Message);
        }
    }

    private void UpdateWeatherUI(WeatherResponse weatherResponse)
    {
        // ��������� �������� ������� UTC
        System.DateTime now = System.DateTime.UtcNow;

        foreach (var period in weatherResponse.properties.periods)
        {
            // ������ ����� � ������������ � UTC
            System.DateTime startTime = System.DateTime.Parse(period.startTime).ToUniversalTime();
            System.DateTime endTime = System.DateTime.Parse(period.endTime).ToUniversalTime();

            // ��� ��������
            Debug.Log($"Checking period: {startTime} - {endTime}, Now: {now}");

            // ������ ������, ��������������� �������� �������
            if (now >= startTime && now < endTime)
            {
                temperatureText.text = $"������� - {period.temperature}{period.temperatureUnit}";
                forecastText.text = period.shortForecast;

                // ��������� ������ ������
                StartCoroutine(LoadWeatherIcon(period.icon));
                return;
            }
        }

        Debug.LogWarning("No matching weather period found!");
    }


    private IEnumerator LoadWeatherIcon(string iconUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(iconUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Icon download error: " + request.error);
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        weatherIcon.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}

[System.Serializable]
public class WeatherResponse
{
    public WeatherProperties properties;
}

[System.Serializable]
public class WeatherProperties
{
    public WeatherPeriod[] periods;
}

[System.Serializable]
public class WeatherPeriod
{
    public string startTime;
    public string endTime;
    public int temperature;
    public string temperatureUnit;
    public string shortForecast;
    public string icon;
}