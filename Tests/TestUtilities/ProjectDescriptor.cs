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
 
using SonarQube.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestUtilities
{
    /// <summary>
    /// Describes the expected contents of a single project
    /// </summary>
    /// <remarks>Used to dynamically create folders and files for tests,
    /// and to check actual results against</remarks>
    public class ProjectDescriptor
    {
        private const string CompilerInputItemGroup = "Compile";
        private const string ContentItemGroup = "Content";

        /// <summary>
        /// Data class to describe a single file in a project
        /// </summary>
        public class FileInProject
        {
            public FileInProject(string itemGroup, string filePath, bool shouldAnalyse)
            {
                this.ItemGroup = itemGroup;
                this.FilePath = filePath;
                this.ShouldBeAnalysed = shouldAnalyse;
            }

            /// <summary>
            /// The path to the file
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// The ItemGroup to which the file belongs
            /// </summary>
            public string ItemGroup { get; set; }

            /// <summary>
            /// Whether the file should be include in analysis or not
            /// </summary>
            public bool ShouldBeAnalysed { get; set; }
        }

        public IList<FileInProject> Files { get; set; }

        public ProjectDescriptor()
        {
            this.AnalysisResults = new List<AnalysisResult>();
            this.Files = new List<FileInProject>();

            // set default encoding
            this.Encoding = Encoding.UTF8;
        }

        #region Public properties

        public string ProjectLanguage { get; set; }

        public Guid ProjectGuid { get; set; }

        public IList<string> ManagedSourceFiles
        {
            get
            {
                return this.Files.Where(f => f.ItemGroup == CompilerInputItemGroup).Select(f => f.FilePath).ToList();
            }
        }

        public bool IsTestProject { get; set; }

        public bool IsExcluded { get; set; }

        public Encoding Encoding { get; set; }

        public List<AnalysisResult> AnalysisResults { get; private set; }

        /// <summary>
        /// The full path to the parent directory
        /// </summary>
        public string ParentDirectoryPath { get; set; }

        /// <summary>
        /// The name of the folder in which the project exists
        /// </summary>
        public string ProjectFolderName { get; set; }

        /// <summary>
        /// The name of the project file
        /// </summary>
        public string ProjectFileName { get; set; }

        public string FullDirectoryPath
        {
            get { return Path.Combine(this.ParentDirectoryPath, this.ProjectFolderName); }
        }

        public string FullFilePath
        {
            get { return Path.Combine(this.FullDirectoryPath, this.ProjectFileName); }
        }

        /// <summary>
        /// The user-friendly name for the project
        /// </summary>
        public string ProjectName
        {

            get
            {
                return Path.GetFileNameWithoutExtension(this.ProjectFileName);
            }
        }

        public IEnumerable<string> FilesToAnalyse
        {
            get
            {
                return this.Files.Where(f => f.ShouldBeAnalysed).Select(f => f.FilePath);
            }
        }

        /// <summary>
        /// List of files that should not be analysed
        /// </summary>
        public IEnumerable<string> FilesNotToAnalyse
        {
            get
            {
                return this.Files.Where(f => !f.ShouldBeAnalysed).Select(f => f.FilePath);
            }
        }

        public bool IsVbProject
        {
            get
            {
                return ProjectLanguages.IsVbProject(this.ProjectLanguage);
            }
        }

        #endregion

        #region Public methods

        public ProjectInfo CreateProjectInfo()
        {
            ProjectInfo info = new ProjectInfo()
            {
                FullPath = this.FullFilePath,
                ProjectLanguage = this.ProjectLanguage,
                ProjectGuid = this.ProjectGuid,
                ProjectName = this.ProjectName,
                ProjectType = this.IsTestProject ? ProjectType.Test : ProjectType.Product,
                IsExcluded = this.IsExcluded,
                Encoding = this.Encoding.WebName,
                AnalysisResults = new List<AnalysisResult>(this.AnalysisResults)
            };

            return info;
        }

        public void AddContentFile(string filePath, bool shouldAnalyse)
        {
            this.Files.Add(new FileInProject(ProjectDescriptor.ContentItemGroup, filePath, shouldAnalyse));
        }

        public void AddCompileInputFile(string filePath, bool shouldAnalyse)
        {
            this.Files.Add(new FileInProject(ProjectDescriptor.CompilerInputItemGroup, filePath, shouldAnalyse));
        }

        #endregion

    }
}
