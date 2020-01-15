// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Threading.Tasks;
using MinimalRune.Editor.Documents;
using MinimalRune.Editor.Status;
using MinimalRune.Editor.Text;
using MinimalRune.Windows.Framework;
using static System.FormattableString;


namespace MinimalRune.Editor.Shader
{
    partial class ShaderExtension
    {
        

        



        

        



        

        

        private bool CanAnalyze()
        {
            var document = _documentService.ActiveDocument as TextDocument;
            return !_isBusy
                   && document != null
                   && document.DocumentType.Name == ShaderDocumentFactory.FxFile;
        }


        private async void Analyze()
        {
            if (_isBusy)
                return;

            var document = _documentService.ActiveDocument as TextDocument;
            if (document == null || document.DocumentType.Name != ShaderDocumentFactory.FxFile)
                return;

            Logger.Debug("Analyzing effect {0}.", document.GetName());

            bool isSaved = !document.IsUntitled && !document.IsModified;
            if (!isSaved)
                isSaved = _documentService.Save(document);

            if (!isSaved || document.Uri == null)
            {
                Logger.Debug("Document was not saved. Analysis canceled by user.");
                return;
            }

            _outputService.Clear();

            var status = new StatusViewModel
            {
                Message = "Analyzing...",
                ShowProgress = true,
                Progress = double.NaN,
            };
            _statusService.Show(status);

            // Disable buttons to avoid reentrance.
            _isBusy = true;
            UpdateCommands();

            try
            {
                var shaderPerf = Editor.Services.GetInstance<ShaderPerf>().ThrowIfMissing();
                var success = await Task.Run(() => shaderPerf.Run(document));

                status.Progress = 100;
                status.IsCompleted = true;

                if (success)
                {
                    status.Message = "Analysis succeeded.";
                }
                else
                {
                    _outputService.Show();
                    status.Message = "Analysis failed.";
                    status.ShowProgress = false;
                }
            }
            catch (Exception exception)
            {
                Logger.Warn(exception, "Analysis failed.");

                _outputService.WriteLine(Invariant($"Exception: {exception.Message}"));
                _outputService.Show();

                status.Message = "Analysis failed.";
                status.ShowProgress = false;
            }

            // Enable buttons.
            _isBusy = false;
            UpdateCommands();

            status.CloseAfterDefaultDurationAsync().Forget();
        }

    }
}
