using UnityEngine;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject weatherTab; // Панель вкладки с погодой
    [SerializeField] private GameObject factsTab;   // Панель вкладки с фактами
    [SerializeField] private WeatherView weatherView; // View для погоды
    [SerializeField] private FactView factView;     // View для фактов

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
