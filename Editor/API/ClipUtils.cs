using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;            // FIXME: Remove all reflection, isolate into TimelineBase.cs
using UnityEditor;
using UnityEngine.Timeline;

namespace Unity.TimelineTools.API
{
    public static class ClipUtils
    {
        public static TimelineClip GetClipForAsset(TimelineAsset timelineAsset, UnityEngine.Object playableAsset)
        {
            return timelineAsset.GetRootTracks()
                                .SelectMany(track => GetClipsInTrack(track, clip => clip.asset == playableAsset))
                                .FirstOrDefault();
        }

        public static TimelineClip[] GetClipsInTrack(TrackAsset track, System.Func<TimelineClip, bool> compare)
        {
            // MikeW : This function is both amazing and terrifying
            //          surely there has to be an easier/better way to do this
            System.Func<TrackAsset, System.Func<TimelineClip, bool>, TimelineClip[]> getClips = null;
            getClips = (track_, compare_) => 
                track_.GetChildTracks()
                    .SelectMany(child => getClips(child, compare_))
                    .Concat(track_.GetClips().Where(compare_))
                    .OrderBy(clip => clip.start)
                    .ToArray();
            return getClips(track, compare);
        }

        public static TimelineClip[] GetAllClipsInTrack(TrackAsset track)
        {
            return GetClipsInTrack(track, clip => true);
        }

        public static TimelineClip[] GetAllClipsInTrack(TrackAsset track, double time)
        {
            return GetClipsInTrack(track, clip => true);
        }

        public static TimelineClip[] GetClipsAtTimeInTrack(TrackAsset track, double time)
        {
            return GetClipsInTrack(track, clip => clip.start <= time && clip.end > time);
        }

        public static TimelineClip[] GetClipsUpToTimeInTrack(TrackAsset track, double time)
        {
            return GetClipsInTrack(track, clip => clip.start <= time);
        }

        public static TimelineClip[] GetClipsFromTimeInTrack(TrackAsset track, double time)
        {
            return GetClipsInTrack(track, clip => clip.end > time);
        }

        public static TimelineClip[] GetClipsBeforeTimeInTrack(TrackAsset track, double time)
        {
            return GetClipsInTrack(track, clip => clip.end < time);
        }

        public static TimelineClip[] GetClipsAfterTimeInTrack(TrackAsset track, double time)
        {
            return GetClipsInTrack(track, clip => clip.start > time);
        }

        public static TimelineClip[] GetClipsInSpanInTrack(TrackAsset track, double start, double end)
        {
            return GetClipsInTrack(track, clip => clip.start >= start && clip.end <= end);
        }

        public static TimelineClip CreateClipOnTrack(Type type, TrackAsset track, double time)
        {
            var timelineHelpers = TimelineBase.timelineHelpers;
            var createClipOnTrack = timelineHelpers.GetMethod("CreateClipOnTrack", new Type[] { typeof(Type), typeof(TrackAsset), typeof(double) });
            return (TimelineClip)createClipOnTrack.Invoke(null, new object[] { type, track, time });
        }

        public static void SetClipExtrapolationMode(TimelineClip clip, string propertyName, TimelineClip.ClipExtrapolation mode)
        {
            // FIXME: get rid of reflection
            var pro = clip.GetType().GetProperty(propertyName);
            pro.SetValue(clip, Convert.ChangeType(mode, pro.PropertyType), null);
        }

        public static TimelineClip GetClip(System.Object obj)
        {
            // FIXME: Get rid of Reflection
            var clipProp = obj.GetType().GetProperty("clip", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return clipProp.GetValue(obj, null) as TimelineClip;
        }

        public static void AlignClipsToHead(UnityEngine.Object[] objects)
        {
            Undo.RecordObjects(objects, "Align Clips to Head");

            List<double> startTimeList = new List<double>();
            List<TimelineClip> clipList = new List<TimelineClip>();

            foreach (var obj in objects)
            {
                var selectedClip = GetClip(obj);
                clipList.Add(selectedClip);
                startTimeList.Add(selectedClip.start);
            }

            double lowest = startTimeList.Min();

            foreach (var clip in clipList)
            {
                clip.start = lowest;
            }
            TimelineWindowUtils.GetWindow().Repaint();
        }

        public static void AlignClipsToTail(UnityEngine.Object[] objects)
        {
            Undo.RecordObjects(objects, "Align Clips to Tail");

            List<double> startTimeList = new List<double>();
            List<TimelineClip> clipList = new List<TimelineClip>();

            foreach (var obj in objects)
            {
                var selectedClip = GetClip(obj);
                clipList.Add(selectedClip);
                startTimeList.Add(selectedClip.end);
            }

            double highest = startTimeList.Max();

            foreach (var clip in clipList)
            {
                clip.start = highest - clip.duration;
            }
            TimelineWindowUtils.GetWindow().Repaint();
        }

        public static void SnapToPrevious(UnityEngine.Object[] objects)
        {
            Undo.RecordObjects(objects, "Snap To Previous's End");
            List<TimelineClip> selectedClips = new List<TimelineClip>();
            for (int i = 0; i < objects.Length; i++)
            {
                var selectedClip = GetClip(objects[i]);
                selectedClips.Add(selectedClip);
            }

            selectedClips = selectedClips.OrderBy(c => c.start).ToList();

            for (int i = 0; i < selectedClips.Count; i++)
            {
                var selectedClip = selectedClips[i];
                var selectedTrack = TrackUtils.GetTrackBasedOnClip(selectedClip);
                var clipsInTrack = (Array)selectedTrack.GetClips();
                var index = Array.IndexOf(clipsInTrack, selectedClip);

                // if found
                if (index > -1)
                {
                    // and not the first clip in track
                    if (index != 0)
                    {
                        var prevClip = clipsInTrack.GetValue(index - 1) as TimelineClip;
                        selectedClip.start = prevClip.end;
                    }
                }
                else { UnityEngine.Debug.LogWarning("Clip not found"); }

            }
            TimelineWindowUtils.GetWindow().Repaint();
        }

        public static void SnapToNext(UnityEngine.Object[] objects)
        {
            Undo.RecordObjects(objects, "Snap To Next's Start");
            List<TimelineClip> selectedClips = new List<TimelineClip>();
            for (int i = 0; i < objects.Length; i++)
            {
                var selectedClip = GetClip(objects[i]);
                selectedClips.Add(selectedClip);
            }

            selectedClips = selectedClips.OrderBy(c => c.start).ToList();
            selectedClips.Reverse();

            for (int i = 0; i < selectedClips.Count; i++)
            {
                var selectedClip = selectedClips[i];
                var selectedTrack = TrackUtils.GetTrackBasedOnClip(selectedClip);
                var clipsInTrack = (Array)selectedTrack.GetClips();
                var index = Array.IndexOf(clipsInTrack, selectedClip);

                // if found
                if (index > -1)
                {
                    // and not the last clip in track
                    if (index != clipsInTrack.Length - 1)
                    {
                        var nextClip = clipsInTrack.GetValue(index + 1) as TimelineClip;
                        selectedClip.start = nextClip.start - selectedClip.duration;
                    }
                }
                else { UnityEngine.Debug.LogWarning("Clip not found"); }

            }
            TimelineWindowUtils.GetWindow().Repaint();
        }

    }

}