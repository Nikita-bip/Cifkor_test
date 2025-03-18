using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FactView : MonoBehaviour, IFactView
{
    [SerializeField] private GameObject loader; // ��������� ��������
    [SerializeField] private GameObject popup; // ����� ��� ������
    [SerializeField] private TextMeshProUGUI popupTitle; // ��������� ������
    [SerializeField] private TextMeshProUGUI popupDescription; // �������� ������
    [SerializeField] private Transform buttonContainer; // ��������� ��� ������
    [SerializeField] private GameObject buttonPrefab; // ������ ������
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

            // ������������� ����� ������ � ������� "1 - name"
            buttonText.text = $"{i + 1} - {fact.Name}";

            // ��������� ������� ��� ������
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
