using UnityEngine;

public class GeneralDevNote : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField, TextArea(3, 10)] private string devNote = "";
#endif
}
