

namespace Unity.TimelineTools.Enumerations
{

    public enum ClipSelection
    {
        None,
        Under,
        Before,
        After
    }

    public enum ClipModificationTarget
    {
        SelectedClipsOnly,
        InspectedTimelineOnly,
        FullTimelineHierarchy
    }

    public enum TRACK_TYPE
    {
        TRACK_UNKNOWN,          // unknown or custom track type
        TRACK_GROUP,
        TRACK_ANIMATION,
        TRACK_AUDIO,
        TRACK_CONTROL,
        TRACK_CINEMACHINE,
        TRACK_ACTIVATION,
    }
}