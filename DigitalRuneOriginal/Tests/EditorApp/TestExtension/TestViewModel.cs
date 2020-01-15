using System;
using MinimalRune.Windows.Framework;
using MinimalRune.Editor;
using MinimalRune.Windows.Themes;


namespace EditorApp
{
    internal sealed class TestViewModel : EditorDockTabItemViewModel
    {
        

        

        internal const string DockIdString = "TestView";



        

        

        private static int NextId = 0;

        private readonly IEditorService _editor;
        private int _debugID = NextId++;



        

        

        public DelegateCommand PrintPreviewCommand { get; private set; }

        public DelegateCommand PrintCommand { get; private set; }

        public DelegateCommand DoSomethingCommand { get; private set; }



        

        

        public TestViewModel(IEditorService editor)
        {
            if (editor == null)
                throw new ArgumentNullException(nameof(editor));

            _editor = editor;
            DisplayName = "TestView";
            DockId = DockIdString;
            Icon = MultiColorGlyphs.Document;

            PrintPreviewCommand = new DelegateCommand(ShowPrintPreview);
            PrintCommand = new DelegateCommand(Print);
            DoSomethingCommand = new DelegateCommand(DoSomething);
        }



        

        

        private void ShowPrintPreview()
        {
            var windowService = _editor.Services.GetInstance<IWindowService>();
            windowService.ShowDialog(new TestPrintDocumentProvider());
        }


        private static void Print()
        {
            new TestPrintDocumentProvider().Print();
        }


        private static void DoSomething()
        {
            //CommandManager.InvalidateRequerySuggested();
        }

    }
}
