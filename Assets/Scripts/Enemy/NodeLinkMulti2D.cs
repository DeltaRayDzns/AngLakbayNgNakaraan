using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Build multiple NodeLink2 connections from one component
/// without modifying A* internals.
/// - HubFromThis: this transform is the start; connect to every end in 'ends'.
/// - RailFromEnds: connect ends[i] -> ends[i+1] (and optionally loop).
/// Under the hood it spawns/deletes child GameObjects that each hold a NodeLink2.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("Pathfinding/NodeLink2 Multi (2D)")]
public class NodeLinkMulti2D : MonoBehaviour
{
    public enum BuildMode { HubFromThis, RailFromEnds }

    [Header("Mode")]
    public BuildMode mode = BuildMode.HubFromThis;
    [Tooltip("If true in Rail mode, connect last -> first too")]
    public bool loopRail = false;

    [Header("Ends")]
    [Tooltip("Endpoints to connect. In Hub mode: this -> each end. In Rail mode: consecutive pairs.")]
    public List<Transform> ends = new List<Transform>();

    [Header("Convenience")]
    [Tooltip("If true, 'ends' will be filled from children of 'endsRoot' (or this transform if empty).")]
    public bool useChildrenAsEnds = false;
    public string endsRootName = ""; // optional folder name under this object

    [Header("Link Settings (applied to all generated NodeLink2)")]
    public float costFactor = 1f;
    public bool oneWay = false;

    [Header("Editor")]
    [Tooltip("Hide generated NodeLink2 children in the hierarchy (editor only).")]
    public bool hideGeneratedChildren = false;

    const string ChildPrefix = "__NL2_";

    void OnEnable()     { SyncNow(); }
    void OnValidate()   { SyncNow(); }
    void OnTransformChildrenChanged() { SyncNow(); }

    void SyncNow()
    {
        // 1) Build the desired connection list
        var pairs = new List<(Vector3 startPos, Transform startAnchor, Transform end)>();

        // Optionally populate ends from children
        if (useChildrenAsEnds)
        {
            ends.Clear();
            Transform root = transform;
            if (!string.IsNullOrEmpty(endsRootName))
            {
                var found = transform.Find(endsRootName);
                if (found) root = found;
            }
            foreach (Transform t in root)
                if (t != transform) ends.Add(t);
        }

        if (mode == BuildMode.HubFromThis)
        {
            foreach (var end in ends)
            {
                if (!end) continue;
                pairs.Add((transform.position, null, end));
            }
        }
        else // RailFromEnds
        {
            if (ends.Count >= 2)
            {
                for (int i = 0; i < ends.Count - 1; i++)
                {
                    var a = ends[i]; var b = ends[i + 1];
                    if (!a || !b) continue;
                    pairs.Add((a.position, a, b));      // start at ends[i], end at ends[i+1]
                }
                if (loopRail)
                {
                    var a = ends[ends.Count - 1];
                    var b = ends[0];
                    if (a && b) pairs.Add((a.position, a, b));
                }
            }
        }

        // 2) Gather existing generated NodeLink2s under this object
        var existing = new List<NodeLink2>();
        foreach (Transform child in transform)
        {
            var link = child.GetComponent<NodeLink2>();
            if (link) existing.Add(link);
        }

        var used = new HashSet<NodeLink2>();

        // 3) Ensure one NodeLink2 per desired pair
        int index = 0;
        foreach (var pair in pairs)
        {
            NodeLink2 link = FindMatching(existing, pair.startAnchor, pair.end);
            if (link == null)
            {
                // Create a child GO for the link
                var go = new GameObject($"{ChildPrefix}{index}_to_{(pair.end ? pair.end.name : "NULL")}");
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.RegisterCreatedObjectUndo(go, "Create NodeLink2");
#endif
                // In Rail mode we parent the start under the start anchor so it follows that transform
                if (pair.startAnchor != null)
                {
                    go.transform.SetParent(pair.startAnchor, false);
                    go.transform.position = pair.startAnchor.position;
                }
                else
                {
                    go.transform.SetParent(transform, false);
                    go.transform.position = pair.startPos;
                }
                link = go.AddComponent<NodeLink2>();
            }
            else
            {
                // keep name tidy
                link.name = $"{ChildPrefix}{index}_to_{(pair.end ? pair.end.name : "NULL")}";
            }

            // Configure
            link.end = pair.end;
            link.costFactor = costFactor;
            link.oneWay = oneWay;

            // Hide in editor if desired
#if UNITY_EDITOR
            if (!Application.isPlaying && hideGeneratedChildren)
            {
                link.gameObject.hideFlags = HideFlags.HideInHierarchy;
                foreach (var c in link.GetComponents<Component>())
                    if (!(c is Transform)) c.hideFlags = HideFlags.HideInInspector;
            }
            else if (!Application.isPlaying)
            {
                link.gameObject.hideFlags = HideFlags.None;
                foreach (var c in link.GetComponents<Component>())
                    if (!(c is Transform)) c.hideFlags = HideFlags.None;
            }
#endif
            used.Add(link);
            index++;
        }

        // 4) Delete stale generated links
        foreach (var link in existing)
        {
            if (!link || used.Contains(link)) continue;
#if UNITY_EDITOR
            if (!Application.isPlaying) Undo.DestroyObjectImmediate(link.gameObject);
            else
#endif
                Destroy(link.gameObject);
        }
    }

    NodeLink2 FindMatching(List<NodeLink2> list, Transform startAnchor, Transform end)
    {
        foreach (var l in list)
        {
            if (!l) continue;
            bool sameEnd = l.end == end;
            bool sameStart =
                (startAnchor == null && l.transform.parent == transform) ||
                (startAnchor != null && l.transform.parent == startAnchor);
            if (sameEnd && sameStart) return l;
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        // Pretty gizmos so you see what will be built
        Gizmos.color = new Color(1f, 0.6f, 0.15f, 0.9f);
        if (mode == BuildMode.HubFromThis)
        {
            foreach (var e in ends)
            {
                if (!e) continue;
                DrawCurve(transform.position, e.position);
            }
        }
        else
        {
            for (int i = 0; i < ends.Count - 1; i++)
            {
                var a = ends[i]; var b = ends[i + 1];
                if (a && b) DrawCurve(a.position, b.position);
            }
            if (loopRail && ends.Count > 1)
            {
                var a = ends[ends.Count - 1]; var b = ends[0];
                if (a && b) DrawCurve(a.position, b.position);
            }
        }
    }

    static void DrawCurve(Vector3 a, Vector3 b)
    {
        Vector3 m = (a + b) * 0.5f + Vector3.up * 0.5f;
        const int steps = 12;
        Vector3 p = a;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 q = Vector3.Lerp(Vector3.Lerp(a, m, t), Vector3.Lerp(m, b, t), t);
            Gizmos.DrawLine(p, q);
            p = q;
        }
    }
}
