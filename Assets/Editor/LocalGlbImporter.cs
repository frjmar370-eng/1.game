#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class LocalGlbImporter
{
    [Serializable] class Root { public Scene[] scenes; public Node[] nodes; public MeshDef[] meshes; public MaterialDef[] materials; public BufferView[] bufferViews; public Accessor[] accessors; }
    [Serializable] class Scene { public int[] nodes; }
    [Serializable] class Node { public string name; public int mesh = -1; public float[] translation; public float[] rotation; public float[] scale; }
    [Serializable] class MeshDef { public string name; public Primitive[] primitives; }
    [Serializable] class Primitive { public Attributes attributes; public int indices = -1; public int material = -1; }
    [Serializable] class Attributes { public int POSITION = -1; }
    [Serializable] class MaterialDef { public Pbr pbrMetallicRoughness; }
    [Serializable] class Pbr { public float[] baseColorFactor; }
    [Serializable] class BufferView { public int buffer; public int byteOffset; public int byteLength; }
    [Serializable] class Accessor { public int bufferView; public int componentType; public int count; public string type; }

    public static GameObject Import(string glbPath, string assetFolder, string rootName)
    {
        if (!File.Exists(glbPath)) return null;
        byte[] bytes = File.ReadAllBytes(glbPath);
        if (bytes.Length < 20 || Encoding.ASCII.GetString(bytes, 0, 4) != "glTF")
            throw new InvalidDataException("Not a GLB file: " + glbPath);

        int jsonLength = BitConverter.ToInt32(bytes, 12);
        int jsonType = BitConverter.ToInt32(bytes, 16);
        if (jsonType != 0x4E4F534A) throw new InvalidDataException("GLB JSON chunk missing.");
        string json = Encoding.UTF8.GetString(bytes, 20, jsonLength).TrimEnd(' ', '\0', '\r', '\n');
        Root root = JsonUtility.FromJson<Root>(json);
        int binHeader = 20 + jsonLength;
        int binLength = BitConverter.ToInt32(bytes, binHeader);
        int binType = BitConverter.ToInt32(bytes, binHeader + 4);
        if (binType != 0x004E4942) throw new InvalidDataException("GLB BIN chunk missing.");
        int binStart = binHeader + 8;

        Directory.CreateDirectory(assetFolder);
        GameObject rootGo = new GameObject(rootName);
        int[] sceneNodes = root.scenes != null && root.scenes.Length > 0 ? root.scenes[0].nodes : null;
        if (sceneNodes == null) return rootGo;
        foreach (int nodeIndex in sceneNodes)
            BuildNode(root, nodeIndex, rootGo.transform, bytes, binStart, assetFolder);
        return rootGo;
    }

    static void BuildNode(Root root, int nodeIndex, Transform parent, byte[] bytes, int binStart, string assetFolder)
    {
        if (nodeIndex < 0 || nodeIndex >= root.nodes.Length) return;
        Node n = root.nodes[nodeIndex];
        GameObject go = new GameObject(string.IsNullOrEmpty(n.name) ? "Node" + nodeIndex : n.name);
        go.transform.SetParent(parent, false);
        if (n.translation != null && n.translation.Length >= 3) go.transform.localPosition = new Vector3(n.translation[0], n.translation[1], n.translation[2]);
        if (n.rotation != null && n.rotation.Length >= 4) go.transform.localRotation = new Quaternion(n.rotation[0], n.rotation[1], n.rotation[2], n.rotation[3]);
        if (n.scale != null && n.scale.Length >= 3) go.transform.localScale = new Vector3(n.scale[0], n.scale[1], n.scale[2]);
        if (n.mesh >= 0 && n.mesh < root.meshes.Length)
        {
            MeshDef md = root.meshes[n.mesh];
            foreach (Primitive p in md.primitives)
            {
                if (p.attributes == null || p.attributes.POSITION < 0) continue;
                Mesh mesh = BuildMesh(root, p, bytes, binStart);
                string safe = Sanitize(rootName: go.name + "_" + (md.name ?? "Mesh"));
                string meshPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + safe + ".asset");
                AssetDatabase.CreateAsset(mesh, meshPath);
                GameObject part = new GameObject(md.name ?? "Mesh");
                part.transform.SetParent(go.transform, false);
                MeshFilter mf = part.AddComponent<MeshFilter>(); mf.sharedMesh = mesh;
                MeshRenderer mr = part.AddComponent<MeshRenderer>(); mr.sharedMaterial = MakeMaterial(root, p.material);
                MeshCollider mc = part.AddComponent<MeshCollider>(); mc.sharedMesh = mesh;
            }
        }
    }

    static Mesh BuildMesh(Root root, Primitive p, byte[] bytes, int binStart)
    {
        Accessor pa = root.accessors[p.attributes.POSITION];
        BufferView pv = root.bufferViews[pa.bufferView];
        int start = binStart + pv.byteOffset;
        Vector3[] vertices = new Vector3[pa.count];
        for (int i = 0; i < pa.count; i++)
        {
            int o = start + i * 12;
            vertices[i] = new Vector3(BitConverter.ToSingle(bytes, o), BitConverter.ToSingle(bytes, o + 4), BitConverter.ToSingle(bytes, o + 8));
        }
        int[] triangles;
        if (p.indices >= 0)
        {
            Accessor ia = root.accessors[p.indices]; BufferView iv = root.bufferViews[ia.bufferView]; int s = binStart + iv.byteOffset;
            triangles = new int[ia.count];
            for (int i = 0; i < ia.count; i++)
                triangles[i] = ia.componentType == 5123 ? BitConverter.ToUInt16(bytes, s + i * 2) : BitConverter.ToInt32(bytes, s + i * 4);
        }
        else { triangles = new int[vertices.Length]; for (int i = 0; i < triangles.Length; i++) triangles[i] = i; }
        Mesh m = new Mesh(); m.name = "GLBMesh"; m.vertices = vertices; m.triangles = triangles; m.RecalculateNormals(); m.RecalculateBounds(); return m;
    }

    static Material MakeMaterial(Root root, int index)
    {
        Color c = new Color(.7f,.7f,.7f,1);
        if (root.materials != null && index >= 0 && index < root.materials.Length && root.materials[index].pbrMetallicRoughness != null)
        {
            float[] a = root.materials[index].pbrMetallicRoughness.baseColorFactor;
            if (a != null && a.Length >= 3) c = new Color(a[0], a[1], a[2], a.Length > 3 ? a[3] : 1);
        }
        Material mat = new Material(Shader.Find("Standard")); mat.name = "GLBMaterial"; mat.color = c; return mat;
    }

    static string Sanitize(string rootName) { foreach (char c in Path.GetInvalidFileNameChars()) rootName = rootName.Replace(c, '_'); return rootName; }
}
#endif