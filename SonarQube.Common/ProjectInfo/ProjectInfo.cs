﻿/*
 * SonarQube Scanner for MSBuild
 * Copyright (C) 2016-2017 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
 
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SonarQube.Common
{
    /// <summary>
    /// Data class to describe a single project
    /// </summary>
    /// <remarks>The class is XML-serializable</remarks>
    [XmlRoot(Namespace = XmlNamespace)]
    public class ProjectInfo
    {
        public const string XmlNamespace = "http://www.sonarsource.com/msbuild/integration/2015/1";

        #region Public properties

        /// <summary>
        /// The project file name
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The project language
        /// </summary>
        public string ProjectLanguage { get; set; }

        /// <summary>
        /// The kind of the project
        /// </summary>
        public ProjectType ProjectType { get; set; }

        /// <summary>
        /// Unique identifier for the project
        /// </summary>
        public Guid ProjectGuid { get; set; }

        /// <summary>
        /// The full name and path of the project file
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Flag indicating whether the project should be excluded from processing
        /// </summary>
        public bool IsExcluded { get; set; }

        /// <summary>
        /// Encoding used for source files if no BOM is present
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// List of analysis results for the project
        /// </summary>
        public List<AnalysisResult> AnalysisResults { get; set; }


        /// <summary>
        /// List of additional analysis settings
        /// </summary>
        public AnalysisProperties AnalysisSettings { get; set; }

        #endregion

        #region Serialization

        /// <summary>
        /// Saves the project to the specified file as XML
        /// </summary>
        public void Save(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            Serializer.SaveModel(this, fileName);
        }


        /// <summary>
        /// Loads and returns project info from the specified XML file
        /// </summary>
        public static ProjectInfo Load(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            ProjectInfo model = Serializer.LoadModel<ProjectInfo>(fileName);
            return model;
        }

        #endregion

    }
}
