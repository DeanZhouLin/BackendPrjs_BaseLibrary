/*****************************************************************
 * Copyright (C) DollMall Corporation. All rights reserved.
 * 
 * Author:   Dolphin Zhang (dolphin@dollmalll.com)
 * Create Date:  11/21/2006 19:11:11
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Com.BaseLibrary.Configuration;
using Com.BaseLibrary.Utility;

namespace Com.BaseLibrary.XmlAccess.Configuration
{


    [XmlRoot("xmlAccess")]
    public class XmlAccessConfiguration
    {
        private const string SECTION_XMLACCESS_CONFIG = "XMLAccessConfigFile";

        private string m_DefaultXmlDataFolder;
        private AlternateXmlDataFolderCollection m_AlternateXmlDataFolders;

        [XmlElement("defaultXmlDataFolder")]
        public string DefaultXmlDataFolder
        {
            get { return m_DefaultXmlDataFolder; }
            set { m_DefaultXmlDataFolder = value; }
        }

        [XmlElement("alternateXmlDataFolders")]
        public AlternateXmlDataFolderCollection AlternateXmlDataFolders
        {
            get { return m_AlternateXmlDataFolders; }
            set { m_AlternateXmlDataFolders = value; }
        }

        public static XmlAccessConfiguration Current
        {
            get
            {
                return ConfigurationManager.LoadConfiguration<XmlAccessConfiguration>(ConfigurationHelper.GetConfigurationFile(SECTION_XMLACCESS_CONFIG));
            }
        }
    }

    public class AlternateXmlDataFolderCollection
    {
        private List<string> m_Folders;

        [XmlElement("folder")]
        public List<string> Folders
        {
            get { return m_Folders; }
            set { m_Folders = value; }
        }
    }
}