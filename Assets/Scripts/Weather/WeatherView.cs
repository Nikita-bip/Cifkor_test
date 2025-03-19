using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class WeatherView : MonoBehaviour, IWeatherView
{
    [SerializeField] private Text _temperatureText;
    [SerializeField] private Image _weatherIcon;
    [SerializeField] private GameObject _loader;

    private WeatherPresenter _presenter;

    [Inject]
    public void Construct(WeatherPresenter presenter)
    {
        this._presenter = presenter;
    }

    private void OnEnable()
    {
        _presenter.StartWeatherUpdates();
    }

    private void OnDisable()
    {
        _presenter.StopWeatherUpdates();
    }

    public void UpdateWeather(WeatherModel model)
    {
        _temperatureText.text = $"Сегодня - {model.Temperature}";
        StartCoroutine(LoadWeatherIcon(model.IconUrl));
    }

    public void ShowError(string message)
    {
        _temperatureText.text = "Ошибка";
    }

    public void ShowLoader(bool isActive)
    {
        _loader.SetActive(isActive);
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
        _presenter.OnWeatherTabSelected();
    }

    public void OnWeatherTabDeselected()
    {
        _presenter.OnWeatherTabDeselected();
    }


    private IEnumerator LoadWeatherIcon(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        _weatherIcon.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}