//script written by Michael Withers
using UnityEngine;
using System.Collections;

//this class is used to attach a mesh to an animated mesh, binding to it and copying its wieghts to allow combined animations
public class MeshAttacher : MonoBehaviour
{
	public SkinnedMeshRenderer target = null;
	public SkinnedMeshRenderer attachable = null;

	void Start()
	{
		Bind(target, attachable);
	}

	void Update()
	{
		Mesh temp = new Mesh();
		attachable.BakeMesh(temp);
		
		for(int i = 0; i < temp.vertexCount; i++)
		{
			//Debug.DrawRay(MeshVertToWorld(i, attachable), Vector3.one * 3);
		}
	}

	//function to return the world location of an animated vert
	public Vector3 MeshVertToWorld(int index, SkinnedMeshRenderer skinnedMesh)
	{
		Mesh bakedMesh = new Mesh();
		skinnedMesh.BakeMesh(bakedMesh);
		bakedMesh.boneWeights = skinnedMesh.sharedMesh.boneWeights;
		bakedMesh.bindposes = skinnedMesh.sharedMesh.bindposes;
		return MeshVertToWorld(index, skinnedMesh, bakedMesh);
	}

	public Vector3 MeshVertToWorld(int index, SkinnedMeshRenderer skinnedMesh, Mesh bakedMesh)
	{
		BoneWeight[] boneWeights = bakedMesh.boneWeights;
		Vector3[] vertices = bakedMesh.vertices;
		return(MeshVertToWorld(index, skinnedMesh, bakedMesh, boneWeights, vertices));
	}

	public Vector3 MeshVertToWorld(int index, SkinnedMeshRenderer skinnedMesh, Mesh bakedMesh, BoneWeight[] boneWeights, Vector3[] vertices)
	{
		//Transform[] bones = skinnedMesh.bones;
		//Matrix4x4[] bindposes = bakedMesh.bindposes;
		/*
		Matrix4x4 localToWorldMatrix = Matrix4x4.identity;
		Matrix4x4 bone0 = bones[boneWeights[index].boneIndex0].localToWorldMatrix * bindposes[boneWeights[index].boneIndex0];
		Matrix4x4 bone1 = bones[boneWeights[index].boneIndex1].localToWorldMatrix * bindposes[boneWeights[index].boneIndex1];
		Matrix4x4 bone2 = bones[boneWeights[index].boneIndex2].localToWorldMatrix * bindposes[boneWeights[index].boneIndex2];
		Matrix4x4 bone3 = bones[boneWeights[index].boneIndex3].localToWorldMatrix * bindposes[boneWeights[index].boneIndex3];
		for(int j = 0; j < 16; j++)
		{
			bone0[j] *= boneWeights[index].weight0;
			bone1[j] *= boneWeights[index].weight1;
			bone2[j] *= boneWeights[index].weight2;
			bone3[j] *= boneWeights[index].weight3;
			localToWorldMatrix[j] = bone0[j] + bone1[j] + bone2[j] + bone3[j];
		}*/
		return Vector3.zero;
		//return localToWorldMatrix.MultiplyPoint3x4(vertices[index]);
	}


	public void Bind(SkinnedMeshRenderer targetMesh, SkinnedMeshRenderer attachableMesh)
	{
		//copy bones from target mesh
		attachableMesh.bones = targetMesh.bones;
		attachableMesh.rootBone = targetMesh.rootBone;

		//copy temporary meshes to use for logic
		Mesh attachableMeshBake = new Mesh();
		Mesh targetMeshBake = new Mesh();
		attachableMesh.BakeMesh(attachableMeshBake);
		targetMesh.BakeMesh(targetMeshBake);
		targetMeshBake.boneWeights = targetMesh.sharedMesh.boneWeights;
		targetMeshBake.bindposes = targetMesh.sharedMesh.bindposes;

		//copied vertice arrays to limit calls to vertices
		Vector3[] targetVertices = targetMeshBake.vertices;
		Vector3[] attachableVertices = attachableMeshBake.vertices;

		//temporary arrys to limit calls to boneweight
		BoneWeight[] attachableBoneWeightsTemp = new BoneWeight[attachableMeshBake.vertexCount];
		BoneWeight[] targetBoneweightsTemp = targetMeshBake.boneWeights;



		//set bindposes
		Matrix4x4[] bindposes = new Matrix4x4[attachableMesh.bones.GetLength(0)];
		for(int i = 0; i < attachableMeshBake.bindposes.GetLength(0); i++)
		{
			bindposes[i] = attachableMesh.bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
		}
		attachableMeshBake.bindposes = bindposes;
		//attachableMesh.sharedMesh.bindposes = targetMeshBake.bindposes;

		//corrects the deformation caused by attaching object to bone structure
		Matrix4x4 rootInverse = Matrix4x4.TRS(
			attachableMesh.rootBone.transform.InverseTransformPoint(attachableMesh.transform.position)
			,Quaternion.Inverse(attachableMesh.rootBone.transform.rotation) * attachableMesh.transform.rotation
			,Vector3.one);
		for(int i = 0; i < attachableVertices.GetLength(0); i++)
		{
			//attachableVertices[i] = rootInverse.MultiplyPoint3x4(attachableVertices[i]);
		}
		//attachableMeshBake.vertices = attachableVertices;

		//finds closest vertex on target mesh to each vertex on attaching mesh and copies it's weights
		for(int i = 0; i < attachableMeshBake.vertexCount; i++)
		{
			int closestVertIndex = 0;
			float closestVertDistance = 0;

			for(int j = 1; j < targetMeshBake.vertexCount; j++)
			{
				float distToVert = Vector3.Distance(
					attachableMesh.rootBone.transform.InverseTransformPoint(attachableVertices[i])
					, MeshVertToWorld(j, targetMesh, targetMeshBake, targetBoneweightsTemp, targetVertices));
				if(distToVert < closestVertDistance)
				{
					closestVertIndex = j;
					closestVertDistance = distToVert;
				}
			}
			attachableBoneWeightsTemp[i] = targetBoneweightsTemp[closestVertIndex];
			//attachableBoneWeightsTemp[i] = targetBoneweightsTemp[Random.Range(0, targetMeshBake.boneWeights.GetLength(0))];
		}
		attachableMeshBake.boneWeights = attachableBoneWeightsTemp;
		attachableMesh.transform.parent = targetMesh.transform.parent;
		attachableMesh.sharedMesh = attachableMeshBake;
	}
}
