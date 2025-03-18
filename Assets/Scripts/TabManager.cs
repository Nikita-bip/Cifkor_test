using UnityEngine;
using Zenject;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject _weatherTab;
    [SerializeField] private GameObject _factsTab;
    [SerializeField] private WeatherView _weatherView;
    [SerializeField] private FactView _factView;

    public bool isWeatherTabActive = true;
    private FactPresenter _factPresenter;

    [Inject]
    public void Construct(FactPresenter factPresenter)
    {
        this._factPresenter = factPresenter;
    }

    public void ShowWeatherTab()
    {
        _weatherTab.SetActive(true);
        _factsTab.SetActive(false);
        isWeatherTabActive = true;

        _factPresenter.CancelFactRequest();
        _weatherView.OnWeatherTabSelected();
    }

    public void ShowFactsTab()
    {
        _weatherTab.SetActive(false);
        _factsTab.SetActive(true);
        isWeatherTabActive = false;

        _weatherView.OnWeatherTabDeselected();
        _factView.OnFactsTabSelected();
    }
}