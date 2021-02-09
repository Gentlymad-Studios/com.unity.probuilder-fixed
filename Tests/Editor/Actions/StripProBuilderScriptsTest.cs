﻿using UObject = UnityEngine.Object;
using NUnit.Framework;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder.Actions;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;

public class StripProBuilderScriptsTest
{
    [Test]
    public void StripProBuilderScripts_RemovesMeshAndPolyShapeComponents()
    {
        var go = new GameObject();
        var poly = go.AddComponent<PolyShape>();
        go.AddComponent<ProBuilderMesh>();
        poly.CreateShapeFromPolygon();

        Assume.That(go.GetComponent<ProBuilderMesh>() != null);
        Assume.That(go.GetComponent<PolyShape>() != null);

        StripProBuilderScripts.DoStrip(go.GetComponent<ProBuilderMesh>());

        Assert.That(go.GetComponent<ProBuilderMesh>() == null);
        Assert.That(go.GetComponent<PolyShape>() == null);

        UObject.DestroyImmediate(go);
    }

    [Test]
    public void StripProBuilderScripts_RemovesMeshAndBezierComponents()
    {
        var go = new GameObject();
        go.AddComponent<ProBuilderMesh>();
        var bezier = go.AddComponent<BezierShape>();
        bezier.Init();
        bezier.Refresh();

        Assume.That(go.GetComponent<ProBuilderMesh>() != null);
        Assume.That(go.GetComponent<BezierShape>() != null);

        StripProBuilderScripts.DoStrip(go.GetComponent<ProBuilderMesh>());

        Assert.That(go.GetComponent<ProBuilderMesh>() == null);
        Assert.That(go.GetComponent<BezierShape>() == null);

        UObject.DestroyImmediate(go);
    }

    [Test]
    public void StripProBuilderScripts_RemovesMeshAndShapeComponents()
    {
        var go = new GameObject();
        var shape = go.AddComponent<ShapeComponent>();
        shape.Rebuild(new Bounds(Vector3.zero, Vector3.one), Quaternion.identity);

        Assume.That(go.GetComponent<ProBuilderMesh>() != null);
        Assume.That(go.GetComponent<ShapeComponent>() != null);

        StripProBuilderScripts.DoStrip(go.GetComponent<ProBuilderMesh>());

        Assert.That(go.GetComponent<ProBuilderMesh>() == null);
        Assert.That(go.GetComponent<ShapeComponent>() == null);

        UObject.DestroyImmediate(go);
    }

    [Test]
    public void OnPostProcessScene_StripProBuilderScripts_RemovesMeshAndShapeOnEnabledObjects()
    {
        var go = new GameObject();
        var shape = go.AddComponent<ShapeComponent>();
        shape.Rebuild(new Bounds(Vector3.zero, Vector3.one), Quaternion.identity);

        Assume.That(go.GetComponent<ProBuilderMesh>() != null);
        Assume.That(go.GetComponent<ShapeComponent>() != null);

        var goChild = GameObject.Instantiate(go, go.transform);
        goChild.name = "Child GO";

        Assume.That(goChild.GetComponent<ProBuilderMesh>() != null);
        Assume.That(goChild.GetComponent<ShapeComponent>() != null);

        UnityScenePostProcessor.OnPostprocessScene();

        Assert.That(go.GetComponent<ProBuilderMesh>() == null);
        Assert.That(go.GetComponent<ShapeComponent>() == null);
        Assert.That(goChild.GetComponent<ProBuilderMesh>() == null);
        Assert.That(goChild.GetComponent<ShapeComponent>() == null);

        UObject.DestroyImmediate(go);
    }

    [Test]
    public void OnPostProcessScene_StripProBuilderScripts_RemovesMeshAndShapeOnDisabledObjects()
    {
        var go = new GameObject("Parent GO");
        var shape = go.AddComponent<ShapeComponent>();
        shape.Rebuild(new Bounds(Vector3.zero, Vector3.one), Quaternion.identity);

        Assume.That(go.GetComponent<ProBuilderMesh>() != null);
        Assume.That(go.GetComponent<ShapeComponent>() != null);

        var goChild = GameObject.Instantiate(go, go.transform);
        goChild.name = "Child GO";

        Assume.That(goChild.GetComponent<ProBuilderMesh>() != null);
        Assume.That(goChild.GetComponent<ShapeComponent>() != null);

        go.SetActive(false);
        goChild.GetComponent<ShapeComponent>().enabled = false;
        goChild.GetComponent<ProBuilderMesh>().enabled = false;

        UnityScenePostProcessor.OnPostprocessScene();

        Assert.That(go.GetComponent<ProBuilderMesh>() == null);
        Assert.That(go.GetComponent<ShapeComponent>() == null);
        Assert.That(goChild.GetComponent<ProBuilderMesh>() == null);
        Assert.That(goChild.GetComponent<ShapeComponent>() == null);

        UObject.DestroyImmediate(go);
    }
}
