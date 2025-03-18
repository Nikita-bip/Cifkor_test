using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class FactPresenter
{
    private bool _isFactsLoaded = false;
    private Coroutine _loadFactsCoroutine;
    private Coroutine _loadFactDetailsCoroutine;
    private readonly IFactView _view;
    private readonly string _baseUrl = "https://dogapi.dog/api/v2/breeds";
    private UnityWebRequestAsyncOperation _currentRequest;

    [Inject]
    public FactPresenter(IFactView view)
    {
        this._view = view;
    }

    public void LoadFacts()
    {
        if (!_isFactsLoaded && _loadFactsCoroutine == null)
        {
            _view.ShowLoader(true);
            _loadFactsCoroutine = _view.StartFactCoroutine(FetchFacts());
        }
    }

    public void OnFactSelected(FactModel fact)
    {
        if (fact == null)
        {
            Debug.LogError("Fact is null!");
            return;
        }

        string name = string.IsNullOrEmpty(fact.Name) ? "No Title Available" : fact.Name;
        string description = string.IsNullOrEmpty(fact.Description) ? "No Description Available." : fact.Description;

        _view.ShowPopup(name, description);
    }


    private IEnumerator FetchFacts()
    {
        UnityWebRequest request = UnityWebRequest.Get(_baseUrl);
        _currentRequest = request.SendWebRequest();
        yield return _currentRequest;

        _view.ShowLoader(false);
        _loadFactsCoroutine = null;
        _currentRequest = null;

        if (request.result != UnityWebRequest.Result.Success)
        {
            _view.ShowError("Failed to load facts.");
            yield break;
        }

        try
        {
            BreedWrapper breedWrapper = JsonUtility.FromJson<BreedWrapper>(request.downloadHandler.text);

            if (breedWrapper?.data == null || breedWrapper.data.Length == 0)
            {
                _view.ShowError("No facts found.");
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

            _isFactsLoaded = true;
            _view.DisplayFacts(facts);
        }
        catch (System.Exception ex)
        {
            _view.ShowError("Error parsing facts: " + ex.Message);
        }
    }

    public void CancelFactRequest()
    {
        if (_currentRequest != null)
        {
            _currentRequest.webRequest.Abort();
            _currentRequest = null;
        }

        if (_loadFactDetailsCoroutine != null)
        {
            _view.StopFactCoroutine(_loadFactDetailsCoroutine);
            _loadFactDetailsCoroutine = null;
        }

        if (_loadFactsCoroutine != null)
        {
            _view.StopFactCoroutine(_loadFactsCoroutine);
            _loadFactsCoroutine = null;
        }

        _view.ShowLoader(false);
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