using System.Collections.Generic;
using System.Reflection;
using Unity.TimelineTools.Enumerations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Unity.TimelineTools.API
{
    public class TrackUtils
    {
        public static TrackAsset GetTrackBasedOnClip(TimelineClip clip)
        {
            // FIXME: get rid of Reflection
            var trackProp = clip.GetType().GetProperty("parentTrack", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return trackProp.GetValue(clip, null) as TrackAsset;
        }


        public static void Reorder_B_below_A(List<TrackAsset> tracks, TrackAsset a, TrackAsset b)
        {
            var a_idx = tracks.IndexOf(a);
            var b_idx = tracks.IndexOf(b);

            var head_idx = a_idx + 1;
            TrackAsset tmp = b;

            for (int i = head_idx; i < b_idx; i++)
            {
                var current = tracks[i];
                tracks[i] = tmp;
                tmp = current;
            }

            tracks[b_idx] = tmp;
        }


        /// <summary>
        /// pass in a track and we'll try and figure out what type it is
        /// currently only supports TRACK_TYPE tracks (ie built in track types)
        /// </summary>
        public static TRACK_TYPE GetTrackType(TrackAsset track)
        {
            if (track as ControlTrack != null)
            {
                return TRACK_TYPE.TRACK_CONTROL;
            }
            else if (track as AnimationTrack != null)
            {
                return TRACK_TYPE.TRACK_ANIMATION;
            }
#if USING_CINEMACHINE
            else if( track as Cinemachine.Timeline.CinemachineTrack != null)
            {
                return TRACK_TYPE.TRACK_CINEMACHINE;
            }
#endif
            else if (track as AudioTrack != null)
            {
                return TRACK_TYPE.TRACK_AUDIO;
            }
            else if (track as GroupTrack != null)
            {
                return TRACK_TYPE.TRACK_GROUP;
            }
            else if (track as ActivationTrack != null)
            {
                return TRACK_TYPE.TRACK_ACTIVATION;
            }
            // TODO
            return TRACK_TYPE.TRACK_UNKNOWN;
        }

        public static TrackAsset CreateTrack(TimelineAsset timelineAsset, TRACK_TYPE type, string name, TrackAsset parent)
        {
            switch (type)
            {
                case TRACK_TYPE.TRACK_GROUP:
                    {
                        return timelineAsset.CreateTrack<GroupTrack>(parent, name);
                    }
                case TRACK_TYPE.TRACK_ANIMATION:
                    {
                        return timelineAsset.CreateTrack<AnimationTrack>(parent, name);
                    }
                case TRACK_TYPE.TRACK_CONTROL:
                    {
                        return timelineAsset.CreateTrack<ControlTrack>(parent, name);
                    }
                case TRACK_TYPE.TRACK_CINEMACHINE:
                    {
#if USING_CINEMACHINE
                        return timelineAsset.CreateTrack<Cinemachine.Timeline.CinemachineTrack>(parent, name);
#else
                        break;
#endif
                    }
                case TRACK_TYPE.TRACK_ACTIVATION:
                    {
                        return timelineAsset.CreateTrack<ActivationTrack>(parent, name);
                    }
            }
            return null;
        }

        public static TrackAsset GetTrack(System.Object obj)
        {
            var clip = ClipUtils.GetClip(obj);
            return clip.parentTrack;
        }

        public static List<TrackAsset> GetTracks(TimelineAsset timeline)
        {
            // FIXME: get rid of Reflection
            // locate track list in timeline
            var propTracks = timeline.GetType().GetProperty("tracks", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return propTracks.GetValue(timeline, null) as List<TrackAsset>;
        }

        public static void ControlTrackSetGameObject(PlayableDirector director, ControlPlayableAsset clip, GameObject obj)
        {
            director.playableGraph.GetResolver().SetReferenceValue(clip.sourceGameObject.exposedName, obj);
        }

        public static UnityEngine.Object GetSourceObjectFromTrack(TrackAsset track, PlayableDirector pd)
        {
            return pd.GetGenericBinding(track);
        }

        public static TimelineClip CreateAnimClipInTrack(string trackName, TimelineAsset timeline)
        {
            var track = timeline.CreateTrack<AnimationTrack>(null, trackName);
            var clip = track.CreateDefaultClip();

            return clip;
        }

        public static TimelineClip CreateControlClipInTrack(string trackName, TimelineAsset timeline)
        {
            var track = timeline.CreateTrack<ControlTrack>(null, trackName);
            var clip = track.CreateDefaultClip();

            return clip;
        }

        /// <summary>
        ///  From a given timeline asset, see if we can find a track with the matching name
        /// </summary>
        public static TrackAsset FindTrackByName(TimelineAsset timelineAsset, string name)
        {
            var trackList = timelineAsset.GetRootTracks();
            var thisTrack = FindTrackByNameInternal(trackList, name);
            if (thisTrack == null)
            {
                UnityEngine.Debug.Log("Could not find track in Timeline with name: " + name);
            }
            return thisTrack;
        }

        /// <summary>
        /// Recursively try and find a track with the desired name
        /// </summary>
        private static TrackAsset FindTrackByNameInternal(IEnumerable<TrackAsset> trackList, string name)
        {
            foreach (var track in trackList)
            {
                if (track.name == name)
                {
                    return track;
                }
                else
                {
                    var childTracks = track.GetChildTracks();
                    return FindTrackByNameInternal(childTracks, name);
                }
            }
            return null;
        }

    }
}