// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using MinimalRune.Collections;


namespace MinimalRune.Editor.Documents
{
    partial class DocumentExtension
    {
        

        

        private readonly MenuManager _menuManager;
        private MergeableNodeCollection<ICommandItem> _contextMenuNodes;



        

        

        /// <inheritdoc/>
        public ICollection<MergeableNodeCollection<ICommandItem>> DockContextMenuNodeCollections { get; } = new Collection<MergeableNodeCollection<ICommandItem>>();


        /// <inheritdoc/>
        public MenuItemViewModelCollection DockContextMenu
        {
            get { return _menuManager.Menu; }
        }



        

        

        private void AddContextMenu()
        {
            var saveMergePoints = new[] { new MergePoint(MergeOperation.InsertBefore, "Close"), MergePoint.Append };
            var insertBeforeFileSeparator = new[] { new MergePoint(MergeOperation.InsertBefore, "FileSeparator"), MergePoint.Append };
            _contextMenuNodes = new MergeableNodeCollection<ICommandItem>
            {
                new MergeableNode<ICommandItem>(CommandItems["Save"], saveMergePoints),
                new MergeableNode<ICommandItem>(CommandItems["SaveAs"], saveMergePoints),
                new MergeableNode<ICommandItem>(CommandItems["Close"], new MergePoint(MergeOperation.Replace, "Close"), MergePoint.Append),
                new MergeableNode<ICommandItem>(CommandItems["CloseAllButThis"], insertBeforeFileSeparator),
                new MergeableNode<ICommandItem>(CommandItems["CloseAll"], insertBeforeFileSeparator),
                new MergeableNode<ICommandItem>(new CommandSeparator("CloseSeparator"), insertBeforeFileSeparator),
                new MergeableNode<ICommandItem>(CommandItems["CopyFullPath"], insertBeforeFileSeparator),
                new MergeableNode<ICommandItem>(CommandItems["OpenContainingFolder"], insertBeforeFileSeparator),
                new MergeableNode<ICommandItem>(new CommandSeparator("UriSeparator"), insertBeforeFileSeparator),
            };

            DockContextMenuNodeCollections.Add(_contextMenuNodes);
        }


        private void RemoveContextMenu()
        {
            DockContextMenuNodeCollections.Remove(_contextMenuNodes);
            _contextMenuNodes = null;
        }


        private void UpdateContextMenu()
        {
            Logger.Debug("Updating document context menu.");

            _menuManager.Update(Editor.DockContextMenuNodeCollections, DockContextMenuNodeCollections);
        }

    }
}
