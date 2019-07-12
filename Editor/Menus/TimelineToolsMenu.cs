using Unity.TimelineTools.API;
using Unity.TimelineTools.Windows;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Unity.TimelineTools.Menu
{
    public class TimelineEditorWindowsMenu
    {
        [MenuItem("Tools/Timeline/Animation Import Manager")]
        static void AnimationImportWindowMenu()
        {
            AnimationImportWindow.ShowWindow();
        }

        [MenuItem("Tools/Timeline/Replace Animation Clip")]
        static void ReplaceAnimClipWindowMenu()
        {
            //ReplaceAnimClipEditor.ShowWindow();
        }
    }

    public class TimelineWindowMenu
    {
        [Shortcut("Timeline/Lock Window", KeyCode.T, ShortcutModifiers.Action)]
        [MenuItem("Tools/Timeline/Toggle Lock Timeline Window %T", false, 2)]
        static void ToggleLockTimelineWin()
        {
            TimelineWindowUtils.ToggleLock();
        }
    }

    public class TimelineAlignmentToolsMenu
    {
        [Shortcut("Timeline/Align Selected Clips to Head", KeyCode.UpArrow, ShortcutModifiers.Alt)]
        [MenuItem("Edit/Align 'selected Clips' to Head")]
        static void AlignToHead()
        {
            ClipUtils.AlignClipsToHead(Selection.objects);
        }

        [Shortcut("Timeline/Align Selected Clips to Tail", KeyCode.DownArrow, ShortcutModifiers.Alt)]
        [MenuItem("Edit/Align 'selected Clips' to Tail")]
        static void AlignToTail()
        {
            ClipUtils.AlignClipsToTail(Selection.objects);
        }

        [Shortcut("Timeline/Snap to Previous Clip", KeyCode.LeftArrow, ShortcutModifiers.Alt)]
        [MenuItem("Edit/Snap to Previous Clip")]
        static void SnapToPrevious()
        {
            ClipUtils.SnapToPrevious(Selection.objects);
        }

        [Shortcut("Timeline/Snap to Next Clip", KeyCode.RightArrow, ShortcutModifiers.Alt)]
        [MenuItem("Edit/Snap to Next Clip")]
        static void SnapToNext()
        {
            ClipUtils.SnapToNext(Selection.objects);
        }
    }
}