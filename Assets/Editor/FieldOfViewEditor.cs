using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{

    protected virtual void OnSceneGUI()
    {
        //Transform posicao = ((FieldOfView)target).transform;

        FieldOfView fow = target as FieldOfView;
        if (fow == null) return;
        Handles.color = Color.red;
        //        Handles.DrawWireArc(fow.transform.position, fow.transform.up, fow.transform.forward, 360f, fow.viewRadius);
        //        Handles.DrawWireArc(fow.transform.position, Vector3.up, fow.transform.forward, 360, fow.viewRadius);
        //Handles.CircleHandleCap(0, fow.transform.position, Quaternion.LookRotation(Vector3.up), fow.viewRadius, EventType.Repaint);

        Handles.CircleHandleCap(
               0,
               fow.transform.position,
               Quaternion.LookRotation(Vector3.up),
               fow.viewRadius/10,
               EventType.Repaint
           );


    }

}
