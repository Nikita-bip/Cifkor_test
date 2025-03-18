using System.Collections;
using UnityEngine;

public interface IFactView
{
    void DisplayFacts(FactModel[] facts);
    void ShowPopup(string title, string description);
    void ShowLoader(bool isActive);
    void ShowError(string message);

    Coroutine StartFactCoroutine(IEnumerator routine);
    void StopFactCoroutine(Coroutine routine);
}