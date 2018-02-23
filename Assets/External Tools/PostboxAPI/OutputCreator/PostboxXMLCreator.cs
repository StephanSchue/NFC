using System.Text;
using System.Xml;

namespace PostboxAPI
{
    /// <summary>
    /// This class en- and decode XML for API and UserSettings
    /// </summary>
    public class PostboxXMLCreator : IPostboxOutputCreator
    {
        private PostboxRequest request;

        /// <summary>
        /// Empty Constructor for normal usage
        /// </summary>
        public PostboxXMLCreator()
        {

        }

        /// <summary>
        /// Constructor prepair the an RequestObject for usage
        /// </summary>
        /// <param name="appId">AppID of the connector class</param>
        /// <param name="deviceId">DeviceID of the connector class</param>
        /// <param name="liceneKey">LiceneKey of the connector class</param>
        /// <param name="apiVersion">API-Version of the connector class</param>
        /// <param name="userName">UserName of the connector class</param>
        /// <param name="userPassword">UserPassword of the connector class</param>
        public PostboxXMLCreator(string appId, string deviceId,
                                 string liceneKey = "", string apiVersion = "1.0",
                                 string userName = "", string userPassword = "")
        {
            request = new PostboxRequest(appId, deviceId, liceneKey, apiVersion, userName, userPassword);
        }

        /// <summary>
        /// Prepair an PostboxRequest for sending via the connector
        /// </summary>
        /// <param name="callName">Callname for API Service</param>
        /// <param name="parameters">Additional Parameters</param>
        /// <returns></returns>
        /// <returns>request as xml-string</returns>
        public string CreateRequest(string callName, params PostboxCallParameter[] parameters)
        {
            XmlDocument doc = new XmlDocument();

            // Create an XML declaration. 
            XmlDeclaration xmldecl;
            xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";

            // Add the new node to the document.
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmldecl, root);

            // --- Create Request ---
            XmlNode requestNode = doc.CreateElement("Request");

            // -- Global-Header --
            XmlNode globalNode = doc.CreateElement("PostBoxGlobal");

            // - Version -
            XmlNode appIdNode = doc.CreateElement("AppID");
            appIdNode.InnerText = request.AppID;
            globalNode.AppendChild(appIdNode);

            // - Version -
            XmlNode versionNode = doc.CreateElement("Version");
            versionNode.InnerText = request.Version;
            globalNode.AppendChild(versionNode);

            // - UserName -
            if (request.UserName != "")
            {
                XmlNode userNameNode = doc.CreateElement("UserName");
                userNameNode.InnerText = request.UserName;
                globalNode.AppendChild(userNameNode);
            }

            // - Password -
            if (request.UserName != "")
            {
                XmlNode userPasswordNode = doc.CreateElement("UserPassword");
                userPasswordNode.InnerText = request.UserPassword;
                globalNode.AppendChild(userPasswordNode);
            }

            // - LicenseKey -
            if (request.UserName != "")
            {
                XmlNode licenseKeyNode = doc.CreateElement("LicenseKey");
                licenseKeyNode.InnerText = request.LicenseKey;
                globalNode.AppendChild(licenseKeyNode);
            }

            // - Device ID -
            if (request.DeviceID != "")
            {
                XmlNode computerIDNode = doc.CreateElement("DeviceID");
                computerIDNode.InnerText = request.DeviceID;
                globalNode.AppendChild(computerIDNode);
            }

            // - CallName -
            XmlNode callNameNode = doc.CreateElement("CallName");
            callNameNode.InnerText = callName;
            globalNode.AppendChild(callNameNode);

            requestNode.AppendChild(globalNode);
            doc.AppendChild(requestNode);

            // -- Params --
            request.ApplyParameters(parameters);
            XmlNode paramsNode = doc.CreateElement("Param");
            XmlDocument xmlData = null;
            foreach (PostboxCallParameter parameter in request.Parameters)
            {
                if (parameter != null)
                {
                    xmlData = Decode(parameter.Value);
                    if (xmlData != null)
                    {
                        XmlNode paramNode = doc.CreateElement(parameter.Key);
                        paramNode.InnerXml = xmlData.InnerXml;
                        paramsNode.AppendChild(paramNode);
                    }
                    else
                    {
                        XmlNode paramNode = doc.CreateElement(parameter.Key);
                        paramNode.InnerText = parameter.Value;
                        paramsNode.AppendChild(paramNode);
                    }
                }
            }

            requestNode.AppendChild(paramsNode);

            // -- Convert Request to XML --
            return doc.OuterXml;
        }

        /// <summary>
        /// Decode an API Response in an XML-Object for better working
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public XmlDocument Decode(string responseString)
        {
            XmlDocument doc = null;

            if (!System.String.IsNullOrEmpty(responseString))
            {
                doc = new XmlDocument();
                try
                {
                    doc.LoadXml(StripSlashes(responseString));
                }
                catch (XmlException ex)
                {
                    doc = null;
                    //PostboxLogbook.Instance.Log("An error occurred while parsing XML -" + ex.Message, PostboxLogbook.NotificationType.Error);
                }
            }

            return doc;
        }

        /// <summary>
        /// Return the given string with indent and line breaks
        /// </summary>
        /// <param name="xmlString">String in xml format</param>
        /// <returns>Formated string</returns>
        public string FormatString(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            string output = null;

            try
            {
                doc.LoadXml(xmlString);

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }

                output = sb.ToString();
            }
            catch (XmlException ex)
            {
                output = null;
                //PostboxLogbook.Instance.Log("An error occurred while parsing XML -" + ex.Message, PostboxLogbook.NotificationType.Error);
            }

            return output;
        }

        /// <summary>
        /// Un-quotes a quoted string
        /// </summary>
        /// <param name="InputTxt">Text string need to be escape with slashes</param>
        public string StripSlashes(string InputTxt)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            string Result = InputTxt;

            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(InputTxt, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2");
                Result = Result.Replace("\\/", "/");
            }
            catch (System.Exception Ex)
            {
                // handle any exception here
            }

            return Result;
        }
    }
}