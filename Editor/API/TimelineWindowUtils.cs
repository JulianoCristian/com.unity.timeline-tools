using System.Reflection;            // FIXME: Remove all reflection, isolate into TimelineBase.cs
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace Unity.TimelineTools.API
{
    /// <summary>
    /// Helper functions for manipulating the Timeline Window
    /// </summary>
    public static class TimelineWindowUtils
    {
        public static EditorWindow GetWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/Sequencing/Timeline");

            var timelineWindowType = Assembly.Load("UnityEditor.Timeline").GetType("UnityEditor.Timeline.TimelineWindow");

            // Assuming there always only one timeline window
            var timeline_window = Resources.FindObjectsOfTypeAll(timelineWindowType)[0] as EditorWindow;
            return timeline_window;
        }

        public static void ToggleLock()
        {
            var window = GetWindow();
            PropertyInfo propertyInfo = window.GetType().GetProperty("locked");
            bool value = (bool)propertyInfo.GetValue(window, null);
            propertyInfo.SetValue(window, !value, null);
            window.Repaint();
        }

        public static bool GetLockStatus()
        {
            var window = GetWindow();
            PropertyInfo propertyInfo = window.GetType().GetProperty("locked");
            return (bool)propertyInfo.GetValue(window, null);
        }

        public static void SetLockStatus(bool newState)
        {
            var window = GetWindow();
            PropertyInfo propertyInfo = window.GetType().GetProperty("locked");
            propertyInfo.SetValue(window, newState, null);
            window.Repaint();
        }

        public static bool GetRecordingMode()
        {
            bool previewMode, recording;

            GetModes(out previewMode, out recording);
            return recording;
        }

        public static bool GetPreviewMode()
        {
            bool previewMode, recording;

            GetModes(out previewMode, out recording);
            return previewMode;
        }

        public static void SetPreviewMode(bool value)
        {
            var timelineWindowType = TimelineBase.timelineWindow;
            var instance = timelineWindowType.GetProperty("instance");
            var timelineWindow = instance.GetValue(null);

            var state = timelineWindowType.GetProperty("state").GetValue(timelineWindow);
            var stateType = state.GetType();

            stateType.GetProperty("previewMode").SetValue(state, value);
            stateType.GetProperty("rebuildGraph").SetValue(state, true);
        }

        public static void SetCursor(double time)
        {
            var state = typeof(TimelineEditor)
                .GetProperty("state", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            var editSequence = state
                .GetType()
                .GetProperty("editSequence")
                .GetValue(state);

            editSequence
                .GetType()
                .GetProperty("time")
                .SetValue(editSequence, time);
        }

        public static double GetCursor()
        {
            var state = typeof(TimelineEditor)
                .GetProperty("state", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            if (state == null)
                return -1.0;

            var editSequence = state
                .GetType()
                .GetProperty("editSequence")
                .GetValue(state);

            double time = (double)editSequence
                .GetType()
                .GetProperty("time")
                .GetValue(editSequence);

            return time;
        }

        public static void Repaint()
        {
            var timelineWindowType = TimelineBase.timelineWindow;
            var instance = timelineWindowType.GetProperty("instance");
            var timelineWindow = instance.GetValue(null);

            var state = timelineWindowType.GetProperty("state").GetValue(timelineWindow);
            var stateType = state.GetType();

            stateType.GetProperty("rebuildGraph").SetValue(state, true); // paranoia
            timelineWindowType.GetMethod("Repaint").Invoke(timelineWindow, null);
        }

        public static object GetState()
        {
            var timelineWindowType = TimelineBase.timelineWindow;
            var instance = timelineWindowType.GetProperty("instance");
            var timelineWindow = instance.GetValue(null);

            return timelineWindowType.GetProperty("state").GetValue(timelineWindow);
        }

        public static void GetModes(out bool previewMode, out bool recording)
        {
            var state = GetState();
            var stateType = state.GetType();

            previewMode = (bool)stateType.GetProperty("previewMode").GetValue(state);
            recording = (bool)stateType.GetProperty("recording").GetValue(state);
        }

    }
}