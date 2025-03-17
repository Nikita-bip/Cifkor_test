using UnityEngine;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject weatherTab; // ������ ������� � �������
    [SerializeField] private GameObject factsTab;   // ������ ������� � �������
    [SerializeField] private WeatherView weatherView; // View ��� ������
    [SerializeField] private FactView factView;     // View ��� ������

    public bool isWeatherTabActive = true;

    public void ShowWeatherTab()
    {
        weatherTab.SetActive(true);
        factsTab.SetActive(false);
        isWeatherTabActive = true;

        // �������� ���������� ������
        weatherView.OnWeatherTabSelected();
    }

    public void ShowFactsTab()
    {
        weatherTab.SetActive(false);
        factsTab.SetActive(true);
        isWeatherTabActive = false;

        // ������������� ���������� ������
        weatherView.OnWeatherTabDeselected();

        // �������� ������
        factView.OnFactsTabSelected();
    }
}
