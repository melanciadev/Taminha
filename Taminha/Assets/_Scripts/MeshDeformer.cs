using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Melancia.Taminha {
	public class MeshDeformer:MonoBehaviour {
		public Vector2 seed = Vector2.zero;
		public Vector2 spawnPos = Vector2.zero;

		public Texture2D bg;
		public MeshFilter filter;
		public LineRenderer line;
		Mesh mesh;

		public Vector2[] points;
		List<int> triangleList;
		
		Vector3[] vertices;
		Vector2[] uv;
		Vector3[] normals;
		int[] triangles;
		int[] defaultTriangles;

		const int pointCount = 64;
		const float eyesProp = 4;
		const float eyesWidth = .15f;

		static Vector2[] defaultPoints = {
			new Vector2(-.061f,+.332f),
			new Vector2(-.121f,+.284f),
			new Vector2(-.175f,+.205f),
			new Vector2(-.200f,+.142f),
			new Vector2(-.228f,+.071f),
			new Vector2(-.254f,-.019f),
			new Vector2(-.274f,-.086f),
			new Vector2(-.295f,-.151f),
			new Vector2(-.314f,-.215f),
			new Vector2(-.336f,-.297f),
			new Vector2(-.280f,-.266f),
			new Vector2(-.220f,-.276f),
			new Vector2(-.153f,-.290f),
			new Vector2(-.080f,-.251f),
			new Vector2(-.013f,-.265f),
			new Vector2(+.039f,-.291f),
			new Vector2(+.092f,-.268f),
			new Vector2(+.142f,-.257f),
			new Vector2(+.189f,-.266f),
			new Vector2(+.249f,-.270f),
			new Vector2(+.313f,-.242f),
			new Vector2(+.298f,-.188f),
			new Vector2(+.268f,-.130f),
			new Vector2(+.251f,-.085f),
			new Vector2(+.234f,-.020f),
			new Vector2(+.225f,+.045f),
			new Vector2(+.216f,+.096f),
			new Vector2(+.199f,+.183f),
			new Vector2(+.168f,+.279f),
			new Vector2(+.134f,+.331f),
			new Vector2(+.083f,+.361f),
			new Vector2(+.011f,+.357f),
		};
		
		void Awake() {
			points = new Vector2[pointCount+4];
			vertices = new Vector3[pointCount+4];
			uv = new Vector2[pointCount+4];
			uv[pointCount] = new Vector2(0,.5f);
			uv[pointCount+1] = new Vector2(1,.5f);
			uv[pointCount+2] = new Vector2(0,1);
			uv[pointCount+3] = new Vector2(1,1);
			normals = new Vector3[pointCount+4];
			for (int a = 0; a < pointCount; a++) {
				normals[a] = -Vector3.forward;
			}
			triangleList = new List<int>();
			defaultTriangles = new int[] {
				pointCount,
				pointCount+3,
				pointCount+1,
				pointCount,
				pointCount+2,
				pointCount+3,
			};
		}

		void Start() {
			mesh = new Mesh();
			mesh.name = "taminha";
			mesh.MarkDynamic();
			filter.sharedMesh = mesh;
			line.positionCount = pointCount;
			ResetPoints();
			ApplyMesh();
		}

		public void ResetPoints() {
			for (int a = 0; a < defaultPoints.Length; a++) {
				points[a*2] = defaultPoints[a];
				points[a*2+1] = Vector2.Lerp(defaultPoints[a],defaultPoints[(a+1)%defaultPoints.Length],.5f);
			}
			for (int a = 0; a < pointCount; a++) {
				points[a] += spawnPos;
			}
		}

		public void FixEyes() {
			const float w = eyesWidth*.5f;
			const float h = w/eyesProp;
			const float z = -.1f;
			float x = 0, y = 0;
			float x0 = 0, x1 = 0, y0 = 0, y1 = 0;
			for (int a = 0; a < pointCount; a++) {
				var xt = points[a].x;
				var yt = points[a].y;
				if (a > 0) {
					if (x0 > xt) x0 = xt;
					if (x1 < xt) x1 = xt;
					if (y0 > yt) y0 = yt;
					if (y1 < yt) y1 = yt;
				} else {
					x0 = xt;
					x1 = xt;
					y0 = yt;
					y1 = yt;
				}
				x += xt;
			}
			x /= pointCount;
			x = Mathf.Clamp(x,x0,x1);
			float factor = 1f/(Mathf.Max(Mathf.Abs(x-x0),Mathf.Abs(x-x1)));
			float ys = 0;
			for (int a = 0; a < pointCount; a++) {
				float t = 1-Mathf.Abs(points[a].x-x)*factor;
				t = (3f-2f*t)*t*t;
				y += points[a].y*t;
				ys += t;
			}
			y = Mathf.Lerp(y/ys,Mathf.Lerp(y0,y1,.8f),.3f);
			points[pointCount] = new Vector3(x-w,y-h,z);
			points[pointCount+1] = new Vector3(x+w,y-h,z);
			points[pointCount+2] = new Vector3(x-w,y+h,z);
			points[pointCount+3] = new Vector3(x+w,y+h,z);
		}

		public bool Deform(Vector2 pos,Vector2 dir) {
			const float maxDist = .25f;
			var apply = false;
			for (int a = 0; a < pointCount; a++) {
				var p = points[a];
				var dist = (p-pos).sqrMagnitude;
				if (dist >= maxDist*maxDist) continue;
				var t = 1-Mathf.Sqrt(dist)/maxDist;
				t = (3f-2f*t)*t*t*.9f;
				points[a] = new Vector2(p.x+dir.x*t,p.y+dir.y*t);
				apply = true;
			}
			return apply;
		}

		public void Move(Vector2 dir) {
			for (int a = 0; a < pointCount; a++) {
				points[a] += dir;
			}
		}
		
		public bool ApplyMesh(bool triangulate = true) {
			bool reset = false;
			if (triangulate) {
				if (!Triangulate()) {
					reset = true;
					ResetPoints();
					triangleList.Clear();
					Triangulate();
				}
				triangleList.AddRange(defaultTriangles);
				triangles = triangleList.ToArray();
				triangleList.Clear();
				FixEyes();
			}
			for (int a = 0; a < points.Length; a++) {
				var perlin = Perlin(points[a]);
				vertices[a] = new Vector3(points[a].x+perlin.x,points[a].y+perlin.y,0);
			}
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.normals = normals;
			mesh.triangles = triangles;
			mesh.RecalculateBounds();
			line.SetPositions(vertices);
			return reset;
		}

		Vector2 Perlin(Vector2 v) {
			const float offset = .003f;
			var x = Mathf.Lerp(-offset,offset,Mathf.PerlinNoise((v.x+seed.x)*100,(v.y+seed.y)*100));
			var y = Mathf.Lerp(-offset,offset,Mathf.PerlinNoise((v.x+seed.y)*100,(v.y+seed.x)*100));
			return new Vector2(x,y);
		}
		
		bool Triangulate() {
			int n = pointCount;
			if (n < 3) return true;

			int[] V = new int[n];
			if (Area() > 0) {
				for (int v = 0; v < n; v++) V[v] = v;
			} else {
				for (int v = 0; v < n; v++) V[v] = (n - 1) - v;
			}

			int nv = n;
			int count = 2 * nv;
			for (int m = 0, v = nv - 1; nv > 2;) {
				if ((count--) <= 0) return false;

				int u = v;
				if (nv <= u) u = 0;
				v = u + 1;
				if (nv <= v) v = 0;
				int w = v + 1;
				if (nv <= w) w = 0;

				if (Snip(u,v,w,nv,V)) {
					triangleList.Add(V[u]);
					triangleList.Add(V[w]);
					triangleList.Add(V[v]);
					m++;
					for (int s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
					nv--;
					count = 2 * nv;
				}
			}

			return true;
		}

		float Area() {
			int n = pointCount;
			float A = 0;
			for (int p = n - 1, q = 0; q < n; p = q++) {
				Vector2 pval = points[p];
				Vector2 qval = points[q];
				A += pval.x * qval.y - qval.x * pval.y;
			}
			return A*.5f;
		}

		bool Snip(int u,int v,int w,int n,int[] V) {
			int p;
			Vector2 A = points[V[u]];
			Vector2 B = points[V[v]];
			Vector2 C = points[V[w]];
			if (Mathf.Epsilon > (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x)) return false;
			for (p = 0; p < n; p++) {
				if (p == u || p == v || p == w) continue;
				Vector2 P = points[V[p]];
				if (InsideTriangle(A,B,C,P)) return false;
			}
			return true;
		}

		bool InsideTriangle(Vector2 A,Vector2 B,Vector2 C,Vector2 P) {
			float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
			float cCROSSap, bCROSScp, aCROSSbp;

			ax = C.x - B.x; ay = C.y - B.y;
			bx = A.x - C.x; by = A.y - C.y;
			cx = B.x - A.x; cy = B.y - A.y;
			apx = P.x - A.x; apy = P.y - A.y;
			bpx = P.x - B.x; bpy = P.y - B.y;
			cpx = P.x - C.x; cpy = P.y - C.y;

			aCROSSbp = ax * bpy - ay * bpx;
			cCROSSap = cx * apy - cy * apx;
			bCROSScp = bx * cpy - by * cpx;

			return aCROSSbp >= 0 && bCROSScp >= 0 && cCROSSap >= 0;
		}

		public Color32[] RasteriseToPixels() {
			var w = bg.width;
			var h = bg.height;
			var w1 = w-1;
			var h1 = h-1;
			var n = w*h;
			var pixels = new Color32[n];
			var black = new Color32(0,0,0,255);
			var white = new Color32(255,255,255,255);
			for (int a = 0; a < n; a++) {
				pixels[a] = black;
			}
			for (int i = 0; i < triangles.Length-6; i += 3) {
				var avf = points[triangles[i]];
				var bvf = points[triangles[i+1]];
				var cvf = points[triangles[i+2]];
				int ax = Mathf.RoundToInt((avf.x+.5f)*w);
				int ay = Mathf.RoundToInt((avf.y+.5f)*h);
				int bx = Mathf.RoundToInt((bvf.x+.5f)*w);
				int by = Mathf.RoundToInt((bvf.y+.5f)*h);
				int cx = Mathf.RoundToInt((cvf.x+.5f)*w);
				int cy = Mathf.RoundToInt((cvf.y+.5f)*h);
				int x0 = w1;
				if (x0 > ax) x0 = ax;
				if (x0 > bx) x0 = bx;
				if (x0 > cx) x0 = cx;
				if (x0 < 0) x0 = 0;
				int x1 = 0;
				if (x1 < ax) x1 = ax;
				if (x1 < bx) x1 = bx;
				if (x1 < cx) x1 = cx;
				if (x1 > w1) x1 = w1;
				int y0 = h1;
				if (y0 > ay) y0 = ay;
				if (y0 > by) y0 = by;
				if (y0 > cy) y0 = cy;
				if (y0 < 0) y0 = 0;
				int y1 = 0;
				if (y1 < ay) y1 = ay;
				if (y1 < by) y1 = by;
				if (y1 < cy) y1 = cy;
				if (y1 > h1) y1 = h1;
				var av = new Vector2(ax,ay);
				var bv = new Vector2(bx,by);
				var cv = new Vector2(cx,cy);
				for (int x = x0; x <= x1; x++) {
					for (int y = y0; y <= y1; y++) {
						if (InsideTriangle(av,cv,bv,new Vector2(x,y))) {
							pixels[x+y*w] = white;
						}
					}
				}
			}
			return pixels;
		}

		public Texture2D RasteriseToTexture() {
			var w = bg.width;
			var h = bg.height;
			var tex = new Texture2D(w,h,TextureFormat.ARGB32,false);
			tex.SetPixels32(RasteriseToPixels());
			tex.Apply();
			return tex;
		}

		public float GetScore(Texture2D tex) {
			if (tex.width != bg.width || tex.height != bg.height) return 0;
			var pixels = tex.GetPixels32();
			return GetScore(pixels);
		}

		public float GetScore(Color32[] pixels) {
			var mask = bg.GetPixels32();
			int num = 0;
			int den = 0;
			for (int a = 0; a < pixels.Length; a++) {
				bool x = pixels[a].r >= 128;
				bool y = mask[a].r >= 128;
				if (y) {
					den++;
					if (x) num++;
				} else if (x) {
					num--;
				}
			}
			return (den == 0 || num <= 0) ? 0 : (num >= den ? 1 : ((float)num/den));
		}

		public float GetScore() {
			return GetScore(RasteriseToPixels());
		}
	}
}