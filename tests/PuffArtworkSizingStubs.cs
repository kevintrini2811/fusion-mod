// Geometry doubles only. No Unity or game assembly is loaded.
namespace UnityEngine;
public class Object { public static implicit operator bool(Object? obj)=>obj!=null; }
public struct Vector3 {
 public float x,y,z;
 public Vector3(float x,float y,float z){this.x=x;this.y=y;this.z=z;}
 public static Vector3 zero=>new();
 public static Vector3 operator *(Vector3 p,float n)=>new(p.x*n,p.y*n,p.z*n);
}
public struct Bounds {
 Vector3 minPoint,maxPoint;
 public Bounds(Vector3 center,Vector3 size){minPoint=new(center.x-size.x/2,center.y-size.y/2,center.z-size.z/2);maxPoint=new(center.x+size.x/2,center.y+size.y/2,center.z+size.z/2);}
 public Vector3 size=>new(maxPoint.x-minPoint.x,maxPoint.y-minPoint.y,maxPoint.z-minPoint.z);
 public Vector3 min=>minPoint;
 public Vector3 center=>new((minPoint.x+maxPoint.x)/2,(minPoint.y+maxPoint.y)/2,(minPoint.z+maxPoint.z)/2);
 public void Encapsulate(Vector3 p){minPoint=new(MathF.Min(minPoint.x,p.x),MathF.Min(minPoint.y,p.y),MathF.Min(minPoint.z,p.z));maxPoint=new(MathF.Max(maxPoint.x,p.x),MathF.Max(maxPoint.y,p.y),MathF.Max(maxPoint.z,p.z));}
}
