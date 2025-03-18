using UnityEngine;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject weatherTab;
    [SerializeField] private GameObject factsTab;
    [SerializeField] private WeatherView weatherView;
    [SerializeField] private FactView factView;    

    public bool isWeatherTabActive = true;

    public void ShowWeatherTab()
    {
        weatherTab.SetActive(true);
        factsTab.SetActive(false);
        isWeatherTabActive = true;

        // Включаем обновление погоды
        weatherView.OnWeatherTabSelected();
    }

    public void ShowFactsTab()
    {
        weatherTab.SetActive(false);
        factsTab.SetActive(true);
        isWeatherTabActive = false;

        // Останавливаем обновление погоды
        weatherView.OnWeatherTabDeselected();

        // Загрузка фактов
        factView.OnFactsTabSelected();
    }
}
