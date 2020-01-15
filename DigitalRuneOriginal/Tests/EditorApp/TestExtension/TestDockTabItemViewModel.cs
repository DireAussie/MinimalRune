using MinimalRune.Windows.Framework;
using MinimalRune.Editor;
using Microsoft.Practices.ServiceLocation;


namespace EditorApp
{
    internal sealed class TestDockTabItemViewModel : EditorDockTabItemViewModel
    {
        

        

        public new const string DockId = "TestView";



        

        

        public DelegateCommand PrintPreviewCommand { get; private set; }

        public DelegateCommand PrintCommand { get; private set; }



        

        

        public TestDockTabItemViewModel()
        {
            DisplayName = "TestView";
            base.DockId = DockId;
            Icon = EditorHelper.GetPackedBitmap("pack://application:,,,/DigitalRune.Editor;component/Resources/Images/Icons.png", 32, 96, 32, 32);

            PrintPreviewCommand = new DelegateCommand(ShowPrintPreview);
            PrintCommand = new DelegateCommand(Print);
        }



        

        

        private static void ShowPrintPreview()
        {
            var windowService = ServiceLocator.Current.GetInstance<IWindowService>();
            windowService.ShowDialog(new TestPrintDocument());
        }


        private static void Print()
        {
            new TestPrintDocument().Print();
        }

    }
}
