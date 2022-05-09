using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCurve : BaseMeshEffect
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 10);
    public float curveScale = 1.0f;
    public RectTransform rectTransform;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (curve[0].time != 0)
        {
            var tmp = curve[0];
            tmp.time = 0;
            curve.MoveKey(0, tmp);
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (curve[curve.length - 1].time != rectTransform.rect.width)
        {
            OnRectTransformDimensionsChange();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        OnRectTransformDimensionsChange();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        rectTransform = GetComponent<RectTransform>();
        OnRectTransformDimensionsChange();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
        {
            return;
        }

        if (rectTransform == null)
        {
            return;
        }

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        for (int i = 0; i < vertices.Count; ++i)
        {
            var vertex = vertices[i];
            vertex.position.y += curve.Evaluate(rectTransform.rect.width * rectTransform.pivot.x + vertex.position.x)*curveScale;
            vertices[i] = vertex;
        }
        
        vh.AddUIVertexTriangleStream(vertices);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        var tmp = curve[curve.length - 1];
        if (rectTransform != null)
        {
            tmp.time = rectTransform.rect.width;
            curve.MoveKey(curve.length - 1, tmp);
        }
    }
}
