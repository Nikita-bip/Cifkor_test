using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FactView : MonoBehaviour, IFactView
{
    [SerializeField] private GameObject loader; // Индикатор загрузки
    [SerializeField] private GameObject popup; // Попап для фактов
    [SerializeField] private TextMeshProUGUI popupTitle; // Заголовок попапа
    [SerializeField] private TextMeshProUGUI popupDescription; // Описание попапа
    [SerializeField] private Transform buttonContainer; // Контейнер для кнопок
    [SerializeField] private GameObject buttonPrefab; // Префаб кнопки
    private FactPresenter presenter;

    [Inject]
    public void Construct(FactPresenter presenter)
    {
        this.presenter = presenter;
    }

    public void OnFactsTabSelected()
    {
        presenter.LoadFacts();
    }

    public void DisplayFacts(FactModel[] facts)
    {
        for (int i = 0; i < facts.Length; i++)
        {
            var fact = facts[i];
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            // Устанавливаем текст кнопки в формате "1 - name"
            buttonText.text = $"{i + 1} - {fact.Name}";

            // Добавляем событие для кнопки
            button.onClick.AddListener(() =>
            {
                presenter.OnFactSelected(fact);
            });
        }
    }

    public void ShowPopup(string title, string description)
    {
        popupTitle.text = title;
        popupDescription.text = description;
        popup.SetActive(true);
    }

    public void ClosePopup()
    {
        popup.SetActive(false);
    }

    public void ShowLoader(bool isActive)
    {
        loader.SetActive(isActive);
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
