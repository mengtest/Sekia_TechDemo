﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionSpray : MonoBehaviour
{

    public Material drawingMat;

    public float intensity = 1f;
    public Color color = Color.white;
    [Range(0.01f, 90f)] public float angle = 30f;
    public float range = 10f;
    public Texture cookie;
    public int shadowMapResolution = 1024;

    //画相机的深度图
    Shader depthRenderShader { get { return Shader.Find("Unlit/depthRender"); } }

    Camera _c;
    new Camera camera
    {
        get
        {
            if (_c == null)
            {
                _c = GetComponent<Camera>();
                if (_c == null)
                    _c = gameObject.AddComponent<Camera>();
                depthOutput = new RenderTexture(shadowMapResolution, shadowMapResolution, 16, RenderTextureFormat.RFloat);
                depthOutput.wrapMode = TextureWrapMode.Clamp;
                depthOutput.Create();
                _c.targetTexture = depthOutput;
                _c.SetReplacementShader(depthRenderShader, "RenderType");
                _c.clearFlags = CameraClearFlags.Nothing;
                _c.nearClipPlane = 0.01f;
                _c.enabled = false;
            }
            return _c;
        }
    }

    RenderTexture depthOutput;

    public void UpdateDrawingMat(Drawable drawable)
    {
        var currentRt = RenderTexture.active;
        RenderTexture.active = depthOutput;
        GL.Clear(true, true, Color.white * camera.farClipPlane);

        camera.fieldOfView = angle;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = range;
        camera.Render(); //绘制Spot光源的深度图

        RenderTexture.active = currentRt;

        var projMatrix = camera.projectionMatrix;
        var worldToDrawerMatrix = transform.worldToLocalMatrix;

        drawingMat.SetVector("_DrawerPos", transform.position);
        drawingMat.SetFloat("_Emission", intensity * Time.smoothDeltaTime);
        drawingMat.SetColor("_Color", color);
        drawingMat.SetMatrix("_WorldToDrawerMatrix", worldToDrawerMatrix);
        drawingMat.SetMatrix("_ProjMatrix", projMatrix);
        drawingMat.SetTexture("_Cookie", cookie);
        drawingMat.SetTexture("_DrawerDepth", depthOutput);

        drawable.Draw(drawingMat); //OverDraw指定模型 写入颜色到UV2空间
        //通过Spot光源的视锥体确定片元 用于笔刷 将Cookie用于笔刷边缘渐变
    }
}
