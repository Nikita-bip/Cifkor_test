using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class WeatherView : MonoBehaviour, IWeatherView
{
    [SerializeField] private Text temperatureText;
    [SerializeField] private Text forecastText;
    [SerializeField] private Image weatherIcon;
    [SerializeField] private GameObject loader; // ��������� ��������

    private WeatherPresenter presenter;

    [Inject]
    public void Construct(WeatherPresenter presenter)
    {
        this.presenter = presenter;
    }

    private void OnEnable()
    {
        // ������ ������ ���������� ������ ��� ��������� �������
        presenter.StartWeatherUpdates();
    }

    private void OnDisable()
    {
        // ��������� ������ ��� ������������ �������
        presenter.StopWeatherUpdates();
    }

    public void UpdateWeather(WeatherModel model)
    {
        temperatureText.text = $"������� - {model.Temperature}";
        forecastText.text = model.Forecast;
        StartCoroutine(LoadWeatherIcon(model.IconUrl));
    }

    public void ShowError(string message)
    {
        temperatureText.text = "������";
        forecastText.text = message;
    }

    public void ShowLoader(bool isActive)
    {
        loader.SetActive(isActive);
    }

    public Coroutine StartWeatherCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopWeatherCoroutine(Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

    public void OnWeatherTabSelected()
    {
        presenter.OnWeatherTabSelected();
    }

    public void OnWeatherTabDeselected()
    {
        presenter.OnWeatherTabDeselected();
    }


    private IEnumerator LoadWeatherIcon(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("������ �������� ������: " + request.error);
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