using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WeatherPresenter
{
    private Coroutine _weatherCoroutine;
    private bool _isWeatherActive = false;
    private UnityWebRequestAsyncOperation _currentRequest;
    private readonly IWeatherView _view;
    private readonly string _weatherApiUrl;

    [Inject]
    public WeatherPresenter(IWeatherView view, [Inject(Id = "WeatherApiUrl")] string apiUrl)
    {
        this._view = view;
        this._weatherApiUrl = apiUrl;
    }

    public void OnWeatherTabSelected()
    {
        _isWeatherActive = true;
        StartWeatherUpdates();
    }

    public void OnWeatherTabDeselected()
    {
        _isWeatherActive = false;
        StopWeatherUpdates();
    }

    public void StartWeatherUpdates()
    {
        if (_weatherCoroutine == null)
        {
            _weatherCoroutine = _view.StartWeatherCoroutine(WeatherRequestLoop());
        }
    }

    public void StopWeatherUpdates()
    {
        if (_weatherCoroutine != null)
        {
            _view.StopWeatherCoroutine(_weatherCoroutine);
            _weatherCoroutine = null;
        }

        if (_currentRequest != null)
        {
            _currentRequest.webRequest.Abort();
        }
    }

    private IEnumerator WeatherRequestLoop()
    {
        while (_isWeatherActive)
        {
            yield return GetWeather();
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator GetWeather()
    {
        if (_currentRequest != null)
        {
            _currentRequest.webRequest.Abort();
        }

        _view.ShowLoader(true);

        UnityWebRequest request = UnityWebRequest.Get(_weatherApiUrl);
        _currentRequest = request.SendWebRequest();
        yield return _currentRequest;

        _view.ShowLoader(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            _view.ShowError("Ошибка загрузки данных.");
            yield break;
        }

        try
        {
            WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);
            WeatherModel model = GetWeatherModelFromResponse(response);
            _view.UpdateWeather(model);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing weather JSON: " + ex.Message);
            _view.ShowError("Ошибка обработки данных.");
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