using UnityEditor;

using UnityEngine;

using System;

namespace Tilemap3DEditor
{
    /// <summary>
    /// A static class that implements a dirty hack that allows a user to 
    /// bind to the EditorApplication.globalEventHandler field that is normally not publically accessable.
    /// </summary>
    [InitializeOnLoad]
    public static class GlobalEventHandler
    {
        public static event Action<Event> onGlobalEvent;

        static GlobalEventHandler()
        {
            string msg = "";
            bool success = false;
            try
            {
                System.Reflection.FieldInfo fieldInfo = typeof(EditorApplication).GetField(
                    "globalEventHandler",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic
                );

                if (fieldInfo != null)
                {
                    EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)fieldInfo.GetValue(null);

                    value -= InvokeEventCallback;
                    value += InvokeEventCallback;

                    fieldInfo.SetValue(null, value);
                    success = true;
                }
                else
                {
                    msg = $"{typeof(EditorApplication)}.globalEventHandler not found.";
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            finally
            {
                if (!success)
                    Debug.LogWarning("[GlobalEventHandler] - Error, unable to register globalEventHandler: " + msg);
            }
        }

        private static void InvokeEventCallback()
        {
            onGlobalEvent?.Invoke(Event.current);
        }
    }
}
