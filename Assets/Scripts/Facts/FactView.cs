using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FactView : MonoBehaviour, IFactView
{
    [SerializeField] private GameObject _loader;
    [SerializeField] private GameObject _popup;
    [SerializeField] private TextMeshProUGUI _popupTitle;
    [SerializeField] private TextMeshProUGUI _popupDescription;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private ButtonPool _buttonPool;

    private FactPresenter _presenter;

    [Inject]
    public void Construct(FactPresenter presenter)
    {
        this._presenter = presenter;
    }

    public void OnFactsTabSelected()
    {
        _presenter.LoadFacts();
    }

    public void DisplayFacts(FactModel[] facts)
    {
        // Очистка существующих кнопок
        foreach (Transform child in _buttonContainer)
        {
            var button = child.gameObject;
            _buttonPool.ReturnButton(button); // Возвращаем в пул
        }

        // Создание новых кнопок
        for (int i = 0; i < facts.Length; i++)
        {
            var fact = facts[i];
            var buttonObj = _buttonPool.GetButton();
            buttonObj.transform.SetParent(_buttonContainer, false);

            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = $"{i + 1} - {fact.Name}";

            var button = buttonObj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _presenter.OnFactSelected(fact));
        }
    }


    public void ShowPopup(string title, string description)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
        {
            Debug.LogWarning("Popup data is missing.");
        }

        _popupTitle.text = title ?? "No Title Available";
        _popupDescription.text = description ?? "No Description Available.";
        _popup.SetActive(true);
    }

    public void ClosePopup()
    {
        _popup.SetActive(false);
    }

    public void ShowLoader(bool isActive)
    {
        _loader.SetActive(isActive);
    }

    public void ShowError(string message)
    {
        Debug.LogError(message);
    }

    public Coroutine StartFactCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopFactCoroutine(Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }
}