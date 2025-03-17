using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherPresenter
{
    private readonly IWeatherView view;
    private readonly string weatherApiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    private Coroutine weatherCoroutine;
    private bool isWeatherActive = false;
    private UnityWebRequestAsyncOperation currentRequest;

    public WeatherPresenter(IWeatherView view)
    {
        this.view = view;
    }

    public void OnWeatherTabSelected()
    {
        isWeatherActive = true;
        StartWeatherUpdates();
    }

    public void OnWeatherTabDeselected()
    {
        isWeatherActive = false;
        StopWeatherUpdates();
    }

    public void StartWeatherUpdates()
    {
        if (weatherCoroutine == null)
        {
            // ��������� �������� ������ ���� ������� �������
            weatherCoroutine = view.StartWeatherCoroutine(WeatherRequestLoop());
        }
    }

    public void StopWeatherUpdates()
    {
        if (weatherCoroutine != null)
        {
            // ������������� ��������, ���� ������� ����� ����������
            view.StopWeatherCoroutine(weatherCoroutine);
            weatherCoroutine = null;
        }

        // �������� ������� ������, ���� �� �����������
        if (currentRequest != null)
        {
            currentRequest.webRequest.Abort();
            Debug.Log("Weather request canceled.");
        }
    }

    private IEnumerator WeatherRequestLoop()
    {
        while (isWeatherActive)
        {
            yield return GetWeather();
            yield return new WaitForSeconds(5f); // ����� ����� ���������
        }
    }

    private IEnumerator GetWeather()
    {
        // ���������� ��������� ��������
        view.ShowLoader(true);

        UnityWebRequest request = UnityWebRequest.Get(weatherApiUrl);
        currentRequest = request.SendWebRequest();
        yield return currentRequest;

        // ������ ��������� ��������
        view.ShowLoader(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Weather request error: " + request.error);
            view.ShowError("������ �������� ������.");
            yield break;
        }

        Debug.Log("Weather Response: " + request.downloadHandler.text);

        // �������������� ������
        try
        {
            WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);
            WeatherModel model = GetWeatherModelFromResponse(response);
            view.UpdateWeather(model);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing weather JSON: " + ex.Message);
            view.ShowError("������ ��������� ������.");
        }
    }

    private WeatherModel GetWeatherModelFromResponse(WeatherResponse response)
    {
        System.DateTime now = System.DateTime.UtcNow;

        foreach (var period in response.properties.periods)
        {
            System.DateTime startTime = System.DateTime.Parse(period.startTime).ToUniversalTime();
            System.DateTime endTime = System.DateTime.Parse(period.endTime).ToUniversalTime();

            if (now >= startTime && now < endTime)
            {
                return new WeatherModel
                {
                    Temperature = $"{period.temperature}{period.temperatureUnit}",
                    Forecast = period.shortForecast,
                    IconUrl = period.icon
                };
            }
        }

        throw new System.Exception("No matching weather period found!");
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