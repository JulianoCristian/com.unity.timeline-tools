using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.TimelineTools.API
{
    /// <summary>
    /// Wrapper for all of the reflection required in the system. 
    /// Good pattern to follow until we can get the internals bridged for Timeline
    /// </summary>
    static class TimelineBase
    {
        public static string errors;

        public static readonly Type timelineHelpers =
            Type.GetType("UnityEditor.Timeline.TimelineHelpers, Unity.Timeline.Editor");

        public static readonly Type selectionManager =
            Type.GetType("UnityEditor.Timeline.SelectionManager, Unity.Timeline.Editor");

        public static readonly Type clipItem =
            Type.GetType("UnityEditor.Timeline.ClipItem, Unity.Timeline.Editor");

        public static readonly Type movingItems =
            Type.GetType("UnityEditor.Timeline.MovingItems, Unity.Timeline.Editor");

        public static readonly Type editMode =
            Type.GetType("UnityEditor.Timeline.EditMode, Unity.Timeline.Editor");

        public static readonly Type editType =
            Type.GetType("UnityEditor.Timeline.EditMode+EditType, Unity.Timeline.Editor");

        public static readonly Type timelineWindow =
            Type.GetType("UnityEditor.Timeline.TimelineWindow, Unity.Timeline.Editor");

        [InitializeOnLoadMethod]
        static void SanityCheck()
        {
            var fields = typeof(TimelineBase)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(Type));

            var failed = fields.Where(field => field.GetValue(null) == null);

            if (failed.Any())
                errors =
                    "Failed to fetch internal timeline types: " +
                    failed.Select(field => field.Name).Aggregate((a, b) => $"{a}, {b}") +
                    $" ({failed.Count()}/{fields.Count()})";
        }
    }
}
