using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class UIReadyNotifier : MonoBehaviour
{
    [Tooltip("Opcional: id para distinguir múltiplos notifiers")]
    public string id;

    public UnityEvent OnEnabled = new UnityEvent();
    public UnityEvent<string> OnEnabledWithId = new UnityEvent<string>();

    void OnEnable()
    {
        OnEnabled.Invoke();
        if (!string.IsNullOrEmpty(id))
            OnEnabledWithId.Invoke(id);
    }
}