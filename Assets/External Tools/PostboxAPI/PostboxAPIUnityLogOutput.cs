using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PostboxAPI
{
    /// <summary>
    /// A Helper class to output the PostboxLogbook Messages in the Unity Console and/or an UI.Text-Element.
    /// </summary>
    public class PostboxAPIUnityLogOutput : MonoBehaviour, IPostboxAPILogOutput
    {
        /// <summary>
        /// Textfield where the LogMessages displayed. <see cref="Text"/>
        /// </summary>
        public Text Textbox;

        /// <summary>
        /// Show in Unity Console on/off
        /// </summary>
        public bool outputInDebugLog = true;

        /// <summary>
        /// Show in Textbox on/off
        /// </summary>
        public bool outputInTextfield = false;

        [Header("Show in Debug")]
        /// <summary>
        /// Show Calls in Unity Console on/off
        /// </summary>
        public bool ShowAPICallsInDebug = true;

        /// <summary>
        /// Show Notification in Unity Console on/off
        /// </summary>
        public bool ShowNotificationInDebug = true;

        /// <summary>
        /// Show Warning in Unity Console on/off
        /// </summary>
        public bool ShowWarningInDebug = true;

        /// <summary>
        /// Show Error in Unity Console on/off
        /// </summary>
        public bool ShowErrorInDebug = true;

        [Header("Show in Textbox")]
        /// <summary>
        /// Show Calls in Textbox on/off
        /// </summary>
        public bool ShowAPICallsInTextbox = true;

        /// <summary>
        /// Show Notification in Textbox on/off
        /// </summary>
        public bool ShowNotificationInTextbox = true;

        /// <summary>
        /// Show Warning in Textbox on/off
        /// </summary>
        public bool ShowWarningInTextbox = true;

        /// <summary>
        /// Show Error in Textbox on/off
        /// </summary>
        public bool ShowErrorInTextbox = true;


        /// <summary>
        /// The count of characters that will be displayed
        /// </summary>
        public int TextClamp = 500;

        /// <summary>
        /// The current message <see cref="PostboxLogMessage"/>.
        /// </summary>
        private PostboxLogMessage output = null;

        /// <summary>
        /// Initialize and search for TextComponent
        /// </summary>
        private void Awake()
        {
            if (Textbox == null)
            {
                Textbox = GetComponent<Text>();
            }
        }

        /// <summary>
        /// Loop to refresh message list
        /// </summary>
        private void FixedUpdate()
        {
            PrintNextMessage();
        }

        /// <summary>
        /// Show the next bufferd Message of the PostboxLogbook
        /// </summary>
        public void PrintNextMessage()
        {
            output = PostboxLogbook.Instance.GetMessage();
            if (output == null)
                return;

            // Print in Textbox
            if (outputInTextfield)
            {
                PrintInTextbox(output, TextClamp);
            }

            // Print in Debug-Log
            if (outputInDebugLog)
            {
                PrintInDebuglog(output);
            }
        }

        /// <summary>
        /// Show LogMessages in Editor Console Window.
        /// The Level NotificationType of the Message Objekt effects on the type of output.
        /// (notification, warning, error)
        /// </summary>
        /// <param name="output">PostboxLogMessage Object</param>
        private void PrintInDebuglog(PostboxLogMessage output)
        {
            switch (output.NotificationLevel)
            {
                case PostboxLogbook.NotificationType.Notification:
                    if (ShowNotificationInDebug)
                        Debug.Log(output.Print());
                    break;
                case PostboxLogbook.NotificationType.Warning:
                    if (ShowWarningInDebug)
                        Debug.LogWarning(output.Print());
                    break;
                case PostboxLogbook.NotificationType.Error:
                    if (ShowErrorInDebug)
                        Debug.LogError(output.Print());
                    break;
                case PostboxLogbook.NotificationType.APICalls:
                    if (ShowAPICallsInDebug)
                        Debug.Log(output.Print());
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Add current Message to Textbox-Output.
        /// </summary>
        /// <param name="output">LogMessage from Logbook</param>
        /// <param name="clamp">Character count for max dawed text.</param>
        private void PrintInTextbox(PostboxLogMessage output, int clamp)
        {
            if (Textbox)
            {
                // Prepair Textbox
                string TextOutputString = output != null ? Textbox.text + output.Print() + "\r\n" : Textbox.text;
                TextOutputString = TextOutputString.Substring((TextOutputString.Length - clamp < 0 ? 0 : TextOutputString.Length - clamp));

                // Show Text in Box
                switch (output.NotificationLevel)
                {
                    case PostboxLogbook.NotificationType.Notification:
                        if (ShowNotificationInTextbox && outputInTextfield)
                            Textbox.text = TextOutputString;
                        break;
                    case PostboxLogbook.NotificationType.Warning:
                        if (ShowWarningInTextbox && outputInTextfield)
                            Textbox.text = TextOutputString;
                        break;
                    case PostboxLogbook.NotificationType.Error:
                        if (ShowErrorInTextbox && outputInTextfield)
                            Textbox.text = TextOutputString;
                        break;
                    case PostboxLogbook.NotificationType.APICalls:
                        if (ShowAPICallsInTextbox && outputInTextfield)
                            Textbox.text = TextOutputString;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogWarning("You have to assign a Textbox to use Output in Textfield.");
            }
        }
    }
}