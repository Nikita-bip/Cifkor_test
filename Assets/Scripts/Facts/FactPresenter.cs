using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class FactPresenter
{
    private bool isFactsLoaded = false;
    private Coroutine loadFactsCoroutine;
    private readonly IFactView view;
    private readonly string baseUrl = "https://dogapi.dog/api/v2/breeds";

    [Inject]
    public FactPresenter(IFactView view)
    {
        this.view = view;
    }

    public void LoadFacts()
    {
        if (!isFactsLoaded && loadFactsCoroutine == null)
        {
            view.ShowLoader(true);
            loadFactsCoroutine = view.StartFactCoroutine(FetchFacts());
        }
    }

    private IEnumerator FetchFacts()
    {
        UnityWebRequest request = UnityWebRequest.Get(baseUrl);
        yield return request.SendWebRequest();

        view.ShowLoader(false);
        loadFactsCoroutine = null;

        if (request.result != UnityWebRequest.Result.Success)
        {
            view.ShowError("Failed to load facts.");
            yield break;
        }

        try
        {
            BreedWrapper breedWrapper = JsonUtility.FromJson<BreedWrapper>(request.downloadHandler.text);

            if (breedWrapper == null || breedWrapper.data == null || breedWrapper.data.Length == 0)
            {
                view.ShowError("No facts found.");
                yield break;
            }

            FactModel[] facts = new FactModel[breedWrapper.data.Length];
            for (int i = 0; i < breedWrapper.data.Length; i++)
            {
                facts[i] = new FactModel
                {
                    Id = breedWrapper.data[i].id,
                    Name = breedWrapper.data[i].attributes.name,
                    Description = breedWrapper.data[i].attributes.description
                };
            }

            isFactsLoaded = true;
            view.DisplayFacts(facts);
        }
        catch (System.Exception ex)
        {
            view.ShowError("Error parsing facts: " + ex.Message);
        }
    }

    public void OnFactSelected(FactModel fact)
    {
        view.ShowPopup(fact.Name, fact.Description);
    }
}
[System.Serializable]
public class BreedWrapper
{
    public Breed[] data;
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
}
