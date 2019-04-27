using UnityEngine;
using System.Collections.Generic;

public class SwarmSystem : MonoBehaviour
{
	public GameObject swarmPrefab;
	public GameObject anchorPrefab;
	public int size = 1;
	
	public bool showGuides = false;
	
	private List<Transform> units = new List<Transform>();
	private List<Transform> anchors = new List<Transform>();
	private List<Vector3> noiseDirecs = new List<Vector3>();
	
	private List<Vector3> boneStarts = new List<Vector3>();
	private List<Vector3> boneEnds = new List<Vector3>();
	private List<Transform> boneTransforms = new List<Transform>();
	private List<Transform> boneTips = new List<Transform>();
	private List<int> boneCenterStarts = new List<int>();
	private List<float> boneCenterDists = new List<float>();
	
	private SkinnedMeshRenderer meshRender;
	
	private List<int> unitTris = new List<int>();
	private List<List<int>> triGraph;
	
	public float Noise {get; set;}
	public float Collapse {get; set;}
	
	private Mesh getCurrentMesh()
	{
		Mesh mesh = new Mesh();
		meshRender.BakeMesh(mesh);
		return mesh;
	}
	
	private bool triangleIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray, out Vector3 intersect)
	{
		Vector3 e1, e2;  

		Vector3 p, q, t;
		float det, invDet, u, v;
		float Epsilon = 0.001f;

		intersect = Vector3.zero;

		e1 = p2 - p1;
		e2 = p3 - p1;

		//calculating determinant 
		p = Vector3.Cross(ray.direction, e2);

		det = Vector3.Dot(e1, p);

		//if determinant is near zero, ray lies in plane of triangle otherwise not
		if (det > -Epsilon && det < Epsilon)
			return false;
			
		invDet = 1.0f / det;

		t = ray.origin - p1;

		u = Vector3.Dot(t, p) * invDet;

		//Check for ray hit
		if (u < 0 || u > 1)
			return false;

		//Prepare to test v parameter
		q = Vector3.Cross(t, e1);

		v = Vector3.Dot(ray.direction, q) * invDet;

		//Check for ray hit
		if (v < 0 || u + v > 1)
			return false;
			
		//ray does intersect
		if ((Vector3.Dot(e2, q) * invDet) > Epsilon)
		{
			intersect = ray.origin + ray.direction * (Vector3.Dot(e2, q)*invDet);
			return true;
		}

		// No hit at all
		return false;
	}
	
	private bool triangleIndexIntersection(int idx, Vector3[] vertices, int[] triangles, Ray ray, out Vector3 intersect)
	{
		return triangleIntersection( meshRender.transform.TransformPoint(vertices[triangles[idx*3]]), 
									 meshRender.transform.TransformPoint(vertices[triangles[idx*3+1]]), 
									 meshRender.transform.TransformPoint(vertices[triangles[idx*3+2]]), 
									 ray, out intersect);
	}
	
	private Vector3 meshIntersection(int unitIdx, Vector3[] vertices, int[] triangles, Vector3 boneCenter)
	{
		Vector3 intersect;
		
		Ray ray = new Ray(boneCenter, anchors[unitIdx].position-boneCenter);
		
		if(unitTris[unitIdx] != -1)
		{
			if(triangleIndexIntersection(unitTris[unitIdx], vertices, triangles, ray, out intersect))
			{
				return intersect;
			}
			
			foreach(int tri in triGraph[unitTris[unitIdx]])
			{
				if(triangleIndexIntersection(tri, vertices, triangles, ray, out intersect))
				{
					unitTris[unitIdx] = tri;
					return intersect;
				}
			}
		}
		
		for(int i=0; i<triangles.Length; i+=3)
		{
			if(triangleIndexIntersection(i/3, vertices, triangles, ray, out intersect))
			{
				unitTris[unitIdx] = i/3;
				return intersect;
			}
		}
		
		return anchors[unitIdx].position;
	}
	
	private void calculateTriGraph()
	{
		Mesh mesh = meshRender.sharedMesh;
		
		Vector3[] vertices = mesh.vertices;
		
		float epsilon = 0.001f;
		
		triGraph = new List<List<int>>();
		
		Dictionary<int, int> vertexAlias = new Dictionary<int, int>();
		
		List<int> vertexByMag = new List<int>();
		
		for(int i=0; i<vertices.Length; i++)
			vertexByMag.Add(i);
		
		vertexByMag.Sort((x,y) => vertices[x].magnitude.CompareTo(vertices[y].magnitude));
		
		for(int i=0; i<vertices.Length; i++)
		{
			int minEq = i;
			int j = i;
			
			while(j >= 0 && vertices[i].magnitude-vertices[j].magnitude < epsilon)
			{
				if((vertices[i]-vertices[j]).magnitude < epsilon)
					minEq = j;
				j--;
			}
			
			vertexAlias.Add(i, minEq);
		}
		
		List<List<int>> vertexTouch = new List<List<int>>();
		
		for(int i=0; i<vertices.Length; i++)
			vertexTouch.Add(new List<int>());
			
		
		for(int i=0; i<mesh.triangles.Length; i+=3)
		{
			vertexTouch[vertexAlias[mesh.triangles[i]]].Add(i/3);
			vertexTouch[vertexAlias[mesh.triangles[i+1]]].Add(i/3);
			vertexTouch[vertexAlias[mesh.triangles[i+2]]].Add(i/3);
		}
		
		for(int i=0; i<mesh.triangles.Length; i+=3)
		{
			List<int> tris = new List<int>();
			
			for(int k=0; k<3; k++)
			{
				foreach(int tri in vertexTouch[vertexAlias[mesh.triangles[i+k]]])
				{
					if(tri != i/3 && !tris.Contains(tri))
						tris.Add(tri);
				}
			}
			
			triGraph.Add(tris);
		}
	}
	
	private void mapBones()
	{
		boneTips.Clear();
		
		foreach(Transform b in meshRender.bones)
		{
			boneTransforms.Add(b.parent);
			
			if(b.GetChild(0).childCount == 0)
			{
				boneTransforms.Add(b);
				boneTips.Add(b);
			}
		}
	}
	
	private void refreshMesh()
	{
		boneStarts.Clear();
		boneEnds.Clear();
		
		foreach(Transform b in meshRender.bones)
		{
			boneStarts.Add(b.parent.position);
			boneEnds.Add(b.position);
			if(boneTips.Contains(b))
			{
				boneStarts.Add(b.position);
				boneEnds.Add(b.GetChild(0).position);
			}
		}
	}
	
	private void calculateBoneCenters()
	{
		boneCenterStarts.Clear();
		boneCenterDists.Clear();
		
		foreach(Transform unit in units)
		{
			float minDist = float.PositiveInfinity;
			Vector3 boneCenter = transform.position;
			int boneIdx = 0;
			
			for(int i=0; i<boneStarts.Count; i++)
			{
				Vector3 start = boneStarts[i];
				Vector3 end = boneEnds[i];
				
				Vector3 c = start + Vector3.Project(unit.position-start, end-start);
				float dist = (c-unit.position).magnitude;
				Vector3 direc = end-start;
				
				if(dist < minDist && (c-start).magnitude < direc.magnitude && (c-end).magnitude < direc.magnitude)
				{
					minDist = dist;
					boneCenter = c;
					boneIdx = i;
				}
			}
			
			boneCenterStarts.Add(boneIdx);
			boneCenterDists.Add((boneCenter-boneStarts[boneIdx]).magnitude);
			unit.parent = boneTransforms[boneIdx];
		}
	}
	
	private Vector3 findBoneCenter(int unitIdx, out Vector3 boneDirec)
	{
		Vector3 start = boneStarts[boneCenterStarts[unitIdx]];
		boneDirec = boneEnds[boneCenterStarts[unitIdx]] - start;
		return start + (boneCenterDists[unitIdx] * boneDirec);
	}
	
	private void randomMeshSpawn()
	{
		Mesh mesh = getCurrentMesh();
		
		List<float> accTriSizes = new List<float>();
		
		for(int i=0; i<mesh.triangles.Length; i+=3)
		{
			float prev = 0;
			
			if(i>0)
				prev = accTriSizes[(i/3)-1];
		
			float area = Vector3.Cross(mesh.vertices[mesh.triangles[i+1]]-mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i+2]]-mesh.vertices[mesh.triangles[i]]).magnitude/2;
			
			accTriSizes.Add(prev+area);
		}
		
		float outOf = accTriSizes[accTriSizes.Count-1];
		
		for(int i=0; i<size; i++)
		{
			float v = Random.value*outOf;
			
			int k=0;
			while(accTriSizes[k+1] < v)
				k++;
			
			float r1 = Mathf.Sqrt(Random.value);
			float r2 = Random.value;
			
			Vector3 p = (1-r1)*mesh.vertices[mesh.triangles[k*3]] + (r1*(1-r2))*mesh.vertices[mesh.triangles[k*3+1]] + (r1*r2)*mesh.vertices[mesh.triangles[k*3+2]];
			
			GameObject anchor = (GameObject)Instantiate(anchorPrefab, meshRender.transform.TransformPoint(p), Quaternion.identity);
			
			GameObject spawn = (GameObject)Instantiate(swarmPrefab, meshRender.transform.TransformPoint(p), Quaternion.identity);
			
			spawn.transform.parent = transform;
			anchor.transform.parent = transform;
			
			units.Add(spawn.transform);
			anchors.Add(anchor.transform);
			noiseDirecs.Add(Random.onUnitSphere);
			
			unitTris.Add(k);
		}
	}
	
	public void activate(bool turnOn)
	{
		foreach(Transform u in units)
			u.gameObject.SetActive(turnOn);
	}
	
	public Transform rootBone()
	{
		return meshRender.bones[0];
	}
	
	public void setNoise(float noise)
	{
		
	}
	
	void Start()
	{
		meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
		
		mapBones();
		
		refreshMesh();
		
		calculateTriGraph();
		
		randomMeshSpawn();
		
		calculateBoneCenters();
	}
	
	void FixedUpdate()
	{
		refreshMesh();
		
		Mesh mesh = getCurrentMesh();
		
		Transform root = meshRender.bones[0].parent;
		
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		
		for(int u=0; u<units.Count; u++)
		{
			Transform unit = units[u];
			
			Vector3 boneDirec;
			
			Vector3 boneCenter = findBoneCenter(u, out boneDirec);
			
			anchors[u].position = boneCenter + (Quaternion.AngleAxis(3f, boneDirec) * (anchors[u].position - boneCenter));
			
			anchors[u].position = meshIntersection(u, vertices, triangles, boneCenter);
			
			unit.position = anchors[u].position + Noise*noiseDirecs[u];
			
			Vector3 collapsedPos = Vector3.Project(unit.position - transform.position, Vector3.right) + transform.position;
			
			unit.position = (1-Collapse)*unit.position + Collapse*collapsedPos;
		}
	}
	
	void OnDrawGizmos()
	{
		if(showGuides)
		{
			meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
			
			mapBones();
			refreshMesh();
			
			Gizmos.color = Color.cyan;
			
			for(int i=0; i<boneStarts.Count; i++)
			{
				Gizmos.DrawLine(boneStarts[i], boneEnds[i]);
			}
			
			Gizmos.color = Color.red;
			
			Mesh mesh = getCurrentMesh();
			
			for(int i=0; i<mesh.triangles.Length; i+=3)
			{
				//Gizmos.DrawSphere(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[i]]), 0.1f);
				//Gizmos.DrawSphere(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[i+1]]), 0.1f);
				//Gizmos.DrawSphere(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[i+2]]), 0.1f);
			}
			
			int tri = 0;
			
			Gizmos.color = Color.cyan;
		
			if(triGraph != null)
			{
				//Debug.Log(triGraph[tri].Count);
				foreach(int t in triGraph[tri])
				{
					Gizmos.DrawLine(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[t*3]]), meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[t*3+1]]));
					Gizmos.DrawLine(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[t*3+1]]), meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[t*3+2]]));
					Gizmos.DrawLine(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[t*3+2]]), meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[t*3]]));
			
				}
			}
			
			Gizmos.color = Color.magenta;
			
			Gizmos.DrawLine(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[tri*3]]),  meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[tri*3+1]]));
			Gizmos.DrawLine(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[tri*3+1]]), meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[tri*3+2]]));
			Gizmos.DrawLine(meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[tri*3+2]]), meshRender.transform.TransformPoint(mesh.vertices[mesh.triangles[tri*3]]));
			
		}
	}
}