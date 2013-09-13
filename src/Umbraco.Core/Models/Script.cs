﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.IO;

namespace Umbraco.Core.Models
{
    /// <summary>
    /// Represents a Script file
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Script : File
    {
        private readonly IContentScriptEditor _scriptEditorConfig;

        public Script(string path)
            : this(path, UmbracoConfiguration.Current.UmbracoSettings.Content.ScriptEditor)
        {
            
        }

        public Script(string path, IContentScriptEditor scriptEditorConfig)
            : base(path)
        {
            _scriptEditorConfig = scriptEditorConfig;
            base.Path = path;
        }

        /// <summary>
        /// Boolean indicating whether the file could be validated
        /// </summary>
        /// <remarks>
        /// The validation logic was previsouly placed in the codebehind of editScript.aspx,
        /// but has been moved to the script file so the validation is central.
        /// </remarks>
        /// <returns>True if file is valid, otherwise false</returns>
        public override bool IsValid()
        {
            //NOTE Since a script file can be both JS, Razor Views, Razor Macros and Xslt
            //it might be an idea to create validations for all 3 and divide the validation 
            //into 4 private methods.
            //See codeEditorSave.asmx.cs for reference.

            var exts = _scriptEditorConfig.ScriptFileTypes.ToList();
            /*if (UmbracoSettings.DefaultRenderingEngine == RenderingEngine.Mvc)
            {
                exts.Add("cshtml");
                exts.Add("vbhtml");
            }*/

            var dirs = SystemDirectories.Scripts;
            /*if (UmbracoSettings.DefaultRenderingEngine == RenderingEngine.Mvc)
                dirs += "," + SystemDirectories.MvcViews;*/

            //Validate file
            var validFile = IOHelper.VerifyEditPath(Path, dirs.Split(','));

            //Validate extension
            var validExtension = IOHelper.VerifyFileExtension(Path, exts);

            return validFile && validExtension;
        }

        /// <summary>
        /// Indicates whether the current entity has an identity, which in this case is a path/name.
        /// </summary>
        /// <remarks>
        /// Overrides the default Entity identity check.
        /// </remarks>
        public override bool HasIdentity
        {
            get { return string.IsNullOrEmpty(Path) == false; }
        }
    }
}