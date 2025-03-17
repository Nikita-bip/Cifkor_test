using UnityEngine;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject weatherTab;
    [SerializeField] private GameObject factsTab;
    [SerializeField] private FactsRequest factsRequest;

    public bool isWeatherTabActive = true;

    public void ShowWeatherTab()
    {
        weatherTab.SetActive(true);
        factsTab.SetActive(false);
        isWeatherTabActive = true;
    }

    public void ShowFactsTab()
    {
        weatherTab.SetActive(false);
        factsTab.SetActive(true);
        isWeatherTabActive = false;
        factsRequest.OnFactsTabSelected();
    }
}
