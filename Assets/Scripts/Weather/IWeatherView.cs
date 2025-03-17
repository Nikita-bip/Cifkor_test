using System.Collections;
using UnityEngine;

public interface IWeatherView
{
    void UpdateWeather(WeatherModel model);
    void ShowError(string message);
    void ShowLoader(bool isActive);
    Coroutine StartWeatherCoroutine(IEnumerator routine);
    void StopWeatherCoroutine(Coroutine routine);
}