﻿/*
 * Project: AVRDUDESS - A GUI for AVRDUDE
 * Author: Zak Kemble, contact@zakkemble.co.uk
 * Copyright: (C) 2013 by Zak Kemble
 * License: GNU GPL v3 (see License.txt)
 * Web: http://blog.zakkemble.co.uk/avrdudess-a-gui-for-avrdude/
 */

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace avrdudess
{
    public class UpdateCheck
    {
        private const string UPDATE_ADDR = "http://versions.zakkemble.co.uk/avrdudess.xml";

        private Form1 mainForm;
        private Config config;
        private long now;
        private Version newVersion;

        public UpdateCheck(Form1 mainForm, Config config)
        {
            this.mainForm = mainForm;
            this.config = config;

            now = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

            // Check once a day
            if (now - config.updateCheck > TimeSpan.FromDays(1).TotalSeconds)
                checkNow();
        }

        private void checkNow()
        {
            Thread t = new Thread(new ThreadStart(tUpdate));
            t.IsBackground = true;
            t.Start();
        }

        public void skipVersion()
        {
            if(newVersion != null)
                config.skipVersion = newVersion;
        }

        private void saveTime()
        {
            config.updateCheck = now;
        }

        private void tUpdate()
        {
            Thread.Sleep(500);

            try
            {
                Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                int major = 0;
                int minor = 0;
                int build = 0;
                int revision = 0;
                long date = 0;
                string updateAddr = "";
                string updateInfo = "";

                // Setup web request
                var request         = (HttpWebRequest)WebRequest.Create(UPDATE_ADDR);
                request.UserAgent = "Mozilla/5.0 (compatible; AVRDUDESS VERSION CHECKER " + currentVersion.ToString() + ")";
                request.ReadWriteTimeout = 30000;
                request.Timeout = 30000;
                request.KeepAlive = false;
                //request.Proxy = null;

                // Do request
                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    // XML
                    using (var reader = XmlReader.Create(responseStream))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                string name = reader.Name;
                                reader.Read();
                                switch (name)
                                {
                                    case "major":
                                        major = reader.ReadContentAsInt();
                                        break;
                                    case "minor":
                                        minor = reader.ReadContentAsInt();
                                        break;
                                    case "build":
                                        build = reader.ReadContentAsInt();
                                        break;
                                    case "revision":
                                        revision = reader.ReadContentAsInt();
                                        break;
                                    case "date":
                                        date = reader.ReadContentAsLong();
                                        break;
                                    case "updateAddr":
                                        updateAddr = reader.ReadContentAsString();
                                        break;
                                    case "updateInfo":
                                        updateInfo = reader.ReadContentAsString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

                newVersion = new Version(major, minor, build, revision);

                // Notify of new update
                if (config.skipVersion != newVersion && currentVersion.CompareTo(newVersion) < 0)
                {
                    string newVersionStr = newVersion.ToString() + " (" + new DateTime(1970, 1, 1).AddSeconds(date).ToLocalTime().ToShortDateString() + ")";

                    mainForm.BeginInvoke(new MethodInvoker(() =>
                    {
                        FormUpdate f = new FormUpdate();
                        f.doUpdateMsg(currentVersion.ToString(), newVersionStr, updateInfo, updateAddr, this);
                    }));
                }

                saveTime();
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
