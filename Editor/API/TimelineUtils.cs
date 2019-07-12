using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor;
using UnityEditor.Timeline;
using Unity.TimelineTools.API;

namespace Unity.TimelineTools.Utilities
{
    
    public static class TimelineUtils
    {
        /// <summary>
        /// Creates a Timeline asset at the given path. The path must exist in advance
        /// </summary>
        /// <param name="timelineAssetPath"></param>
        /// <returns></returns>
        public static TimelineAsset CreateTimelineAssetAtPath(string timelineAssetPath)
        {
            var timeline_asset = ScriptableObject.CreateInstance<TimelineAsset>();
            var uniquePlayableName = AssetDatabase.GenerateUniqueAssetPath(timelineAssetPath);
            AssetDatabase.CreateAsset(timeline_asset, uniquePlayableName);

            return timeline_asset;
        }

        /// <summary>
        /// Generate a new Timeline asset with the given name in the 'Assets/Timeline' folder of the project
        /// </summary>
        /// <param name="timelineAssetName"></param>
        /// <returns></returns>
        public static TimelineAsset CreateTimelineAsset(string timelineAssetName)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Timeline"))
            {
                AssetDatabase.CreateFolder("Assets", "Timeline");
            }
            var ta = ScriptableObjectUtility.CreateAssetType<TimelineAsset>("Assets/Timeline", timelineAssetName);
            return ta;
        }

        public static GameObject CreatePlayableDirectorObject(string name = null)
        {
            var gameObj = new GameObject(name);
            gameObj.AddComponent<PlayableDirector>();

            return gameObj;
        }

        public static PlayableDirector SetPlayableAsset(GameObject go, TimelineAsset asset)
        {
            var dir = go.GetComponent<PlayableDirector>();
            dir.playableAsset = asset;

            return dir;
        }
   
        public static TimelineAsset GetCurrentActiveTimeline()
        {
            return TimelineEditor.inspectedAsset;
        }

        public static void SetTimeline(TimelineAsset timeline, PlayableDirector director)
        {
            var window = TimelineBase.timelineWindow;

            var method = window.GetType().GetMethod("SetCurrentTimeline", new[] { typeof(PlayableDirector), typeof(TimelineClip) });
            var retVal = method.Invoke(window, new object[] { director, null });

            Verify(retVal);
        }

        public static PlayableDirector GetDirectorFromTimeline(TimelineAsset timeline)
        {
            var directors = Resources.FindObjectsOfTypeAll<PlayableDirector>();
            var director = Array.Find(directors, d => d.playableAsset == timeline);

            return director;
        }

        public static TimelineAsset GetTimelineAssetFromDirector(PlayableDirector director)
        {
            if (director == null)
                return null;
            return director.playableAsset as TimelineAsset;
        }

        public static void SetPlayheadByFrame(PlayableDirector director, float fps, double gotoFrame)
        {
            var setTimeMethod = director.GetType().GetMethod("set_time");
            var retVal = setTimeMethod.Invoke(director, new object[] { gotoFrame / fps });

            Verify(retVal);

        }

        public static void SetPlayheadBySeconds(PlayableDirector director, double gotoTime)
        {
            var setTimeMethod = director.GetType().GetMethod("set_time");
            var retVal = setTimeMethod.Invoke(director, new object[] { gotoTime });

            Verify(retVal);
        }

        public static void Verify(System.Object obj)
        {
            if (obj != null)
            {
                StackTrace stackTrace = new StackTrace();
                UnityEngine.Debug.Log("Error in TimelineBase: " + stackTrace.GetFrame(1).GetMethod().Name);
            }
        }
    }

}