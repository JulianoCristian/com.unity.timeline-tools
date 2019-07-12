using System;
using System.Collections.Generic;
using Unity.TimelineTools.Enumerations;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Unity.TimelineTools.API
{
    public static class SelectionUtils
    {
        public static IEnumerable<TrackAsset> GetSelectedTracks()
        {
            var selectionManager = TimelineBase.selectionManager;
            var selectedClips = selectionManager.GetMethod("SelectedTracks");

            return (IEnumerable<TrackAsset>)selectedClips.Invoke(selectionManager, null);
        }

        public static IEnumerable<TimelineClip> GetSelectedClips()
        {
            var selectionManager = TimelineBase.selectionManager;
            var selectedClips = selectionManager.GetMethod("SelectedClips");

            return (IEnumerable<TimelineClip>)selectedClips.Invoke(selectionManager, null);
        }

        public static void SelectClipsFromCursor(ClipSelection selection)
        {
            var director = TimelineEditor.inspectedDirector;
            var asset = TimelineEditor.inspectedAsset;
            double time = TimelineWindowUtils.GetCursor();

            var selectionManager = TimelineBase.selectionManager;
            var clear = selectionManager.GetMethod("Clear");
            var add = selectionManager.GetMethod("Add", new Type[] { typeof(TimelineClip) });

            clear.Invoke(null, null);

            foreach (var track in asset.GetRootTracks())
                if (selection == ClipSelection.Under)
                    foreach (var clip in ClipUtils.GetClipsAtTimeInTrack(track, time))
                        add.Invoke(null, new object[] { clip });
                else if (selection == ClipSelection.Before)
                    foreach (var clip in ClipUtils.GetClipsBeforeTimeInTrack(track, time))
                        add.Invoke(null, new object[] { clip });
                else if (selection == ClipSelection.After)
                    foreach (var clip in ClipUtils.GetClipsAfterTimeInTrack(track, time))
                        add.Invoke(null, new object[] { clip });

            TimelineWindowUtils.Repaint();
        }

        public static void SelectClipsInSpanInTrack(TrackAsset track, double start, double end)
        {
            var selectionManager = TimelineBase.selectionManager;
            var clear = selectionManager.GetMethod("Clear");
            var add = selectionManager.GetMethod("Add", new Type[] { typeof(TimelineClip) });

            clear.Invoke(null, null);

            foreach (var clip in ClipUtils.GetClipsInSpanInTrack(track, start, end))
                add.Invoke(null, new object[] { clip });

            TimelineWindowUtils.Repaint();
        }

    }
}