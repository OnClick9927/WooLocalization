using System;
using System.Collections.Generic;
using UnityEngine;
using WooLocalization;

[UnityEditor.CustomEditor(typeof(ActorTest))]
public class ActorTestEditor : WooLocalization.LocalizationBehaviorEditor<ActorTest>
{

}
[Flags]
public enum FlagEnum
{
    A = 2, B = 4, C = 8, D = 16
}
public enum TestEnum
{
    A, B, C, D
}
public class ActorTest : WooLocalization.LocalizationBehavior
{
    protected override List<ILocalizationActor> GetActors()
    {
        return new List<ILocalizationActor>() {
     _enum, _enum_flag,   _string_private, _string,_go,_int,_float,_bool,_double,_long,_color,_v3,_v2,_v4,_v2i,_v3i,_rect,_recti,_bounds,_curve
       };
    }
    private ObjectActor<string> _string_private = new ObjectActor<string>(true);
    public ObjectActor<FlagEnum> _enum_flag = new ObjectActor<FlagEnum>(true);


    public ObjectActor<TestEnum> _enum = new ObjectActor<TestEnum>(true);

    public ObjectActor<string> _string = new ObjectActor<string>(true);
    public ObjectActor<UnityEngine.GameObject> _go = new ObjectActor<GameObject>(true);
    public ObjectActor<int> _int = new ObjectActor<int>(true);
    public ObjectActor<float> _float = new ObjectActor<float>(true);
    public ObjectActor<bool> _bool = new ObjectActor<bool>(true);
    public ObjectActor<double> _double = new ObjectActor<double>(true);
    public ObjectActor<long> _long = new ObjectActor<long>(true);
    public ObjectActor<UnityEngine.Color> _color = new ObjectActor<Color>(true);
    public ObjectActor<UnityEngine.Vector3> _v3 = new ObjectActor<UnityEngine.Vector3>(true);
    public ObjectActor<UnityEngine.Vector2> _v2 = new ObjectActor<UnityEngine.Vector2>(true);
    public ObjectActor<UnityEngine.Vector4> _v4 = new ObjectActor<UnityEngine.Vector4>(true);
    public ObjectActor<UnityEngine.Vector2Int> _v2i = new ObjectActor<UnityEngine.Vector2Int>(true);
    public ObjectActor<UnityEngine.Vector3Int> _v3i = new ObjectActor<UnityEngine.Vector3Int>(true);
    public ObjectActor<UnityEngine.Rect> _rect = new ObjectActor<Rect>(true);
    public ObjectActor<UnityEngine.RectInt> _recti = new ObjectActor<RectInt>(true);
    public ObjectActor<UnityEngine.Bounds> _bounds = new ObjectActor<Bounds>(true);
    public ObjectActor<UnityEngine.AnimationCurve> _curve = new ObjectActor<AnimationCurve>(true);

}
