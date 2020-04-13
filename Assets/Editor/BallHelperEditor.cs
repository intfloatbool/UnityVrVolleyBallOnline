using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VrVolleyball.EditorHelpers
{
    [CustomEditor(typeof(BallOnline))]
    public class BallHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BallOnline ball = (BallOnline)target;

            if(GUILayout.Button("Add debug force"))
            {
                ball.AffectToBall(ball.DebugForce, ball.DebugStrength);
            }
        }
    }

}
