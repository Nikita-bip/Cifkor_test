using System.Collections.Generic;
using UnityEngine;

public class ButtonPool : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    public GameObject GetButton()
    {
        if (_pool.Count > 0)
        {
            var button = _pool.Dequeue();
            button.SetActive(true);
            return button;
        }

        return Instantiate(_buttonPrefab);
    }

    public void ReturnButton(GameObject button)
    {
        button.SetActive(false);
        _pool.Enqueue(button);
    }
}