using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private string weatherApiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    [SerializeField] private Text temperatureText; // Текстовое поле для вывода температуры
    [SerializeField] private Text forecastText; // Текстовое поле для прогноза
    [SerializeField] private Image weatherIcon; // Иконка погоды
    [SerializeField] private GameObject loader; // Индикатор загрузки
    [SerializeField] private TabManager tabManager; // Ссылка на менеджер вкладок

    private UnityWebRequestAsyncOperation currentRequest; // Хранение текущего запроса
    private Coroutine weatherCoroutine; // Запущенная корутина для периодических запросов

    private void OnEnable()
    {
        // Запускаем периодические запросы на получение погоды
        weatherCoroutine = StartCoroutine(WeatherRequestLoop());
    }

    private void OnDisable()
    {
        // Останавливаем запросы при отключении вкладки
        if (weatherCoroutine != null)
        {
            StopCoroutine(weatherCoroutine);
            weatherCoroutine = null;
        }

        // Отменяем текущий запрос, если он активен
        if (currentRequest != null)
        {
            currentRequest.webRequest.Abort();
            Debug.Log("Weather request canceled.");
        }
    }

    private IEnumerator WeatherRequestLoop()
    {
        // Цикл запросов каждые 5 секунд
        while (true)
        {
            if (tabManager.isWeatherTabActive)
            {
                yield return StartCoroutine(GetWeather());
            }

            yield return new WaitForSeconds(5f); // Пауза между запросами
        }
    }

    private IEnumerator GetWeather()
    {
        // Если есть текущий запрос, отменяем его
        if (currentRequest != null)
        {
            currentRequest.webRequest.Abort();
            Debug.Log("Previous weather request aborted.");
        }

        // Показать индикатор загрузки
        loader.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get(weatherApiUrl);
        currentRequest = request.SendWebRequest(); // Сохраняем текущий запрос
        yield return currentRequest;

        // Скрыть загрузчик
        loader.SetActive(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Weather request error: " + request.error);
            yield break;
        }

        Debug.Log("Weather Response: " + request.downloadHandler.text);

        // Десериализация данных
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
        // Получение текущего времени UTC
        System.DateTime now = System.DateTime.UtcNow;

        foreach (var period in weatherResponse.properties.periods)
        {
            // Парсим время и конвертируем в UTC
            System.DateTime startTime = System.DateTime.Parse(period.startTime).ToUniversalTime();
            System.DateTime endTime = System.DateTime.Parse(period.endTime).ToUniversalTime();

            // Лог проверки
            Debug.Log($"Checking period: {startTime} - {endTime}, Now: {now}");

            // Найден период, соответствующий текущему времени
            if (now >= startTime && now < endTime)
            {
                temperatureText.text = $"Сегодня - {period.temperature}{period.temperatureUnit}";
                forecastText.text = period.shortForecast;

                // Загружаем иконку погоды
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