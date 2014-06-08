﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TETCSharpClient;
using TETCSharpClient.Data;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts
{
    class UnityGazeUtils : GazeUtils
    {
        /// <summary>
        /// Maps a GazeData gaze point (RawCoordinates or SmoothedCoordinates) to Unity screen space. 
        /// Note that gaze points have origo in top left corner, whilst Unity uses lower left.
        /// </summary>
        /// <param name="gp"/>gaze point to map</param>
        /// <returns>2d point mapped to unity window space</returns>
        public static Point2D getGazeCoordsToUnityWindowCoords(Point2D gp)
        {
#if UNITY_EDITOR
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			Vector2 windowPos = new Vector2(EditorWindow.GetWindow(T).position.x,EditorWindow.GetWindow(T).position.y);
//			Debug.Log("windowPos: " + windowPos.x + "," + windowPos.y);
			double rx = gp.X - windowPos.x;
			double ry = (GazeManager.Instance.ScreenResolutionHeight - gp.Y) - windowPos.y;
#else
			double rx = gp.X * ((double)Screen.width / GazeManager.Instance.ScreenResolutionWidth);
			double ry = (GazeManager.Instance.ScreenResolutionHeight - gp.Y) * ((double)Screen.height / GazeManager.Instance.ScreenResolutionHeight);
#endif
            return new Point2D(rx, ry);
        }

        /// <summary>
        /// Convert a Point2D to Unity vector.
        /// </summary>
        /// <param name="gp"/>gaze point to convert</param>
        /// <returns>a vector representation of point</returns>
        public static Vector2 Point2DToVec2(Point2D gp)
        {
            return new Vector2((float)gp.X, (float)gp.Y);
        }

        /// <summary>
        /// Converts a relative point to screen point in pixels using Unity classes
        /// </summary>
        public static Point2D getRelativeToScreenSpace(Point2D gp)
        {
            return getRelativeToScreenSpace(gp, Screen.width, Screen.height);
        }
    }
}
