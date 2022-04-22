using System;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public class ScoreData : IComponentData
{
    public TMP_Text ScoreUIText;
    public int Score;
}
