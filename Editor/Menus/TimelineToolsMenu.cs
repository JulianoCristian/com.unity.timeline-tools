using Unity.TimelineTools.API;
//using Unity.TimelineTools.Windows;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Unity.TimelineTools.Menu
{
    public class TimelineEditorWindowsMenu
    {
        [MenuItem("Window/Sequencing/Animation Import Manager")]
        static void AnimationImportWindowMenu()
        {
            //AnimationImportWindow.ShowWindow();
        }

        [MenuItem("Window/Sequencing/Replace Animation Clip")]
        static void ReplaceAnimClipWindowMenu()
        {
            //ReplaceAnimClipEditor.ShowWindow();
        }
    }

    public class TimelineWindowMenu
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Timeline/Lock Window", KeyCode.T, ShortcutModifiers.Action)]
#endif
        [MenuItem("Tools/Timeline/Toggle Lock Timeline Window %T", false, 2)]
        static void ToggleLockTimelineWin()
        {
            TimelineWindowUtils.ToggleLock();
        }
    }

    public class TimelineAlignmentToolsMenu
    {
        private const int defaultMenuPriority = 10;
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Timeline/Align Selected Clips to Head", KeyCode.UpArrow, ShortcutModifiers.Alt)]
#endif
        [MenuItem("Edit/Align/Align 'selected Clips' to Head", false, defaultMenuPriority)]
        static void AlignToHead()
        {
            ClipUtils.AlignClipsToHead(Selection.objects);
        }
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Timeline/Align Selected Clips to Tail", KeyCode.DownArrow, ShortcutModifiers.Alt)]
#endif
        [MenuItem("Edit/Align/Align 'selected Clips' to Tail", false, defaultMenuPriority)]
        static void AlignToTail()
        {
            ClipUtils.AlignClipsToTail(Selection.objects);
        }

#if UNITY_2019_1_OR_NEWER
        [Shortcut("Timeline/Snap to Previous Clip", KeyCode.LeftArrow, ShortcutModifiers.Alt)]
#endif
        [MenuItem("Edit/Snap/Snap to Previous Clip", false, defaultMenuPriority)]
        static void SnapToPrevious()
        {
            ClipUtils.SnapToPrevious(Selection.objects);
        }

#if UNITY_2019_1_OR_NEWER
        [Shortcut("Timeline/Snap to Next Clip", KeyCode.RightArrow, ShortcutModifiers.Alt)]
#endif
        [MenuItem("Edit/Snap/Snap to Next Clip", false, defaultMenuPriority)]
        static void SnapToNext()
        {
            ClipUtils.SnapToNext(Selection.objects);
        }
    }
}