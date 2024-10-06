using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.U2D.Common;

namespace UnityEngine.U2D.Animation
{
    internal enum SpriteSkinValidationResult
    {
        SpriteNotFound,
        SpriteHasNoSkinningInformation,
        SpriteHasNoWeights,
        RootTransformNotFound,
        InvalidTransformArray,
        InvalidTransformArrayLength,
        TransformArrayContainsNull,
        InvalidBoneWeights,

        Ready
    }

    internal class NativeByteArray
    {
        public int Length => array.Length;
        public bool IsCreated => array.IsCreated;
        public byte this[int index] => array[index];

        public NativeArray<byte> array
        { get; }

        public NativeByteArray(NativeArray<byte> array)
        {
            this.array = array;
        }

        public void Dispose() => array.Dispose();
    }

    public static class SpriteSkinUtility
    {
        internal static SpriteSkinValidationResult Validate(this SpriteSkin spriteSkin)
        {
            var sprite = spriteSkin.sprite;
            if (sprite == null)
                return SpriteSkinValidationResult.SpriteNotFound;

            var bindPoses = sprite.GetBindPoses();
            var bindPoseCount = bindPoses.Length;

            if (bindPoseCount == 0)
                return SpriteSkinValidationResult.SpriteHasNoSkinningInformation;

            if (spriteSkin.rootBone == null)
                return SpriteSkinValidationResult.RootTransformNotFound;

            if (spriteSkin.boneTransforms == null)
                return SpriteSkinValidationResult.InvalidTransformArray;

            if (bindPoseCount != spriteSkin.boneTransforms.Length)
                return SpriteSkinValidationResult.InvalidTransformArrayLength;
            
            foreach (var boneTransform in spriteSkin.boneTransforms)
            {
                if (boneTransform == null)
                    return SpriteSkinValidationResult.TransformArrayContainsNull;
            }

            var boneWeights = spriteSkin.spriteBoneWeights;
            if (!BurstedSpriteSkinUtilities.ValidateBoneWeights(in boneWeights, bindPoseCount))
                return SpriteSkinValidationResult.InvalidBoneWeights;
            
            return SpriteSkinValidationResult.Ready;
        }

        internal static void CreateBoneHierarchy(this SpriteSkin spriteSkin)
        {
            if (spriteSkin.spriteRenderer.sprite == null)
                throw new InvalidOperationException("SpriteRenderer has no Sprite set");

            var spriteBones = spriteSkin.spriteRenderer.sprite.GetBones();
            var transforms = new Transform[spriteBones.Length];
            Transform root = null;

            for (var i = 0; i < spriteBones.Length; ++i)
            {
                CreateGameObject(i, spriteBones, transforms, spriteSkin.transform);
                if (spriteBones[i].parentId < 0 && root == null)
                    root = transforms[i];
            }

            spriteSkin.rootBone = root;
            spriteSkin.boneTransforms = transforms;
        }

        internal static int GetVertexStreamSize(this Sprite sprite)
        {
            var vertexStreamSize = 12;
            if (sprite.HasVertexAttribute(Rendering.VertexAttribute.Normal))
                vertexStreamSize = vertexStreamSize + 12;
            if (sprite.HasVertexAttribute(Rendering.VertexAttribute.Tangent))
                vertexStreamSize = vertexStreamSize + 16;
            return vertexStreamSize;
        }

        internal static int GetVertexStreamOffset(this Sprite sprite, Rendering.VertexAttribute channel )
        {
            var hasPosition = sprite.HasVertexAttribute(Rendering.VertexAttribute.Position);
            var hasNormals = sprite.HasVertexAttribute(Rendering.VertexAttribute.Normal);
            var hasTangents = sprite.HasVertexAttribute(Rendering.VertexAttribute.Tangent);

            switch(channel)
            {
                case Rendering.VertexAttribute.Position:
                    return hasPosition ? 0 : -1;
                case Rendering.VertexAttribute.Normal:
                    return hasNormals ? 12 : -1;
                case Rendering.VertexAttribute.Tangent:
                    return hasTangents ? (hasNormals ? 24 : 12) : -1;
            }
            return -1;
        }

        static void CreateGameObject(int index, SpriteBone[] spriteBones, Transform[] transforms, Transform root)
        {
            if (transforms[index] == null)
            {
                var spriteBone = spriteBones[index];
                if (spriteBone.parentId >= 0)
                    CreateGameObject(spriteBone.parentId, spriteBones, transforms, root);

                var go = new GameObject(spriteBone.name);
                var transform = go.transform;
                if (spriteBone.parentId >= 0)
                    transform.SetParent(transforms[spriteBone.parentId]);
                else
                    transform.SetParent(root);
                transform.localPosition = spriteBone.position;
                transform.localRotation = spriteBone.rotation;
                transform.localScale = Vector3.one;
                transforms[index] = transform;
            }
        }

        public static void ResetBindPose(this SpriteSkin spriteSkin) //test
        {
            if (!spriteSkin.isValid)
                throw new InvalidOperationException("SpriteSkin is not valid");

            var spriteBones = spriteSkin.spriteRenderer.sprite.GetBones();
            var boneTransforms = spriteSkin.boneTransforms;

            for (int i = 0; i < boneTransforms.Length; ++i)
            {
                var boneTransform = boneTransforms[i];
                var spriteBone = spriteBones[i];

                if (spriteBone.parentId != -1)
                {
                    boneTransform.localPosition = spriteBone.position;
                    boneTransform.localRotation = spriteBone.rotation;
                    boneTransform.localScale = Vector3.one;
                }
            }
        }

        static int GetHash(Matrix4x4 matrix)
        {
            unsafe
            {
                var b = (uint*)&matrix;
                {
                    var c = (char*)b;
                    return (int)math.hash(c, 16 * sizeof(float));
                }
            }
        }

        internal static int CalculateTransformHash(this SpriteSkin spriteSkin)
        {
            var bits = 0;
            var boneTransformHash = GetHash(spriteSkin.transform.localToWorldMatrix) >> bits;
            bits++;
            foreach (var transform in spriteSkin.boneTransforms)
            {
                boneTransformHash ^= GetHash(transform.localToWorldMatrix) >> bits;
                bits = (bits + 1) % 8;
            }
            return boneTransformHash;
        }

        internal unsafe static void Deform(Sprite sprite, Matrix4x4 rootInv, NativeSlice<Vector3> vertices, NativeSlice<Vector4> tangents, NativeSlice<BoneWeight> boneWeights, NativeArray<Matrix4x4> boneTransforms, NativeSlice<Matrix4x4> bindPoses, NativeArray<byte> deformableVertices)
        {
            var verticesFloat3 = vertices.SliceWithStride<float3>();
            var tangentsFloat4 = tangents.SliceWithStride<float4>();
            var bindPosesFloat4x4 = bindPoses.SliceWithStride<float4x4>();
            var spriteVertexCount = sprite.GetVertexCount();
            var spriteVertexStreamSize = sprite.GetVertexStreamSize();
            var boneTransformsFloat4x4 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float4x4>(boneTransforms.GetUnsafePtr(), boneTransforms.Length, Allocator.None);

            byte* deformedPosOffset = (byte*)NativeArrayUnsafeUtility.GetUnsafePtr(deformableVertices);            
            NativeSlice<float3> deformableVerticesFloat3 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(deformedPosOffset, spriteVertexStreamSize, spriteVertexCount);
            NativeSlice<float4> deformableTangentsFloat4 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float4>(deformedPosOffset, spriteVertexStreamSize, 1);   // Just Dummy.
            if (sprite.HasVertexAttribute(Rendering.VertexAttribute.Tangent))
            {
                byte* deformedTanOffset = deformedPosOffset + sprite.GetVertexStreamOffset(Rendering.VertexAttribute.Tangent);
                deformableTangentsFloat4 = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float4>(deformedTanOffset, spriteVertexStreamSize, spriteVertexCount);
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var handle1 = CreateSafetyChecks<float4x4>(ref boneTransformsFloat4x4);
            var handle2 = CreateSafetyChecks<float3>(ref deformableVerticesFloat3);
            var handle3 = CreateSafetyChecks<float4>(ref deformableTangentsFloat4);
#endif

            if (sprite.HasVertexAttribute(Rendering.VertexAttribute.Tangent))
                Deform(rootInv, verticesFloat3, tangentsFloat4, boneWeights, boneTransformsFloat4x4, bindPosesFloat4x4, deformableVerticesFloat3, deformableTangentsFloat4);
            else
                Deform(rootInv, verticesFloat3, boneWeights, boneTransformsFloat4x4, bindPosesFloat4x4, deformableVerticesFloat3);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSafetyChecks(handle1);
            DisposeSafetyChecks(handle2);
            DisposeSafetyChecks(handle3);
#endif
        }

        internal static void Deform(float4x4 rootInv, NativeSlice<float3> vertices, NativeSlice<BoneWeight> boneWeights, NativeArray<float4x4> boneTransforms, NativeSlice<float4x4> bindPoses, NativeSlice<float3> deformed)
        {
            if (boneTransforms.Length == 0)
                return;

            for (var i = 0; i < boneTransforms.Length; i++)
            {
                var bindPoseMat = bindPoses[i];
                var boneTransformMat = boneTransforms[i];
                boneTransforms[i] = math.mul(rootInv, math.mul(boneTransformMat, bindPoseMat));
            }

            for (var i = 0; i < vertices.Length; i++)
            {
                var bone0 = boneWeights[i].boneIndex0;
                var bone1 = boneWeights[i].boneIndex1;
                var bone2 = boneWeights[i].boneIndex2;
                var bone3 = boneWeights[i].boneIndex3;

                var vertex = vertices[i];
                deformed[i] =
                    math.transform(boneTransforms[bone0], vertex) * boneWeights[i].weight0 +
                    math.transform(boneTransforms[bone1], vertex) * boneWeights[i].weight1 +
                    math.transform(boneTransforms[bone2], vertex) * boneWeights[i].weight2 +
                    math.transform(boneTransforms[bone3], vertex) * boneWeights[i].weight3;
            }
        }

        internal static void Deform(float4x4 rootInv, NativeSlice<float3> vertices, NativeSlice<float4> tangents, NativeSlice<BoneWeight> boneWeights, NativeArray<float4x4> boneTransforms, NativeSlice<float4x4> bindPoses, NativeSlice<float3> deformed, NativeSlice<float4> deformedTangents)
        {
            if(boneTransforms.Length == 0)
                return;

            for (var i = 0; i < boneTransforms.Length; i++)
            {
                var bindPoseMat = bindPoses[i];
                var boneTransformMat = boneTransforms[i];
                boneTransforms[i] = math.mul(rootInv,  math.mul(boneTransformMat, bindPoseMat));
            }

            for (var i = 0; i < vertices.Length; i++)
            {
                var bone0 = boneWeights[i].boneIndex0;
                var bone1 = boneWeights[i].boneIndex1;
                var bone2 = boneWeights[i].boneIndex2;
                var bone3 = boneWeights[i].boneIndex3;
                
                var vertex = vertices[i];
                deformed[i] =
                    math.transform(boneTransforms[bone0], vertex) * boneWeights[i].weight0 +
                    math.transform(boneTransforms[bone1], vertex) * boneWeights[i].weight1 +
                    math.transform(boneTransforms[bone2], vertex) * boneWeights[i].weight2 +
                    math.transform(boneTransforms[bone3], vertex) * boneWeights[i].weight3;

                var tangent = new float4(tangents[i].xyz, 0.0f);

                tangent =
                    math.mul(boneTransforms[bone0], tangent) * boneWeights[i].weight0 +
                    math.mul(boneTransforms[bone1], tangent) * boneWeights[i].weight1 +
                    math.mul(boneTransforms[bone2], tangent) * boneWeights[i].weight2 +
                    math.mul(boneTransforms[bone3], tangent) * boneWeights[i].weight3;

                deformedTangents[i] = new float4(math.normalize(tangent.xyz), tangents[i].w);
            }
        }

        internal static void Deform(Sprite sprite, Matrix4x4 invRoot, Transform[] boneTransformsArray, NativeArray<byte> deformVertexData)
        {
            Debug.Assert(sprite != null);
            Debug.Assert(sprite.GetVertexCount() == (deformVertexData.Length / sprite.GetVertexStreamSize()));
            
            var vertices = sprite.GetVertexAttribute<Vector3>(UnityEngine.Rendering.VertexAttribute.Position);
            var tangents = sprite.GetVertexAttribute<Vector4>(UnityEngine.Rendering.VertexAttribute.Tangent);
            var boneWeights = sprite.GetVertexAttribute<BoneWeight>(UnityEngine.Rendering.VertexAttribute.BlendWeight);
            var bindPoses = sprite.GetBindPoses();
            
            Debug.Assert(bindPoses.Length == boneTransformsArray.Length);
            Debug.Assert(boneWeights.Length == sprite.GetVertexCount());
            
            var boneTransforms = new NativeArray<Matrix4x4>(boneTransformsArray.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (var i = 0; i < boneTransformsArray.Length; ++i)
                boneTransforms[i] = boneTransformsArray[i].localToWorldMatrix;

            Deform(sprite, invRoot, vertices, tangents, boneWeights, boneTransforms, bindPoses, deformVertexData);

            boneTransforms.Dispose();
        }
        
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        static AtomicSafetyHandle CreateSafetyChecks<T>(ref NativeArray<T> array) where T : struct
        {
            var handle = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(handle, true);
            AtomicSafetyHandle.UseSecondaryVersion(ref handle);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle<T>(ref array, handle);
            return handle;
        }

        static AtomicSafetyHandle CreateSafetyChecks<T>(ref NativeSlice<T> array) where T : struct
        {
            var handle = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(handle, true);
            AtomicSafetyHandle.UseSecondaryVersion(ref handle);
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle<T>(ref array, handle);
            return handle;
        }

        static void DisposeSafetyChecks(AtomicSafetyHandle handle)
        {
            AtomicSafetyHandle.Release(handle);
        }
#endif

        internal static void Bake(this SpriteSkin spriteSkin, NativeArray<byte> deformVertexData)
        {
            if (!spriteSkin.isValid)
                throw new Exception("Bake error: invalid SpriteSkin");

            var sprite = spriteSkin.spriteRenderer.sprite;
            var boneTransformsArray = spriteSkin.boneTransforms;
            Deform(sprite, Matrix4x4.identity, boneTransformsArray, deformVertexData);
        }

        internal static unsafe void CalculateBounds(this SpriteSkin spriteSkin)
        {
            Debug.Assert(spriteSkin.isValid);
            var sprite = spriteSkin.sprite;
            
            var deformVertexData = new NativeArray<byte>(sprite.GetVertexStreamSize() * sprite.GetVertexCount(), Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var dataPtr = deformVertexData.GetUnsafePtr();
            var deformedPosSlice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<Vector3>(dataPtr, sprite.GetVertexStreamSize(), sprite.GetVertexCount());
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref deformedPosSlice, NativeArrayUnsafeUtility.GetAtomicSafetyHandle(deformVertexData));
#endif
            
            spriteSkin.Bake(deformVertexData);
            UpdateBounds(spriteSkin, deformVertexData);
            deformVertexData.Dispose();
        }

        internal static Bounds CalculateSpriteSkinBounds(NativeSlice<float3> deformablePositions)
        {
            var min = deformablePositions[0];
            var max = deformablePositions[0];

            for (int j = 1; j < deformablePositions.Length; ++j)
            {
                min = math.min(min, deformablePositions[j]);
                max = math.max(max, deformablePositions[j]);
            }
            
            var ext = (max - min) * 0.5F;
            var ctr = min + ext;
            var bounds = new Bounds();
            bounds.center = ctr;
            bounds.extents = ext;
            return bounds;
        }

        internal static unsafe void UpdateBounds(this SpriteSkin spriteSkin, NativeArray<byte> deformedVertices)
        {
            var deformedPosOffset = (byte*)NativeArrayUnsafeUtility.GetUnsafePtr(deformedVertices);            
            var spriteVertexCount = spriteSkin.sprite.GetVertexCount();
            var spriteVertexStreamSize = spriteSkin.sprite.GetVertexStreamSize();            
            var deformedPositions = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<float3>(deformedPosOffset, spriteVertexStreamSize, spriteVertexCount);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var handle = CreateSafetyChecks<float3>(ref deformedPositions);
#endif
            spriteSkin.bounds = CalculateSpriteSkinBounds(deformedPositions);
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSafetyChecks(handle);
#endif           
            InternalEngineBridge.SetLocalAABB(spriteSkin.spriteRenderer, spriteSkin.bounds);
        }
    }
    
    [BurstCompile]
    internal static class BurstedSpriteSkinUtilities
    {
        [BurstCompile]
        internal static bool ValidateBoneWeights(in NativeCustomSlice<BoneWeight> boneWeights, int bindPoseCount)
        {
            var boneWeightCount = boneWeights.Length; 
            for (var i = 0; i < boneWeightCount; ++i)
            {
                var boneWeight = boneWeights[i];
                var idx0 = boneWeight.boneIndex0;
                var idx1 = boneWeight.boneIndex1;
                var idx2 = boneWeight.boneIndex2;
                var idx3 = boneWeight.boneIndex3;
                
                if ((idx0 < 0 || idx0 >= bindPoseCount) ||
                    (idx1 < 0 || idx1 >= bindPoseCount) ||
                    (idx2 < 0 || idx2 >= bindPoseCount) ||
                    (idx3 < 0 || idx3 >= bindPoseCount))
                    return false;
            }

            return true;
        }        
    }    
}
