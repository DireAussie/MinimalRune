﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using MinimalRune.Editor.Documents;
using MinimalRune.Windows.Themes;


namespace MinimalRune.Editor.Text
{
    /// <summary>
    /// Handles text documents.
    /// </summary>
    internal class TextDocumentFactory : DocumentFactory
    {
        

        

        internal const string AnyDocumentTypeName = "All files";



        

        

        private readonly IEditorService _editor;



        

        



        

        

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDocumentFactory"/> class.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="editor"/> is <see langword="null"/>.
        /// </exception>
        public TextDocumentFactory(IEditorService editor)
            : base("Text Editor")
        {
            if (editor == null)
                throw new ArgumentNullException(nameof(editor));

            _editor = editor;

            // ----- Initialize supported document types.
            // Text Files (*.txt)
            var textDocumentType = new DocumentType(
                name: "Text file",
                factory: this,
                icon: MultiColorGlyphs.Document,
                fileExtensions: new[] { ".txt" },
                isCreatable: true,
                isLoadable: true,
                isSavable: true,
                priority: 10);

            // XML Files (*.XML)
            var xmlDocumentType = new DocumentType(
                name: "XML file",
                factory: this,
                icon: MultiColorGlyphs.DocumentXml,
                fileExtensions: new[] { ".xml" },
                isCreatable: false,
                isLoadable: true,
                isSavable: true,
                priority: 0);

            // All Files (*.*)
            var anyDocumentType = new DocumentType(
                name: AnyDocumentTypeName,
                factory: this,
                icon: null,
                fileExtensions: new[] { ".*" },
                isCreatable: false,
                isLoadable: true,
                isSavable: true,
                priority: 0);

            DocumentTypes = new[]
            {
                textDocumentType,
                xmlDocumentType,
                anyDocumentType
            };
        }



        

            

        /// <inheritdoc/>
        protected override Document OnCreate(DocumentType documentType)
        {
            var document = new TextDocument(_editor, documentType);

            TextIntelliSense.EnableIntelliSense(document);

            return document;
        }

    }
}
