using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ETeam.FeelJoon;

[CustomEditor(typeof(EnemyController))]
public class EnemyController_Editor : Editor
{
    void OnSceneGUI()
    {
        EnemyController enemy = target as EnemyController;

        Handles.color = Color.blue;
        Handles.DrawWireDisc(enemy.transform.position, Vector3.up, enemy.viewRadius);


        Handles.color = Color.red;
        Handles.DrawWireDisc(enemy.transform.position, Vector3.up, enemy.attackRange);
    }
}
