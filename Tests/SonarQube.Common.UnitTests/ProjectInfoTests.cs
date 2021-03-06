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
 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TestUtilities;

namespace SonarQube.Common.UnitTests
{
    [TestClass]
    public class ProjectInfoTests
    {
        public TestContext TestContext { get; set; }

        #region Tests

        [TestMethod]
        public void ProjectInfo_Serialization_InvalidFileName()
        {
            // 0. Setup
            ProjectInfo pi = new ProjectInfo();

            // 1a. Missing file name - save
            AssertException.Expects<ArgumentNullException>(() => pi.Save(null));
            AssertException.Expects<ArgumentNullException>(() => pi.Save(string.Empty));
            AssertException.Expects<ArgumentNullException>(() => pi.Save("\r\t "));

            // 1b. Missing file name - load
            AssertException.Expects<ArgumentNullException>(() => ProjectInfo.Load(null));
            AssertException.Expects<ArgumentNullException>(() => ProjectInfo.Load(string.Empty));
            AssertException.Expects<ArgumentNullException>(() => ProjectInfo.Load("\r\t "));
        }

        [TestMethod]
        [Description("Checks ProjectInfo can be serialized and deserialized")]
        public void ProjectInfo_Serialization_SaveAndReload()
        {
            // 0. Setup
            string testFolder = TestUtils.CreateTestSpecificFolder(this.TestContext);

            Guid projectGuid = Guid.NewGuid();

            ProjectInfo originalProjectInfo = new ProjectInfo();
            originalProjectInfo.FullPath = "c:\\fullPath\\project.proj";
            originalProjectInfo.ProjectLanguage = "a language";
            originalProjectInfo.ProjectType = ProjectType.Product;
            originalProjectInfo.ProjectGuid = projectGuid;
            originalProjectInfo.ProjectName = "MyProject";
            originalProjectInfo.Encoding = "MyEncoding";

            string fileName = Path.Combine(testFolder, "ProjectInfo1.xml");

            SaveAndReloadProjectInfo(originalProjectInfo, fileName);
        }

        [TestMethod]
        [Description("Checks analysis results can be serialized and deserialized")]
        public void ProjectInfo_Serialization_AnalysisResults()
        {
            // 0. Setup
            string testFolder = TestUtils.CreateTestSpecificFolder(this.TestContext);

            Guid projectGuid = Guid.NewGuid();

            ProjectInfo originalProjectInfo = new ProjectInfo();
            originalProjectInfo.ProjectGuid = projectGuid;

            // 1. Null list
            SaveAndReloadProjectInfo(originalProjectInfo, Path.Combine(testFolder, "ProjectInfo_AnalysisResults1.xml"));

            // 2. Empty list
            originalProjectInfo.AnalysisResults = new List<AnalysisResult>();
            SaveAndReloadProjectInfo(originalProjectInfo, Path.Combine(testFolder, "ProjectInfo_AnalysisResults2.xml"));

            // 3. Non-empty list
            originalProjectInfo.AnalysisResults.Add(new AnalysisResult() { Id = string.Empty, Location = string.Empty }); // empty item
            originalProjectInfo.AnalysisResults.Add(new AnalysisResult() { Id = "Id1", Location = "location1" });
            originalProjectInfo.AnalysisResults.Add(new AnalysisResult() { Id = "Id2", Location = "location2" });
            SaveAndReloadProjectInfo(originalProjectInfo, Path.Combine(testFolder, "ProjectInfo_AnalysisResults3.xml"));
        }

        #endregion

        #region Helper methods

        private ProjectInfo SaveAndReloadProjectInfo(ProjectInfo original, string outputFileName)
        {
            Assert.IsFalse(File.Exists(outputFileName), "Test error: file should not exist at the start of the test. File: {0}", outputFileName);
            original.Save(outputFileName);
            Assert.IsTrue(File.Exists(outputFileName), "Failed to create the output file. File: {0}", outputFileName);
            this.TestContext.AddResultFile(outputFileName);

            ProjectInfo reloadedProjectInfo = ProjectInfo.Load(outputFileName);
            Assert.IsNotNull(reloadedProjectInfo, "Reloaded project info should not be null");

            ProjectInfoAssertions.AssertExpectedValues(original, reloadedProjectInfo);
            return reloadedProjectInfo;
        }

        #endregion
    }
}
