using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

[GenerateAuthoringComponent]
public struct InputData : IComponentData
{
    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode RightKey;
    public KeyCode LeftKey;
    public KeyCode SpaceKey;
    public PointerInputModule.MouseButtonEventData MouseButtonEventData;

}
