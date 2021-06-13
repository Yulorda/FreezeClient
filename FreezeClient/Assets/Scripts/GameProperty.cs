using UnityEngine;

//TODO Обнозначно ТУДУ надо это раскидать все, где эти методы конвертации должны быть не знаю
[CreateAssetMenu]
public class GameProperty : ScriptableObject
{
    public Vector2 distance;

}

public static class ExtensionVectors
{
    public static Vector3 XZ(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public static Vector2 XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}


