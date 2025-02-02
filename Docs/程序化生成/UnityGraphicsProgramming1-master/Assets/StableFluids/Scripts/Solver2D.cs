﻿using UnityEngine;

namespace StableFluid
{
    public class Solver2D : SolverBase
    {
        #region Initialize

        protected override void InitializeComputeShader()
        {
            width        = Screen.width;
            height       = Screen.height;
            solverTex    = CreateRenderTexture(width >> lod, height >> lod, 0, RenderTextureFormat.ARGBFloat, solverTex);
            densityTex   = CreateRenderTexture(width >> lod, height >> lod, 0, RenderTextureFormat.RHalf, densityTex);
            velocityTex  = CreateRenderTexture(width >> lod, height >> lod, 0, RenderTextureFormat.RGHalf, velocityTex);
            prevTex      = CreateRenderTexture(width >> lod, height >> lod, 0, RenderTextureFormat.ARGBHalf, prevTex);

            Shader.SetGlobalTexture(solverTexId, solverTex);

            computeShader.SetFloat(diffId, diff);
            computeShader.SetFloat(viscId, visc);
            computeShader.SetFloat(dtId, Time.deltaTime);
            computeShader.SetFloat(velocityCoefId, velocityCoef);
            computeShader.SetFloat(densityCoefId, densityCoef);
        }

        #endregion

        #region StableFluid gpu kernel steps

        protected override void DensityStep()
        {
            //Add density source to density field
            if (SorceTex != null)
            {
                computeShader.SetTexture(kernelMap[ComputeKernels.AddSourceDensity], sourceId, SorceTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AddSourceDensity], densityId, densityTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AddSourceDensity], prevId, prevTex);
                computeShader.Dispatch(kernelMap[ComputeKernels.AddSourceDensity], Mathf.CeilToInt(solverTex.width / gpuThreads.x), Mathf.CeilToInt(solverTex.height / gpuThreads.y), 1);
            }

            //Diffuse density
            computeShader.SetTexture(kernelMap[ComputeKernels.DiffuseDensity], densityId, densityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.DiffuseDensity], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.DiffuseDensity], Mathf.CeilToInt(solverTex.width / gpuThreads.x), Mathf.CeilToInt(solverTex.height / gpuThreads.y), 1);

            //Swap density
            computeShader.SetTexture(kernelMap[ComputeKernels.SwapDensity], densityId, densityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.SwapDensity], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.SwapDensity], Mathf.CeilToInt(solverTex.width / gpuThreads.x), Mathf.CeilToInt(solverTex.height / gpuThreads.y), 1);

            if (isDensityOnly)
            {
                //Advection using external velocity field via ForceTex.
                computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensityFromExt], densityId, densityTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensityFromExt], prevId, prevTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensityFromExt], velocityId, velocityTex);
                if (SorceTex != null) computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensityFromExt], sourceId, SorceTex);
                computeShader.Dispatch(kernelMap[ComputeKernels.AdvectDensity], Mathf.CeilToInt(solverTex.width / gpuThreads.x), Mathf.CeilToInt(solverTex.height / gpuThreads.y), 1);
            }
            else
            {
                //Advection using velocity solver
                computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensity], densityId, densityTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensity], prevId, prevTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AdvectDensity], velocityId, velocityTex);
                computeShader.Dispatch(kernelMap[ComputeKernels.AdvectDensity], Mathf.CeilToInt(solverTex.width / gpuThreads.x), Mathf.CeilToInt(solverTex.height / gpuThreads.y), 1);
            }
        }

        protected override void VelocityStep()
        {
            //Add velocity source to velocity field
            if (SorceTex != null)
            {
                computeShader.SetTexture(kernelMap[ComputeKernels.AddSourceVelocity], sourceId, SorceTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AddSourceVelocity], velocityId, velocityTex);
                computeShader.SetTexture(kernelMap[ComputeKernels.AddSourceVelocity], prevId, prevTex);
                computeShader.Dispatch(kernelMap[ComputeKernels.AddSourceVelocity], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);
            }

            //Diffuse velocity
            computeShader.SetTexture(kernelMap[ComputeKernels.DiffuseVelocity], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.DiffuseVelocity], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.DiffuseVelocity], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Project
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep1], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep1], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.ProjectStep1], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Project
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep2], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.ProjectStep2], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Project
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep3], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep3], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.ProjectStep3], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Swap velocity
            computeShader.SetTexture(kernelMap[ComputeKernels.SwapVelocity], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.SwapVelocity], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.SwapVelocity], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Advection
            computeShader.SetTexture(kernelMap[ComputeKernels.AdvectVelocity], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.AdvectVelocity], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.AdvectVelocity], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Project
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep1], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep1], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.ProjectStep1], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Project
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep2], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.ProjectStep2], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);

            //Project
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep3], velocityId, velocityTex);
            computeShader.SetTexture(kernelMap[ComputeKernels.ProjectStep3], prevId, prevTex);
            computeShader.Dispatch(kernelMap[ComputeKernels.ProjectStep3], Mathf.CeilToInt(velocityTex.width / gpuThreads.x), Mathf.CeilToInt(velocityTex.height / gpuThreads.y), 1);
        }

        #endregion
    }
}