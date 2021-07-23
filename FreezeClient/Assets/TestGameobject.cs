using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestGameobject : MonoBehaviour
{
    public ReactiveProperty<int> testID = new ReactiveProperty<int>();
    private ReactiveProperty<int> testID2 = new ReactiveProperty<int>();
}
