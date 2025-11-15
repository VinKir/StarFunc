using UnityEngine;

[CreateAssetMenu(fileName = "TheoryTopic", menuName = "Theory/Topic")]
public class TheoryTopic : ScriptableObject
{
    public string topicName;
    public TheoryPart[] parts; // части темы (слайды / анимации)
}

[System.Serializable]
public class TheoryPart
{
    [TextArea(3, 8)]
    public string description;
    public AnimationType animationType;
}

public enum AnimationType
{
    LinearFunction1,
    LinearFunction2,
    LinearFunction3,
    TrigSin,
    TrigCos,
    Custom
}
