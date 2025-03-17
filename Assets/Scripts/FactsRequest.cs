using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FactsRequest : MonoBehaviour
{
    [SerializeField] private string baseUrl = "https://dogapi.dog/api/v2/breeds";
    [SerializeField] private GameObject loader; // ��������� ��������
    [SerializeField] private GameObject popup; // ����� ��� ������
    [SerializeField] private TextMeshProUGUI popupTitle; // ��������� ������
    [SerializeField] private TextMeshProUGUI popupDescription; // �������� � ������
    [SerializeField] private Transform buttonContainer; // ��������� ��� ������
    [SerializeField] private GameObject buttonPrefab; // ������ ������

    private UnityWebRequestAsyncOperation currentRequest; // ��� ������������ �������� �������

    private void Start()
    {
        // ��������� ������ ������ � ������� ������
        StartCoroutine(LoadFacts());
    }

    private IEnumerator LoadFacts()
    {
        loader.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get(baseUrl);
        yield return request.SendWebRequest();

        loader.SetActive(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request error: " + request.error);
            yield break;
        }

        Debug.Log("Full Response: " + request.downloadHandler.text);

        try
        {
            BreedWrapper breedWrapper = JsonUtility.FromJson<BreedWrapper>(request.downloadHandler.text);

            if (breedWrapper == null || breedWrapper.data == null || breedWrapper.data.Length == 0)
            {
                Debug.LogError("Data array is null or empty!");
                yield break;
            }

            Debug.Log($"Number of breeds: {breedWrapper.data.Length}");

            for (int i = 0; i < Mathf.Min(10, breedWrapper.data.Length); i++)
            {
                CreateButton(i + 1, breedWrapper.data[i]);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
        }
    }


    //private void CreateButton(int index, Breed breed)
    //{
    //    GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
    //    Button button = buttonObj.GetComponent<Button>();
    //    TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

    //    // ������������� ����� ������
    //    buttonText.text = $"{index} - {breed.attributes.name}";

    //    // ��������� ������� ��� ������, ������� GetFactById
    //    button.onClick.AddListener(() => StartCoroutine(GetFactById(breed.id)));
    //}

    private void CreateButton(int index, Breed breed)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        // ������������� ����� ������
        buttonText.text = $"{index} - {breed.attributes.name}";

        // ��������� ������� ��� ������
        button.onClick.AddListener(() =>
        {
            string title = string.IsNullOrEmpty(breed.attributes.name) ? "No Title Available" : breed.attributes.name;
            string description = string.IsNullOrEmpty(breed.attributes.description) ? "No Description Available." : breed.attributes.description;

            Debug.Log($"Title: {title}, Description: {description}");
            ShowPopup(title, description);
        });
    }


    private IEnumerator GetFactById(string factId)
    {
        string url = $"{baseUrl}/{factId}";

        // ���� ���� ������� ������, �������� ���
        if (currentRequest != null)
        {
            currentRequest.webRequest.Abort();
            Debug.Log("Previous request aborted.");
        }

        // �������� ���������
        loader.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get(url);
        currentRequest = request.SendWebRequest(); // ��������� ������� ������
        yield return currentRequest;

        // ������ ���������
        loader.SetActive(false);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request error: " + request.error);
            yield break;
        }

        Debug.Log("Fact Response: " + request.downloadHandler.text);

        // �������������� � ����� ������
        try
        {
            BreedWrapper factWrapper = JsonUtility.FromJson<BreedWrapper>("{\"data\":[" + request.downloadHandler.text + "]}");
            ShowPopup(factWrapper.data[0].attributes.name, factWrapper.data[0].attributes.description);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
        }
    }


    private void ShowPopup(string title, string description)
    {
        // �������� ������
        Debug.Log("Title: " + title);
        Debug.Log("Description: " + description);

        // ������������� �����
        popupTitle.text = string.IsNullOrEmpty(title) ? "No Title Available" : title;
        popupDescription.text = string.IsNullOrEmpty(description) ? "No Description Available." : description;

        // ���������� �����
        popup.SetActive(true);
    }

    public void ClosePopup()
    {
        popup.SetActive(false);
    }
}

[System.Serializable]
public class BreedWrapper
{
    public Breed[] data; // ��� ������, ��������������� "data" � JSON
}

[System.Serializable]
public class Breed
{
    public string id;
    public string type;
    public Attributes attributes;
}

[System.Serializable]
public class Attributes
{
    public string name;
    public string description;
    public Life life;
    public Weight male_weight;
    public Weight female_weight;
    public bool hypoallergenic;
}

[System.Serializable]
public class Life
{
    public int max;
    public int min;
}

[System.Serializable]
public class Weight
{
    public int max;
    public int min;
}